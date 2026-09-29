module Funogram.Tools

open System
open System.IO
open System.Net.Http
open System.Net.Http.Headers
open System.Runtime.CompilerServices
open System.Text
open System.Text.Json
open System.Text.Json.Serialization
open Funogram
open Funogram.Types
open Microsoft.Extensions.Logging

[<assembly:InternalsVisibleTo("Funogram.Tests")>]
[<assembly:InternalsVisibleTo("Funogram.Telegram")>]
do ()

open System.Collections.Concurrent
open Funogram.Converters
open TypeShape.Core

/// Request logging levels:
/// - Trace: method, status, duration, request and response bodies
/// - Debug: method, status, duration
/// - Error: transport exceptions (always, regardless of Trace/Debug)
module internal RequestLogger =
  let private completedEvent = EventId(1, "RequestCompleted")
  let private failedEvent = EventId(2, "RequestFailed")

  /// Created for every request, so it must stay cheap: the token is masked
  /// and the elapsed time is computed only when something is actually logged
  type Scope =
    { Logger: ILogger
      Method: string
      Token: BotToken
      Request: string
      StartedAt: int64 }

  let private start (config: BotConfig) methodName request =
    { Logger = config.Logger
      Method = methodName
      Token = config.Token
      Request = request
      StartedAt = Diagnostics.Stopwatch.GetTimestamp() }

  let private elapsedMs (scope: Scope) =
    (Diagnostics.Stopwatch.GetTimestamp() - scope.StartedAt) * 1000L / Diagnostics.Stopwatch.Frequency

  /// Keeps a multipart value on a single log line
  let private escapeValue (value: string) =
    value.Replace("\\", "\\\\").Replace("\r", "\\r").Replace("\n", "\\n").Replace("\t", "\\t")

  let private formatMultipartAsync (content: MultipartFormDataContent) =
    task {
      let parts = ResizeArray<string>()
      try
        for item in content do
          let disposition = item.Headers.ContentDisposition
          let name = disposition.Name.Trim('"')
          if not (String.IsNullOrEmpty disposition.FileName) then
            parts.Add($"{name}=[file {disposition.FileName.Trim('"')}]")
          else
            let! value = item.ReadAsStringAsync()
            parts.Add($"{name}={escapeValue value}")
      with _ -> ()
      return String.Join(", ", parts)
    }

  let startMultipartAsync (config: BotConfig) methodName (content: MultipartFormDataContent) (hasData: bool) =
    async {
      let! request =
        if hasData && config.Logger.IsEnabled LogLevel.Trace then
          formatMultipartAsync content |> Async.AwaitTask
        else
          async.Return ""
      return start config methodName request
    }

  let startJson (config: BotConfig) methodName (data: byte[]) =
    let request = if config.Logger.IsEnabled LogLevel.Trace then Encoding.UTF8.GetString data else ""
    start config methodName request

  let completedAsync (scope: Scope) (statusCode: int) (stream: Stream) =
    async {
      let logger = scope.Logger
      let elapsed = elapsedMs scope
      if logger.IsEnabled LogLevel.Trace then
        let! data = stream.AsyncRead(int stream.Length)
        stream.Seek(0L, SeekOrigin.Begin) |> ignore
        let response = Encoding.UTF8.GetString data
        if String.IsNullOrEmpty scope.Request then
          logger.LogTrace(
            completedEvent,
            "{BotMethod} → {StatusCode} in {ElapsedMs} ms (bot {Bot})\n\n← {Response}",
            scope.Method, statusCode, elapsed, scope.Token, response)
        else
          logger.LogTrace(
            completedEvent,
            "{BotMethod} → {StatusCode} in {ElapsedMs} ms (bot {Bot})\n→ {Request}\n\n← {Response}",
            scope.Method, statusCode, elapsed, scope.Token, scope.Request, response)
      elif logger.IsEnabled LogLevel.Debug then
        logger.LogDebug(
          completedEvent,
          "{BotMethod} → {StatusCode} in {ElapsedMs} ms (bot {Bot})",
          scope.Method, statusCode, elapsed, scope.Token)
    }

  let failed (scope: Scope) (statusCode: int) (e: exn) =
    let elapsed = elapsedMs scope
    let e =
      match e with
      | :? AggregateException as ae when ae.InnerExceptions.Count = 1 -> ae.InnerException
      | e -> e
    if statusCode < 0 then
      scope.Logger.LogError(
        failedEvent, e,
        "{BotMethod} failed after {ElapsedMs} ms (bot {Bot})",
        scope.Method, elapsed, scope.Token)
    else
      scope.Logger.LogError(
        failedEvent, e,
        "{BotMethod} failed with {StatusCode} after {ElapsedMs} ms (bot {Bot})",
        scope.Method, statusCode, elapsed, scope.Token)

/// Shared JSON serializer settings used by Funogram for request and response payloads.
///
/// Reuse this instance when serializing or deserializing Funogram types so external code
/// stays aligned with the library's snake_case wire format, union handling, Unix timestamps,
/// and null-skipping behavior.
let options =
  let o =
    JsonSerializerOptions(
      WriteIndented = false,
      PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
      DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    )
  o.Converters.Add(DiscriminatedUnionConverterFactory())
  o.Converters.Add(UnixTimestampConverter())
  o.Converters.Add(OptionConverterFactory())
  o.Converters.Add(SafeUpdateConverterFactory())
  o

let private getUrl (config: BotConfig) methodName = 
  let botToken = $"{config.ApiEndpointUrl}{config.Token.Reveal()}"
  if config.IsTest then
    $"{botToken}/test/{methodName}"
  else
    $"{botToken}/{methodName}"

let internal parseJsonUtf8<'a> (config: BotConfig) (data: byte[]) =
  JsonSerializer.Deserialize<'a>(data, config.JsonOptions)

let internal parseJsonUtf8Stream<'a> (config: BotConfig) (data: Stream) =
  JsonSerializer.Deserialize<'a>(data, config.JsonOptions)

let private toApiResult<'a> (statusCode: int) (response: ApiResponse<'a>) =
  match response with
  | x when x.Ok && x.Result.IsSome -> Ok x.Result.Value
  | x when x.Description.IsSome && x.ErrorCode.IsSome ->
    Error (ApiError.Rejected { Description = x.Description.Value; ErrorCode = x.ErrorCode.Value; Parameters = x.Parameters })
  | _ ->
    Error (ApiError.InvalidResponse (statusCode, JsonException "Malformed Bot API response"))

