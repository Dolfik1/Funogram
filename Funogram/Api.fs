module Funogram.Api

open Funogram.Types
open Funogram.RequestsTypes
open Funogram.Helpers

open System
open System.Reflection

let private getSnakeCaseName (name: string) =
    let chars =
        seq {
            let chars = name.ToCharArray()
            for i in 0 .. chars.Length - 1 do
                if i > 0 && Char.IsUpper(chars.[i]) then
                    yield '_'
                yield chars.[i]
        }

    let c = String.Concat(chars).ToLower()
    printfn "%s -> %s" name c
    c

let private getArgs (body: IRequestBase<'a>) =
    let props = body.GetType().GetTypeInfo().GetProperties() |> Array.toList
    props |> List.map (fun f -> (getSnakeCaseName f.Name, f.GetValue(body)))

let api (token: string) (body: IRequestBase<'a>) = 
    Api.MakeRequestAsync<'a> (token, body.MethodName, (getArgs body))

let sendMessage chatId text =
    { sendMessageReqBase with ChatId = ChatId.Int chatId; Text = text }

let sendMessageToChannel channelUsername text =
    { sendMessageReqBase with ChatId = ChatId.String channelUsername; Text = text }

let sendMessageBase chatId text parseMode disableWebPagePreview disableNotification replyToMessageId replyMarkup =
    { ChatId = chatId; Text = text; ParseMode = parseMode; 
    DisableWebPagePreview = disableWebPagePreview; DisableNotification = disableNotification; 
    ReplyToMessageId = replyToMessageId; ReplyMarkup = replyMarkup }

let sendMessageMarkup chatId text replyMarkup =
    { sendMessageReqBase with ChatId = ChatId.Int chatId; Text = text; ReplyMarkup = replyMarkup }

let sendMessageReply chatId text replyToMessageId =
    { sendMessageReqBase with ChatId = ChatId.Int chatId; Text = text; ReplyToMessageId = replyToMessageId }

let getMe = GetMeReq()

let sendPhotoBase chatId photo caption disableNotification replyToMessageId replyMarkup =
    { ChatId = chatId; Photo = photo; Caption = caption;
    DisableNotification = disableNotification;
    ReplyToMessageId = replyToMessageId; ReplyMarkup = replyMarkup }

let sendPhoto chatId photo caption =
    sendPhotoBase (ChatId.Int chatId) photo (Some caption) None None None

let getChat chatId = { ChatId = ChatId.Int chatId }
let getChatByName chatUsername = { ChatId = ChatId.String chatUsername }
let getUserProfilePhotosBase userId offset limit = 
    { UserId = userId; Offset = Some offset; Limit = Some limit; }
let getUserProfilePhotos userId = { UserId = userId; Offset = None; Limit = None; }
let forwardMessageBase chatId fromChatId messageId disableNotification =
    { ChatId = chatId; FromChatId = fromChatId; MessageId = messageId; DisableNotification = disableNotification }
let forwardMessage chatId fromChatId messageId = 
    forwardMessageBase (ChatId.Int chatId) (ChatId.Int fromChatId) messageId None