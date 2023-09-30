module Funogram.Tools

open System
open System.Net
open System.Net.Http
open System.Runtime.CompilerServices
open Utf8Json
open Utf8Json.Resolvers

[<assembly:InternalsVisibleTo("Funogram.Tests")>]
[<assembly:InternalsVisibleTo("Funogram.Telegram")>]
do ()

open System.Collections.Concurrent
open System.IO
open System.Linq.Expressions
open Funogram.Types
open Funogram.Resolvers
open TypeShape.Core
open Utf8Json.FSharp

let internal formatters: IJsonFormatter[] = [|
  FunogramUnixTimestampDateTimeFormatter()
|]

let internal resolvers: IJsonFormatterResolver[] =[|
  FunogramResolver.Instance
  FSharpResolver.Instance
  StandardResolver.ExcludeNullSnakeCase
|]

let internal resolver =
  Resolvers.CompositeResolver.Create(
    formatters,
    resolvers
  )

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
    match JsonSerializer.Deserialize<ApiResponse<'a>>(data, resolver) with
    | x when x.Ok && x.Result.IsSome -> Ok x.Result.Value
    | x when x.Description.IsSome && x.ErrorCode.IsSome ->
      Error { Description = x.Description.Value
              ErrorCode = x.ErrorCode.Value }
    | _ ->
      Error { Description = "Unknown error"
              ErrorCode = -1 }
  with ex ->
    let json = System.Text.Encoding.UTF8.GetString data
    let message = sprintf "%s in %s" ex.Message json
    ArgumentException(message, ex) |> raise

let parseJsonStream<'a> (data: Stream) =
  try
    JsonSerializer.Deserialize<'a>(data, resolver) |> Ok
  with ex ->
    if data.CanSeek then
      data.Seek(0L, SeekOrigin.Begin) |> ignore
      use sr = new StreamReader(data)
      let message = sprintf "%s in %s" ex.Message (sr.ReadToEnd())
      ArgumentException(message, ex) :> Exception |> Result.Error
    else
      Exception("Can't parse json") |> Result.Error

let internal parseJsonStreamApiResponse<'a> (data: Stream) =
  match parseJsonStream<Types.ApiResponse<'a>> data with
  | Ok x when x.Ok && x.Result.IsSome -> Ok x.Result.Value

  | Ok x when x.Description.IsSome && x.ErrorCode.IsSome ->
    Error { Description = x.Description.Value; ErrorCode = x.ErrorCode.Value }

  | Error e ->
    Error { Description = e.Message; ErrorCode = -1 }

  | _ ->
    Error { Description = "Unknown error"; ErrorCode = -1 }