/// Telegram accepted the request (ok = true), but its result does not match the expected type.
/// Anything else means the response itself is not a valid Bot API response (e.g. an HTML page from a proxy)
let private isResultError (e: exn) =
  match e with
  | :? JsonException as e -> not (isNull e.Path) && e.Path.StartsWith("$.result", StringComparison.Ordinal)
  | _ -> false

/// The stream belongs to the caller, so it is left open
let private readRaw (data: Stream) =
  if data.CanSeek then
    data.Seek(0L, SeekOrigin.Begin) |> ignore
    use sr = new StreamReader(data, Encoding.UTF8, false, 1024, true)
    sr.ReadToEnd()
  else
    ""

let internal parseJsonResponseUtf8<'a> (config: BotConfig) (statusCode: int) (data: byte[]) =
  try
    parseJsonUtf8<ApiResponse<'a>> config data |> toApiResult statusCode
  with
  | e when isResultError e -> Error (ApiError.UnexpectedResult (Encoding.UTF8.GetString data, e))
  | e -> Error (ApiError.InvalidResponse (statusCode, e))

let internal parseJsonResponseUtf8Stream<'a> (config: BotConfig) (statusCode: int) (data: Stream) =
  try
    parseJsonUtf8Stream<ApiResponse<'a>> config data |> toApiResult statusCode
  with
  | e when isResultError e -> Error (ApiError.UnexpectedResult (readRaw data, e))
  | e -> Error (ApiError.InvalidResponse (statusCode, e))

let toJsonUtf8 (config: BotConfig) (o: obj) = JsonSerializer.SerializeToUtf8Bytes(o, config.JsonOptions)

let toJsonString (config: BotConfig) (o: obj) = JsonSerializer.Serialize(o, config.JsonOptions)

