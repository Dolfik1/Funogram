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
    let getUrl token methodName = sprintf "https://api.telegram.org/bot%s/%s" token methodName

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
            Error { Description = x.Description.Value; ErrorCode = x.ErrorCode.Value }
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
        (   /// Bot token
            token: string, 
            // 
            chatId: Types.ChatId, 
            // 
            text: string,
            // 
            parseMode: Types.ParseMode option, 
            // 
            disableWebPagePreview: bool option,
            // 
            disableNotification: bool option,
            // 
            replyToMessageId: int64 option,
            // 
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

    /// Use this method to forward messages of any kind. On success, the sent Message is returned.
    static member ForwardMessage 
        (
            /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            chatId: Types.ChatId,
            /// Unique identifier for the chat where the original message was sent (or channel username in the format @channelusername)
            fromChatId: Types.ChatId,
            /// Unique identifier for the chat where the original message was sent (or channel username in the format @channelusername)
            messageId: int,
            /// Sends the message silently. iOS users will not receive a notification, Android users will receive a notification with no sound.
            ?disableNotification: bool
        ) = Telegram.ForwardMessageBaseAsync(token, chatId, fromChatId, messageId, disableNotification) |> Async.RunSynchronously

    /// Use this method to forward messages of any kind. On success, the sent Message is returned.
    static member ForwardMessageAsync 
        (
            /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            chatId: Types.ChatId,
            /// Unique identifier for the chat where the original message was sent (or channel username in the format @channelusername)
            fromChatId: Types.ChatId,
            /// Unique identifier for the chat where the original message was sent (or channel username in the format @channelusername)
            messageId: int,
            /// Sends the message silently. iOS users will not receive a notification, Android users will receive a notification with no sound.
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
                [ "user_id", userId.ToString()
                  "offset", Helpers.toString offset
                  "limit", Helpers.toString limit ])

    /// Use this method to get a list of profile pictures for a user. Returns a UserProfilePhotos object.
    static member GetUserProfilePhotos
        (   /// Bot token
            token: string,
            /// Unique identifier of the target user
            userId: int,
            /// Sequential number of the first photo to be returned. By default, all photos are returned.
            offset: int option,
            /// Limits the number of photos to be retrieved. Values between 1—100 are accepted. Defaults to 100.
            limit: int option
        ) = Telegram.GetUserProfilePhotosBaseAsync(token, userId, offset, limit) |> Async.RunSynchronously
        
    // Use this method to get a list of profile pictures for a user. Returns a UserProfilePhotos object.
    static member GetUserProfilePhotosAsync
        (   /// Bot token
            token: string,
            /// Unique identifier of the target user
            userId: int,
            /// Sequential number of the first photo to be returned. By default, all photos are returned.
            offset: int option,
            /// Limits the number of photos to be retrieved. Values between 1—100 are accepted. Defaults to 100.
            limit: int option
        ) = Telegram.GetUserProfilePhotosBaseAsync(token, userId, offset, limit)


    /// Use this method to get basic info about a file and prepare it for downloading. For the moment, bots can download files of up to 20MB in size. On success, a File object is returned. The file can then be downloaded via the link https://api.telegram.org/file/bot<token>/<file_path>, where <file_path> is taken from the response. It is guaranteed that the link will be valid for at least 1 hour. When the link expires, a new one can be requested by calling getFile again.
    static member internal GetFileAsync 
        (   /// Bot token
            token: string,
            /// File identifier to get info about
            fileId: string
        ) = Telegram.MakeRequestAsync<Types.File> (token, 
                "getFile",
                [ "user_id", fileId ])

    /// Use this method to get basic info about a file and prepare it for downloading. For the moment, bots can download files of up to 20MB in size. On success, a File object is returned. The file can then be downloaded via the link https://api.telegram.org/file/bot<token>/<file_path>, where <file_path> is taken from the response. It is guaranteed that the link will be valid for at least 1 hour. When the link expires, a new one can be requested by calling getFile again.
    static member internal GetFile 
        (   /// Bot token
            token: string,
            /// File identifier to get info about
            fileId: string
        ) = Telegram.GetFileAsync(token, fileId) |> Async.RunSynchronously

    /// Use this method to kick a user from a group or a supergroup. In the case of supergroups, the user will not be able to return to the group on their own using invite links, etc., unless unbanned first. The bot must be an administrator in the group for this to work. Returns True on success.
    /// Note: This will method only work if the ‘All Members Are Admins’ setting is off in the target group. Otherwise members may only be removed by the group's creator or by the member that added them.
    static member KickChatMemberAsync
        (   ///  Bot token
            token: string,
            /// Unique identifier for the target group or username of the target supergroup (in the format @supergroupusername)
            chatId: Types.ChatId,
            /// Unique identifier of the target user
            userId: int
        ) = Telegram.MakeRequestAsync<bool>(token, 
                "kickChatMember",
                [ "chat_id", Helpers.getChatIdString chatId
                  "user_id", userId.ToString() ])

    /// Use this method to kick a user from a group or a supergroup. In the case of supergroups, the user will not be able to return to the group on their own using invite links, etc., unless unbanned first. The bot must be an administrator in the group for this to work. Returns True on success. Note: This will method only work if the ‘All Members Are Admins’ setting is off in the target group. Otherwise members may only be removed by the group's creator or by the member that added them.
    static member KickChatMember
        (   /// Bot token
            token: string,
            /// Unique identifier for the target group or username of the target supergroup (in the format @supergroupusername)
            chatId: Types.ChatId,
            /// Unique identifier of the target user
            userId: int
        ) = Telegram.KickChatMemberAsync(token, chatId, userId) |> Async.RunSynchronously
    
    /// Use this method to unban a previously kicked user in a supergroup or channel. The user will not return to the group or channel automatically, but will be able to join via link, etc. The bot must be an administrator for this to work. Returns True on success.
    static member UnbanChatMemberAsync
        (   ///  Bot token
            token: string,
            /// Unique identifier for the target group or username of the target supergroup (in the format @supergroupusername)
            chatId: Types.ChatId,
            /// Unique identifier of the target user
            userId: int
        ) = Telegram.MakeRequestAsync<bool>(token, 
                "unbanChatMember",
                [ "chat_id", Helpers.getChatIdString chatId
                  "user_id", userId.ToString() ])

    /// Use this method to unban a previously kicked user in a supergroup or channel. The user will not return to the group or channel automatically, but will be able to join via link, etc. The bot must be an administrator for this to work. Returns True on success.
    static member UnbanChatMember
        (   /// Bot token
            token: string,
            /// Unique identifier for the target group or username of the target supergroup (in the format @supergroupusername)
            chatId: Types.ChatId,
            /// Unique identifier of the target user
            userId: int
        ) = Telegram.UnbanChatMember(token, chatId, userId)

    /// Use this method for your bot to leave a group, supergroup or channel. Returns True on success.
    static member LeaveChatAsync
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target supergroup or channel (in the format @channelusername)
            chatId: Types.ChatId
        ) = Telegram.MakeRequestAsync<bool>(token, "leaveChat", [ "chat_id", Helpers.getChatIdString chatId ])

    /// Use this method for your bot to leave a group, supergroup or channel. Returns True on success.
    static member LeaveChat
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target supergroup or channel (in the format @channelusername)
            chatId: Types.ChatId
        ) = Telegram.LeaveChatAsync(token, chatId) |> Async.RunSynchronously

    /// Use this method to get up to date information about the chat (current name of the user for one-on-one conversations, current username of a user, group or channel, etc.). Returns a Chat object on success.
    static member GetChatAsync
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target supergroup or channel (in the format @channelusername)
            chatId: Types.ChatId
        ) = Telegram.MakeRequestAsync<Types.Chat>(token, "getChat", [ "chat_id", Helpers.getChatIdString chatId ])

    /// Use this method to get up to date information about the chat (current name of the user for one-on-one conversations, current username of a user, group or channel, etc.). Returns a Chat object on success.
    static member GetChat
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target supergroup or channel (in the format @channelusername)
            chatId: Types.ChatId
        ) = Telegram.GetChatAsync(token, chatId) |> Async.RunSynchronously

    /// Use this method to get a list of administrators in a chat. On success, returns an Array of ChatMember objects that contains information about all chat administrators except other bots. If the chat is a group or a supergroup and no administrators were appointed, only the creator will be returned.
    static member GetChatAdministratorsAsync
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target supergroup or channel (in the format @channelusername)
            chatId: Types.ChatId
        ) = Telegram.MakeRequestAsync<seq<Types.ChatMember>>(token, "getChatAdministrators", [ "chat_id", Helpers.getChatIdString chatId ])

    /// Use this method to get a list of administrators in a chat. On success, returns an Array of ChatMember objects that contains information about all chat administrators except other bots. If the chat is a group or a supergroup and no administrators were appointed, only the creator will be returned.
    static member GetChatAdministrators
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target supergroup or channel (in the format @channelusername)
            chatId: Types.ChatId
        ) = Telegram.GetChatAdministratorsAsync(token, chatId) |> Async.RunSynchronously
    
    /// Use this method to get the number of members in a chat. Returns Int on success.
    static member GetChatMembersCountAsync
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target supergroup or channel (in the format @channelusername)
            chatId: Types.ChatId
        ) = Telegram.MakeRequestAsync<int>(token, "getChatMembersCount", [ "chat_id", Helpers.getChatIdString chatId ])
    
    /// Use this method to get the number of members in a chat. Returns Int on success.
    static member GetChatMembersCount
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target supergroup or channel (in the format @channelusername)
            chatId: Types.ChatId
        ) = Telegram.MakeRequestAsync<int>(token, "getChatMembersCount", [ "chat_id", Helpers.getChatIdString chatId ]) |> Async.RunSynchronously

    /// Use this method to get information about a member of a chat. Returns a ChatMember object on success.
    static member GetChatMemberAsync
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target supergroup or channel (in the format @channelusername)
            chatId: Types.ChatId,
            /// Unique identifier of the target user
            userId: int
        ) = Telegram.MakeRequestAsync<Types.ChatMember>(token, 
                "getChatMember",
                [ "chat_id", Helpers.getChatIdString chatId
                  "user_id", userId.ToString() ])

    /// Use this method to get information about a member of a chat. Returns a ChatMember object on success.
    static member GetChatMember
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target supergroup or channel (in the format @channelusername)
            chatId: Types.ChatId,
            /// Unique identifier of the target user
            userId: int
        ) = Telegram.GetChatMemberAsync(token, chatId, userId) |> Async.RunSynchronously

    