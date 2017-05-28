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

    let getChatIdStringOption (chatId: Types.ChatId option) = chatId |> Option.map getChatIdString |> Option.defaultValue ""

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

    static member internal AnswerCallbackQueryBaseAsync
        (
            token: string,
            callbackQueryId: string option,
            text: string option,
            showAlert: bool option,
            url: string option,
            cacheTime: int option
        ) = Telegram.MakeRequestAsync<bool>(token, 
                "answerCallbackQuery",
                [ "callback_query_id", Helpers.toString callbackQueryId
                  "text", Helpers.toString text
                  "show_alert", Helpers.toString showAlert
                  "url", Helpers.toString url
                  "cache_time", Helpers.toString cacheTime ])

    /// Use this method to send answers to callback queries sent from inline keyboards. The answer will be displayed to the user as a notification at the top of the chat screen or as an alert. On success, True is returned.
    /// Alternatively, the user can be redirected to the specified Game URL. For this option to work, you must first create a game for your bot via BotFather and accept the terms. Otherwise, you may use links like telegram.me/your_bot?start=XXXX that open your bot with a parameter.
    static member AnswerCallbackQueryAsync
        (   /// Bot token
            token: string,
            /// Unique identifier for the query to be answered
            ?callbackQueryId: string,
            /// Text of the notification. If not specified, nothing will be shown to the user, 0-200 characters
            ?text: string,
            /// If true, an alert will be shown by the client instead of a notification at the top of the chat screen. Defaults to false.
            ?showAlert: bool,
            /// URL that will be opened by the user's client. If you have created a Game and accepted the conditions via @Botfather, specify the URL that opens your game – note that this will only work if the query comes from a callback_game button.
            /// Otherwise, you may use links like telegram.me/your_bot?start=XXXX that open your bot with a parameter.
            ?url: string,
            /// The maximum amount of time in seconds that the result of the callback query may be cached client-side. Telegram apps will support caching starting in version 3.14. Defaults to 0.
            ?cacheTime: int
        ) = Telegram.AnswerCallbackQueryBaseAsync(token, callbackQueryId, text, showAlert, url, cacheTime)

    /// Use this method to send answers to callback queries sent from inline keyboards. The answer will be displayed to the user as a notification at the top of the chat screen or as an alert. On success, True is returned.
    /// Alternatively, the user can be redirected to the specified Game URL. For this option to work, you must first create a game for your bot via BotFather and accept the terms. Otherwise, you may use links like telegram.me/your_bot?start=XXXX that open your bot with a parameter.
    static member AnswerCallbackQuery
        (   /// Bot token
            token: string,
            /// Unique identifier for the query to be answered
            ?callbackQueryId: string,
            /// Text of the notification. If not specified, nothing will be shown to the user, 0-200 characters
            ?text: string,
            /// If true, an alert will be shown by the client instead of a notification at the top of the chat screen. Defaults to false.
            ?showAlert: bool,
            /// URL that will be opened by the user's client. If you have created a Game and accepted the conditions via @Botfather, specify the URL that opens your game – note that this will only work if the query comes from a callback_game button.
            /// Otherwise, you may use links like telegram.me/your_bot?start=XXXX that open your bot with a parameter.
            ?url: string,
            /// The maximum amount of time in seconds that the result of the callback query may be cached client-side. Telegram apps will support caching starting in version 3.14. Defaults to 0.
            ?cacheTime: int
        ) = Telegram.AnswerCallbackQueryBaseAsync(token, callbackQueryId, text, showAlert, url, cacheTime) |> Async.RunSynchronously

    static member internal EditMessageTextBaseAsync
        (
            token: string,
            chatId: Types.ChatId option,
            messageId: int option,
            inlineMessageId: string option,
            text: string,
            parseMode: Types.ParseMode option,
            disableWebPagePreview: bool option,
            replyMarkup: Types.InlineKeyboardMarkup option
        ) = Telegram.MakeRequestAsync<Types.EditMessageResult>(token,
                "editMessageText",
                [ "chat_id", Helpers.toString chatId
                  "message_id", Helpers.toString messageId
                  "inline_message_id", Helpers.toString inlineMessageId
                  "text", text
                  "parse_mode", Helpers.parseModeName parseMode
                  "disable_web_page_preview", Helpers.toString disableWebPagePreview
                  "reply_markup", Helpers.serializeOptionObject replyMarkup ])
    
    /// Use this method to edit text and game messages sent by the bot or via the bot (for inline bots). On success, if edited message is sent by the bot, the edited Message is returned, otherwise True is returned
    static member EditMessageTextAsync
        (
            /// Bot token
            token: string,
            /// New text of the message
            text: string,
            /// Required if inline_message_id is not specified. Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            ?chatId: Types.ChatId,
            /// Required if inline_message_id is not specified. Identifier of the sent message
            ?messageId: int,
            /// Required if chat_id and message_id are not specified. Identifier of the inline message
            ?inlineMessageId: string,
            /// Send Markdown or HTML, if you want Telegram apps to show bold, italic, fixed-width text or inline URLs in your bot's message.
            ?parseMode: Types.ParseMode,
            /// Disables link previews for links in this message
            ?disableWebPagePreview: bool,
            /// A JSON-serialized object for an inline keyboard.
            ?replyMarkup: Types.InlineKeyboardMarkup
        ) = Telegram.EditMessageTextBaseAsync(token, chatId, messageId, inlineMessageId, text, parseMode, disableWebPagePreview, replyMarkup)
    
    /// Use this method to edit text and game messages sent by the bot or via the bot (for inline bots). On success, if edited message is sent by the bot, the edited Message is returned, otherwise True is returned
    static member EditMessageText
        (
            /// Bot token
            token: string,
            /// New text of the message
            text: string,
            /// Required if inline_message_id is not specified. Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            ?chatId: Types.ChatId,
            /// Required if inline_message_id is not specified. Identifier of the sent message
            ?messageId: int,
            /// Required if chat_id and message_id are not specified. Identifier of the inline message
            ?inlineMessageId: string,
            /// Send Markdown or HTML, if you want Telegram apps to show bold, italic, fixed-width text or inline URLs in your bot's message.
            ?parseMode: Types.ParseMode,
            /// Disables link previews for links in this message
            ?disableWebPagePreview: bool,
            /// A JSON-serialized object for an inline keyboard.
            ?replyMarkup: Types.InlineKeyboardMarkup
        ) = Telegram.EditMessageTextBaseAsync(token, chatId, messageId, inlineMessageId, text, parseMode, disableWebPagePreview, replyMarkup) |> Async.RunSynchronously

    static member internal EditMessageCaptionBaseAsync
        (
            token: string,
            chatId: Types.ChatId option,
            messageId: int option,
            inlineMessageId: string option,
            caption: string option,
            replyMarkup: Types.InlineKeyboardMarkup option
        ) = Telegram.MakeRequestAsync<Types.EditMessageResult>(token,
                "editMessageCaption",
                [ "chat_id", Helpers.getChatIdStringOption chatId
                  "message_id", Helpers.toString messageId
                  "inline_message_id", Helpers.toString inlineMessageId
                  "caption", Helpers.toString caption
                  "reply_markup", Helpers.serializeOptionObject replyMarkup ])
    
    /// Use this method to edit captions of messages sent by the bot or via the bot (for inline bots). On success, if edited message is sent by the bot, the edited Message is returned, otherwise True is returned
    static member EditMessageCaptionAsync
        (   /// Bot token
            token: string,
            /// Required if inline_message_id is not specified. Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            ?chatId: Types.ChatId,
            /// Required if inline_message_id is not specified. Identifier of the sent message
            ?messageId: int,
            /// Required if chat_id and message_id are not specified. Identifier of the inline message
            ?inlineMessageId: string,
            /// New caption of the message
            ?caption: string,
            /// A JSON-serialized object for an inline keyboard.
            ?replyMarkup: Types.InlineKeyboardMarkup
        ) = Telegram.EditMessageCaptionBaseAsync(token, chatId, messageId, inlineMessageId, caption, replyMarkup)
    
    /// Use this method to edit captions of messages sent by the bot or via the bot (for inline bots). On success, if edited message is sent by the bot, the edited Message is returned, otherwise True is returned
    static member EditMessageCaption
        (   /// Bot token
            token: string,
            /// Required if inline_message_id is not specified. Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            ?chatId: Types.ChatId,
            /// Required if inline_message_id is not specified. Identifier of the sent message
            ?messageId: int,
            /// Required if chat_id and message_id are not specified. Identifier of the inline message
            ?inlineMessageId: string,
            /// New caption of the message
            ?caption: string,
            /// A JSON-serialized object for an inline keyboard.
            ?replyMarkup: Types.InlineKeyboardMarkup
        ) = Telegram.EditMessageCaptionBaseAsync(token, chatId, messageId, inlineMessageId, caption, replyMarkup) |> Async.RunSynchronously
    
    static member internal EditMessageReplyMarkupBaseAsync
        (
            token: string,
            chatId: Types.ChatId option,
            messageId: int option,
            inlineMessageId: string option,
            replyMarkup: Types.InlineKeyboardMarkup option
        ) = Telegram.MakeRequestAsync<Types.EditMessageResult>(token,
                "editMessageReplyMarkup",
                [ "chat_id", Helpers.getChatIdStringOption chatId
                  "message_id", Helpers.toString messageId
                  "inline_message_id", Helpers.toString inlineMessageId
                  "reply_markup", Helpers.serializeOptionObject replyMarkup ])

    /// Use this method to edit only the reply markup of messages sent by the bot or via the bot (for inline bots). On success, if edited message is sent by the bot, the edited Message is returned, otherwise True is returned.
    static member EditMessageReplyMarkupAsync
        (   /// Bot token
            token: string,
            /// Required if inline_message_id is not specified. Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            ?chatId: Types.ChatId,
            /// Required if inline_message_id is not specified. Identifier of the sent message
            ?messageId: int,
            /// Required if chat_id and message_id are not specified. Identifier of the inline message
            ?inlineMessageId: string,
            /// A JSON-serialized object for an inline keyboard.
            ?replyMarkup: Types.InlineKeyboardMarkup
        ) = Telegram.EditMessageReplyMarkupBaseAsync(token, chatId, messageId, inlineMessageId, replyMarkup)
        
    /// Use this method to edit only the reply markup of messages sent by the bot or via the bot (for inline bots). On success, if edited message is sent by the bot, the edited Message is returned, otherwise True is returned.
    static member EditMessageReplyMarkup
        (   /// Bot token
            token: string,
            /// Required if inline_message_id is not specified. Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            ?chatId: Types.ChatId,
            /// Required if inline_message_id is not specified. Identifier of the sent message
            ?messageId: int,
            /// Required if chat_id and message_id are not specified. Identifier of the inline message
            ?inlineMessageId: string,
            /// A JSON-serialized object for an inline keyboard.
            ?replyMarkup: Types.InlineKeyboardMarkup
        ) = Telegram.EditMessageReplyMarkupBaseAsync(token, chatId, messageId, inlineMessageId, replyMarkup) |> Async.RunSynchronously    
    
    /// Use this method to delete a message. A message can only be deleted if it was sent less than 48 hours ago. Any such recently sent outgoing message may be deleted. Additionally, if the bot is an administrator in a group chat, it can delete any message. If the bot is an administrator in a supergroup, it can delete messages from any other user and service messages about people joining or leaving the group (other types of service messages may only be removed by the group creator). In channels, bots can only remove their own messages. Returns True on success. 
    static member DeleteMessageAsync
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            chatId: Types.ChatId,
            /// Identifier of the message to delete
            messageId: int
        ) = Telegram.MakeRequestAsync<Types.EditMessageResult>(token,
                "deleteMessage",
                [ "chatId", Helpers.getChatIdString chatId
                  "message_id", messageId.ToString() ])   

    /// Use this method to delete a message. A message can only be deleted if it was sent less than 48 hours ago. Any such recently sent outgoing message may be deleted. Additionally, if the bot is an administrator in a group chat, it can delete any message. If the bot is an administrator in a supergroup, it can delete messages from any other user and service messages about people joining or leaving the group (other types of service messages may only be removed by the group creator). In channels, bots can only remove their own messages. Returns True on success. 
    static member DeleteMessage
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            chatId: Types.ChatId,
            /// Identifier of the message to delete
            messageId: int
        ) = Telegram.DeleteMessageAsync(token, chatId, messageId) |> Async.RunSynchronously

        
    static member internal AnswerInlineQueryBaseAsync
        (
            token: string,
            inlineQueryId: string,
            results: seq<Types.InlineQueryResult>,
            cacheTime: int option,
            isPersonal: bool option,
            nextOffset: string option,
            switchPmText: string option,
            switchPmParameter: string option
        ) = Telegram.MakeRequestAsync<Types.EditMessageResult>(token,
                "answerInlineQuery",
                [ "inline_query_id", inlineQueryId
                  "results", Helpers.serializeObject results
                  "cache_time", Helpers.toString cacheTime
                  "is_personal", Helpers.toString isPersonal
                  "next_offset", Helpers.toString nextOffset
                  "switch_pm_text", Helpers.toString switchPmText
                  "switch_pm_parameter", Helpers.toString switchPmParameter ])
    
    /// Use this method to send answers to an inline query. On success, True is returned. No more than 50 results per query are allowed.
    static member AnswerInlineQueryAsync
        (   /// Bot token
            token: string,
            /// Unique identifier for the answered query
            inlineQueryId: string,
            /// A JSON-serialized array of results for the inline query
            results: seq<Types.InlineQueryResult>,
            /// The maximum amount of time in seconds that the result of the inline query may be cached on the server. Defaults to 300.
            ?cacheTime: int,
            /// Pass True, if results may be cached on the server side only for the user that sent the query. By default, results may be returned to any user who sends the same query
            ?isPersonal: bool,
            /// Pass the offset that a client should send in the next query with the same text to receive more results. Pass an empty string if there are no more results or if you don‘t support pagination. Offset length can’t exceed 64 bytes.
            ?nextOffset: string,
            /// If passed, clients will display a button with specified text that switches the user to a private chat with the bot and sends the bot a start message with the parameter switch_pm_parameter
            ?switchPmText: string,
            /// Deep-linking parameter for the /start message sent to the bot when user presses the switch button. 1-64 characters, only A-Z, a-z, 0-9, _ and - are allowed.
            ?switchPmParameter: string
        ) = Telegram.AnswerInlineQueryBaseAsync(token, inlineQueryId, results, cacheTime, isPersonal, nextOffset, switchPmText, switchPmParameter)
    
    /// Use this method to send answers to an inline query. On success, True is returned. No more than 50 results per query are allowed.
    static member AnswerInlineQuery
        (   /// Bot token
            token: string,
            /// Unique identifier for the answered query
            inlineQueryId: string,
            /// A JSON-serialized array of results for the inline query
            results: seq<Types.InlineQueryResult>,
            /// The maximum amount of time in seconds that the result of the inline query may be cached on the server. Defaults to 300.
            ?cacheTime: int,
            /// Pass True, if results may be cached on the server side only for the user that sent the query. By default, results may be returned to any user who sends the same query
            ?isPersonal: bool,
            /// Pass the offset that a client should send in the next query with the same text to receive more results. Pass an empty string if there are no more results or if you don‘t support pagination. Offset length can’t exceed 64 bytes.
            ?nextOffset: string,
            /// If passed, clients will display a button with specified text that switches the user to a private chat with the bot and sends the bot a start message with the parameter switch_pm_parameter
            ?switchPmText: string,
            /// Deep-linking parameter for the /start message sent to the bot when user presses the switch button. 1-64 characters, only A-Z, a-z, 0-9, _ and - are allowed.
            ?switchPmParameter: string
        ) = Telegram.AnswerInlineQueryBaseAsync(token, inlineQueryId, results, cacheTime, isPersonal, nextOffset, switchPmText, switchPmParameter) |> Async.RunSynchronously

    