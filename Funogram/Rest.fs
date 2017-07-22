module Funogram.Rest

open Newtonsoft.Json
open Newtonsoft.Json.Serialization
open System
open System.Runtime.CompilerServices
open Tools

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
              [ "chat_id", box (Tools.getChatIdString chatId)
                "text", box text
                "parse_mode", box (Tools.parseModeName parseMode)
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
                [ "chat_id", box (Tools.getChatIdString chatId)
                  "from_chat_id", box (Tools.getChatIdString fromChatId)
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
                [ "chat_id", box (Tools.getChatIdString chatId)
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
                [ "chat_id", box (Tools.getChatIdString chatId)
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
        ) = Api.MakeRequestAsync<bool>(token, "leaveChat", [ "chat_id", box (Tools.getChatIdString chatId) ])

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
        ) = Api.MakeRequestAsync<Types.Chat>(token, "getChat", [ "chat_id", box (Tools.getChatIdString chatId) ])

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
        ) = Api.MakeRequestAsync<seq<Types.ChatMember>>(token, "getChatAdministrators", [ "chat_id", box (Tools.getChatIdString chatId) ])

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
        ) = Api.MakeRequestAsync<int>(token, "getChatMembersCount", [ "chat_id", box (Tools.getChatIdString chatId) ])
    
    /// Use this method to get the number of members in a chat. Returns Int on success.
    static member GetChatMembersCount
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target supergroup or channel (in the format @channelusername)
            chatId: Types.ChatId
        ) = Api.MakeRequestAsync<int>(token, "getChatMembersCount", [ "chat_id", box (Tools.getChatIdString chatId) ]) |> Async.RunSynchronously

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
                [ "chat_id", box (Tools.getChatIdString chatId)
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
                  "parse_mode", box (Tools.parseModeName parseMode)
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
                [ "chat_id", box (Tools.getChatIdStringOption chatId)
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
                [ "chat_id", box (Tools.getChatIdStringOption chatId)
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
                [ "chatId", box (Tools.getChatIdString chatId)
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
                [ "chat_id", box (Tools.getChatIdString chatId)
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
                [ "chat_id", box (Tools.getChatIdString chatId)
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
                [ "chat_id", box (Tools.getChatIdString chatId)
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
                [ "chat_id", box (Tools.getChatIdString chatId)
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
                [ "chat_id", box (Tools.getChatIdString chatId)
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
                [ "chat_id", box (Tools.getChatIdString chatId)
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
                [ "chat_id", box (Tools.getChatIdString chatId)
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
            ?duration: int,
            /// Video width and height
            ?length: int,
            /// Sends the message silently. Users will receive a notification with no sound.
            ?disableNotification: bool,
            /// If the message is a reply, ID of the original message
            ?replyToMessageId: int,
            /// Additional interface options. A JSON-serialized object for an inline keyboard, custom reply keyboard, instructions to remove reply keyboard or to force a reply from the user.
            ?replyMarkup: Types.Markup
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
            ?duration: int,
            /// Video width and height
            ?length: int,
            /// Sends the message silently. Users will receive a notification with no sound.
            ?disableNotification: bool,
            /// If the message is a reply, ID of the original message
            ?replyToMessageId: int,
            /// Additional interface options. A JSON-serialized object for an inline keyboard, custom reply keyboard, instructions to remove reply keyboard or to force a reply from the user.
            ?replyMarkup: Types.Markup
        ) = Telegram.SendVideoNoteBaseAsync(token, chatId, videoNote, duration, length, disableNotification, replyToMessageId, replyMarkup) |> Async.RunSynchronously

    static member internal SendLocationBaseAsync
        (   
            token: string,
            chatId: Types.ChatId,
            latitude: float,
            longitude: float,
            disableNotification: bool option,
            replyToMessageId: int option,
            replyMarkup: Types.Markup option
        ) = Api.MakeRequestAsync<Types.Message>(token,
                "sendLocation",
                [ "chat_id", box (Tools.getChatIdString chatId)
                  "latitude", box latitude
                  "longitude", box longitude
                  "disable_notification", box disableNotification
                  "reply_to_message_id", box replyToMessageId
                  "reply_markup", box replyMarkup ])

    /// Use this method to send point on the map
    static member SendLocationAsync
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            chatId: Types.ChatId,
            /// Latitude of location
            latitude: float,
            /// Longitude of location
            longitude: float,
            /// Sends the message silently. Users will receive a notification with no sound.
            ?disableNotification: bool,
            /// If the message is a reply, ID of the original message
            ?replyToMessageId: int,
            /// Additional interface options. A JSON-serialized object for an inline keyboard, custom reply keyboard, instructions to remove reply keyboard or to force a reply from the user.
            ?replyMarkup: Types.Markup
        ) = Telegram.SendLocationBaseAsync(token, chatId, latitude, longitude, disableNotification, replyToMessageId, replyMarkup)

    /// Use this method to send point on the map
    static member SendLocation
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            chatId: Types.ChatId,
            /// Latitude of location
            latitude: float,
            /// Longitude of location
            longitude: float,
            /// Sends the message silently. Users will receive a notification with no sound.
            ?disableNotification: bool,
            /// If the message is a reply, ID of the original message
            ?replyToMessageId: int,
            /// Additional interface options. A JSON-serialized object for an inline keyboard, custom reply keyboard, instructions to remove reply keyboard or to force a reply from the user.
            ?replyMarkup: Types.Markup
        ) = Telegram.SendLocationBaseAsync(token, chatId, latitude, longitude, disableNotification, replyToMessageId, replyMarkup) |> Async.RunSynchronously

    static member internal SendVenueBaseAsync
        (
            token: string,
            chatId: Types.ChatId,
            latitude: float,
            longitude: float,
            title: string,
            address: string,
            foursquareId: string option,
            disableNotification: bool option,
            replyToMessageId: int option,
            replyMarkup: Types.Markup option
        ) = Api.MakeRequestAsync<Types.Message>(token,
                "sendVenue",
                [ "chat_id", box (Tools.getChatIdString chatId)
                  "latitude", box latitude
                  "longitude", box longitude
                  "longitude", box longitude
                  "title", box title
                  "address", box address
                  "foursquare_id", box foursquareId
                  "disable_notification", box disableNotification
                  "reply_to_message_id", box replyToMessageId
                  "reply_markup", box replyMarkup ])

    /// Use this method to send information about a venue
    static member SendVenueAsync
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            chatId: Types.ChatId,
            /// Latitude of the venue
            latitude: float,
            /// Longitude of the venue
            longitude: float,
            /// Name of the venue
            title: string,
            /// Address of the venue
            address: string,
            /// Foursquare identifier of the venue
            ?foursquareId: string,
            /// Sends the message silently. Users will receive a notification with no sound
            ?disableNotification: bool,
            /// If the message is a reply, ID of the original message
            ?replyToMessageId: int,
            /// Additional interface options. A JSON-serialized object for an inline keyboard, custom reply keyboard, instructions to remove reply keyboard or to force a reply from the user.
            ?replyMarkup: Types.Markup
        ) = Telegram.SendVenueBaseAsync(token, chatId, latitude, longitude, title, address, foursquareId, disableNotification, replyToMessageId, replyMarkup)


    /// Use this method to send information about a venue
    static member SendVenue
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            chatId: Types.ChatId,
            /// Latitude of the venue
            latitude: float,
            /// Longitude of the venue
            longitude: float,
            /// Name of the venue
            title: string,
            /// Address of the venue
            address: string,
            /// Foursquare identifier of the venue
            ?foursquareId: string,
            /// Sends the message silently. Users will receive a notification with no sound
            ?disableNotification: bool,
            /// If the message is a reply, ID of the original message
            ?replyToMessageId: int,
            /// Additional interface options. A JSON-serialized object for an inline keyboard, custom reply keyboard, instructions to remove reply keyboard or to force a reply from the user.
            ?replyMarkup: Types.Markup
        ) = Telegram.SendVenueBaseAsync(token, chatId, latitude, longitude, title, address, foursquareId, disableNotification, replyToMessageId, replyMarkup) |> Async.RunSynchronously

    static member internal SendContactBaseAsync
        (
            token: string,
            chatId: Types.ChatId,
            phoneNumber: string,
            firstName: string,
            lastName: string option,
            disableNotification: bool option,
            replyToMessageId: int option,
            replyMarkup: Types.Markup option
        ) = Api.MakeRequestAsync<Types.Message>(token,
                "sendContact",
                [ "chat_id", box (Tools.getChatIdString chatId)
                  "phone_number", box phoneNumber
                  "first_name", box firstName
                  "last_name", box lastName
                  "disable_notification", box disableNotification
                  "reply_to_message_id", box replyToMessageId
                  "reply_markup", box replyMarkup ])

    /// Use this method to send phone contacts
    static member SendContactAsync
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            chatId: Types.ChatId,
            /// Contact's phone number
            phoneNumber: string,
            /// Contact's first name
            firstName: string,
            /// Contact's last name
            ?lastName: string,
            /// Sends the message silently. Users will receive a notification with no sound.
            ?disableNotification: bool,
            /// If the message is a reply, ID of the original message
            ?replyToMessageId: int,
            /// Additional interface options. A JSON-serialized object for an inline keyboard, custom reply keyboard, instructions to remove keyboard or to force a reply from the user.
            ?replyMarkup: Types.Markup
        ) = Telegram.SendContactBaseAsync(token, chatId, phoneNumber, firstName, lastName, disableNotification, replyToMessageId, replyMarkup)
    
    /// Use this method to send phone contacts
    static member SendContact
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            chatId: Types.ChatId,
            /// Contact's phone number
            phoneNumber: string,
            /// Contact's first name
            firstName: string,
            /// Contact's last name
            ?lastName: string,
            /// Sends the message silently. Users will receive a notification with no sound.
            ?disableNotification: bool,
            /// If the message is a reply, ID of the original message
            ?replyToMessageId: int,
            /// Additional interface options. A JSON-serialized object for an inline keyboard, custom reply keyboard, instructions to remove keyboard or to force a reply from the user.
            ?replyMarkup: Types.Markup
        ) = Telegram.SendContactBaseAsync(token, chatId, phoneNumber, firstName, lastName, disableNotification, replyToMessageId, replyMarkup) |> Async.RunSynchronously

    /// Use this method when you need to tell the user that something is happening on the bot's side. The status is set for 5 seconds or less (when a message arrives from your bot, Telegram clients clear its typing status).
    static member SendChatActionAsync
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            chatId: Types.ChatId,
            /// Type of action to broadcast
            action: Types.ChatAction
        ) = Api.MakeRequestAsync<bool>(token,
                "sendChatAction",
                [ "chat_id", box (Tools.getChatIdString chatId)
                  "action", box action ])
    
    /// Use this method when you need to tell the user that something is happening on the bot's side. The status is set for 5 seconds or less (when a message arrives from your bot, Telegram clients clear its typing status).
    static member SendChatAction
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            chatId: Types.ChatId,
            /// Type of action to broadcast
            action: Types.ChatAction
        ) = Telegram.SendChatActionAsync(token, chatId, action) |> Async.RunSynchronously

    static member internal SendGameBaseAsync
        (
            token: string,
            chatId: int,
            gameShortName: string,
            disableNotification: bool option,
            replyToMessageId: int option,
            replyMarkup: Types.Markup option
        ) = Api.MakeRequestAsync<Types.Message>(token,
                "sendGame",
                [ "chat_id", box chatId
                  "game_short_name", box gameShortName
                  "disable_notification", box disableNotification
                  "reply_to_message_id", box replyToMessageId
                  "reply_markup", box replyMarkup ])

    /// Use this method to send a game
    static member SendGameAsync
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat
            chatId: int,
            /// Short name of the game, serves as the unique identifier for the game. Set up your games via Botfather.
            gameShortName: string,
            /// Sends the message silently. Users will receive a notification with no sound
            ?disableNotification: bool,
            /// If the message is a reply, ID of the original message
            ?replyToMessageId: int,
            /// A JSON-serialized object for an inline keyboard. If empty, one ‘Play game_title’ button will be shown. If not empty, the first button must launch the game
            ?replyMarkup: Types.Markup
        ) = Telegram.SendGameBaseAsync(token, chatId, gameShortName, disableNotification, replyToMessageId, replyMarkup)
                  
    /// Use this method to send a game
    static member SendGame
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat
            chatId: int,
            /// Short name of the game, serves as the unique identifier for the game. Set up your games via Botfather.
            gameShortName: string,
            /// Sends the message silently. Users will receive a notification with no sound
            ?disableNotification: bool,
            /// If the message is a reply, ID of the original message
            ?replyToMessageId: int,
            /// A JSON-serialized object for an inline keyboard. If empty, one ‘Play game_title’ button will be shown. If not empty, the first button must launch the game
            ?replyMarkup: Types.Markup
        ) = Telegram.SendGameBaseAsync(token, chatId, gameShortName, disableNotification, replyToMessageId, replyMarkup) |> Async.RunSynchronously

    static member internal SetGameScoreBaseAsync
        (
            token: string,
            userId: int,
            score: uint32,
            force: bool option,
            disableEditMessage: bool option,
            chatId: int option,
            messageId: int option,
            inlineMessageId: string option
        ) = Api.MakeRequestAsync<Types.EditMessageResult>(token,
                "setGameScore",
                [ "user_id", box userId
                  "score", box score
                  "force", box force 
                  "disable_edit_message", box disableEditMessage
                  "chat_id", box chatId
                  "message_id", box messageId
                  "inline_message_id", box inlineMessageId ])

    /// Use this method to set the score of the specified user in a game. On success, if the message was sent by the bot, returns the edited Message, otherwise returns True. Returns an error, if the new score is not greater than the user's current score in the chat and force is False.
    static member SetGameScoreAsync
        (   /// Bot token
            token: string,
            /// User identifier
            userId: int,
            /// New score, must be non-negative
            score: uint32,
            /// Pass True, if the high score is allowed to decrease. This can be useful when fixing mistakes or banning cheaters
            ?force: bool,
            /// Pass True, if the game message should not be automatically edited to include the current scoreboard
            ?disableEditMessage: bool,
            /// Required if inline_message_id is not specified. Unique identifier for the target chat
            ?chatId: int,
            /// Required if inline_message_id is not specified. Identifier of the sent message
            ?messageId: int,
            /// Required if chat_id and message_id are not specified. Identifier of the inline message
            ?inlineMessageId: string
        ) = Telegram.SetGameScoreBaseAsync(token, userId, score, force, disableEditMessage, chatId, messageId, inlineMessageId)

    /// Use this method to set the score of the specified user in a game. On success, if the message was sent by the bot, returns the edited Message, otherwise returns True. Returns an error, if the new score is not greater than the user's current score in the chat and force is False.
    static member SetGameScore
        (   /// Bot token
            token: string,
            /// User identifier
            userId: int,
            /// New score, must be non-negative
            score: uint32,
            /// Pass True, if the high score is allowed to decrease. This can be useful when fixing mistakes or banning cheaters
            ?force: bool,
            /// Pass True, if the game message should not be automatically edited to include the current scoreboard
            ?disableEditMessage: bool,
            /// Required if inline_message_id is not specified. Unique identifier for the target chat
            ?chatId: int,
            /// Required if inline_message_id is not specified. Identifier of the sent message
            ?messageId: int,
            /// Required if chat_id and message_id are not specified. Identifier of the inline message
            ?inlineMessageId: string
        ) = Telegram.SetGameScoreBaseAsync(token, userId, score, force, disableEditMessage, chatId, messageId, inlineMessageId) |> Async.RunSynchronously

    static member internal GetGameHighScoresBaseAsync
        (
            token: string,
            userId: int,
            chatId: int option,
            messageId: int option,
            inlineMessageId: string option
        ) = Api.MakeRequestAsync<Types.GameHighScore seq>(token,
                "getGameHighScores",
                [ "user_id", box userId
                  "chat_id", box chatId
                  "message_id", box messageId
                  "inline_message_id", box inlineMessageId ])
                  
    /// Use this method to get data for high score tables. Will return the score of the specified user and several of his neighbors in a game
    /// This method will currently return scores for the target user, plus two of his closest neighbors on each side. Will also return the top three users if the user and his neighbors are not among them. Please note that this behavior is subject to change.
    static member GetGameHighScoresAsync
        (   /// Bot token
            token: string,
            /// Target user id
            userId: int,
            /// Required if inline_message_id is not specified. Unique identifier for the target chat
            ?chatId: int,
            /// Required if inline_message_id is not specified. Identifier of the sent message
            ?messageId: int,
            /// Required if chat_id and message_id are not specified. Identifier of the inline message
            ?inlineMessageId: string
        ) = Telegram.GetGameHighScoresBaseAsync(token, userId, chatId, messageId, inlineMessageId)
                  
    /// Use this method to get data for high score tables. Will return the score of the specified user and several of his neighbors in a game
    /// This method will currently return scores for the target user, plus two of his closest neighbors on each side. Will also return the top three users if the user and his neighbors are not among them. Please note that this behavior is subject to change.
    static member GetGameHighScores
        (   /// Bot token
            token: string,
            /// Target user id
            userId: int,
            /// Required if inline_message_id is not specified. Unique identifier for the target chat
            ?chatId: int,
            /// Required if inline_message_id is not specified. Identifier of the sent message
            ?messageId: int,
            /// Required if chat_id and message_id are not specified. Identifier of the inline message
            ?inlineMessageId: string
        ) = Telegram.GetGameHighScoresBaseAsync(token, userId, chatId, messageId, inlineMessageId) |> Async.RunSynchronously

    static member internal RestrictChatMemberBaseAsync
        (
            token: string,
            chatId: Types.ChatId,
            userId: int,
            untilDate: DateTime option,
            canSendMessages: bool option,
            canSendMediaMessages: bool option,
            canSendOtherMessages: bool option,
            canAddWebPagePreviews: bool option
        ) = Api.MakeRequestAsync<bool>(token,
                "restrictChatMember",
                [ "user_id", box userId
                  "chat_id", box (Tools.getChatIdString chatId)
                  "until_date", box untilDate
                  "can_send_messages", box canSendMessages
                  "can_send_media_messages", box canSendMediaMessages
                  "can_send_other_messages", box canSendOtherMessages
                  "can_add_web_page_previews", box canAddWebPagePreviews ])

    /// Use this method to restrict a user in a supergroup. The bot must be an administrator in the supergroup for this to work and must have the appropriate admin rights. Pass True for all boolean parameters to lift restrictions from a user
    static member RestrictChatMemberAsync
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target supergroup (in the format @supergroupusername)
            chatId: Types.ChatId,
            /// Unique identifier of the target user
            userId: int,
            /// Date when restrictions will be lifted for the user, unix time. If user is restricted for more than 366 days or less than 30 seconds from the current time, they are considered to be restricted forever
            ?untilDate: DateTime,
            /// Pass True, if the user can send text messages, contacts, locations and venues
            ?canSendMessages: bool,
            /// Pass True, if the user can send audios, documents, photos, videos, video notes and voice notes, implies can_send_messages
            ?canSendMediaMessages: bool,
            /// Pass True, if the user can send animations, games, stickers and use inline bots, implies can_send_media_messages
            ?canSendOtherMessages: bool,
            /// Pass True, if the user may add web page previews to their messages, implies can_send_media_messages
            ?canAddWebPagePreviews: bool
        ) = Telegram.RestrictChatMemberBaseAsync(token, chatId, userId, untilDate, canSendMessages, canSendMediaMessages, canSendOtherMessages, canAddWebPagePreviews)
    
    /// Use this method to restrict a user in a supergroup. The bot must be an administrator in the supergroup for this to work and must have the appropriate admin rights. Pass True for all boolean parameters to lift restrictions from a user
    static member RestrictChatMember
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target supergroup (in the format @supergroupusername)
            chatId: Types.ChatId,
            /// Unique identifier of the target user
            userId: int,
            /// Date when restrictions will be lifted for the user, unix time. If user is restricted for more than 366 days or less than 30 seconds from the current time, they are considered to be restricted forever
            ?untilDate: DateTime,
            /// Pass True, if the user can send text messages, contacts, locations and venues
            ?canSendMessages: bool,
            /// Pass True, if the user can send audios, documents, photos, videos, video notes and voice notes, implies can_send_messages
            ?canSendMediaMessages: bool,
            /// Pass True, if the user can send animations, games, stickers and use inline bots, implies can_send_media_messages
            ?canSendOtherMessages: bool,
            /// Pass True, if the user may add web page previews to their messages, implies can_send_media_messages
            ?canAddWebPagePreviews: bool
        ) = Telegram.RestrictChatMemberBaseAsync(token, chatId, userId, untilDate, canSendMessages, canSendMediaMessages, canSendOtherMessages, canAddWebPagePreviews) |> Async.RunSynchronously

    static member internal PromoteChatMemberBaseAsync
        (
            token: string,
            chatId: Types.ChatId,
            userId: int,
            canChangeInfo: bool option,
            canPostMessages: bool option,
            canEditMessages: bool option,
            canDeleteMessages: bool option,
            canInviteUsers: bool option,
            canRestrictMembers: bool option,
            canPinMessages: bool option,
            canPromoteMembers: bool option
        ) = Api.MakeRequestAsync<bool>(token,
                "promoteChatMember",
                [ "user_id", box userId
                  "chat_id", box (Tools.getChatIdString chatId)
                  "can_change_info", box canChangeInfo
                  "can_post_messages", box canPostMessages
                  "can_edit_messages", box canEditMessages
                  "can_delete_messages", box canDeleteMessages
                  "can_invite_users", box canInviteUsers
                  "can_restrict_members", box canRestrictMembers
                  "can_pin_messages", box canPinMessages
                  "can_promote_members", box canPromoteMembers ])

    /// Use this method to promote or demote a user in a supergroup or a channel. The bot must be an administrator in the chat for this to work and must have the appropriate admin rights. Pass False for all boolean parameters to demote a user
    static member PromoteChatMemberAsync
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            chatId: Types.ChatId,
            /// Unique identifier of the target user
            userId: int,
            /// Pass True, if the administrator can change chat title, photo and other settings
            canChangeInfo: bool option,
            /// Pass True, if the administrator can create channel posts, channels only
            canPostMessages: bool option,
            /// Pass True, if the administrator can edit messages of other users, channels only
            canEditMessages: bool option,
            /// Pass True, if the administrator can delete messages of other users
            canDeleteMessages: bool option,
            /// Pass True, if the administrator can invite new users to the chat
            canInviteUsers: bool option,
            /// Pass True, if the administrator can restrict, ban or unban chat members
            canRestrictMembers: bool option,
            /// Pass True, if the administrator can pin messages, supergroups only
            canPinMessages: bool option,
            /// Pass True, if the administrator can add new administrators with a subset of his own privileges or demote administrators that he has promoted, directly or indirectly (promoted by administrators that were appointed by him)
            canPromoteMembers: bool option
        ) = Telegram.PromoteChatMemberBaseAsync(token, chatId, userId, canChangeInfo, canPostMessages, canEditMessages, canDeleteMessages, canInviteUsers, canRestrictMembers, canPinMessages, canPromoteMembers)

    /// Use this method to promote or demote a user in a supergroup or a channel. The bot must be an administrator in the chat for this to work and must have the appropriate admin rights. Pass False for all boolean parameters to demote a user
    static member PromoteChatMember
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            chatId: Types.ChatId,
            /// Unique identifier of the target user
            userId: int,
            /// Pass True, if the administrator can change chat title, photo and other settings
            canChangeInfo: bool option,
            /// Pass True, if the administrator can create channel posts, channels only
            canPostMessages: bool option,
            /// Pass True, if the administrator can edit messages of other users, channels only
            canEditMessages: bool option,
            /// Pass True, if the administrator can delete messages of other users
            canDeleteMessages: bool option,
            /// Pass True, if the administrator can invite new users to the chat
            canInviteUsers: bool option,
            /// Pass True, if the administrator can restrict, ban or unban chat members
            canRestrictMembers: bool option,
            /// Pass True, if the administrator can pin messages, supergroups only
            canPinMessages: bool option,
            /// Pass True, if the administrator can add new administrators with a subset of his own privileges or demote administrators that he has promoted, directly or indirectly (promoted by administrators that were appointed by him)
            canPromoteMembers: bool option
        ) = Telegram.PromoteChatMemberBaseAsync(token, chatId, userId, canChangeInfo, canPostMessages, canEditMessages, canDeleteMessages, canInviteUsers, canRestrictMembers, canPinMessages, canPromoteMembers) |> Async.RunSynchronously

    static member internal KickChatMemberBaseAsync
        (
            token: string,
            chatId: Types.ChatId,
            userId: int,
            untilDate: DateTime option
        ) = Api.MakeRequestAsync<bool>(token,
                "kickChatMember",
                [ "chat_id", box (Tools.getChatIdString chatId)
                  "user_id", box userId
                  "until_date", box untilDate ])

    /// Use this method to kick a user from a group, a supergroup or a channel. In the case of supergroups and channels, the user will not be able to return to the group on their own using invite links, etc., unless unbanned first. The bot must be an administrator in the chat for this to work and must have the appropriate admin rights
    /// Note: In regular groups (non-supergroups), this method will only work if the ‘All Members Are Admins’ setting is off in the target group. Otherwise members may only be removed by the group's creator or by the member that added them.
    static member KickChatMemberAsync
        (   /// Bot token
            token: string,
            /// Unique identifier for the target group or username of the target supergroup or channel (in the format @channelusername)
            chatId: Types.ChatId,
            /// Unique identifier of the target user
            userId: int,
            /// Date when the user will be unbanned, unix time. If user is banned for more than 366 days or less than 30 seconds from the current time they are considered to be banned forever
            ?untilDate: DateTime
        ) = Telegram.KickChatMemberBaseAsync(token, chatId, userId, untilDate)

    /// Use this method to kick a user from a group, a supergroup or a channel. In the case of supergroups and channels, the user will not be able to return to the group on their own using invite links, etc., unless unbanned first. The bot must be an administrator in the chat for this to work and must have the appropriate admin rights
    /// Note: In regular groups (non-supergroups), this method will only work if the ‘All Members Are Admins’ setting is off in the target group. Otherwise members may only be removed by the group's creator or by the member that added them.
    static member KickChatMember
        (   /// Bot token
            token: string,
            /// Unique identifier for the target group or username of the target supergroup or channel (in the format @channelusername)
            chatId: Types.ChatId,
            /// Unique identifier of the target user
            userId: int,
            /// Date when the user will be unbanned, unix time. If user is banned for more than 366 days or less than 30 seconds from the current time they are considered to be banned forever
            ?untilDate: DateTime
        ) = Telegram.KickChatMemberBaseAsync(token, chatId, userId, untilDate) |> Async.RunSynchronously
    
    /// Use this method to export an invite link to a supergroup or a channel. The bot must be an administrator in the chat for this to work and must have the appropriate admin rights
    static member ExportChatInviteLinkAsync
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            chatId: Types.ChatId
        ) = Api.MakeRequestAsync<string>(token,  "exportChatInviteLink", [ "chat_id", box (Tools.getChatIdString chatId) ])

    /// Use this method to export an invite link to a supergroup or a channel. The bot must be an administrator in the chat for this to work and must have the appropriate admin rights
    static member ExportChatInviteLink
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            chatId: Types.ChatId
        ) = Telegram.ExportChatInviteLinkAsync(token, chatId) |> Async.RunSynchronously

    /// Use this method to set a new profile photo for the chat. Photos can't be changed for private chats. The bot must be an administrator in the chat for this to work and must have the appropriate admin rights
    /// Note: In regular groups (non-supergroups), this method will only work if the ‘All Members Are Admins’ setting is off in the target group.
    static member SetChatPhotoAsync
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            chatId: Types.ChatId,
            /// New chat photo
            photo: Types.FileToSend
        ) = Api.MakeRequestAsync<string>(token,  
                "setChatPhoto", 
                [ "chat_id", box (Tools.getChatIdString chatId)
                  "photo", box photo ])

    /// Use this method to set a new profile photo for the chat. Photos can't be changed for private chats. The bot must be an administrator in the chat for this to work and must have the appropriate admin rights
    /// Note: In regular groups (non-supergroups), this method will only work if the ‘All Members Are Admins’ setting is off in the target group.
    static member SetChatPhoto
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            chatId: Types.ChatId,
            /// New chat photo
            photo: Types.FileToSend
        ) = Telegram.SetChatPhotoAsync(token, chatId, photo) |> Async.RunSynchronously

    /// Use this method to delete a chat photo. Photos can't be changed for private chats. The bot must be an administrator in the chat for this to work and must have the appropriate admin rights
    /// Note: In regular groups (non-supergroups), this method will only work if the ‘All Members Are Admins’ setting is off in the target group.
    static member DeleteChatPhotoAsync
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            chatId: Types.ChatId
        ) = Api.MakeRequestAsync<string>(token, "deleteChatPhoto", [ "chat_id", box (Tools.getChatIdString chatId) ])

    /// Use this method to delete a chat photo. Photos can't be changed for private chats. The bot must be an administrator in the chat for this to work and must have the appropriate admin rights
    /// Note: In regular groups (non-supergroups), this method will only work if the ‘All Members Are Admins’ setting is off in the target group.
    static member DeleteChatPhoto
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            chatId: Types.ChatId
        ) = Telegram.DeleteChatPhotoAsync(token, chatId) |> Async.RunSynchronously

    /// Use this method to change the title of a chat. Titles can't be changed for private chats. The bot must be an administrator in the chat for this to work and must have the appropriate admin rights. 
    /// Note: In regular groups (non-supergroups), this method will only work if the ‘All Members Are Admins’ setting is off in the target group.
    static member SetChatTitleAsync
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            chatId: Types.ChatId,
            /// New chat title, 1-255 characters
            title: string
        ) = Api.MakeRequestAsync<string>(token, 
                "setChatTitle", 
                [ "chat_id", box (Tools.getChatIdString chatId)
                  "title", box title ])

    /// Use this method to change the title of a chat. Titles can't be changed for private chats. The bot must be an administrator in the chat for this to work and must have the appropriate admin rights. 
    /// Note: In regular groups (non-supergroups), this method will only work if the ‘All Members Are Admins’ setting is off in the target group.
    static member SetChatTitle
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            chatId: Types.ChatId,
            /// New chat title, 1-255 characters
            title: string
        ) = Telegram.SetChatTitleAsync(token, chatId, title) |> Async.RunSynchronously

    /// Use this method to change the title of a chat. Titles can't be changed for private chats. The bot must be an administrator in the chat for this to work and must have the appropriate admin rights
    /// Note: In regular groups (non-supergroups), this method will only work if the ‘All Members Are Admins’ setting is off in the target group.
    static member SetChatDescriptionAsync
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            chatId: Types.ChatId,
            /// New chat description, 0-255 characters
            description: string
        ) = Api.MakeRequestAsync<string>(token, 
                "setChatDescription", 
                [ "chat_id", box (Tools.getChatIdString chatId)
                  "description", box description ])

    /// Use this method to change the title of a chat. Titles can't be changed for private chats. The bot must be an administrator in the chat for this to work and must have the appropriate admin rights
    /// Note: In regular groups (non-supergroups), this method will only work if the ‘All Members Are Admins’ setting is off in the target group.
    static member SetChatDescription
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target channel (in the format @channelusername)
            chatId: Types.ChatId,
            /// New chat description, 0-255 characters
            description: string
        ) = Telegram.SetChatDescriptionAsync(token, chatId, description) |> Async.RunSynchronously

    static member internal PinChatMessageBaseAsync
        (
            token: string,
            chatId: Types.ChatId,
            messageId: int,
            disableNotification: bool option
        ) = Api.MakeRequestAsync<bool>(token,
                "pinChatMessage",
                [ "chat_id", box (Tools.getChatIdString chatId)
                  "message_id", box messageId
                  "disable_notification", box disableNotification ])
    
    /// Use this method to pin a message in a supergroup. The bot must be an administrator in the chat for this to work and must have the appropriate admin rights
    static member PinChatMessageAsync
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target supergroup (in the format @supergroupusername)
            chatId: Types.ChatId,
            /// Identifier of a message to pin
            messageId: int,
            /// Pass True, if it is not necessary to send a notification to all group members about the new pinned message
            disableNotification: bool option
        ) = Telegram.PinChatMessageBaseAsync(token, chatId, messageId, disableNotification)
    
    /// Use this method to pin a message in a supergroup. The bot must be an administrator in the chat for this to work and must have the appropriate admin rights
    static member PinChatMessage
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target supergroup (in the format @supergroupusername)
            chatId: Types.ChatId,
            /// Identifier of a message to pin
            messageId: int,
            /// Pass True, if it is not necessary to send a notification to all group members about the new pinned message
            disableNotification: bool option
        ) = Telegram.PinChatMessageBaseAsync(token, chatId, messageId, disableNotification) |> Async.RunSynchronously
   
    /// Use this method to unpin a message in a supergroup chat. The bot must be an administrator in the chat for this to work and must have the appropriate admin rights
    static member UnpinChatMessageAsync
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target supergroup (in the format @supergroupusername)
            chatId: Types.ChatId
        ) = Api.MakeRequestAsync<bool>(token, "unpinChatMessage", [ "chat_id", box (Tools.getChatIdString chatId) ])
   
    /// Use this method to unpin a message in a supergroup chat. The bot must be an administrator in the chat for this to work and must have the appropriate admin rights
    static member UnpinChatMessage
        (   /// Bot token
            token: string,
            /// Unique identifier for the target chat or username of the target supergroup (in the format @supergroupusername)
            chatId: Types.ChatId
        ) = Telegram.UnpinChatMessageAsync(token, chatId) |> Async.RunSynchronously