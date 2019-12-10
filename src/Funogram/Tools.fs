module Funogram.Tools

open System
open System.Net.Http
open System.Runtime.CompilerServices
open Utf8Json
open Utf8Json.Resolvers

[<assembly:InternalsVisibleTo("Funogram.Tests")>]
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
  sprintf "%s%s/%s" (config.ApiEndpointUrl |> string) config.Token methodName

let internal getUnix (date: DateTime) = 
  Convert.ToInt64(date.Subtract(DateTime(1970, 1, 1)).TotalSeconds)

let internal parseJson<'a> (data: byte[]) =
  match (JsonSerializer.Deserialize<Types.ApiResponse<'a>>(data, resolver)) with
  | x when x.Ok && x.Result.IsSome -> Ok x.Result.Value
  | x when x.Description.IsSome && x.ErrorCode.IsSome -> 
    Error { Description = x.Description.Value
            ErrorCode = x.ErrorCode.Value }
  | _ -> 
    Error { Description = "Unknown error"
            ErrorCode = -1 }

[<ReflectedDefinition>]
let toJson (o: 'a) = JsonSerializer.Serialize<'a>(o, resolver)

let private toJsonMethodInfo =
  System.Reflection.Assembly.GetExecutingAssembly()
    .GetType("Funogram.Tools").GetMethod("toJson")

// json request serializer
let private serializers = ConcurrentDictionary<Type, Func<IBotRequest, byte[]>>()
let private generateSerializer tp =
  let sourceParam = Expression.Parameter(typeof<IBotRequest>)
  let convert = Expression.Convert(sourceParam, tp)
  let method = toJsonMethodInfo.MakeGenericMethod(tp)
  let call = Expression.Call(method, convert)
  Expression.Lambda<Func<IBotRequest, byte[]>>(call, [sourceParam]).Compile()
  
let toJsonBotRequest (request: IBotRequest) =
  let toJson =
    serializers.GetOrAdd(
      request.GetType(),
      Func<Type, Func<IBotRequest, byte[]>>(generateSerializer))
  toJson.Invoke(request)  

module Api =
  type FileType = (string * Stream)
  
  let fileFinders = ConcurrentDictionary<Type, IBotRequest -> FileType[]>()
  
  let mkBaseFinderMethod =
    System.Reflection.Assembly.GetExecutingAssembly()
      .GetType("Funogram.Tools")
      .GetNestedType("Api")
      .GetMethod("mkBaseFinder")

  let generateFileFinder (tp: Type) =
    let method = mkBaseFinderMethod.MakeGenericMethod(tp)
    Expression.Lambda<Func<IBotRequest -> FileType[]>>(Expression.Call(method))
      .Compile().Invoke()
 
  let rec mkFindFiles<'T> () : 'T -> FileType[] =
    let mkFindInMember (shape : IShapeMember<'DeclaringType>) =
     shape.Accept { new IMemberVisitor<'DeclaringType, 'DeclaringType -> FileType[]> with
       member __.Visit (shape : ShapeMember<'DeclaringType, 'Field>) =
         let inFieldFinder = mkFindFiles<'Field>()
         inFieldFinder << shape.Get }

    let wrap(p : 'a -> FileType[]) = unbox<'T -> FileType[]> p
    match shapeof<'T> with
    | Shape.FSharpOption s ->
      s.Element.Accept {
        new ITypeVisitor<'T -> FileType[]> with
          member __.Visit<'a> () =
            let tp = mkFindFiles<'a>()
            wrap(function None -> [||] | Some t -> tp t)
      }
    | Shape.FSharpList s ->
      s.Element.Accept {
        new ITypeVisitor<'T -> FileType[]> with
          member __.Visit<'a> () =
            let tp = mkFindFiles<'a>()
            wrap(fun ts -> ts |> List.map tp |> Array.concat)
      }
    | Shape.Array s when s.Rank = 1 ->
      s.Element.Accept {
        new ITypeVisitor<'T -> FileType[]> with
          member __.Visit<'a> () =
            let tp = mkFindFiles<'a> ()
            wrap(fun ts -> ts |> Array.map tp |> Array.concat)
        }
        
    // TUPLE

    | Shape.FSharpSet s ->
      s.Accept {
        new IFSharpSetVisitor<'T -> FileType[]> with
          member __.Visit<'a when 'a : comparison> () =
            let tp = mkFindFiles<'a>()
            wrap(fun (s:Set<'a>) -> s |> Seq.map tp |> Array.concat)
     }
    | Shape.FSharpUnion (:? ShapeFSharpUnion<'T> as shape) ->
      let cases : ShapeFSharpUnionCase<'T> [] = shape.UnionCases // all union cases
      let mkUnionCasePrinter (case : ShapeFSharpUnionCase<'T>) =
        let fieldPrinters = case.Fields |> Array.map mkFindInMember
        fun (u:'T) ->
          fieldPrinters
          |> Seq.map (fun fp -> fp u) 
          |> Array.concat

      let casePrinters = cases |> Array.map mkUnionCasePrinter // generate printers for all union cases
      fun (u:'T) ->
        let tag : int = shape.GetTag u // get the underlying tag for the union case
        casePrinters.[tag] u
     | Shape.FSharpRecord (:? ShapeFSharpRecord<'T> as shape) ->
      let fieldPrinters : (string * ('T -> FileType[])) [] = 
        shape.Fields |> Array.map (fun f -> f.Label, mkFindInMember f)

      fun (r:'T) ->
        fieldPrinters
        |> Seq.map (fun (_, fp) -> fp r)
        |> Array.concat
    | _ ->
      fun _ -> [||]
  
  let mkBaseFinder<'a when 'a :> IBotRequest> () =
    let fn = mkFindFiles<'a> ()
    fun (request: IBotRequest) -> fn (request :?> 'a)
  
  let makeRequestAsync config (request: 'b when 'b :> IRequestBase<'a>) =
    async {
      let client = config.Client
      let url = getUrl config request.MethodName
      
      let findFiles =
        fileFinders.GetOrAdd(
          request.GetType(),
          Func<Type, IBotRequest -> FileType[]>(generateFileFinder)
        )

      let files = findFiles request
      let content =
        if files.Length > 0 then
          (*
          use form = new MultipartFormDataContent()
          files 
          |> Seq.iter (
            fun (name, value) -> 
              let content, fileName = Api.ConvertParameterValue(value)
                        
              if fileName.IsSome then form.Add (content, name, fileName.Value)
              else form.Add(content, name))
          *)
          failwith "Error"
        else
          let bytes = toJsonBotRequest request
          let result = new ByteArrayContent(bytes)
          result.Headers.Remove("Content-Type") |> ignore
          result.Headers.Add("Content-Type", "application/json")
          result :> HttpContent
                      
      let! result = 
        client.PostAsync(url, content)
        |> Async.AwaitTask
                         
      let! bytes = result.Content.ReadAsByteArrayAsync() |> Async.AwaitTask
      return parseJson<'a> bytes
    }