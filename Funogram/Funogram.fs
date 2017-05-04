namespace Funogram

open FunHttp
open FunHttp.HttpRequestHeaders
open Newtonsoft.Json
open System.Collections.Generic
open System.Runtime.CompilerServices

[<assembly: InternalsVisibleTo("Funogram.Tests")>]
do()

module internal Helpers =
    let getUrl token methodName = "https://api.telegram.org/bot" + token + "/" + methodName
   
    let jsonOpts = 
        JsonSerializerSettings(ContractResolver = JsonHelpers.SnakeCaseContractResolver())
    let parseJson<'a> str = JsonConvert.DeserializeObject<'a>(str, jsonOpts)
    let serializeObject (o: 'a) = JsonConvert.SerializeObject(o, jsonOpts)
    let serializeOptionObject (o: 'a option) = 
        match o with
            | None -> ""
            | _ -> serializeObject o

        
    let inline toString arg = arg |> Option.map string |> Option.defaultValue ""
    let parseModeName parseMode = 
        match parseMode with
        | None -> ""
        | _ -> match parseMode.Value with
                | Types.ParseMode.HTML -> "HTML"
                | Types.ParseMode.Markdown -> "Markdown"

    let getFormValues (param: (string * string) list option) =
        let p = (param |> Option.defaultValue []) 
                |> List.filter (fun (x, y) -> System.String.IsNullOrWhiteSpace(y) |> not)
        match p with
        | [] -> ""
        | _ -> (param.Value 
            |> Seq.map (fun (x, y) -> sprintf "%s=%s" x y) 
            |> String.concat "&")
    let getUrlArgs (param: (string * string) list option) =
        "?" + (getFormValues param)

[<AbstractClass>]
type Telegram private() =
    static member internal MakeRequestAsync<'a> 
        (
            token: string,
            methodName: string,
            ?param: (string * string) list,
            ?httpMethod: string
        ) = 
        async {
            let method = httpMethod |> Option.defaultWith (fun x -> HttpMethod.Get)
            return 
                match method with
                | "GET" -> Http.RequestString ((Helpers.getUrl token methodName) + (Helpers.getUrlArgs param)) 
                                    |> Helpers.parseJson<'a>
                | "POST" -> Http.RequestString
                                ((Helpers.getUrl token methodName), 
                                httpMethod = "POST",
                                headers = [ ContentType HttpContentTypes.FormValues ],
                                body = TextRequest ((Helpers.getFormValues param)))
                            |> Helpers.parseJson<'a>
                | _ -> failwith "Unsupported request"
        }

    /// Receive incoming updates using long polling
    static member internal GetUpdatesBaseAsync (token: string, offset: int64 option, limit: int option, timeout: int option) =
        Telegram.MakeRequestAsync<Types.Update>
            ( token, 
              "getUpdates",
              [ "offset", Helpers.toString offset
                "limit", Helpers.toString limit
                "timeout", Helpers.toString timeout ])

    /// Receive incoming updates using long polling
    static member GetUpdatesAsync token offset limit timeout =
        Telegram.GetUpdatesBaseAsync(token, offset, limit, timeout)

    /// Receive incoming updates using long polling
    static member GetUpdates token offset limit timeout =
        Telegram.GetUpdatesBaseAsync(token, offset, limit, timeout) |> Async.RunSynchronously

    /// Returns basic information about the bot in form of a User object.
    static member GetMeAsync token =
        Telegram.MakeRequestAsync<Types.User>(token, "getMe")

    /// Returns basic information about the bot in form of a User object.        
    static member GetMe token =
        Telegram.GetMeAsync token |> Async.RunSynchronously

    static member SendMessageBaseAsync
        (
            token: string, 
            chatId: obj, 
            text: string,
            parseMode: Types.ParseMode option, 
            disableWebPagePreview: bool option,
            disableNotification: bool option,
            replyToMessageId: int64 option,
            replyMarkup: Types.Markup option
        ) =
        Telegram.MakeRequestAsync<Types.Message>
            ( token, 
              "sendMessage",
              [ "chat_id", chatId |> string
                "text", text
                "parse_mode", Helpers.parseModeName parseMode
                "disable_web_page_preview", Helpers.toString disableWebPagePreview
                "disable_notification", Helpers.toString disableNotification
                "reply_to_message_id", Helpers.toString replyToMessageId
                "reply_markup", Helpers.serializeOptionObject replyMarkup ])

    /// Use this method to send text messages. On success, the sent Message is returned
    static member SendMessageAsync
        (
            token: string, 
            chatId: string, 
            text: string,
            ?parseMode: Types.ParseMode, 
            ?disableWebPagePreview: bool,
            ?disableNotification: bool,
            ?replyToMessageId: int64,
            ?replyMarkup: Types.Markup
        ) = Telegram.SendMessageBaseAsync
                (token, chatId, text, parseMode, disableNotification, disableNotification, replyToMessageId, replyMarkup)

    /// Use this method to send text messages. On success, the sent Message is returned
    static member SendMessageAsync
            (
                token: string, 
                chatId: int, 
                text: string,
                ?parseMode: Types.ParseMode, 
                ?disableWebPagePreview: bool,
                ?disableNotification: bool,
                ?replyToMessageId: int64,
                ?replyMarkup: Types.Markup
            ) = Telegram.SendMessageBaseAsync
                    (token, chatId, text, parseMode, disableNotification, disableNotification, replyToMessageId, replyMarkup)

    