namespace Funogram

open FunHttp
open FunHttp.HttpRequestHeaders
open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open System.Runtime.CompilerServices

[<assembly: InternalsVisibleTo("Funogram.Tests")>]
do()

module internal Helpers =
    open Types


    let getUrl token methodName = "https://api.telegram.org/bot" + token + "/" + methodName

    let jsonOpts = 
        JsonSerializerSettings(
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = DefaultContractResolver(
                NamingStrategy = SnakeCaseNamingStrategy()),
            Converters = [| OptionConverter() |],
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor)

    let parseJson<'a> str = 
        match (JsonConvert.DeserializeObject<Types.ApiResponse<'a>>(str, jsonOpts)) with
        | x when x.Ok && x.Result.IsSome -> Ok x.Result.Value
        | x when x.Description.IsSome && x.ErrorCode.IsSome -> 
            Error { Description = "Unknown error"; ErrorCode = -1 }
        | _ -> Error { Description = "Unknown error"; ErrorCode = -1 }

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
                | HTML -> "HTML"
                | Markdown -> "Markdown"

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

    let getChatIdString (chatId: Types.ChatId) =
        match chatId with
        | ChatIdInt v -> v |> string
        | ChatIdLong v -> v |> string
        | ChatIdString v -> v

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
        Telegram.MakeRequestAsync<seq<Types.Update>>
            ( token, 
              "getUpdates",
              [ "offset", Helpers.toString offset
                "limit", Helpers.toString limit
                "timeout", Helpers.toString timeout ])

    /// Receive incoming updates using long polling
    static member GetUpdatesAsync (token: string, ?offset: int64, ?limit: int, ?timeout: int) =
        Telegram.GetUpdatesBaseAsync(token, offset, limit, timeout)

    /// Receive incoming updates using long polling
    static member GetUpdates (token: string, ?offset: int64, ?limit: int, ?timeout: int) =
        Telegram.GetUpdatesBaseAsync(token, offset, limit, timeout) |> Async.RunSynchronously

    /// Returns basic information about the bot in form of a User object.
    static member GetMeAsync token =
        Telegram.MakeRequestAsync<Types.User>(token, "getMe")

    /// Returns basic information about the bot in form of a User object.        
    static member GetMe token =
        Telegram.GetMeAsync token |> Async.RunSynchronously

    static member internal SendMessageBaseAsync
        (
            token: string, 
            chatId: Types.ChatId, 
            text: string,
            parseMode: Types.ParseMode option, 
            disableWebPagePreview: bool option,
            disableNotification: bool option,
            replyToMessageId: int64 option,
            replyMarkup: Types.Markup option
        ) =
        Telegram.MakeRequestAsync<Types.Message>
            (token, 
              "sendMessage",
              [ "chat_id", Helpers.getChatIdString chatId
                "text", text
                "parse_mode", Helpers.parseModeName parseMode
                "disable_web_page_preview", Helpers.toString disableWebPagePreview
                "disable_notification", Helpers.toString disableNotification
                "reply_to_message_id", Helpers.toString replyToMessageId
                "reply_markup", Helpers.serializeOptionObject replyMarkup ])

    /// Use this method to send text messages. On success, the sent Message is returned
    static member SendMessage
        (
            token: string, 
            chatId: Types.ChatId, 
            text: string,
            ?parseMode: Types.ParseMode, 
            ?disableWebPagePreview: bool,
            ?disableNotification: bool,
            ?replyToMessageId: int64,
            ?replyMarkup: Types.Markup
        ) = Telegram.SendMessageBaseAsync
                (token, chatId, text, parseMode, disableWebPagePreview, disableNotification, replyToMessageId, replyMarkup) |> Async.RunSynchronously

    /// Use this method to send text messages. On success, the sent Message is returned
    static member SendMessageAsync
            (
                token: string, 
                chatId: Types.ChatId, 
                text: string,
                ?parseMode: Types.ParseMode, 
                ?disableWebPagePreview: bool,
                ?disableNotification: bool,
                ?replyToMessageId: int64,
                ?replyMarkup: Types.Markup
            ) = Telegram.SendMessageBaseAsync
                    (token, chatId, text, parseMode, disableWebPagePreview, disableNotification, replyToMessageId, replyMarkup)

    static member internal ForwardMessageBaseAsync 
        (
            token: string,
            chatId: Types.ChatId,
            fromChatId: Types.ChatId,
            messageId: int,
            disableNotification: bool option
        ) = Telegram.MakeRequestAsync<Types.Message> (token, 
                "forwardMessage",
                [ "chat_id", Helpers.getChatIdString chatId
                  "from_chat_id", Helpers.getChatIdString fromChatId
                  "disable_notification", Helpers.toString disableNotification
                  "message_id", messageId.ToString() ])

    // Use this method to forward messages of any kind. On success, the sent Message is returned.
    static member ForwardMessage 
        (
            // Bot token
            token: string,
            // Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            chatId: Types.ChatId,
            // Unique identifier for the chat where the original message was sent (or channel username in the format @channelusername)
            fromChatId: Types.ChatId,
            // Unique identifier for the chat where the original message was sent (or channel username in the format @channelusername)
            messageId: int,
            // Sends the message silently. iOS users will not receive a notification, Android users will receive a notification with no sound.
            ?disableNotification: bool
        ) = Telegram.ForwardMessageBaseAsync(token, chatId, fromChatId, messageId, disableNotification) |> Async.RunSynchronously

    // Use this method to forward messages of any kind. On success, the sent Message is returned.
    static member ForwardMessageAsync 
        (
            // Bot token
            token: string,
            // Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            chatId: Types.ChatId,
            // Unique identifier for the chat where the original message was sent (or channel username in the format @channelusername)
            fromChatId: Types.ChatId,
            // Unique identifier for the chat where the original message was sent (or channel username in the format @channelusername)
            messageId: int,
            // Sends the message silently. iOS users will not receive a notification, Android users will receive a notification with no sound.
            ?disableNotification: bool
        ) = Telegram.ForwardMessageBaseAsync(token, chatId, fromChatId, messageId, disableNotification)

    
(*
    TODO send* methods
    sendPhoto
    sendAudio
    sendDocument
    sendSticker
    sendVideo
    sendVoice
    sendVideoNote
    sendLocation
    sendVenue
    sendContact
    sendChatAction
*)

    static member internal GetUserProfilePhotosBaseAsync 
        (
            token: string,
            userId: int,
            offset: int option,
            limit: int option
        ) = Telegram.MakeRequestAsync<Types.UserProfilePhotos> (token, 
                "getUserProfilePhotos",
                [ "userId", userId.ToString()
                  "offset", Helpers.toString offset
                  "limit", Helpers.toString limit ])

    // Use this method to get a list of profile pictures for a user. Returns a UserProfilePhotos object.
    static member GetUserProfilePhotos
        (   // Bot token
            token: string,
            // Unique identifier of the target user
            userId: int,
            // Sequential number of the first photo to be returned. By default, all photos are returned.
            offset: int option,
            // Limits the number of photos to be retrieved. Values between 1—100 are accepted. Defaults to 100.
            limit: int option
        ) = Telegram.GetUserProfilePhotosBaseAsync(token, userId, offset, limit) |> Async.RunSynchronously
        
    // Use this method to get a list of profile pictures for a user. Returns a UserProfilePhotos object.
    static member GetUserProfilePhotosAsync
        (   // Bot token
            token: string,
            // Unique identifier of the target user
            userId: int,
            // Sequential number of the first photo to be returned. By default, all photos are returned.
            offset: int option,
            // Limits the number of photos to be retrieved. Values between 1—100 are accepted. Defaults to 100.
            limit: int option
        ) = Telegram.GetUserProfilePhotosBaseAsync(token, userId, offset, limit)