[<ReflectedDefinition>]
let toJson (o: 'a) = JsonSerializer.Serialize<'a>(o, resolver)

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
  type File = string * Stream

  let isFile (case: ShapeFSharpUnionCase<'T>) =
    case.Fields
    |> Array.map (fun x -> x.Member.Type) = [|typeof<string>;typeof<Stream>|]

  let readFile =
    fun (x: 'T) (case: ShapeFSharpUnionCase<'T>) ->
      let a =
        case.Fields.[0].Accept {
          new IMemberVisitor<'T, 'T -> string> with
            member __.Visit (shape : ShapeMember<'T, 'a>) =
              let cast c = (box c) :?> string
              shape.Get >> cast
          }

      let b =
        case.Fields.[1].Accept {
           new IMemberVisitor<'T, 'T -> Stream> with
             member __.Visit (shape : ShapeMember<'T, 'b>) =
               let cast c = (box c) :?> Stream
               shape.Get >> cast
        }
      (a x, b x)

  let fileFinders = ConcurrentDictionary<Type, obj>()
  let rec mkFilesFinder<'T> () : 'T -> File[] =
    let mkMemberFinder (shape : IShapeMember<'T>) =
       shape.Accept { new IMemberVisitor<'T, 'T -> File[]> with
         member __.Visit (shape : ShapeMember<'T, 'a>) =
          let fieldFinder = mkFilesFinder<'a>()
          fieldFinder << shape.Get }
    let wrap(p : 'a -> File[]) = unbox<'T -> File[]> p

    match shapeof<'T> with
    | Shape.FSharpOption s ->
      s.Element.Accept {
        new ITypeVisitor<'T -> File[]> with
          member __.Visit<'a> () =
            let tp = mkFilesFinder<'a>()
            wrap(function None -> [||] | Some t -> (tp t))
      }
    | Shape.FSharpList s ->
      s.Element.Accept {
        new ITypeVisitor<'T -> File[]> with
          member __.Visit<'a> () =
            let tp = mkFilesFinder<'a>()
            wrap(fun ts -> ts |> Seq.map tp |> Array.concat)
        }

    | Shape.Array s when s.Rank = 1 ->
      s.Element.Accept {
        new ITypeVisitor<'T -> File[]> with
          member __.Visit<'a> () =
            let tp = mkFilesFinder<'a> ()
            fun (t: 'T) ->
              let r = t |> box :?> seq<'a>
              r |> Seq.map tp |> Array.concat
      }

    | Shape.Tuple (:? ShapeTuple<'T> as shape) ->
      let mkElemFinder (shape : IShapeMember<'T>) =
        shape.Accept { new IMemberVisitor<'T, 'T -> File[]> with
          member __.Visit (shape : ShapeMember<'T, 'Field>) =
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
          member __.Visit<'a when 'a : comparison> () =
            let tp = mkFilesFinder<'a>()
            wrap(fun (s:Set<'a>) -> s |> Seq.map tp |> Array.concat)
      }
    | Shape.FSharpRecord (:? ShapeFSharpRecord<'T> as shape) ->
      let fieldPrinters : ('T -> File[]) [] =
        shape.Fields |> Array.map (fun f -> mkMemberFinder f)

      fun (r:'T) ->
        fieldPrinters |> Seq.map (fun fp -> fp r) |> Array.concat
    | Shape.FSharpUnion (:? ShapeFSharpUnion<'T> as shape) ->
      let cases : ShapeFSharpUnionCase<'T> [] = shape.UnionCases // all union cases
      let mkUnionCasePrinter (case : ShapeFSharpUnionCase<'T>) =
        let readFile =
          if isFile case then
            readFile |> Some
          else None

        let fieldPrinters = case.Fields |> Array.map mkMemberFinder
        fun (x: 'T) ->
          match readFile with
          | Some fn ->
            [|fn x case|]
          | None ->
            fieldPrinters
            |> Seq.map (fun fp -> fp x)
            |> Array.concat

      let casePrinters = cases |> Array.map mkUnionCasePrinter // generate printers for all union cases
      fun (u:'T) ->
        let tag : int = shape.GetTag u // get the underlying tag for the union case
        casePrinters.[tag] u
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
       member __.Visit (shape : ShapeMember<'DeclaringType, 'Field>) =
         let inFieldFinder = mkRequestGenerator<'Field>()
         inFieldFinder << shape.Get }

    let wrap(p : 'a -> string -> MultipartFormDataContent -> bool) =
      unbox<'T -> string -> MultipartFormDataContent -> bool> p

    let addFiles (a: 'v) (data: MultipartFormDataContent) =
      let finder =
        fileFinders.GetOrAdd(typeof<'v>, Func<Type, obj>(fun x -> mkFilesFinder<'v> () |> box))
        |> unbox<'v -> File[]>
      let files = finder a
      files |> Seq.iter (fun (name, stream) -> data.Add(new StreamContent(stream), name, name))

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
          |> Array.map (fun (prop, fp) -> fp x (getSnakeCaseName prop) data)
          |> Array.contains true
        else
          let json = toJson x
          data.Add(new ByteArrayContent(json), prop)
          addFiles x data
          true
    | Shape.FSharpOption s ->
      s.Element.Accept {
        new ITypeVisitor<'T -> string -> MultipartFormDataContent -> bool> with
          member __.Visit<'a> () =
            let tp = mkRequestGenerator<'a>()
            wrap(fun x prop data ->
              match x with
              | None -> false
              | Some t -> tp t prop data)
      }
    | Shape.FSharpList s ->
      s.Element.Accept {
        new ITypeVisitor<'T -> string -> MultipartFormDataContent -> bool> with
          member __.Visit<'a> () =
            fun x prop data ->
              let json = toJson x
              data.Add(new ByteArrayContent(json), prop)
              addFiles x data
              true
      }
    | Shape.Array s when s.Rank = 1 ->
      s.Element.Accept {
        new ITypeVisitor<'T -> string -> MultipartFormDataContent -> bool> with
          member __.Visit<'a> () =
            fun x prop data ->
              let json = toJson x
              data.Add(new ByteArrayContent(json), prop)
              addFiles x data
              true
        }
    | Shape.FSharpSet s ->
      s.Accept {
        new IFSharpSetVisitor<'T -> string -> MultipartFormDataContent -> bool> with
          member __.Visit<'a when 'a : comparison> () =
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
          if isFile case then
            readFile |> Some
          else None

        if isEnum then
          let name = getSnakeCaseName case.CaseInfo.Name
          fun _ (prop: string) (data: MultipartFormDataContent) ->
            data.Add(strf "%s" name, prop) $ true
        else
          fun (x: 'T) (prop: string) (data: MultipartFormDataContent) ->
            match readFile with
            | Some fn ->
              let (n, s) = fn x case
              data.Add(new StreamContent(s), prop, n) $ true
            | None ->
              let fieldPrinters = case.Fields |> Array.map mkGenerateInMember
              fieldPrinters
              |> Array.map (fun fp -> fp x prop data)
              |> Array.contains true

      let casePrinters = cases |> Array.map mkUnionCasePrinter // generate printers for all union cases
      fun (u:'T) ->
        let tag : int = shape.GetTag u // get the underlying tag for the union case
        casePrinters.[tag] u
    | _ ->
      fun _ _ _ -> false

  let mkBaseGenerator<'a when 'a :> IBotRequest> () =
    let fn = mkRequestGenerator<'a> ()
    fun (request: IBotRequest) -> fn (request :?> 'a) ""

  let makeRequestAsync config (request: IRequestBase<'a>) =
    async {
      let client = config.Client
      let url = getUrl config request.MethodName

      let serialize =
        multipartSerializers.GetOrAdd(
          request.GetType(),
          Func<Type, IBotRequest -> MultipartFormDataContent -> bool>(generateMultipartSerializer)
        )

      use content = new MultipartFormDataContent()
      let hasData = serialize request content

      let! result =
        if hasData then client.PostAsync(url, content) |> Async.AwaitTask
        else client.GetAsync(url) |> Async.AwaitTask

      if result.StatusCode = HttpStatusCode.OK then
        use! stream = result.Content.ReadAsStreamAsync() |> Async.AwaitTask
        return parseJsonStreamApiResponse<'a> stream
      else
        return Error { Description = "HTTP_ERROR"; ErrorCode = int result.StatusCode }
    }


  let makeJsonBodyRequestAsync config (request: IRequestBase<'a>) =
    async {
      let client = config.Client
      let url = getUrl config request.MethodName

      use ms = new MemoryStream()
      JsonSerializer.Serialize(ms, request, resolver)

      use content = new StreamContent(ms)
      let! result = client.PostAsync(url, content) |> Async.AwaitTask

      use! stream = result.Content.ReadAsStreamAsync() |> Async.AwaitTask
      return parseJsonStreamApiResponse<'a> stream
    }