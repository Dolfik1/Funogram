module Funogram.RequestsTypes

open Types

type IRequestBase<'a> = 
    abstract MethodName: string

type SendMessageReq = 
    { ChatId: ChatId
      Text: string
      ParseMode: ParseMode option
      DisableWebPagePreview: bool option
      DisableNotification: bool option
      ReplyToMessageId: int64 option
      ReplyMarkup: Markup option }
    interface IRequestBase<Message> with
        member x.MethodName = "sendMessage"
let internal sendMessageReqBase =
    { ChatId = ChatId.Int(0L); Text = ""; ParseMode = None;
    DisableWebPagePreview = None; DisableNotification = None;
    ReplyToMessageId = None; ReplyMarkup = None }

type GetMeReq() =
    interface IRequestBase<User> with
        member x.MethodName = "getMe"

type SendPhotoReq =
    { ChatId: ChatId
      Photo: FileToSend
      Caption: string option
      DisableNotification: bool option
      ReplyToMessageId: int64 option
      ReplyMarkup: Markup option }
    interface IRequestBase<Message> with
        member x.MethodName = "sendPhoto"

type GetChatReq =
    { ChatId: ChatId }
    interface IRequestBase<Chat> with
        member x.MethodName = "getChat"
    
type GetUserProfilePhotosReq =
    { UserId: int64
      Offset: int option
      Limit: int option }
    interface IRequestBase<UserProfilePhotos> with
        member x.MethodName = "getUserProfilePhotos"

type ForwardMessageReq =
    { ChatId: ChatId
      FromChatId: ChatId
      DisableNotification: bool option
      MessageId: int64 }
    interface IRequestBase<Message> with
        member x.MethodName = "forwardMessage"