module Api =
  type File =
    | Stream of string * Stream
    | Bytes of string * byte[]

  let isFileStream (case: ShapeFSharpUnionCase<'T>) =
    case.Fields.Length = 2 && case.Fields[0].Member.Type = typeof<string> && case.Fields[1].Member.Type = typeof<Stream>
        
  let isFileBytes (case: ShapeFSharpUnionCase<'T>) =
    case.Fields.Length = 2 && case.Fields[0].Member.Type = typeof<string> && case.Fields[1].Member.Type = typeof<byte[]>
        
  let readFileStream =
    fun (x: 'T) (case: ShapeFSharpUnionCase<'T>) ->
      let a = 
        case.Fields[0].Accept {
          new IMemberVisitor<'T, 'T -> string> with
            member _.Visit (shape : ShapeMember<'T, 'a>) =
              let cast c = (box c) :?> string
              shape.Get >> cast
          }

      let b = 
        case.Fields[1].Accept {
           new IMemberVisitor<'T, 'T -> Stream> with
             member _.Visit (shape : ShapeMember<'T, 'b>) =
               let cast c = (box c) :?> Stream
               shape.Get >> cast
        }

      File.Stream (a x, b x)
  
  let readFileBytes =
    fun (x: 'T) (case: ShapeFSharpUnionCase<'T>) ->
      let a = 
        case.Fields[0].Accept {
          new IMemberVisitor<'T, 'T -> string> with
            member _.Visit (shape : ShapeMember<'T, 'a>) =
              let cast c = (box c) :?> string
              shape.Get >> cast
          }

      let b = 
        case.Fields[1].Accept {
           new IMemberVisitor<'T, 'T -> byte[]> with
             member _.Visit (shape : ShapeMember<'T, 'b>) =
               let cast c = (box c) :?> byte[]
               shape.Get >> cast
        }

      File.Bytes (a x, b x)
  
  let fileFinders = ConcurrentDictionary<Type, obj>()

  let rec mkFilesFinderCached<'T> (cache: System.Collections.Generic.Dictionary<Type, obj>) : 'T -> File[] =
    match cache.TryGetValue typeof<'T> with
    | true, cell ->
      // Recursive reference: defer to the finder that is still being built.
      let cell = unbox<('T -> File[]) ref> cell
      fun x -> cell.Value x
    | _ ->
      let cell : ('T -> File[]) ref = ref (fun _ -> Array.empty)
      cache[typeof<'T>] <- box cell

      let mkMemberFinder (shape : IShapeMember<'T>) =
         shape.Accept { new IMemberVisitor<'T, 'T -> File[]> with
           member _.Visit (shape : ShapeMember<'T, 'a>) =
            let fieldFinder = mkFilesFinderCached<'a> cache
            fieldFinder << shape.Get }
      let wrap(p : 'a -> File[]) = unbox<'T -> File[]> p

      let finder =
        match shapeof<'T> with
        | Shape.FSharpOption s ->
          s.Element.Accept {
            new ITypeVisitor<'T -> File[]> with
              member _.Visit<'a> () =
                let tp = mkFilesFinderCached<'a> cache
                wrap(function None -> [||] | Some t -> (tp t))
          }
        | Shape.FSharpList s ->
          s.Element.Accept {
            new ITypeVisitor<'T -> File[]> with
              member _.Visit<'a> () =
                let tp = mkFilesFinderCached<'a> cache
                wrap(fun ts -> ts |> Seq.map tp |> Array.concat)
            }

        | Shape.Array s when s.Rank = 1 ->
          s.Element.Accept {
            new ITypeVisitor<'T -> File[]> with
              member _.Visit<'a> () =
                let tp = mkFilesFinderCached<'a> cache
                fun (t: 'T) ->
                  let r = t |> box :?> seq<'a>
                  r |> Seq.map tp |> Array.concat
          }

        | Shape.Tuple (:? ShapeTuple<'T> as shape) ->
          let mkElemFinder (shape : IShapeMember<'T>) =
            shape.Accept { new IMemberVisitor<'T, 'T -> File[]> with
              member _.Visit (shape : ShapeMember<'T, 'Field>) =
                let fieldFinder = mkFilesFinderCached<'Field> cache
                fieldFinder << shape.Get }

          let elemPrinters : ('T -> File[]) [] = shape.Elements |> Array.map mkElemFinder

          fun (r:'T) ->
            elemPrinters
            |> Seq.map (fun ep -> ep r)
            |> Array.concat

        | Shape.FSharpSet s ->
          s.Accept {
            new IFSharpSetVisitor<'T -> File[]> with
              member _.Visit<'a when 'a : comparison> () =
                let tp = mkFilesFinderCached<'a> cache
                wrap(fun (s:Set<'a>) -> s |> Seq.map tp |> Array.concat)
          }
        | Shape.FSharpRecord (:? ShapeFSharpRecord<'T> as shape) ->
          let fieldPrinters : ('T -> File[]) [] =
            shape.Fields |> Array.map mkMemberFinder

          fun (r:'T) ->
            fieldPrinters |> Seq.map (fun fp -> fp r) |> Array.concat
        | Shape.FSharpUnion (:? ShapeFSharpUnion<'T> as shape) ->
          let cases : ShapeFSharpUnionCase<'T> [] = shape.UnionCases // all union cases
          let mkUnionCasePrinter (case : ShapeFSharpUnionCase<'T>) =
            let readFile =
              if isFileStream case then
                readFileStream |> Some
              elif isFileBytes case then
                readFileBytes |> Some
              else None

            let fieldPrinters = case.Fields |> Array.map mkMemberFinder
            fun (x: 'T) ->
              match readFile with
              | Some fn ->
                [| fn x case |]
              | None ->
                fieldPrinters
                |> Seq.map (fun fp -> fp x)
                |> Array.concat

          let casePrinters = cases |> Array.map mkUnionCasePrinter // generate printers for all union cases
          fun (u:'T) ->
            let tag : int = shape.GetTag u // get the underlying tag for the union case
            casePrinters[tag] u
        | _ -> fun _ -> [||]

      cell.Value <- finder
      finder

  let mkFilesFinder<'T> () : 'T -> File[] =
    mkFilesFinderCached<'T> (System.Collections.Generic.Dictionary())

  let multipartSerializers = ConcurrentDictionary<Type, Lazy<BotConfig -> IBotRequest -> MultipartFormDataContent -> bool>>()
  
  let rec mkRequestGeneratorCached<'T> (cache: System.Collections.Generic.Dictionary<Type, obj>)
    : 'T -> BotConfig -> string -> MultipartFormDataContent -> bool =
    match cache.TryGetValue typeof<'T> with
    | true, cell ->
      let cell = unbox<('T -> BotConfig -> string -> MultipartFormDataContent -> bool) ref> cell
      fun x config prop data -> cell.Value x config prop data
    | _ ->
      let cell : ('T -> BotConfig -> string -> MultipartFormDataContent -> bool) ref = ref (fun _ _ _ _ -> false)
      cache[typeof<'T>] <- box cell

      let inline ($) _ x = x

      let mkGenerateInMember (shape : IShapeMember<'DeclaringType>) =
        shape.Accept { new IMemberVisitor<'DeclaringType, 'DeclaringType -> BotConfig -> string -> MultipartFormDataContent -> bool> with
          member _.Visit (shape : ShapeMember<'DeclaringType, 'Field>) =
            let inFieldFinder = mkRequestGeneratorCached<'Field> cache
            inFieldFinder << shape.Get }

      let wrap(p : 'a -> BotConfig -> string -> MultipartFormDataContent -> bool) =
        unbox<'T -> BotConfig -> string -> MultipartFormDataContent -> bool> p

      let addFiles (a: 'v) (data: MultipartFormDataContent) =
        let finder =
          fileFinders.GetOrAdd(typeof<'v>, Func<Type, obj>(fun x -> mkFilesFinder<'v> () |> box))
          |> unbox<'v -> File[]>
        let files = finder a
        files |> Seq.iter (fun x ->
          match x with
          | File.Stream (name, stream) -> data.Add(new StreamContent(stream), name, name)
          | File.Bytes (name, bytes) -> data.Add(new ByteArrayContent(bytes), name, name)
        )

      let inline str (a: 'a) = new StringContent(string a)
      let generator =
        match shapeof<'T> with
        | Shape.Bool ->
          wrap(fun (x: bool) _ prop data -> data.Add((if x then str "true" else str "false"), prop) $ true)
        | Shape.Int16 ->
          wrap(fun (x: int16) _ prop data -> data.Add(str x, prop) $ true)
        | Shape.Int32 ->
          wrap(fun (x: int32) _ prop data -> data.Add(str x, prop) $ true)
        | Shape.Int64 ->
          wrap(fun (x: int64) _ prop data -> data.Add(str x, prop) $ true)
        | Shape.Decimal ->
          wrap(fun (x: decimal) _ prop data -> data.Add(str x, prop) $ true)
        | Shape.Double ->
          wrap(fun (x: float) _ prop data -> data.Add(str x, prop) $ true)
        | Shape.Uri ->
          wrap(fun (x: Uri) _ prop data -> data.Add(str x, prop) $ true)
        | Shape.UInt16 ->
          wrap(fun (x: uint16) _ prop data -> data.Add(str x, prop) $ true)
        | Shape.UInt32 ->
          wrap(fun (x: uint32) _ prop data -> data.Add(str x, prop) $ true)
        | Shape.UInt64 ->
          wrap(fun (x: uint64) _ prop data -> data.Add(str x, prop) $ true)
        | Shape.Byte ->
          wrap(fun (x: byte) _ prop data -> data.Add(str x, prop) $ true)
        | Shape.SByte ->
          wrap(fun (x: sbyte) _ prop data -> data.Add(str x, prop) $ true)
        | Shape.String ->
          wrap(fun (x: string) _ prop data -> data.Add(str x, prop) $ true)
        | Shape.DateTimeOffset ->
          wrap(fun (x: DateTimeOffset) _ prop data -> data.Add(str (x.ToUnixTimeSeconds()), prop) $ true)
        | Shape.FSharpRecord (:? ShapeFSharpRecord<'T> as shape) ->
          let fieldPrinters : (string * ('T -> BotConfig -> string -> MultipartFormDataContent -> bool)) [] = 
            shape.Fields |> Array.map (fun f -> f.Label, mkGenerateInMember f)

          fun (x: 'T) (config: BotConfig) prop data ->
            if String.IsNullOrEmpty(prop) then
              fieldPrinters
              |> Array.map (fun (prop, fp) -> fp x config (StringUtils.toSnakeCase prop) data)
              |> Array.contains true
            else
              let json = toJsonUtf8 config x
              data.Add(new ByteArrayContent(json), prop)
              addFiles x data
              true
        | Shape.FSharpOption s ->
          s.Element.Accept {
            new ITypeVisitor<'T -> BotConfig -> string -> MultipartFormDataContent -> bool> with
              member _.Visit<'a> () =
                let tp = mkRequestGeneratorCached<'a> cache
                wrap(fun x config prop data ->
                  match x with
                  | None -> false
                  | Some t -> tp t config prop data)
          }
        | Shape.FSharpList s ->
          s.Element.Accept {
            new ITypeVisitor<'T -> BotConfig -> string -> MultipartFormDataContent -> bool> with
              member _.Visit<'a> () =
                fun x config prop data ->
                  let json = toJsonUtf8 config x
                  data.Add(new ByteArrayContent(json), prop)
                  addFiles x data
                  true
          }
        | Shape.Array s when s.Rank = 1 ->
          s.Element.Accept {
            new ITypeVisitor<'T -> BotConfig -> string -> MultipartFormDataContent -> bool> with
              member _.Visit<'a> () =
                fun x config prop data ->
                  let json = toJsonUtf8 config x
                  data.Add(new ByteArrayContent(json), prop)
                  addFiles x data
                  true
            }
        | Shape.FSharpSet s ->
          s.Accept {
            new IFSharpSetVisitor<'T -> BotConfig -> string -> MultipartFormDataContent -> bool> with
              member _.Visit<'a when 'a : comparison> () =
                fun x config prop data ->
                  let json = toJsonUtf8 config x
                  data.Add(new ByteArrayContent(json), prop)
                  addFiles x data
                  true
         }
        | Shape.FSharpUnion (:? ShapeFSharpUnion<'T> as shape) ->
          let cases : ShapeFSharpUnionCase<'T> [] = shape.UnionCases // all union cases
          let mkUnionCasePrinter (case : ShapeFSharpUnionCase<'T>) =
            let isEnum = case.Fields.Length = 0

            let readFile =
              if isFileStream case then
                readFileStream |> Some
              elif isFileBytes case then
                readFileBytes |> Some
              else None

            if isEnum then
              let name = StringUtils.caseName case.CaseInfo
              fun _ (_: BotConfig) (prop: string) (data: MultipartFormDataContent) ->
                data.Add(str name, prop) $ true
            else
              let fieldPrinters = case.Fields |> Array.map mkGenerateInMember
              fun (x: 'T) (config: BotConfig) (prop: string) (data: MultipartFormDataContent) ->
                match readFile with
                | Some fn ->
                  let file = fn x case
                  match file with
                  | Stream (name, stream) ->
                    data.Add(new StreamContent(stream), prop, name) $ true
                  | Bytes (name, bytes) ->
                    data.Add(new ByteArrayContent(bytes), prop, name) $ true
                | None ->
                  fieldPrinters
                  |> Array.map (fun fp -> fp x config prop data) 
                  |> Array.contains true

          let casePrinters = cases |> Array.map mkUnionCasePrinter // generate printers for all union cases
          fun (u:'T) ->
            let tag : int = shape.GetTag u // get the underlying tag for the union case
            casePrinters[tag] u
        | _ ->
          fun _ _ _ _ -> false

      cell.Value <- generator
      generator

  let mkRequestGenerator<'T> () =
    mkRequestGeneratorCached<'T> (System.Collections.Generic.Dictionary())

  let generateMultipartSerializer (tp: Type) : BotConfig -> IBotRequest -> MultipartFormDataContent -> bool =
    TypeShape.Create(tp).Accept {
      new ITypeVisitor<BotConfig -> IBotRequest -> MultipartFormDataContent -> bool> with
        member _.Visit<'a> () =
          let fn = mkRequestGenerator<'a> ()
          fun config request -> fn (unbox<'a> request) config ""
    }
  
  let makeRequestAsync<'a> config (request: IBotRequest) =
    async {
      let! ct = Async.CancellationToken
      let client = config.Client
      let url = getUrl config request.MethodName
      let serialize =
        multipartSerializers.GetOrAdd(
          request.GetType(),
          Func<Type, Lazy<BotConfig -> IBotRequest -> MultipartFormDataContent -> bool>>(fun tp ->
            lazy generateMultipartSerializer tp)
        ).Value

      use content = new MultipartFormDataContent()
      let hasData = serialize config request content
      
      let! log = RequestLogger.startMultipartAsync config request.MethodName content hasData

      let mutable statusCode = -1
      try
        let! result =
          if hasData then client.PostAsync(url, content, cancellationToken = ct) |> Async.AwaitTask
          else client.GetAsync(url, cancellationToken = ct) |> Async.AwaitTask

        statusCode <- result.StatusCode |> int

        use! stream = result.Content.ReadAsStreamAsync() |> Async.AwaitTask
        do! RequestLogger.completedAsync log statusCode stream
        return parseJsonResponseUtf8Stream<'a> config statusCode stream
      with
      | e when not ct.IsCancellationRequested ->
        RequestLogger.failed log statusCode e
        return Error (ApiError.Network e)
    }

  let makeJsonBodyRequestAsync<'a, 'b when 'a :> IRequestBase<'b>> config (request: 'a): Async<Result<'b, ApiError>> =
    async {
      let! ct = Async.CancellationToken
      let client = config.Client
      let url = getUrl config request.MethodName
      
      let bytes = toJsonUtf8 config request
      let log = RequestLogger.startJson config request.MethodName bytes

      let mutable statusCode = -1
      try
        use content = new ByteArrayContent(bytes)
        content.Headers.ContentType <- MediaTypeHeaderValue.Parse("application/json")
        let! result = client.PostAsync(url, content, cancellationToken = ct) |> Async.AwaitTask
        statusCode <- result.StatusCode |> int
        
        use! stream = result.Content.ReadAsStreamAsync() |> Async.AwaitTask
        do! RequestLogger.completedAsync log statusCode stream
        return parseJsonResponseUtf8Stream<'b> config statusCode stream
      with
      | e when not ct.IsCancellationRequested ->
        RequestLogger.failed log statusCode e
        return Error (ApiError.Network e)
    }