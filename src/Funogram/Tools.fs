module Funogram.Tools

open System
open System.IO
open System.Net.Http
open System.Net.Http.Headers
open System.Runtime.CompilerServices
open System.Text
open System.Text.Json
open System.Text.Json.Serialization
open Funogram.Types

[<assembly:InternalsVisibleTo("Funogram.Tests")>]
[<assembly:InternalsVisibleTo("Funogram.Telegram")>]
do ()

open System.Collections.Concurrent
open System.Linq.Expressions
open Funogram.Converters
open TypeShape.Core

module internal RequestLogger =
  type Logger =
    {
      Text: StringBuilder
      Logger: IBotLogger
    }
  
  let createIfRequired (config: BotConfig) =
    match config.RequestLogger with
    | Some logger when logger.Enabled -> { Text = StringBuilder(); Logger = logger } |> Some
    | _ -> None
  
  let appendReqAsync (url: string) (content: MultipartFormDataContent) (hasData: bool) (logger: Logger) =
    task {
      let req = if hasData then "POST" else "GET"
      let sb = logger.Text.Append("Req: ").Append(req).Append(" ").AppendLine(url)
      try
        if hasData then
          sb.AppendLine("multipart/form-data") |> ignore
          for item in content do
            sb.Append(item.Headers.ContentDisposition.Name) |> ignore
            match item with
            | :? StringContent as s ->
              let! s = s.ReadAsStringAsync()
              sb.Append("=").Append(s).AppendLine() |> ignore
            | :? ByteArrayContent as b ->
              let fileName = item.Headers.ContentDisposition.FileName
              if String.IsNullOrEmpty(fileName) then
                let! b = b.ReadAsByteArrayAsync()
                let s = Encoding.UTF8.GetString(b)
                sb.Append("=").AppendLine(s) |> ignore
              else
                sb.Append("=[file ").Append(fileName).AppendLine("]") |> ignore
            | _ -> ()
      with | _ -> ()
    }
  
  let appendReqJson (url: string) (data: byte[]) (logger: Logger) =
    logger.Text
      .Append("Req: POST ")
      .AppendLine(url)
      .AppendLine("application/json")
      .AppendLine(Encoding.UTF8.GetString(data)) |> ignore
    
  
  let appendResAndWriteAsync (stream: Stream) (logger: Logger) =
    async {
      let! data = stream.AsyncRead(int stream.Length)
      logger.Text.Append("Res: ").Append(Encoding.UTF8.GetString(data)) |> ignore
      stream.Seek(0, SeekOrigin.Begin) |> ignore
      logger.Logger.Log(logger.Text.ToString())
    }
  
  let appendResExceptionAndWrite (e: exn) (logger: Logger) =
    logger.Text.Append("Res: ").Append(e.ToString()) |> ignore
    logger.Logger.Log(logger.Text.ToString())

let internal options =
  let o =
    JsonSerializerOptions(
      WriteIndented = false,
      PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
      DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    )
  o.Converters.Add(DiscriminatedUnionConverterFactory())
  o.Converters.Add(UnixTimestampDateTimeConverter())
  o.Converters.Add(OptionConverterFactory())
  o

let private getUrl (config: BotConfig) methodName = 
  let botToken = sprintf "%s%s" (config.ApiEndpointUrl |> string) config.Token
  
  if config.IsTest then
    sprintf "%s/test/%s" botToken methodName
  else
    sprintf "%s/%s" botToken methodName

let internal getUnix (date: DateTime) = 
  Convert.ToInt64(date.Subtract(DateTime(1970, 1, 1)).TotalSeconds)

let internal parseJson<'a> (data: byte[]) =
  try
    match JsonSerializer.Deserialize<ApiResponse<'a>>(data, options) with
    | x when x.Ok && x.Result.IsSome -> Ok x.Result.Value
    | x when x.Description.IsSome && x.ErrorCode.IsSome -> 
      Error { Description = x.Description.Value
              ErrorCode = x.ErrorCode.Value }
    | x -> 
      Error { Description = "Unknown error"
              ErrorCode = -1 }
  with ex ->
    let json = Encoding.UTF8.GetString data
    let message = sprintf "%s in %s" ex.Message json
    ArgumentException(message, ex) |> raise

