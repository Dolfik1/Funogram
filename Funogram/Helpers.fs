namespace Funogram

open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open System
open System.Net.Http
open System.Reflection
open System.Runtime.CompilerServices
open System.Text

[<assembly: InternalsVisibleTo("Funogram.Tests")>]
do()

module internal Helpers =
    open Types
    let getUrl token methodName = sprintf "https://api.telegram.org/bot%s/%s" token methodName

    let jsonOpts = 
        JsonSerializerSettings(
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = DefaultContractResolver(
                NamingStrategy = SnakeCaseNamingStrategy()),
            Converters = [| OptionConverter(); DuConverter() |],
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor)

    let parseJson<'a> str = 
        match (JsonConvert.DeserializeObject<Types.ApiResponse<'a>>(str, jsonOpts)) with
        | x when x.Ok && x.Result.IsSome -> Ok x.Result.Value
        | x when x.Description.IsSome && x.ErrorCode.IsSome -> 
            Error { Description = x.Description.Value; ErrorCode = x.ErrorCode.Value }
        | _ -> Error { Description = "Unknown error"; ErrorCode = -1 }

    let serializeObject (o: 'a) = JsonConvert.SerializeObject(o, jsonOpts)

    let parseModeName parseMode = 
        match parseMode with
        | None -> None
        | _ -> match parseMode.Value with
                | HTML -> Some "HTML"
                | Markdown -> Some "Markdown"

    let getChatIdString (chatId: Types.ChatId) =
        match chatId with
        | Int v -> v |> string
        | Long v -> v |> string
        | String v -> v

    let getChatIdStringOption (chatId: Types.ChatId option) = chatId |> Option.map getChatIdString |> Option.defaultValue ""

    let isOption (t: Type) = t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition() = typedefof<option<_>>

    let (|SomeObj|_|) =
      let ty = typedefof<option<_>>
      fun (a:obj) ->
        let aty = a.GetType().GetTypeInfo()
        let v = aty.GetProperty("Value")
        if aty.IsGenericType && aty.GetGenericTypeDefinition() = ty then
          if isNull(a) then None
          else Some(v.GetValue(a, [| |]))
        else None
       
[<AbstractClass>]
type Api private() =
    static member private Client = new HttpClient()
    static member private ConvertParameterValue (value: obj): (HttpContent * string option) = 
        let typeInfo = value.GetType().GetTypeInfo()

        if value :? bool then
            (new StringContent(value.ToString().ToLower()) :> HttpContent, None)
        elif typeInfo.IsPrimitive then
            (new StringContent(value.ToString(), Encoding.UTF8) :> HttpContent, None)
        elif (value :? Types.FileToSend) then
            let vl = value :?> Types.FileToSend
            match vl with
            | Types.Url x -> (new StringContent(x.ToString()) :> HttpContent, None)
            | Types.FileId x -> (new StringContent(x) :> HttpContent, None)
            | Types.File (name, content) -> (new StreamContent(content) :> HttpContent, Some name)
        else
            (new StringContent(Helpers.serializeObject value) :> HttpContent, None)
    
    static member private DowncastOptionObj = 
        let ty = typedefof<option<_>>
        fun (a:obj) ->
            let aty = a.GetType().GetTypeInfo()
            let v = aty.GetProperty("Value")
            if aty.IsGenericType && aty.GetGenericTypeDefinition() = ty then
              if isNull(a) then None
              else Some(v.GetValue(a, [| |]))
            else None

    static member internal MakeRequestAsync<'a> 
        (
            token: string,
            methodName: string,
            ?param: (string * obj) list
        ) = 

        async {
            
            let url = Helpers.getUrl token methodName

            if param.IsNone || param.Value.Length = 0 then
                return Api.Client.GetStringAsync(url) 
                        |> Async.AwaitTask 
                        |> Async.RunSynchronously 
                        |> Helpers.parseJson<'a>
            else
                let paramValues = param.Value |> List.choose (fun (key, value) -> 
                    match value with 
                    | null -> None
                    | Helpers.SomeObj(o) -> 
                        Some (key, o)
                    | _ -> 
                        if Helpers.isOption (value.GetType()) then None
                        else Some (key, value))

                if paramValues |> Seq.exists (fun (a, b) -> (b :? Types.FileToSend)) then
                    use form = new MultipartFormDataContent()
                    paramValues |> Seq.iter (fun (name, value) ->  
                        let content, fileName = Api.ConvertParameterValue(value)
                        if fileName.IsSome then
                            form.Add(content, name, fileName.Value)
                        else
                            form.Add(content, name)
                    )
                                                
                    let result = Api.Client.PostAsync(url, form)
                                |> Async.AwaitTask 
                                |> Async.RunSynchronously
                    return Helpers.parseJson<'a> (result.Content.ReadAsStringAsync() |> Async.AwaitTask |> Async.RunSynchronously)
                else
                    let json = Helpers.serializeObject (paramValues |> dict)
                    let result = new StringContent(json, Encoding.UTF8, "application/json")

                    let result = Api.Client.PostAsync(url, result)
                                |> Async.AwaitTask 
                                |> Async.RunSynchronously
                    return Helpers.parseJson<'a> (result.Content.ReadAsStringAsync() |> Async.AwaitTask |> Async.RunSynchronously)
                    
        }