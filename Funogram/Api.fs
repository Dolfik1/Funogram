namespace Funogram

open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open System.Runtime.CompilerServices

[<assembly: InternalsVisibleTo("Funogram.Tests")>]
do()

[<AbstractClass>]
type Telegram private() =
    /// Receive incoming updates using long polling
    static member GetUpdatesBaseAsync (token: string, offset: int64 option, limit: int option, timeout: int option) =
        Api.MakeRequestAsync<seq<Types.Update>>
            ( token, 
              "getUpdates",
              [ "offset", box offset
                "limit", box limit
                "timeout", box timeout ])

    /// Receive incoming updates using long polling
    static member GetUpdatesAsync (token: string, ?offset: int64, ?limit: int, ?timeout: int) =
        Telegram.GetUpdatesBaseAsync(token, offset, limit, timeout)

    /// Receive incoming updates using long polling
    static member GetUpdates (token: string, ?offset: int64, ?limit: int, ?timeout: int) =
        Telegram.GetUpdatesBaseAsync(token, offset, limit, timeout) |> Async.RunSynchronously

    /// Returns basic information about the bot in form of a User object.
    static member GetMeAsync token =
        Api.MakeRequestAsync<Types.User>(token, "getMe")

    /// Returns basic information about the bot in form of a User object.        
    static member GetMe token =
        Telegram.GetMeAsync token |> Async.RunSynchronously

    static member SendMessageBaseAsync
        (   
            token: string, 
            chatId: Types.ChatId, 
            text: string,
            parseMode: Types.ParseMode option, 
            disableWebPagePreview: bool option,
            disableNotification: bool option,
            replyToMessageId: int option,
            replyMarkup: Types.Markup option
        ) =
        Api.MakeRequestAsync<Types.Message>
            (token, 
              "sendMessage",
              [ "chat_id", box (Helpers.getChatIdString chatId)
                "text", box text
                "parse_mode", box (Helpers.parseModeName parseMode)
                "disable_web_page_preview", box disableWebPagePreview
                "disable_notification", box disableNotification
                "reply_to_message_id", box replyToMessageId
                "reply_markup", box replyMarkup ])

    /// Use this method to send text messages. On success, the sent Message is returned
    static member SendMessage
        (
            token: string, 
            chatId: Types.ChatId, 
            text: string,
            ?parseMode: Types.ParseMode, 
            ?disableWebPagePreview: bool,
            ?disableNotification: bool,
            ?replyToMessageId: int,
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
                ?replyToMessageId: int,
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
        ) = Api.MakeRequestAsync<Types.Message> (token, 
                "forwardMessage",
                [ "chat_id", box (Helpers.getChatIdString chatId)
                  "from_chat_id", box (Helpers.getChatIdString fromChatId)
                  "disable_notification", box disableNotification
                  "message_id", box messageId ])

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
        ) = Api.MakeRequestAsync<Types.UserProfilePhotos> (token, 
                "getUserProfilePhotos",
                [ "user_id", box userId
                  "offset", box offset
                  "limit", box limit ])

    /// Use this method to get a list of profile pictures for a user. Returns a UserProfilePhotos object.
    static member GetUserProfilePhotos
        (   /// Bot token
            token: string,
            /// Unique identifier of the target user
            userId: int,
            /// Sequential number of the first photo to be returned. By default, all photos are returned.
            ?offset: int,
            /// Limits the number of photos to be retrieved. Values between 1—100 are accepted. Defaults to 100.
            ?limit: int
        ) = Telegram.GetUserProfilePhotosBaseAsync(token, userId, offset, limit) |> Async.RunSynchronously
        
    // Use this method to get a list of profile pictures for a user. Returns a UserProfilePhotos object.
    static member GetUserProfilePhotosAsync
        (   /// Bot token
            token: string,
            /// Unique identifier of the target user
            userId: int,
            /// Sequential number of the first photo to be returned. By default, all photos are returned.
            ?offset: int,
            /// Limits the number of photos to be retrieved. Values between 1—100 are accepted. Defaults to 100.
            ?limit: int
        ) = Telegram.GetUserProfilePhotosBaseAsync(token, userId, offset, limit)


    /// Use this method to get basic info about a file and prepare it for downloading. For the moment, bots can download files of up to 20MB in size. On success, a File object is returned. The file can then be downloaded via the link https://api.telegram.org/file/bot<token>/<file_path>, where <file_path> is taken from the response. It is guaranteed that the link will be valid for at least 1 hour. When the link expires, a new one can be requested by calling getFile again.
    static member internal GetFileAsync 
        (   /// Bot token
            token: string,
            /// File identifier to get info about
            fileId: string
        ) = Api.MakeRequestAsync<Types.File> (token, 
                "getFile",
                [ "user_id", box fileId ])

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
        ) = Api.MakeRequestAsync<bool>(token, 
                "kickChatMember",
                [ "chat_id", box (Helpers.getChatIdString chatId)
                  "user_id", box userId ])

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
        ) = Api.MakeRequestAsync<bool>(token, 
                "unbanChatMember",
                [ "chat_id", box (Helpers.getChatIdString chatId)
                  "user_id", box userId ])

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
        ) = Api.MakeRequestAsync<bool>(token, "leaveChat", [ "chat_id", box (Helpers.getChatIdString chatId) ])

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
        ) = Api.MakeRequestAsync<Types.Chat>(token, "getChat", [ "chat_id", box (Helpers.getChatIdString chatId) ])

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
        ) = Api.MakeRequestAsync<seq<Types.ChatMember>>(token, "getChatAdministrators", [ "chat_id", box (Helpers.getChatIdString chatId) ])

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
        ) = Api.MakeRequestAsync<int>(token, "getChatMembersCount", [ "chat_id", box (Helpers.getChatIdString chatId) ])
    
    /// Use this method to get the number of members in a chat. Returns Int on success.
    static member GetChatMembersCount
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target supergroup or channel (in the format @channelusername)
            chatId: Types.ChatId
        ) = Api.MakeRequestAsync<int>(token, "getChatMembersCount", [ "chat_id", box (Helpers.getChatIdString chatId) ]) |> Async.RunSynchronously

    /// Use this method to get information about a member of a chat. Returns a ChatMember object on success.
    static member GetChatMemberAsync
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target supergroup or channel (in the format @channelusername)
            chatId: Types.ChatId,
            /// Unique identifier of the target user
            userId: int
        ) = Api.MakeRequestAsync<Types.ChatMember>(token, 
                "getChatMember",
                [ "chat_id", box (Helpers.getChatIdString chatId)
                  "user_id", box userId ])

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
        ) = Api.MakeRequestAsync<bool>(token, 
                "answerCallbackQuery",
                [ "callback_query_id", box callbackQueryId
                  "text", box text
                  "show_alert", box showAlert
                  "url", box url
                  "cache_time", box cacheTime ])

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
        ) = Api.MakeRequestAsync<Types.EditMessageResult>(token,
                "editMessageText",
                [ "chat_id", box chatId
                  "message_id", box messageId
                  "inline_message_id", box inlineMessageId
                  "text", box text
                  "parse_mode", box (Helpers.parseModeName parseMode)
                  "disable_web_page_preview", box disableWebPagePreview
                  "reply_markup", box replyMarkup ])
    
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
        ) = Api.MakeRequestAsync<Types.EditMessageResult>(token,
                "editMessageCaption",
                [ "chat_id", box (Helpers.getChatIdStringOption chatId)
                  "message_id", box messageId
                  "inline_message_id", box inlineMessageId
                  "caption", box caption
                  "reply_markup", box replyMarkup ])
    
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
        ) = Api.MakeRequestAsync<Types.EditMessageResult>(token,
                "editMessageReplyMarkup",
                [ "chat_id", box (Helpers.getChatIdStringOption chatId)
                  "message_id", box messageId
                  "inline_message_id", box inlineMessageId
                  "reply_markup", box replyMarkup ])

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
        ) = Api.MakeRequestAsync<Types.EditMessageResult>(token,
                "deleteMessage",
                [ "chatId", box (Helpers.getChatIdString chatId)
                  "message_id", box messageId ])   

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
        ) = Api.MakeRequestAsync<Types.EditMessageResult>(token,
                "answerInlineQuery",
                [ "inline_query_id", box inlineQueryId
                  "results", box results
                  "cache_time", box cacheTime
                  "is_personal", box isPersonal
                  "next_offset", box nextOffset
                  "switch_pm_text", box switchPmText
                  "switch_pm_parameter", box switchPmParameter ])
    
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

    static member internal SendInvoiceBaseAsync
        (
            token: string,
            chatId: int,
            title: string,
            payload: string,
            providerToken: string,
            startParameter: string,
            currency: string,
            prices: Types.LabeledPrice seq,
            photoUrl: string option,
            description: string option,
            photoSize: int option,
            photoWidth: int option,
            photoHeight: int option,
            needName: bool option,
            needPhoneNumber: bool option,
            needEmail: bool option,
            needShippingAddress: bool option,
            isFlexible: bool option,
            disableNotification: bool option,
            replyToMessageId: int option,
            replyMarkup: Types.InlineKeyboardMarkup option
        ) = Api.MakeRequestAsync<Types.Message>(token,
                "answerInlineQuery",
                [ "chat_id", box chatId
                  "title", box title
                  "payload", box payload
                  "provider_token", box providerToken
                  "start_parameter", box startParameter
                  "currency", box currency
                  "prices", box prices
                  "photo_url", box photoUrl
                  "description", box description
                  "photo_size", box photoSize
                  "photo_width", box photoWidth
                  "photo_height", box photoHeight
                  "need_name", box needName
                  "need_phone_number", box needPhoneNumber
                  "need_email", box needEmail
                  "need_shipping_address", box needShippingAddress
                  "is_flexible", box isFlexible
                  "disable_notification", box disableNotification
                  "reply_to_message_id", box replyToMessageId
                  "reply_markup", box replyMarkup ])

    /// Use this method to send invoices
    static member internal SendInvoiceAsync
        (   /// Bot token
            token: string,
            /// Unique identifier for the target private chat
            chatId: int,
            /// Product name, 1-32 characters
            title: string,
            /// Bot-defined invoice payload, 1-128 bytes. This will not be displayed to the user, use for your internal processes.
            payload: string,
            /// Payments provider token, obtained via Botfather
            providerToken: string,
            /// Unique deep-linking parameter that can be used to generate this invoice when used as a start parameter
            startParameter: string,
            /// Three-letter ISO 4217 currency code, see more on currencies
            currency: string,
            /// Price breakdown, a list of components (e.g. product price, tax, discount, delivery cost, delivery tax, bonus, etc.)
            prices: Types.LabeledPrice seq,
            /// photo of the goods or a marketing image for a service. People like it better when they see what they are paying for.
            ?photoUrl: string,
            /// Product description, 0-255 characters
            ?description: string,
            /// Photo size
            ?photoSize: int,
            /// Photo width
            ?photoWidth: int,
            /// Photo height
            ?photoHeight: int,
            /// Pass True, if you require the user's full name to complete the order
            ?needName: bool,
            /// Pass True, if you require the user's phone number to complete the order
            ?needPhoneNumber: bool,
            /// Pass True, if you require the user's email to complete the order
            ?needEmail: bool,
            /// Pass True, if you require the user's shipping address to complete the order
            ?needShippingAddress: bool,
            /// Pass True, if the final price depends on the shipping method
            ?isFlexible: bool,
            /// Sends the message silently. Users will receive a notification with no sound.
            ?disableNotification: bool,
            /// If the message is a reply, ID of the original message
            ?replyToMessageId: int,
            /// A JSON-serialized object for an inline keyboard. If empty, one 'Pay total price' button will be shown. If not empty, the first button must be a Pay button
            ?replyMarkup: Types.InlineKeyboardMarkup
        ) = Telegram.SendInvoiceBaseAsync(token, chatId, title, payload, providerToken, startParameter, currency, prices, photoUrl, description, 
                photoSize, photoWidth, photoHeight, needName, needPhoneNumber, needEmail, needShippingAddress, isFlexible, disableNotification, replyToMessageId, replyMarkup)

    /// Use this method to send invoices
    static member internal SendInvoice
        (   /// Bot token
            token: string,
            /// Unique identifier for the target private chat
            chatId: int,
            /// Product name, 1-32 characters
            title: string,
            /// Bot-defined invoice payload, 1-128 bytes. This will not be displayed to the user, use for your internal processes.
            payload: string,
            /// Payments provider token, obtained via Botfather
            providerToken: string,
            /// Unique deep-linking parameter that can be used to generate this invoice when used as a start parameter
            startParameter: string,
            /// Three-letter ISO 4217 currency code, see more on currencies
            currency: string,
            /// Price breakdown, a list of components (e.g. product price, tax, discount, delivery cost, delivery tax, bonus, etc.)
            prices: Types.LabeledPrice seq,
            /// photo of the goods or a marketing image for a service. People like it better when they see what they are paying for.
            ?photoUrl: string,
            /// Product description, 0-255 characters
            ?description: string,
            /// Photo size
            ?photoSize: int,
            /// Photo width
            ?photoWidth: int,
            /// Photo height
            ?photoHeight: int,
            /// Pass True, if you require the user's full name to complete the order
            ?needName: bool,
            /// Pass True, if you require the user's phone number to complete the order
            ?needPhoneNumber: bool,
            /// Pass True, if you require the user's email to complete the order
            ?needEmail: bool,
            /// Pass True, if you require the user's shipping address to complete the order
            ?needShippingAddress: bool,
            /// Pass True, if the final price depends on the shipping method
            ?isFlexible: bool,
            /// Sends the message silently. Users will receive a notification with no sound.
            ?disableNotification: bool,
            /// If the message is a reply, ID of the original message
            ?replyToMessageId: int,
            /// A JSON-serialized object for an inline keyboard. If empty, one 'Pay total price' button will be shown. If not empty, the first button must be a Pay button
            ?replyMarkup: Types.InlineKeyboardMarkup
        ) = Telegram.SendInvoiceBaseAsync(token, chatId, title, payload, providerToken, startParameter, currency, prices, photoUrl, description, 
                photoSize, photoWidth, photoHeight, needName, needPhoneNumber, needEmail, needShippingAddress, isFlexible, disableNotification, 
                replyToMessageId, replyMarkup) |> Async.RunSynchronously

    static member internal AnswerShippingQueryBaseAsync
        (   
            token: string,
            shippingQueryId: string,
            ok: bool,
            shippingOptions: Types.ShippingOption seq option,
            errorMessage: string option
        ) = Api.MakeRequestAsync<bool>(token,
                "answerInlineQuery",
                [ "shipping_query_id", box shippingQueryId
                  "ok", box ok
                  "shipping_options", box shippingOptions
                  "error_message", box errorMessage ])

    /// If you sent an invoice requesting a shipping address and the parameter is_flexible was specified, the Bot API will send an Update with a shipping_query field to the bot. Use this method to reply to shipping queries
    static member AnswerShippingQueryAsync
        (   /// Bot token
            token: string,
            /// Unique identifier for the query to be answered
            shippingQueryId: string,
            /// Specify True if delivery to the specified address is possible and False if there are any problems (for example, if delivery to the specified address is not possible)
            ok: bool,
            /// Required if ok is True. A JSON-serialized array of available shipping options.
            ?shippingOptions: Types.ShippingOption seq,
            /// Required if ok is False. Error message in human readable form that explains why it is impossible to complete the order (e.g. "Sorry, delivery to your desired address is unavailable'). Telegram will display this message to the user.
            ?errorMessage: string
        ) = Telegram.AnswerShippingQueryBaseAsync(token, shippingQueryId, ok, shippingOptions, errorMessage)

    /// If you sent an invoice requesting a shipping address and the parameter is_flexible was specified, the Bot API will send an Update with a shipping_query field to the bot. Use this method to reply to shipping queries
    static member AnswerShippingQuery
        (   /// Bot token
            token: string,
            /// Unique identifier for the query to be answered
            shippingQueryId: string,
            /// Specify True if delivery to the specified address is possible and False if there are any problems (for example, if delivery to the specified address is not possible)
            ok: bool,
            /// Required if ok is True. A JSON-serialized array of available shipping options.
            ?shippingOptions: Types.ShippingOption seq,
            /// Required if ok is False. Error message in human readable form that explains why it is impossible to complete the order (e.g. "Sorry, delivery to your desired address is unavailable'). Telegram will display this message to the user.
            ?errorMessage: string
        ) = Telegram.AnswerShippingQueryBaseAsync(token, shippingQueryId, ok, shippingOptions, errorMessage) |> Async.RunSynchronously

    static member internal AnswerPreCheckoutQueryBaseAsync
        (   
            token: string,
            preCheckoutQueryId: string,
            ok: bool,
            errorMessage: string option
        ) = Api.MakeRequestAsync<bool>(token,
                "answerInlineQuery",
                [ "pre_checkout_query_id", box preCheckoutQueryId
                  "ok", box ok
                  "error_message", box errorMessage ])

    /// Once the user has confirmed their payment and shipping details, the Bot API sends the final confirmation in the form of an Update with the field pre_checkout_query. Use this method to respond to such pre-checkout queries. On success, True is returned. Note: The Bot API must receive an answer within 10 seconds after the pre-checkout query was sent.
    static member AnswerPreCheckoutQueryAsync
        (   /// Bot token
            token: string,
            /// Unique identifier for the query to be answered
            preCheckoutQueryId: string,
            /// Specify True if everything is alright (goods are available, etc.) and the bot is ready to proceed with the order. Use False if there are any problems.
            ok: bool,
            /// Required if ok is False. Error message in human readable form that explains the reason for failure to proceed with the checkout (e.g. "Sorry, somebody just bought the last of our amazing black T-shirts while you were busy filling out your payment details. Please choose a different color or garment!"). Telegram will display this message to the user.
            ?errorMessage: string
        ) = Telegram.AnswerPreCheckoutQueryBaseAsync(token, preCheckoutQueryId, ok, errorMessage)

    /// Once the user has confirmed their payment and shipping details, the Bot API sends the final confirmation in the form of an Update with the field pre_checkout_query. Use this method to respond to such pre-checkout queries. On success, True is returned. Note: The Bot API must receive an answer within 10 seconds after the pre-checkout query was sent.
    static member AnswerPreCheckoutQuery
        (   /// Bot token
            token: string,
            /// Unique identifier for the query to be answered
            preCheckoutQueryId: string,
            /// Specify True if everything is alright (goods are available, etc.) and the bot is ready to proceed with the order. Use False if there are any problems.
            ok: bool,
            /// Required if ok is False. Error message in human readable form that explains the reason for failure to proceed with the checkout (e.g. "Sorry, somebody just bought the last of our amazing black T-shirts while you were busy filling out your payment details. Please choose a different color or garment!"). Telegram will display this message to the user.
            ?errorMessage: string
        ) = Telegram.AnswerPreCheckoutQueryBaseAsync(token, preCheckoutQueryId, ok, errorMessage) |> Async.RunSynchronously

    static member internal SendPhotoBaseAsync
        (   
            token: string,
            chatId: Types.ChatId,
            photo: Types.FileToSend,
            caption: string option,
            disableNotification: bool option,
            replyToMessageId: int option,
            replyMarkup: Types.Markup option
        ) = Api.MakeRequestAsync<Types.Message>(token,
                "sendPhoto",
                [ "chat_id", box (Helpers.getChatIdString chatId)
                  "photo", box photo
                  "caption", box caption
                  "disable_notification", box disableNotification
                  "reply_to_message_id", box replyToMessageId
                  "reply_markup", box replyMarkup ])

    /// Use this method to send photos
    static member SendPhotoAsync
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            chatId: Types.ChatId,
            /// Photo to send. Pass a file_id as String to send a photo that exists on the Telegram servers (recommended), pass an HTTP URL as a String for Telegram to get a photo from the Internet, or upload a new photo.
            photo: Types.FileToSend,
            /// Photo caption (may also be used when resending photos by file_id), 0-200 characters
            ?caption: string,
            /// Sends the message silently. Users will receive a notification with no sound.
            ?disableNotification: bool,
            /// If the message is a reply, ID of the original message
            ?replyToMessageId: int,
            /// Additional interface options. A JSON-serialized object for an inline keyboard, custom reply keyboard, instructions to remove reply keyboard or to force a reply from the user
            ?replyMarkup: Types.Markup
        ) = Telegram.SendPhotoBaseAsync(token, chatId, photo, caption, disableNotification, replyToMessageId, replyMarkup)
    
    /// Use this method to send photos. On success, the sent Message is returned.
    static member SendPhoto
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            chatId: Types.ChatId,
            /// Photo to send. Pass a file_id as String to send a photo that exists on the Telegram servers (recommended), pass an HTTP URL as a String for Telegram to get a photo from the Internet, or upload a new photo.
            photo: Types.FileToSend,
            /// Photo caption (may also be used when resending photos by file_id), 0-200 characters
            ?caption: string,
            /// Sends the message silently. Users will receive a notification with no sound.
            ?disableNotification: bool,
            /// If the message is a reply, ID of the original message
            ?replyToMessageId: int,
            /// Additional interface options. A JSON-serialized object for an inline keyboard, custom reply keyboard, instructions to remove reply keyboard or to force a reply from the user
            ?replyMarkup: Types.Markup
        ) = Telegram.SendPhotoBaseAsync(token, chatId, photo, caption, disableNotification, replyToMessageId, replyMarkup) |> Async.RunSynchronously
    
    static member internal SendAudioBaseAsync
        (   
            token: string,
            chatId: Types.ChatId,
            audio: Types.FileToSend,
            caption: string option,
            duration: int option,
            performer: string option,
            title: string option,
            disableNotification: bool option,
            replyToMessageId: int option,
            replyMarkup: Types.Markup option
        ) = Api.MakeRequestAsync<Types.Message>(token,
                "sendAudio",
                [ "chat_id", box (Helpers.getChatIdString chatId)
                  "audio", box audio
                  "caption", box caption
                  "duration", box duration
                  "performer", box performer
                  "title", box title
                  "disable_notification", box disableNotification
                  "reply_to_message_id", box replyToMessageId
                  "reply_markup", box replyMarkup ])

    /// Use this method to send audio files, if you want Telegram clients to display them in the music player. Your audio must be in the .mp3 format. On success, the sent Message is returned. Bots can currently send audio files of up to 50 MB in size, this limit may be changed in the future.
    /// For sending voice messages, use the sendVoice method instead.
    static member SendAudioAsync
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            chatId: Types.ChatId,
            /// Audio file to send. Pass a file_id as String to send an audio file that exists on the Telegram servers (recommended), pass an HTTP URL as a String for Telegram to get an audio file from the Internet, or upload a new one 
            audio: Types.FileToSend,
            /// Audio caption, 0-200 characters
            ?caption: string,
            /// Duration of the audio in seconds
            ?duration: int,
            /// Performer
            ?performer: string,
            /// Track name
            ?title: string,
            /// Sends the message silently. Users will receive a notification with no sound.
            ?disableNotification: bool,
            /// If the message is a reply, ID of the original message
            ?replyToMessageId: int,
            /// Additional interface options. A JSON-serialized object for an inline keyboard, custom reply keyboard, instructions to remove reply keyboard or to force a reply from the user
            ?replyMarkup: Types.Markup
        ) = Telegram.SendAudioBaseAsync(token, chatId, audio, caption, duration, performer, title, disableNotification, replyToMessageId, replyMarkup)

    /// Use this method to send audio files, if you want Telegram clients to display them in the music player. Your audio must be in the .mp3 format. On success, the sent Message is returned. Bots can currently send audio files of up to 50 MB in size, this limit may be changed in the future.
    /// For sending voice messages, use the sendVoice method instead.
    static member SendAudio
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            chatId: Types.ChatId,
            /// Audio file to send. Pass a file_id as String to send an audio file that exists on the Telegram servers (recommended), pass an HTTP URL as a String for Telegram to get an audio file from the Internet, or upload a new one 
            audio: Types.FileToSend,
            /// Audio caption, 0-200 characters
            ?caption: string,
            /// Duration of the audio in seconds
            ?duration: int,
            /// Performer
            ?performer: string,
            /// Track name
            ?title: string,
            /// Sends the message silently. Users will receive a notification with no sound.
            ?disableNotification: bool,
            /// If the message is a reply, ID of the original message
            ?replyToMessageId: int,
            /// Additional interface options. A JSON-serialized object for an inline keyboard, custom reply keyboard, instructions to remove reply keyboard or to force a reply from the user
            ?replyMarkup: Types.Markup
        ) = Telegram.SendAudioBaseAsync(token, chatId, audio, caption, duration, performer, title, disableNotification, replyToMessageId, replyMarkup) |> Async.RunSynchronously

    static member internal SendDocumentBaseAsync
        (   
            token: string,
            chatId: Types.ChatId,
            document: Types.FileToSend,
            caption: string option,
            disableNotification: bool option,
            replyToMessageId: int option,
            replyMarkup: Types.Markup option
        ) = Api.MakeRequestAsync<Types.Message>(token,
                "sendDocument",
                [ "chat_id", box (Helpers.getChatIdString chatId)
                  "document", box document
                  "caption", box caption
                  "disable_notification", box disableNotification
                  "reply_to_message_id", box replyToMessageId
                  "reply_markup", box replyMarkup ])

    /// Use this method to send general files. On success, the sent Message is returned. Bots can currently send files of any type of up to 50 MB in size, this limit may be changed in the future.
    static member SendDocumentAsync
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target channel (in the format
            chatId: Types.ChatId,
            /// File to send. Pass a file_id as String to send a file that exists on the Telegram servers (recommended), pass an HTTP URL as a String for Telegram to get a file from the Internet, or upload a new one
            document: Types.FileToSend,
            /// Document caption (may also be used when resending documents by file_id), 0-200 characters
            ?caption: string,
            /// Sends the message silently. Users will receive a notification with no sound.
            ?disableNotification: bool,
            /// If the message is a reply, ID of the original message
            ?replyToMessageId: int,
            /// Additional interface options. A JSON-serialized object for an inline keyboard, custom reply keyboard, instructions to remove reply keyboard or to force a reply from the user.
            ?replyMarkup: Types.Markup
        ) = Telegram.SendDocumentBaseAsync(token, chatId, document, caption, disableNotification, replyToMessageId, replyMarkup)
    
    /// Use this method to send general files. On success, the sent Message is returned. Bots can currently send files of any type of up to 50 MB in size, this limit may be changed in the future.
    static member SendDocument
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target channel (in the format
            chatId: Types.ChatId,
            /// File to send. Pass a file_id as String to send a file that exists on the Telegram servers (recommended), pass an HTTP URL as a String for Telegram to get a file from the Internet, or upload a new one
            document: Types.FileToSend,
            /// Document caption (may also be used when resending documents by file_id), 0-200 characters
            ?caption: string,
            /// Sends the message silently. Users will receive a notification with no sound.
            ?disableNotification: bool,
            /// If the message is a reply, ID of the original message
            ?replyToMessageId: int,
            /// Additional interface options. A JSON-serialized object for an inline keyboard, custom reply keyboard, instructions to remove reply keyboard or to force a reply from the user.
            ?replyMarkup: Types.Markup
        ) = Telegram.SendDocumentBaseAsync(token, chatId, document, caption, disableNotification, replyToMessageId, replyMarkup) |> Async.RunSynchronously

    static member internal SendStickerBaseAsync
        (   
            token: string,
            chatId: Types.ChatId,
            sticker: Types.FileToSend,
            disableNotification: bool option,
            replyToMessageId: int option,
            replyMarkup: Types.Markup option
        ) = Api.MakeRequestAsync<Types.Message>(token,
                "sendSticker",
                [ "chat_id", box (Helpers.getChatIdString chatId)
                  "sticker", box sticker
                  "disable_notification", box disableNotification
                  "reply_to_message_id", box replyToMessageId
                  "reply_markup", box replyMarkup ])
    
    /// Use this method to send .webp stickers
    static member SendStickerAsync
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            chatId: Types.ChatId,
            /// Sticker to send. Pass a file_id as String to send a file that exists on the Telegram servers (recommended), pass an HTTP URL as a String for Telegram to get a .webp file from the Internet, or upload a new one 
            sticker: Types.FileToSend,
            /// Sends the message silently. Users will receive a notification with no sound.
            ?disableNotification: bool,
            /// If the message is a reply, ID of the original message
            ?replyToMessageId: int,
            /// Additional interface options. A JSON-serialized object for an inline keyboard, custom reply keyboard, instructions to remove reply keyboard or to force a reply from the user
            ?replyMarkup: Types.Markup
        ) = Telegram.SendStickerBaseAsync(token, chatId, sticker, disableNotification, replyToMessageId, replyMarkup)

    /// Use this method to send .webp stickers
    static member SendSticker
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            chatId: Types.ChatId,
            /// Sticker to send. Pass a file_id as String to send a file that exists on the Telegram servers (recommended), pass an HTTP URL as a String for Telegram to get a .webp file from the Internet, or upload a new one 
            sticker: Types.FileToSend,
            /// Sends the message silently. Users will receive a notification with no sound.
            ?disableNotification: bool,
            /// If the message is a reply, ID of the original message
            ?replyToMessageId: int,
            /// Additional interface options. A JSON-serialized object for an inline keyboard, custom reply keyboard, instructions to remove reply keyboard or to force a reply from the user
            ?replyMarkup: Types.Markup
        ) = Telegram.SendStickerBaseAsync(token, chatId, sticker, disableNotification, replyToMessageId, replyMarkup) |> Async.RunSynchronously

    static member internal SendVideoBaseAsync
        (   
            token: string,
            chatId: Types.ChatId,
            video: Types.FileToSend,
            duration: int option,
            width: int option,
            height: int option,
            caption: string option,
            disableNotification: bool option,
            replyToMessageId: int option,
            replyMarkup: Types.Markup option
        ) = Api.MakeRequestAsync<Types.Message>(token,
                "sendVideo",
                [ "chat_id", box (Helpers.getChatIdString chatId)
                  "video", box video
                  "duration", box duration
                  "width", box width
                  "height", box height
                  "caption", box caption
                  "disable_notification", box disableNotification
                  "reply_to_message_id", box replyToMessageId
                  "reply_markup", box replyMarkup ])

    /// Use this method to send video files, Telegram clients support mp4 videos (other formats may be sent as Document). On success, the sent Message is returned. Bots can currently send video files of up to 50 MB in size, this limit may be changed in the future.
    static member SendVideoAsync
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            chatId: Types.ChatId,
            /// Video to send. Pass a file_id as String to send a video that exists on the Telegram servers (recommended), pass an HTTP URL as a String for Telegram to get a video from the Internet, or upload a new video 
            video: Types.FileToSend,
            /// Duration of sent video in seconds
            ?duration: int,
            /// Video width
            ?width: int,
            /// Video height
            ?height: int,
            /// Video caption (may also be used when resending videos by file_id), 0-200 characters
            ?caption: string,
            /// Sends the message silently. Users will receive a notification with no sound.
            ?disableNotification: bool,
            /// If the message is a reply, ID of the original message
            ?replyToMessageId: int,
            /// Additional interface options. A JSON-serialized object for an inline keyboard, custom reply keyboard, instructions to remove reply keyboard or to force a reply from the user.
            ?replyMarkup: Types.Markup
        ) = Telegram.SendVideoBaseAsync(token, chatId, video, duration, width, height, caption, disableNotification, replyToMessageId, replyMarkup)

    /// Use this method to send video files, Telegram clients support mp4 videos (other formats may be sent as Document). On success, the sent Message is returned. Bots can currently send video files of up to 50 MB in size, this limit may be changed in the future.
    static member SendVideo
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            chatId: Types.ChatId,
            /// Video to send. Pass a file_id as String to send a video that exists on the Telegram servers (recommended), pass an HTTP URL as a String for Telegram to get a video from the Internet, or upload a new video 
            video: Types.FileToSend,
            /// Duration of sent video in seconds
            ?duration: int,
            /// Video width
            ?width: int,
            /// Video height
            ?height: int,
            /// Video caption (may also be used when resending videos by file_id), 0-200 characters
            ?caption: string,
            /// Sends the message silently. Users will receive a notification with no sound.
            ?disableNotification: bool,
            /// If the message is a reply, ID of the original message
            ?replyToMessageId: int,
            /// Additional interface options. A JSON-serialized object for an inline keyboard, custom reply keyboard, instructions to remove reply keyboard or to force a reply from the user.
            ?replyMarkup: Types.Markup
        ) = Telegram.SendVideoBaseAsync(token, chatId, video, duration, width, height, caption, disableNotification, replyToMessageId, replyMarkup) |> Async.RunSynchronously

    static member internal SendVoiceBaseAsync
        (   
            token: string,
            chatId: Types.ChatId,
            voice: Types.FileToSend,
            caption: string option,
            duration: int option,
            disableNotification: bool option,
            replyToMessageId: int option,
            replyMarkup: Types.Markup option
        ) = Api.MakeRequestAsync<Types.Message>(token,
                "sendVoice",
                [ "chat_id", box (Helpers.getChatIdString chatId)
                  "voice", box voice
                  "caption", box caption
                  "duration", box duration
                  "disable_notification", box disableNotification
                  "reply_to_message_id", box replyToMessageId
                  "reply_markup", box replyMarkup ])
    
    /// Use this method to send audio files, if you want Telegram clients to display the file as a playable voice message. For this to work, your audio must be in an .ogg file encoded with OPUS (other formats may be sent as Audio or Document). On success, the sent Message is returned. Bots can currently send voice messages of up to 50 MB in size, this limit may be changed in the future.
    static member SendVoiceAsync
        (   /// Bot token   
            token: string,
            /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            chatId: Types.ChatId,
            /// Audio file to send. Pass a file_id as String to send a file that exists on the Telegram servers (recommended), pass an HTTP URL as a String for Telegram to get a file from the Internet, or upload a new one
            voice: Types.FileToSend,
            /// Voice message caption, 0-200 characters
            ?caption: string,
            /// Duration of the voice message in seconds
            ?duration: int,
            /// Sends the message silently. Users will receive a notification with no sound.
            ?disableNotification: bool,
            /// If the message is a reply, ID of the original message
            ?replyToMessageId: int,
            /// Additional interface options. A JSON-serialized object for an inline keyboard, custom reply keyboard, instructions to remove reply keyboard or to force a reply from the user.
            ?replyMarkup: Types.Markup
        ) = Telegram.SendVoiceBaseAsync(token, chatId, voice, caption, duration, disableNotification, replyToMessageId, replyMarkup)
    
    /// Use this method to send audio files, if you want Telegram clients to display the file as a playable voice message. For this to work, your audio must be in an .ogg file encoded with OPUS (other formats may be sent as Audio or Document). On success, the sent Message is returned. Bots can currently send voice messages of up to 50 MB in size, this limit may be changed in the future.
    static member SendVoice
        (   /// Bot token   
            token: string,
            /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            chatId: Types.ChatId,
            /// Audio file to send. Pass a file_id as String to send a file that exists on the Telegram servers (recommended), pass an HTTP URL as a String for Telegram to get a file from the Internet, or upload a new one
            voice: Types.FileToSend,
            /// Voice message caption, 0-200 characters
            ?caption: string,
            /// Duration of the voice message in seconds
            ?duration: int,
            /// Sends the message silently. Users will receive a notification with no sound.
            ?disableNotification: bool,
            /// If the message is a reply, ID of the original message
            ?replyToMessageId: int,
            /// Additional interface options. A JSON-serialized object for an inline keyboard, custom reply keyboard, instructions to remove reply keyboard or to force a reply from the user.
            ?replyMarkup: Types.Markup
        ) = Telegram.SendVoiceBaseAsync(token, chatId, voice, caption, duration, disableNotification, replyToMessageId, replyMarkup) |> Async.RunSynchronously

    static member internal SendVideoNoteBaseAsync
        (   
            token: string,
            chatId: Types.ChatId,
            videoNote: Types.FileToSend,
            duration: int option,
            length: int option,
            disableNotification: bool option,
            replyToMessageId: int option,
            replyMarkup: Types.Markup option
        ) = Api.MakeRequestAsync<Types.Message>(token,
                "sendVideoNote",
                [ "chat_id", box (Helpers.getChatIdString chatId)
                  "video_note", box videoNote
                  "duration", box duration
                  "length", box length
                  "disable_notification", box disableNotification
                  "reply_to_message_id", box replyToMessageId
                  "reply_markup", box replyMarkup ])

    /// As of v.4.0, Telegram clients support rounded square mp4 videos of up to 1 minute long. Use this method to send video messages
    static member SendVideoNoteAsync
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            chatId: Types.ChatId,
            /// Video note to send. Pass a file_id as String to send a video note that exists on the Telegram servers (recommended) or upload a new video. Sending video notes by a URL is currently unsupported
            videoNote: Types.FileToSend,
            /// Duration of sent video in seconds
            duration: int option,
            /// Video width and height
            length: int option,
            /// Sends the message silently. Users will receive a notification with no sound.
            disableNotification: bool option,
            /// If the message is a reply, ID of the original message
            replyToMessageId: int option,
            /// Additional interface options. A JSON-serialized object for an inline keyboard, custom reply keyboard, instructions to remove reply keyboard or to force a reply from the user.
            replyMarkup: Types.Markup option
        ) = Telegram.SendVideoNoteBaseAsync(token, chatId, videoNote, duration, length, disableNotification, replyToMessageId, replyMarkup)

    /// As of v.4.0, Telegram clients support rounded square mp4 videos of up to 1 minute long. Use this method to send video messages
    static member SendVideoNote
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            chatId: Types.ChatId,
            /// Video note to send. Pass a file_id as String to send a video note that exists on the Telegram servers (recommended) or upload a new video. Sending video notes by a URL is currently unsupported
            videoNote: Types.FileToSend,
            /// Duration of sent video in seconds
            duration: int option,
            /// Video width and height
            length: int option,
            /// Sends the message silently. Users will receive a notification with no sound.
            disableNotification: bool option,
            /// If the message is a reply, ID of the original message
            replyToMessageId: int option,
            /// Additional interface options. A JSON-serialized object for an inline keyboard, custom reply keyboard, instructions to remove reply keyboard or to force a reply from the user.
            replyMarkup: Types.Markup option
        ) = Telegram.SendVideoNoteBaseAsync(token, chatId, videoNote, duration, length, disableNotification, replyToMessageId, replyMarkup) |> Async.RunSynchronously