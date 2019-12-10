module Funogram.Tools

open System
open System.Net.Http
open System.Reflection
open System.Runtime.CompilerServices
open System.Text
open Utf8Json
open Utf8Json.Resolvers

[<assembly:InternalsVisibleTo("Funogram.Tests")>]
do ()

open System.Collections.Concurrent
open System.Linq.Expressions
open Funogram.RequestsTypes
open Funogram.Resolvers
open Types
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
  sprintf "%s%s/%s" (config.TelegramServerUrl |> string) config.Token methodName

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
  
  Expression.Lambda<Func<IBotRequest, byte[]>>(call, [ sourceParam]).Compile()
  
let toJsonBotRequest (request: IBotRequest) =
  let toJson =
    serializers.GetOrAdd(
      request.GetType(),
      Func<Type, Func<IBotRequest, byte[]>>(generateSerializer))
  toJson.Invoke(request)  



let internal getChatIdString (chatId: Types.ChatId) = 
  match chatId with
  | Int v -> v |> string
  | String v -> v

let internal getChatIdStringOption (chatId: Types.ChatId option) = 
  chatId
  |> Option.map getChatIdString
  |> Option.defaultValue ""

let private isOption (t: Type) = 
  t.GetTypeInfo().IsGenericType 
  && t.GetGenericTypeDefinition() = typedefof<option<_>>

let internal (|SomeObj|_|) = 
  let ty = typedefof<option<_>>
  fun (a: obj) -> 
    let aty = a.GetType().GetTypeInfo()
    let v = aty.GetProperty("Value")
    if aty.IsGenericType && aty.GetGenericTypeDefinition() = ty then 
      if isNull (a) then None else Some(v.GetValue(a, [||]))
    else None

module Api2 =
  let makeRequestAsync config (request: 'b when 'b :> IRequestBase<'a>) =
    async {
      let client = config.Client
      let url = getUrl config request.MethodName
      
      let bytes = toJsonBotRequest request
      let result = new ByteArrayContent(bytes)
      result.Headers.Remove("Content-Type") |> ignore
      result.Headers.Add("Content-Type", "application/json")
                      
      let! result = 
        client.PostAsync(url, result)
        |> Async.AwaitTask
                         
      let! bytes = result.Content.ReadAsByteArrayAsync() |> Async.AwaitTask
      return parseJson<'a> bytes
    }

[<AbstractClass>]
type internal ApiUnused private () =

  static member private ConvertParameterValue(value: obj): HttpContent * string option = 
      let isPrimitive = value.GetType().IsPrimitive
      match value with
      | :? bool as value ->
        new StringContent(value.ToString().ToLower()) :> HttpContent, None
      | :? string as value -> new StringContent(value) :> HttpContent, None
      | :? DateTime as value -> new StringContent(getUnix value |> string) :> HttpContent, None
      | :? Types.FileToSend as value ->
        match value with
        | Types.Url x -> 
          (new StringContent(x.ToString()) :> HttpContent, None)
        | Types.FileId x -> (new StringContent(x) :> HttpContent, None)
        | Types.File(name, content) -> 
          (new StreamContent(content) :> HttpContent, Some name)
      | _ ->
        if isPrimitive then (new StringContent(value.ToString(), Encoding.UTF8) :> HttpContent, None)
        else new ByteArrayContent(toJson value) :> HttpContent, None
    
  static member private DowncastOptionObj = 
    let ty = typedefof<option<_>>
    fun (a: obj) -> 
      let aty = a.GetType().GetTypeInfo()
      let v = aty.GetProperty("Value")
      if aty.IsGenericType && aty.GetGenericTypeDefinition() = ty then 
        if isNull (a) then None else Some(v.GetValue(a, [||]))
      else None
    
  static member internal MakeRequestAsync<'a>(config: BotConfig, request: IRequestBase<'a>) = 
    async {
      let client = config.Client
      let url = getUrl config request.MethodName
      (*
      if paramValues |> Seq.exists (fun (_, b) -> (b :? Types.FileToSend)) then 
        use form = new MultipartFormDataContent()
        paramValues 
        |> Seq.iter (
          fun (name, value) -> 
            let content, fileName = Api.ConvertParameterValue(value)
                      
            if fileName.IsSome then form.Add (content, name, fileName.Value)
            else form.Add(content, name))
                        
        let! result = 
          client.PostAsync(url, form)
          |> Async.AwaitTask
                        
        let! bytes = result.Content.ReadAsByteArrayAsync() |> Async.AwaitTask
        return parseJson<'a> bytes
      else *)
      
      let bytes = toJson request
      let text = Encoding.UTF8.GetString(bytes)
      let result = new ByteArrayContent(bytes)
      result.Headers.Remove("Content-Type") |> ignore
      result.Headers.Add("Content-Type", "application/json")
                    
      let! result = 
        client.PostAsync(url, result)
        |> Async.AwaitTask
                       
      let! bytes = result.Content.ReadAsByteArrayAsync() |> Async.AwaitTask
      return parseJson<'a> bytes
    }