let internal parseJsonStream<'a> (data: Stream) =
  try
    JsonSerializer.Deserialize<'a>(data, options) |> Ok
  with ex ->
    if data.CanSeek then 
      data.Seek(0L, SeekOrigin.Begin) |> ignore
      use sr = new StreamReader(data)
      let message = sprintf "%s in %s" ex.Message (sr.ReadToEnd())
      ArgumentException(message, ex) :> Exception |> Result.Error
    else
      Exception("Unable to parse json") |> Result.Error

let internal parseJsonStreamApiResponse<'a> (data: Stream) =
  match parseJsonStream<ApiResponse<'a>> data with
  | Ok x when x.Ok && x.Result.IsSome -> Ok x.Result.Value

  | Ok x when x.Description.IsSome && x.ErrorCode.IsSome -> 
    Error { Description = x.Description.Value; ErrorCode = x.ErrorCode.Value }

  | Error e -> 
    Error { Description = e.Message; ErrorCode = -1 }

  | _ -> 
    Error { Description = "Unknown error"; ErrorCode = -1 }

[<ReflectedDefinition>]
let toJson (o: 'a) = JsonSerializer.SerializeToUtf8Bytes<'a>(o, options)

let private toJsonMethodInfo =
  System.Reflection.Assembly.GetExecutingAssembly()
    .GetType("Funogram.Tools").GetMethod("toJson")

// json request serializer
let private jsonSerializers = ConcurrentDictionary<Type, Func<IBotRequest, byte[]>>()
let private generateSerializer tp =
  let sourceParam = Expression.Parameter(typeof<IBotRequest>)
  let convert = Expression.Convert(sourceParam, tp)
  let method = toJsonMethodInfo.MakeGenericMethod(tp)
  let call = Expression.Call(method, convert)
  Expression.Lambda<Func<IBotRequest, byte[]>>(call, [sourceParam]).Compile()
  
let toJsonBotRequest (request: IBotRequest) =
  let toJson =
    jsonSerializers.GetOrAdd(
      request.GetType(),
      Func<Type, Func<IBotRequest, byte[]>>(generateSerializer))
  toJson.Invoke(request)  

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
  let rec mkFilesFinder<'T> () : 'T -> File[] =
    let mkMemberFinder (shape : IShapeMember<'T>) =
       shape.Accept { new IMemberVisitor<'T, 'T -> File[]> with
         member _.Visit (shape : ShapeMember<'T, 'a>) =
          let fieldFinder = mkFilesFinder<'a>()
          fieldFinder << shape.Get }    
    let wrap(p : 'a -> File[]) = unbox<'T -> File[]> p
    
    match shapeof<'T> with
    | Shape.FSharpOption s ->
      s.Element.Accept {
        new ITypeVisitor<'T -> File[]> with
          member _.Visit<'a> () =
            let tp = mkFilesFinder<'a>()
            wrap(function None -> [||] | Some t -> (tp t))
      }
    | Shape.FSharpList s ->
      s.Element.Accept {
        new ITypeVisitor<'T -> File[]> with
          member _.Visit<'a> () =
            let tp = mkFilesFinder<'a>()
            wrap(fun ts -> ts |> Seq.map tp |> Array.concat)
        }

    | Shape.Array s when s.Rank = 1 ->
      s.Element.Accept {
        new ITypeVisitor<'T -> File[]> with
          member _.Visit<'a> () =
            let tp = mkFilesFinder<'a> ()
            fun (t: 'T) ->
              let r = t |> box :?> seq<'a>
              r |> Seq.map tp |> Array.concat
      }
        
    | Shape.Tuple (:? ShapeTuple<'T> as shape) ->
      let mkElemFinder (shape : IShapeMember<'T>) =
        shape.Accept { new IMemberVisitor<'T, 'T -> File[]> with
          member _.Visit (shape : ShapeMember<'T, 'Field>) =
            let fieldFinder = mkFilesFinder<'Field>()
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
            let tp = mkFilesFinder<'a>()
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

  let multipartSerializers = ConcurrentDictionary<Type, IBotRequest -> MultipartFormDataContent -> bool>()
  
  let mkBaseGeneratorMethod =
    System.Reflection.Assembly.GetExecutingAssembly()
      .GetType("Funogram.Tools")
      .GetNestedType("Api")
      .GetMethod("mkBaseGenerator")

  let generateMultipartSerializer (tp: Type) =
    let method = mkBaseGeneratorMethod.MakeGenericMethod(tp)
    Expression.Lambda<Func<IBotRequest -> MultipartFormDataContent -> bool>>(Expression.Call(method))
      .Compile().Invoke()

  let rec mkRequestGenerator<'T> () : 'T -> string -> MultipartFormDataContent -> bool =
    
    let inline ($) _ x = x
  
    let mkGenerateInMember (shape : IShapeMember<'DeclaringType>) =
      shape.Accept { new IMemberVisitor<'DeclaringType, 'DeclaringType -> string -> MultipartFormDataContent -> bool> with
        member _.Visit (shape : ShapeMember<'DeclaringType, 'Field>) =
          let inFieldFinder = mkRequestGenerator<'Field>()
          inFieldFinder << shape.Get }

    let wrap(p : 'a -> string -> MultipartFormDataContent -> bool) =
      unbox<'T -> string -> MultipartFormDataContent -> bool> p

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
    
    let strf a b = new StringContent(sprintf a b)
    
    match shapeof<'T> with
    | Shape.Bool ->
      wrap(fun x prop data -> data.Add(strf "%b" x, prop) $ true)
    | Shape.Int16 ->
      wrap(fun (x: int16) prop data -> data.Add(strf "%i" x, prop) $ true)
    | Shape.Int32 ->
      wrap(fun x prop data -> data.Add(strf "%i" x, prop) $ true)
    | Shape.Int64 ->
      wrap(fun (x: int64) prop data -> data.Add(strf "%i" x, prop) $ true)
    | Shape.Decimal ->
      wrap(fun x prop data -> data.Add(strf "%f" x, prop) $ true)
    | Shape.Double ->
      wrap(fun x prop data -> data.Add(strf "%f" x, prop) $ true)
    | Shape.Uri ->
      wrap(fun (x: Uri) prop data -> data.Add(strf "%O" x, prop) $ true)
    | Shape.UInt16 ->
      wrap(fun (x: uint16) prop data -> data.Add(strf "%i" x, prop) $ true)
    | Shape.UInt32 ->
      wrap(fun (x: uint32) prop data -> data.Add(strf "%i" x, prop) $ true)
    | Shape.UInt64 ->
      wrap(fun (x: uint32) prop data -> data.Add(strf "%i" x, prop) $ true)
    | Shape.Byte ->
      wrap(fun (x: byte) prop data -> data.Add(strf "%i" x, prop) $ true)
    | Shape.SByte ->
      wrap(fun (x: byte) prop data -> data.Add(strf "%i" x, prop) $ true)
    | Shape.String ->
      wrap(fun x prop data -> data.Add(strf "%s" x, prop) $ true)
    | Shape.DateTime ->
      wrap(fun x prop data -> data.Add(strf "%i" (toUnix x), prop) $ true)
    | Shape.FSharpRecord (:? ShapeFSharpRecord<'T> as shape) ->
      let fieldPrinters : (string * ('T -> string -> MultipartFormDataContent -> bool)) [] = 
        shape.Fields |> Array.map (fun f -> f.Label, mkGenerateInMember f)

      fun (x: 'T) prop data ->
        if String.IsNullOrEmpty(prop) then
          fieldPrinters
          |> Array.map (fun (prop, fp) -> fp x (StringUtils.toSnakeCase prop) data)
          |> Array.contains true
        else
          let json = toJson x
          data.Add(new ByteArrayContent(json), prop)
          addFiles x data
          true
    | Shape.FSharpOption s ->
      s.Element.Accept {
        new ITypeVisitor<'T -> string -> MultipartFormDataContent -> bool> with
          member _.Visit<'a> () =
            let tp = mkRequestGenerator<'a>()
            wrap(fun x prop data ->
              match x with
              | None -> false
              | Some t -> tp t prop data)
      }
    | Shape.FSharpList s ->
      s.Element.Accept {
        new ITypeVisitor<'T -> string -> MultipartFormDataContent -> bool> with
          member _.Visit<'a> () =
            fun x prop data ->
              let json = toJson x
              data.Add(new ByteArrayContent(json), prop)
              addFiles x data
              true
      }
    | Shape.Array s when s.Rank = 1 ->
      s.Element.Accept {
        new ITypeVisitor<'T -> string -> MultipartFormDataContent -> bool> with
          member _.Visit<'a> () =
            fun x prop data ->
              let json = toJson x
              data.Add(new ByteArrayContent(json), prop)
              addFiles x data
              true
        }
    | Shape.FSharpSet s ->
      s.Accept {
        new IFSharpSetVisitor<'T -> string -> MultipartFormDataContent -> bool> with
          member _.Visit<'a when 'a : comparison> () =
            fun x prop data ->
              let json = toJson x
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
          fun _ (prop: string) (data: MultipartFormDataContent) ->
            data.Add(strf "%s" name, prop) $ true
        else
          fun (x: 'T) (prop: string) (data: MultipartFormDataContent) ->
            match readFile with
            | Some fn ->
              let file = fn x case
              match file with
              | Stream (name, stream) ->
                data.Add(new StreamContent(stream), prop, name) $ true
              | Bytes (name, bytes) ->
                data.Add(new ByteArrayContent(bytes), prop, name) $ true
            | None ->
              let fieldPrinters = case.Fields |> Array.map mkGenerateInMember
              fieldPrinters
              |> Array.map (fun fp -> fp x prop data) 
              |> Array.contains true

      let casePrinters = cases |> Array.map mkUnionCasePrinter // generate printers for all union cases
      fun (u:'T) ->
        let tag : int = shape.GetTag u // get the underlying tag for the union case
        casePrinters[tag] u
    | _ ->
      fun _ _ _ -> false
  
  let mkBaseGenerator<'a when 'a :> IBotRequest> () =
    let fn = mkRequestGenerator<'a> ()
    fun (request: IBotRequest) -> fn (request :?> 'a) ""
  
  let makeRequestAsync<'a> config (request: IBotRequest) =
    async {
      let! ct = Async.CancellationToken
      let client = config.Client
      let url = getUrl config request.MethodName
      let serialize =
        multipartSerializers.GetOrAdd(
          request.GetType(),
          Func<Type, IBotRequest -> MultipartFormDataContent -> bool>(generateMultipartSerializer)
        )

      use content = new MultipartFormDataContent()
      let hasData = serialize request content
      
      let logger = RequestLogger.createIfRequired config
      match logger with
      | Some logger -> do! logger |> RequestLogger.appendReqAsync url content hasData |> Async.AwaitTask
      | _ -> ()

      let mutable statusCode = -1
      try
        let! result =
          if hasData then client.PostAsync(url, content, cancellationToken = ct) |> Async.AwaitTask
          else client.GetAsync(url, cancellationToken = ct) |> Async.AwaitTask
        
        statusCode <- result.StatusCode |> int
        
        use! stream = result.Content.ReadAsStreamAsync() |> Async.AwaitTask
        match logger with
        | Some logger -> do! logger |> RequestLogger.appendResAndWriteAsync stream
        | _ -> ()
        return parseJsonStreamApiResponse<'a> stream
      with
      | e ->
        logger |> Option.iter (RequestLogger.appendResExceptionAndWrite e)
        return Error { Description = "HTTP_ERROR"; ErrorCode = statusCode }
    }

  let makeJsonBodyRequestAsync<'a, 'b when 'a :> IRequestBase<'b>> config (request: 'a) =
    async {
      let! ct = Async.CancellationToken
      let client = config.Client
      let url = getUrl config request.MethodName
      
      let logger = RequestLogger.createIfRequired config
      
      let bytes = JsonSerializer.SerializeToUtf8Bytes(request, options)
      logger |> Option.iter (RequestLogger.appendReqJson url bytes)
      
      let mutable statusCode = -1
      try
        use content = new ByteArrayContent(bytes)
        content.Headers.ContentType <- MediaTypeHeaderValue.Parse("application/json")
        let! result = client.PostAsync(url, content, cancellationToken = ct) |> Async.AwaitTask
        statusCode <- result.StatusCode |> int
        
        use! stream = result.Content.ReadAsStreamAsync() |> Async.AwaitTask
        match logger with
        | Some logger -> do! logger |> RequestLogger.appendResAndWriteAsync stream
        | _ -> ()
        return parseJsonStreamApiResponse<'a> stream
      with
      | e ->
        logger |> Option.iter (RequestLogger.appendResExceptionAndWrite e)
        return Error { Description = "HTTP_ERROR"; ErrorCode = statusCode }
    }