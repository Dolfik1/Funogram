[<Microsoft.FSharp.Core.RequireQualifiedAccess>]
module Funogram.Telegram.Req

open Funogram.Types
open Types
open System
    
type GetUpdates =
  {
    Offset: int64 option
    Limit: int64 option
    Timeout: int64 option
    AllowedUpdates: string[] option
  }
  static member Make(?offset: int64, ?limit: int64, ?timeout: int64, ?allowedUpdates: string[]) = 
    {
      Offset = offset
      Limit = limit
      Timeout = timeout
      AllowedUpdates = allowedUpdates
    }
  interface IRequestBase<Update[]> with
    member _.MethodName = "getUpdates"
    
type SetWebhook =
  {
    Url: string
    Certificate: InputFile option
    IpAddress: string option
    MaxConnections: int64 option
    AllowedUpdates: string[] option
    DropPendingUpdates: bool option
    SecretToken: string option
  }
  static member Make(url: string, ?certificate: InputFile, ?ipAddress: string, ?maxConnections: int64, ?allowedUpdates: string[], ?dropPendingUpdates: bool, ?secretToken: string) = 
    {
      Url = url
      Certificate = certificate
      IpAddress = ipAddress
      MaxConnections = maxConnections
      AllowedUpdates = allowedUpdates
      DropPendingUpdates = dropPendingUpdates
      SecretToken = secretToken
    }
  interface IRequestBase<bool> with
    member _.MethodName = "setWebhook"
    
type DeleteWebhook =
  {
    DropPendingUpdates: bool option
  }
  static member Make(?dropPendingUpdates: bool) = 
    {
      DropPendingUpdates = dropPendingUpdates
    }
  interface IRequestBase<bool> with
    member _.MethodName = "deleteWebhook"
    
type GetWebhookInfo() =
  static member Make() = GetWebhookInfo()
  interface IRequestBase<WebhookInfo> with
    member _.MethodName = "getWebhookInfo"
    
type GetMe() =
  static member Make() = GetMe()
  interface IRequestBase<User> with
    member _.MethodName = "getMe"
    
type LogOut() =
  static member Make() = LogOut()
  interface IRequestBase<bool> with
    member _.MethodName = "logOut"
    
type Close() =
  static member Make() = Close()
  interface IRequestBase<bool> with
    member _.MethodName = "close"
    
type SendMessage =
  {
    ChatId: ChatId
    MessageThreadId: int64 option
    Text: string
    ParseMode: ParseMode option
    Entities: MessageEntity[] option
    LinkPreviewOptions: LinkPreviewOptions option
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyParameters: ReplyParameters option
    ReplyMarkup: Markup option
  }
  static member Make(chatId: ChatId, text: string, ?messageThreadId: int64, ?parseMode: ParseMode, ?entities: MessageEntity[], ?linkPreviewOptions: LinkPreviewOptions, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    {
      ChatId = chatId
      MessageThreadId = messageThreadId
      Text = text
      ParseMode = parseMode
      Entities = entities
      LinkPreviewOptions = linkPreviewOptions
      DisableNotification = disableNotification
      ProtectContent = protectContent
      ReplyParameters = replyParameters
      ReplyMarkup = replyMarkup
    }
  static member Make(chatId: int64, text: string, ?messageThreadId: int64, ?parseMode: ParseMode, ?entities: MessageEntity[], ?linkPreviewOptions: LinkPreviewOptions, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    SendMessage.Make(ChatId.Int chatId, text, ?messageThreadId = messageThreadId, ?parseMode = parseMode, ?entities = entities, ?linkPreviewOptions = linkPreviewOptions, ?disableNotification = disableNotification, ?protectContent = protectContent, ?replyParameters = replyParameters, ?replyMarkup = replyMarkup)
  static member Make(chatId: string, text: string, ?messageThreadId: int64, ?parseMode: ParseMode, ?entities: MessageEntity[], ?linkPreviewOptions: LinkPreviewOptions, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    SendMessage.Make(ChatId.String chatId, text, ?messageThreadId = messageThreadId, ?parseMode = parseMode, ?entities = entities, ?linkPreviewOptions = linkPreviewOptions, ?disableNotification = disableNotification, ?protectContent = protectContent, ?replyParameters = replyParameters, ?replyMarkup = replyMarkup)
  interface IRequestBase<Message> with
    member _.MethodName = "sendMessage"
    
type ForwardMessage =
  {
    ChatId: ChatId
    MessageThreadId: int64 option
    FromChatId: ChatId
    DisableNotification: bool option
    ProtectContent: bool option
    MessageId: int64
  }
  static member Make(chatId: ChatId, fromChatId: ChatId, messageId: int64, ?messageThreadId: int64, ?disableNotification: bool, ?protectContent: bool) = 
    {
      ChatId = chatId
      MessageThreadId = messageThreadId
      FromChatId = fromChatId
      DisableNotification = disableNotification
      ProtectContent = protectContent
      MessageId = messageId
    }
  static member Make(chatId: int64, fromChatId: int64, messageId: int64, ?messageThreadId: int64, ?disableNotification: bool, ?protectContent: bool) = 
    ForwardMessage.Make(ChatId.Int chatId, ChatId.Int fromChatId, messageId, ?messageThreadId = messageThreadId, ?disableNotification = disableNotification, ?protectContent = protectContent)
  static member Make(chatId: string, fromChatId: string, messageId: int64, ?messageThreadId: int64, ?disableNotification: bool, ?protectContent: bool) = 
    ForwardMessage.Make(ChatId.String chatId, ChatId.String fromChatId, messageId, ?messageThreadId = messageThreadId, ?disableNotification = disableNotification, ?protectContent = protectContent)
  interface IRequestBase<Message> with
    member _.MethodName = "forwardMessage"
    
type ForwardMessages =
  {
    ChatId: ChatId
    MessageThreadId: int64 option
    FromChatId: ChatId
    MessageIds: int64[]
    DisableNotification: bool option
    ProtectContent: bool option
  }
  static member Make(chatId: ChatId, fromChatId: ChatId, messageIds: int64[], ?messageThreadId: int64, ?disableNotification: bool, ?protectContent: bool) = 
    {
      ChatId = chatId
      MessageThreadId = messageThreadId
      FromChatId = fromChatId
      MessageIds = messageIds
      DisableNotification = disableNotification
      ProtectContent = protectContent
    }
  static member Make(chatId: int64, fromChatId: int64, messageIds: int64[], ?messageThreadId: int64, ?disableNotification: bool, ?protectContent: bool) = 
    ForwardMessages.Make(ChatId.Int chatId, ChatId.Int fromChatId, messageIds, ?messageThreadId = messageThreadId, ?disableNotification = disableNotification, ?protectContent = protectContent)
  static member Make(chatId: string, fromChatId: string, messageIds: int64[], ?messageThreadId: int64, ?disableNotification: bool, ?protectContent: bool) = 
    ForwardMessages.Make(ChatId.String chatId, ChatId.String fromChatId, messageIds, ?messageThreadId = messageThreadId, ?disableNotification = disableNotification, ?protectContent = protectContent)
  interface IRequestBase<MessageId[]> with
    member _.MethodName = "forwardMessages"
    
type CopyMessage =
  {
    ChatId: ChatId
    MessageThreadId: int64 option
    FromChatId: ChatId
    MessageId: int64
    Caption: string option
    ParseMode: ParseMode option
    CaptionEntities: MessageEntity[] option
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyParameters: ReplyParameters option
    ReplyMarkup: Markup option
  }
  static member Make(chatId: ChatId, fromChatId: ChatId, messageId: int64, ?messageThreadId: int64, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    {
      ChatId = chatId
      MessageThreadId = messageThreadId
      FromChatId = fromChatId
      MessageId = messageId
      Caption = caption
      ParseMode = parseMode
      CaptionEntities = captionEntities
      DisableNotification = disableNotification
      ProtectContent = protectContent
      ReplyParameters = replyParameters
      ReplyMarkup = replyMarkup
    }
  static member Make(chatId: int64, fromChatId: int64, messageId: int64, ?messageThreadId: int64, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    CopyMessage.Make(ChatId.Int chatId, ChatId.Int fromChatId, messageId, ?messageThreadId = messageThreadId, ?caption = caption, ?parseMode = parseMode, ?captionEntities = captionEntities, ?disableNotification = disableNotification, ?protectContent = protectContent, ?replyParameters = replyParameters, ?replyMarkup = replyMarkup)
  static member Make(chatId: string, fromChatId: string, messageId: int64, ?messageThreadId: int64, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    CopyMessage.Make(ChatId.String chatId, ChatId.String fromChatId, messageId, ?messageThreadId = messageThreadId, ?caption = caption, ?parseMode = parseMode, ?captionEntities = captionEntities, ?disableNotification = disableNotification, ?protectContent = protectContent, ?replyParameters = replyParameters, ?replyMarkup = replyMarkup)
  interface IRequestBase<MessageId> with
    member _.MethodName = "copyMessage"
    
type CopyMessages =
  {
    ChatId: ChatId
    MessageThreadId: int64 option
    FromChatId: ChatId
    MessageIds: int64[]
    DisableNotification: bool option
    ProtectContent: bool option
    RemoveCaption: bool option
  }
  static member Make(chatId: ChatId, fromChatId: ChatId, messageIds: int64[], ?messageThreadId: int64, ?disableNotification: bool, ?protectContent: bool, ?removeCaption: bool) = 
    {
      ChatId = chatId
      MessageThreadId = messageThreadId
      FromChatId = fromChatId
      MessageIds = messageIds
      DisableNotification = disableNotification
      ProtectContent = protectContent
      RemoveCaption = removeCaption
    }
  static member Make(chatId: int64, fromChatId: int64, messageIds: int64[], ?messageThreadId: int64, ?disableNotification: bool, ?protectContent: bool, ?removeCaption: bool) = 
    CopyMessages.Make(ChatId.Int chatId, ChatId.Int fromChatId, messageIds, ?messageThreadId = messageThreadId, ?disableNotification = disableNotification, ?protectContent = protectContent, ?removeCaption = removeCaption)
  static member Make(chatId: string, fromChatId: string, messageIds: int64[], ?messageThreadId: int64, ?disableNotification: bool, ?protectContent: bool, ?removeCaption: bool) = 
    CopyMessages.Make(ChatId.String chatId, ChatId.String fromChatId, messageIds, ?messageThreadId = messageThreadId, ?disableNotification = disableNotification, ?protectContent = protectContent, ?removeCaption = removeCaption)
  interface IRequestBase<MessageId[]> with
    member _.MethodName = "copyMessages"
    
type SendPhoto =
  {
    ChatId: ChatId
    MessageThreadId: int64 option
    Photo: InputFile
    Caption: string option
    ParseMode: ParseMode option
    CaptionEntities: MessageEntity[] option
    HasSpoiler: bool option
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyParameters: ReplyParameters option
    ReplyMarkup: Markup option
  }
  static member Make(chatId: ChatId, photo: InputFile, ?messageThreadId: int64, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?hasSpoiler: bool, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    {
      ChatId = chatId
      MessageThreadId = messageThreadId
      Photo = photo
      Caption = caption
      ParseMode = parseMode
      CaptionEntities = captionEntities
      HasSpoiler = hasSpoiler
      DisableNotification = disableNotification
      ProtectContent = protectContent
      ReplyParameters = replyParameters
      ReplyMarkup = replyMarkup
    }
  static member Make(chatId: int64, photo: InputFile, ?messageThreadId: int64, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?hasSpoiler: bool, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    SendPhoto.Make(ChatId.Int chatId, photo, ?messageThreadId = messageThreadId, ?caption = caption, ?parseMode = parseMode, ?captionEntities = captionEntities, ?hasSpoiler = hasSpoiler, ?disableNotification = disableNotification, ?protectContent = protectContent, ?replyParameters = replyParameters, ?replyMarkup = replyMarkup)
  static member Make(chatId: string, photo: InputFile, ?messageThreadId: int64, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?hasSpoiler: bool, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    SendPhoto.Make(ChatId.String chatId, photo, ?messageThreadId = messageThreadId, ?caption = caption, ?parseMode = parseMode, ?captionEntities = captionEntities, ?hasSpoiler = hasSpoiler, ?disableNotification = disableNotification, ?protectContent = protectContent, ?replyParameters = replyParameters, ?replyMarkup = replyMarkup)
  interface IRequestBase<Message> with
    member _.MethodName = "sendPhoto"
    
type SendAudio =
  {
    ChatId: ChatId
    MessageThreadId: int64 option
    Audio: InputFile
    Caption: string option
    ParseMode: ParseMode option
    CaptionEntities: MessageEntity[] option
    Duration: int64 option
    Performer: string option
    Title: string option
    Thumbnail: InputFile option
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyParameters: ReplyParameters option
    ReplyMarkup: Markup option
  }
  static member Make(chatId: ChatId, audio: InputFile, ?messageThreadId: int64, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?duration: int64, ?performer: string, ?title: string, ?thumbnail: InputFile, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    {
      ChatId = chatId
      MessageThreadId = messageThreadId
      Audio = audio
      Caption = caption
      ParseMode = parseMode
      CaptionEntities = captionEntities
      Duration = duration
      Performer = performer
      Title = title
      Thumbnail = thumbnail
      DisableNotification = disableNotification
      ProtectContent = protectContent
      ReplyParameters = replyParameters
      ReplyMarkup = replyMarkup
    }
  static member Make(chatId: int64, audio: InputFile, ?messageThreadId: int64, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?duration: int64, ?performer: string, ?title: string, ?thumbnail: InputFile, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    SendAudio.Make(ChatId.Int chatId, audio, ?messageThreadId = messageThreadId, ?caption = caption, ?parseMode = parseMode, ?captionEntities = captionEntities, ?duration = duration, ?performer = performer, ?title = title, ?thumbnail = thumbnail, ?disableNotification = disableNotification, ?protectContent = protectContent, ?replyParameters = replyParameters, ?replyMarkup = replyMarkup)
  static member Make(chatId: string, audio: InputFile, ?messageThreadId: int64, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?duration: int64, ?performer: string, ?title: string, ?thumbnail: InputFile, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    SendAudio.Make(ChatId.String chatId, audio, ?messageThreadId = messageThreadId, ?caption = caption, ?parseMode = parseMode, ?captionEntities = captionEntities, ?duration = duration, ?performer = performer, ?title = title, ?thumbnail = thumbnail, ?disableNotification = disableNotification, ?protectContent = protectContent, ?replyParameters = replyParameters, ?replyMarkup = replyMarkup)
  interface IRequestBase<Message> with
    member _.MethodName = "sendAudio"
    
type SendDocument =
  {
    ChatId: ChatId
    MessageThreadId: int64 option
    Document: InputFile
    Thumbnail: InputFile option
    Caption: string option
    ParseMode: ParseMode option
    CaptionEntities: MessageEntity[] option
    DisableContentTypeDetection: bool option
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyParameters: ReplyParameters option
    ReplyMarkup: Markup option
  }
  static member Make(chatId: ChatId, document: InputFile, ?messageThreadId: int64, ?thumbnail: InputFile, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?disableContentTypeDetection: bool, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    {
      ChatId = chatId
      MessageThreadId = messageThreadId
      Document = document
      Thumbnail = thumbnail
      Caption = caption
      ParseMode = parseMode
      CaptionEntities = captionEntities
      DisableContentTypeDetection = disableContentTypeDetection
      DisableNotification = disableNotification
      ProtectContent = protectContent
      ReplyParameters = replyParameters
      ReplyMarkup = replyMarkup
    }
  static member Make(chatId: int64, document: InputFile, ?messageThreadId: int64, ?thumbnail: InputFile, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?disableContentTypeDetection: bool, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    SendDocument.Make(ChatId.Int chatId, document, ?messageThreadId = messageThreadId, ?thumbnail = thumbnail, ?caption = caption, ?parseMode = parseMode, ?captionEntities = captionEntities, ?disableContentTypeDetection = disableContentTypeDetection, ?disableNotification = disableNotification, ?protectContent = protectContent, ?replyParameters = replyParameters, ?replyMarkup = replyMarkup)
  static member Make(chatId: string, document: InputFile, ?messageThreadId: int64, ?thumbnail: InputFile, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?disableContentTypeDetection: bool, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    SendDocument.Make(ChatId.String chatId, document, ?messageThreadId = messageThreadId, ?thumbnail = thumbnail, ?caption = caption, ?parseMode = parseMode, ?captionEntities = captionEntities, ?disableContentTypeDetection = disableContentTypeDetection, ?disableNotification = disableNotification, ?protectContent = protectContent, ?replyParameters = replyParameters, ?replyMarkup = replyMarkup)
  interface IRequestBase<Message> with
    member _.MethodName = "sendDocument"
    
type SendVideo =
  {
    ChatId: ChatId
    MessageThreadId: int64 option
    Video: InputFile
    Duration: int64 option
    Width: int64 option
    Height: int64 option
    Thumbnail: InputFile option
    Caption: string option
    ParseMode: ParseMode option
    CaptionEntities: MessageEntity[] option
    HasSpoiler: bool option
    SupportsStreaming: bool option
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyParameters: ReplyParameters option
    ReplyMarkup: Markup option
  }
  static member Make(chatId: ChatId, video: InputFile, ?messageThreadId: int64, ?duration: int64, ?width: int64, ?height: int64, ?thumbnail: InputFile, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?hasSpoiler: bool, ?supportsStreaming: bool, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    {
      ChatId = chatId
      MessageThreadId = messageThreadId
      Video = video
      Duration = duration
      Width = width
      Height = height
      Thumbnail = thumbnail
      Caption = caption
      ParseMode = parseMode
      CaptionEntities = captionEntities
      HasSpoiler = hasSpoiler
      SupportsStreaming = supportsStreaming
      DisableNotification = disableNotification
      ProtectContent = protectContent
      ReplyParameters = replyParameters
      ReplyMarkup = replyMarkup
    }
  static member Make(chatId: int64, video: InputFile, ?messageThreadId: int64, ?duration: int64, ?width: int64, ?height: int64, ?thumbnail: InputFile, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?hasSpoiler: bool, ?supportsStreaming: bool, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    SendVideo.Make(ChatId.Int chatId, video, ?messageThreadId = messageThreadId, ?duration = duration, ?width = width, ?height = height, ?thumbnail = thumbnail, ?caption = caption, ?parseMode = parseMode, ?captionEntities = captionEntities, ?hasSpoiler = hasSpoiler, ?supportsStreaming = supportsStreaming, ?disableNotification = disableNotification, ?protectContent = protectContent, ?replyParameters = replyParameters, ?replyMarkup = replyMarkup)
  static member Make(chatId: string, video: InputFile, ?messageThreadId: int64, ?duration: int64, ?width: int64, ?height: int64, ?thumbnail: InputFile, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?hasSpoiler: bool, ?supportsStreaming: bool, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    SendVideo.Make(ChatId.String chatId, video, ?messageThreadId = messageThreadId, ?duration = duration, ?width = width, ?height = height, ?thumbnail = thumbnail, ?caption = caption, ?parseMode = parseMode, ?captionEntities = captionEntities, ?hasSpoiler = hasSpoiler, ?supportsStreaming = supportsStreaming, ?disableNotification = disableNotification, ?protectContent = protectContent, ?replyParameters = replyParameters, ?replyMarkup = replyMarkup)
  interface IRequestBase<Message> with
    member _.MethodName = "sendVideo"
    
type SendAnimation =
  {
    ChatId: ChatId
    MessageThreadId: int64 option
    Animation: InputFile
    Duration: int64 option
    Width: int64 option
    Height: int64 option
    Thumbnail: InputFile option
    Caption: string option
    ParseMode: ParseMode option
    CaptionEntities: MessageEntity[] option
    HasSpoiler: bool option
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyParameters: ReplyParameters option
    ReplyMarkup: Markup option
  }
  static member Make(chatId: ChatId, animation: InputFile, ?messageThreadId: int64, ?duration: int64, ?width: int64, ?height: int64, ?thumbnail: InputFile, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?hasSpoiler: bool, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    {
      ChatId = chatId
      MessageThreadId = messageThreadId
      Animation = animation
      Duration = duration
      Width = width
      Height = height
      Thumbnail = thumbnail
      Caption = caption
      ParseMode = parseMode
      CaptionEntities = captionEntities
      HasSpoiler = hasSpoiler
      DisableNotification = disableNotification
      ProtectContent = protectContent
      ReplyParameters = replyParameters
      ReplyMarkup = replyMarkup
    }
  static member Make(chatId: int64, animation: InputFile, ?messageThreadId: int64, ?duration: int64, ?width: int64, ?height: int64, ?thumbnail: InputFile, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?hasSpoiler: bool, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    SendAnimation.Make(ChatId.Int chatId, animation, ?messageThreadId = messageThreadId, ?duration = duration, ?width = width, ?height = height, ?thumbnail = thumbnail, ?caption = caption, ?parseMode = parseMode, ?captionEntities = captionEntities, ?hasSpoiler = hasSpoiler, ?disableNotification = disableNotification, ?protectContent = protectContent, ?replyParameters = replyParameters, ?replyMarkup = replyMarkup)
  static member Make(chatId: string, animation: InputFile, ?messageThreadId: int64, ?duration: int64, ?width: int64, ?height: int64, ?thumbnail: InputFile, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?hasSpoiler: bool, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    SendAnimation.Make(ChatId.String chatId, animation, ?messageThreadId = messageThreadId, ?duration = duration, ?width = width, ?height = height, ?thumbnail = thumbnail, ?caption = caption, ?parseMode = parseMode, ?captionEntities = captionEntities, ?hasSpoiler = hasSpoiler, ?disableNotification = disableNotification, ?protectContent = protectContent, ?replyParameters = replyParameters, ?replyMarkup = replyMarkup)
  interface IRequestBase<Message> with
    member _.MethodName = "sendAnimation"
    
type SendVoice =
  {
    ChatId: ChatId
    MessageThreadId: int64 option
    Voice: InputFile
    Caption: string option
    ParseMode: ParseMode option
    CaptionEntities: MessageEntity[] option
    Duration: int64 option
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyParameters: ReplyParameters option
    ReplyMarkup: Markup option
  }
  static member Make(chatId: ChatId, voice: InputFile, ?messageThreadId: int64, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?duration: int64, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    {
      ChatId = chatId
      MessageThreadId = messageThreadId
      Voice = voice
      Caption = caption
      ParseMode = parseMode
      CaptionEntities = captionEntities
      Duration = duration
      DisableNotification = disableNotification
      ProtectContent = protectContent
      ReplyParameters = replyParameters
      ReplyMarkup = replyMarkup
    }
  static member Make(chatId: int64, voice: InputFile, ?messageThreadId: int64, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?duration: int64, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    SendVoice.Make(ChatId.Int chatId, voice, ?messageThreadId = messageThreadId, ?caption = caption, ?parseMode = parseMode, ?captionEntities = captionEntities, ?duration = duration, ?disableNotification = disableNotification, ?protectContent = protectContent, ?replyParameters = replyParameters, ?replyMarkup = replyMarkup)
  static member Make(chatId: string, voice: InputFile, ?messageThreadId: int64, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?duration: int64, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    SendVoice.Make(ChatId.String chatId, voice, ?messageThreadId = messageThreadId, ?caption = caption, ?parseMode = parseMode, ?captionEntities = captionEntities, ?duration = duration, ?disableNotification = disableNotification, ?protectContent = protectContent, ?replyParameters = replyParameters, ?replyMarkup = replyMarkup)
  interface IRequestBase<Message> with
    member _.MethodName = "sendVoice"
    
type SendVideoNote =
  {
    ChatId: ChatId
    MessageThreadId: int64 option
    VideoNote: InputFile
    Duration: int64 option
    Length: int64 option
    Thumbnail: InputFile option
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyParameters: ReplyParameters option
    ReplyMarkup: Markup option
  }
  static member Make(chatId: ChatId, videoNote: InputFile, ?messageThreadId: int64, ?duration: int64, ?length: int64, ?thumbnail: InputFile, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    {
      ChatId = chatId
      MessageThreadId = messageThreadId
      VideoNote = videoNote
      Duration = duration
      Length = length
      Thumbnail = thumbnail
      DisableNotification = disableNotification
      ProtectContent = protectContent
      ReplyParameters = replyParameters
      ReplyMarkup = replyMarkup
    }
  static member Make(chatId: int64, videoNote: InputFile, ?messageThreadId: int64, ?duration: int64, ?length: int64, ?thumbnail: InputFile, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    SendVideoNote.Make(ChatId.Int chatId, videoNote, ?messageThreadId = messageThreadId, ?duration = duration, ?length = length, ?thumbnail = thumbnail, ?disableNotification = disableNotification, ?protectContent = protectContent, ?replyParameters = replyParameters, ?replyMarkup = replyMarkup)
  static member Make(chatId: string, videoNote: InputFile, ?messageThreadId: int64, ?duration: int64, ?length: int64, ?thumbnail: InputFile, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    SendVideoNote.Make(ChatId.String chatId, videoNote, ?messageThreadId = messageThreadId, ?duration = duration, ?length = length, ?thumbnail = thumbnail, ?disableNotification = disableNotification, ?protectContent = protectContent, ?replyParameters = replyParameters, ?replyMarkup = replyMarkup)
  interface IRequestBase<Message> with
    member _.MethodName = "sendVideoNote"
    
type SendMediaGroup =
  {
    ChatId: ChatId
    MessageThreadId: int64 option
    Media: InputMedia[]
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyParameters: ReplyParameters option
  }
  static member Make(chatId: ChatId, media: InputMedia[], ?messageThreadId: int64, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters) = 
    {
      ChatId = chatId
      MessageThreadId = messageThreadId
      Media = media
      DisableNotification = disableNotification
      ProtectContent = protectContent
      ReplyParameters = replyParameters
    }
  static member Make(chatId: int64, media: InputMedia[], ?messageThreadId: int64, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters) = 
    SendMediaGroup.Make(ChatId.Int chatId, media, ?messageThreadId = messageThreadId, ?disableNotification = disableNotification, ?protectContent = protectContent, ?replyParameters = replyParameters)
  static member Make(chatId: string, media: InputMedia[], ?messageThreadId: int64, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters) = 
    SendMediaGroup.Make(ChatId.String chatId, media, ?messageThreadId = messageThreadId, ?disableNotification = disableNotification, ?protectContent = protectContent, ?replyParameters = replyParameters)
  interface IRequestBase<Message[]> with
    member _.MethodName = "sendMediaGroup"
    
type SendLocation =
  {
    ChatId: ChatId
    MessageThreadId: int64 option
    Latitude: float
    Longitude: float
    HorizontalAccuracy: float option
    LivePeriod: int64 option
    Heading: int64 option
    ProximityAlertRadius: int64 option
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyParameters: ReplyParameters option
    ReplyMarkup: Markup option
  }
  static member Make(chatId: ChatId, latitude: float, longitude: float, ?messageThreadId: int64, ?horizontalAccuracy: float, ?livePeriod: int64, ?heading: int64, ?proximityAlertRadius: int64, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    {
      ChatId = chatId
      MessageThreadId = messageThreadId
      Latitude = latitude
      Longitude = longitude
      HorizontalAccuracy = horizontalAccuracy
      LivePeriod = livePeriod
      Heading = heading
      ProximityAlertRadius = proximityAlertRadius
      DisableNotification = disableNotification
      ProtectContent = protectContent
      ReplyParameters = replyParameters
      ReplyMarkup = replyMarkup
    }
  static member Make(chatId: int64, latitude: float, longitude: float, ?messageThreadId: int64, ?horizontalAccuracy: float, ?livePeriod: int64, ?heading: int64, ?proximityAlertRadius: int64, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    SendLocation.Make(ChatId.Int chatId, latitude, longitude, ?messageThreadId = messageThreadId, ?horizontalAccuracy = horizontalAccuracy, ?livePeriod = livePeriod, ?heading = heading, ?proximityAlertRadius = proximityAlertRadius, ?disableNotification = disableNotification, ?protectContent = protectContent, ?replyParameters = replyParameters, ?replyMarkup = replyMarkup)
  static member Make(chatId: string, latitude: float, longitude: float, ?messageThreadId: int64, ?horizontalAccuracy: float, ?livePeriod: int64, ?heading: int64, ?proximityAlertRadius: int64, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    SendLocation.Make(ChatId.String chatId, latitude, longitude, ?messageThreadId = messageThreadId, ?horizontalAccuracy = horizontalAccuracy, ?livePeriod = livePeriod, ?heading = heading, ?proximityAlertRadius = proximityAlertRadius, ?disableNotification = disableNotification, ?protectContent = protectContent, ?replyParameters = replyParameters, ?replyMarkup = replyMarkup)
  interface IRequestBase<Message> with
    member _.MethodName = "sendLocation"
    
type SendVenue =
  {
    ChatId: ChatId
    MessageThreadId: int64 option
    Latitude: float
    Longitude: float
    Title: string
    Address: string
    FoursquareId: string option
    FoursquareType: string option
    GooglePlaceId: string option
    GooglePlaceType: string option
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyParameters: ReplyParameters option
    ReplyMarkup: Markup option
  }
  static member Make(chatId: ChatId, latitude: float, longitude: float, title: string, address: string, ?messageThreadId: int64, ?foursquareId: string, ?foursquareType: string, ?googlePlaceId: string, ?googlePlaceType: string, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    {
      ChatId = chatId
      MessageThreadId = messageThreadId
      Latitude = latitude
      Longitude = longitude
      Title = title
      Address = address
      FoursquareId = foursquareId
      FoursquareType = foursquareType
      GooglePlaceId = googlePlaceId
      GooglePlaceType = googlePlaceType
      DisableNotification = disableNotification
      ProtectContent = protectContent
      ReplyParameters = replyParameters
      ReplyMarkup = replyMarkup
    }
  static member Make(chatId: int64, latitude: float, longitude: float, title: string, address: string, ?messageThreadId: int64, ?foursquareId: string, ?foursquareType: string, ?googlePlaceId: string, ?googlePlaceType: string, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    SendVenue.Make(ChatId.Int chatId, latitude, longitude, title, address, ?messageThreadId = messageThreadId, ?foursquareId = foursquareId, ?foursquareType = foursquareType, ?googlePlaceId = googlePlaceId, ?googlePlaceType = googlePlaceType, ?disableNotification = disableNotification, ?protectContent = protectContent, ?replyParameters = replyParameters, ?replyMarkup = replyMarkup)
  static member Make(chatId: string, latitude: float, longitude: float, title: string, address: string, ?messageThreadId: int64, ?foursquareId: string, ?foursquareType: string, ?googlePlaceId: string, ?googlePlaceType: string, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    SendVenue.Make(ChatId.String chatId, latitude, longitude, title, address, ?messageThreadId = messageThreadId, ?foursquareId = foursquareId, ?foursquareType = foursquareType, ?googlePlaceId = googlePlaceId, ?googlePlaceType = googlePlaceType, ?disableNotification = disableNotification, ?protectContent = protectContent, ?replyParameters = replyParameters, ?replyMarkup = replyMarkup)
  interface IRequestBase<Message> with
    member _.MethodName = "sendVenue"
    
type SendContact =
  {
    ChatId: ChatId
    MessageThreadId: int64 option
    PhoneNumber: string
    FirstName: string
    LastName: string option
    Vcard: string option
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyParameters: ReplyParameters option
    ReplyMarkup: Markup option
  }
  static member Make(chatId: ChatId, phoneNumber: string, firstName: string, ?messageThreadId: int64, ?lastName: string, ?vcard: string, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    {
      ChatId = chatId
      MessageThreadId = messageThreadId
      PhoneNumber = phoneNumber
      FirstName = firstName
      LastName = lastName
      Vcard = vcard
      DisableNotification = disableNotification
      ProtectContent = protectContent
      ReplyParameters = replyParameters
      ReplyMarkup = replyMarkup
    }
  static member Make(chatId: int64, phoneNumber: string, firstName: string, ?messageThreadId: int64, ?lastName: string, ?vcard: string, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    SendContact.Make(ChatId.Int chatId, phoneNumber, firstName, ?messageThreadId = messageThreadId, ?lastName = lastName, ?vcard = vcard, ?disableNotification = disableNotification, ?protectContent = protectContent, ?replyParameters = replyParameters, ?replyMarkup = replyMarkup)
  static member Make(chatId: string, phoneNumber: string, firstName: string, ?messageThreadId: int64, ?lastName: string, ?vcard: string, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    SendContact.Make(ChatId.String chatId, phoneNumber, firstName, ?messageThreadId = messageThreadId, ?lastName = lastName, ?vcard = vcard, ?disableNotification = disableNotification, ?protectContent = protectContent, ?replyParameters = replyParameters, ?replyMarkup = replyMarkup)
  interface IRequestBase<Message> with
    member _.MethodName = "sendContact"
    
type SendPoll =
  {
    ChatId: ChatId
    MessageThreadId: int64 option
    Question: string
    Options: string[]
    IsAnonymous: bool option
    Type: string option
    AllowsMultipleAnswers: bool option
    CorrectOptionId: int64 option
    Explanation: string option
    ExplanationParseMode: string option
    ExplanationEntities: MessageEntity[] option
    OpenPeriod: int64 option
    CloseDate: int64 option
    IsClosed: bool option
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyParameters: ReplyParameters option
    ReplyMarkup: Markup option
  }
  static member Make(chatId: ChatId, question: string, options: string[], ?protectContent: bool, ?disableNotification: bool, ?isClosed: bool, ?closeDate: int64, ?openPeriod: int64, ?explanationEntities: MessageEntity[], ?explanation: string, ?replyParameters: ReplyParameters, ?correctOptionId: int64, ?allowsMultipleAnswers: bool, ?``type``: string, ?isAnonymous: bool, ?messageThreadId: int64, ?explanationParseMode: string, ?replyMarkup: Markup) = 
    {
      ChatId = chatId
      MessageThreadId = messageThreadId
      Question = question
      Options = options
      IsAnonymous = isAnonymous
      Type = ``type``
      AllowsMultipleAnswers = allowsMultipleAnswers
      CorrectOptionId = correctOptionId
      Explanation = explanation
      ExplanationParseMode = explanationParseMode
      ExplanationEntities = explanationEntities
      OpenPeriod = openPeriod
      CloseDate = closeDate
      IsClosed = isClosed
      DisableNotification = disableNotification
      ProtectContent = protectContent
      ReplyParameters = replyParameters
      ReplyMarkup = replyMarkup
    }
  static member Make(chatId: int64, question: string, options: string[], ?protectContent: bool, ?disableNotification: bool, ?isClosed: bool, ?closeDate: int64, ?openPeriod: int64, ?explanationEntities: MessageEntity[], ?explanation: string, ?replyParameters: ReplyParameters, ?correctOptionId: int64, ?allowsMultipleAnswers: bool, ?``type``: string, ?isAnonymous: bool, ?messageThreadId: int64, ?explanationParseMode: string, ?replyMarkup: Markup) = 
    SendPoll.Make(ChatId.Int chatId, question, options, ?protectContent = protectContent, ?disableNotification = disableNotification, ?isClosed = isClosed, ?closeDate = closeDate, ?openPeriod = openPeriod, ?explanationEntities = explanationEntities, ?explanation = explanation, ?replyParameters = replyParameters, ?correctOptionId = correctOptionId, ?allowsMultipleAnswers = allowsMultipleAnswers, ?``type`` = ``type``, ?isAnonymous = isAnonymous, ?messageThreadId = messageThreadId, ?explanationParseMode = explanationParseMode, ?replyMarkup = replyMarkup)
  static member Make(chatId: string, question: string, options: string[], ?protectContent: bool, ?disableNotification: bool, ?isClosed: bool, ?closeDate: int64, ?openPeriod: int64, ?explanationEntities: MessageEntity[], ?explanation: string, ?replyParameters: ReplyParameters, ?correctOptionId: int64, ?allowsMultipleAnswers: bool, ?``type``: string, ?isAnonymous: bool, ?messageThreadId: int64, ?explanationParseMode: string, ?replyMarkup: Markup) = 
    SendPoll.Make(ChatId.String chatId, question, options, ?protectContent = protectContent, ?disableNotification = disableNotification, ?isClosed = isClosed, ?closeDate = closeDate, ?openPeriod = openPeriod, ?explanationEntities = explanationEntities, ?explanation = explanation, ?replyParameters = replyParameters, ?correctOptionId = correctOptionId, ?allowsMultipleAnswers = allowsMultipleAnswers, ?``type`` = ``type``, ?isAnonymous = isAnonymous, ?messageThreadId = messageThreadId, ?explanationParseMode = explanationParseMode, ?replyMarkup = replyMarkup)
  interface IRequestBase<Message> with
    member _.MethodName = "sendPoll"
    
type SendDice =
  {
    ChatId: ChatId
    MessageThreadId: int64 option
    Emoji: string option
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyParameters: ReplyParameters option
    ReplyMarkup: Markup option
  }
  static member Make(chatId: ChatId, ?messageThreadId: int64, ?emoji: string, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    {
      ChatId = chatId
      MessageThreadId = messageThreadId
      Emoji = emoji
      DisableNotification = disableNotification
      ProtectContent = protectContent
      ReplyParameters = replyParameters
      ReplyMarkup = replyMarkup
    }
  static member Make(chatId: int64, ?messageThreadId: int64, ?emoji: string, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    SendDice.Make(ChatId.Int chatId, ?messageThreadId = messageThreadId, ?emoji = emoji, ?disableNotification = disableNotification, ?protectContent = protectContent, ?replyParameters = replyParameters, ?replyMarkup = replyMarkup)
  static member Make(chatId: string, ?messageThreadId: int64, ?emoji: string, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    SendDice.Make(ChatId.String chatId, ?messageThreadId = messageThreadId, ?emoji = emoji, ?disableNotification = disableNotification, ?protectContent = protectContent, ?replyParameters = replyParameters, ?replyMarkup = replyMarkup)
  interface IRequestBase<Message> with
    member _.MethodName = "sendDice"
    
type SendChatAction =
  {
    ChatId: ChatId
    MessageThreadId: int64 option
    Action: ChatAction
  }
  static member Make(chatId: ChatId, action: ChatAction, ?messageThreadId: int64) = 
    {
      ChatId = chatId
      MessageThreadId = messageThreadId
      Action = action
    }
  static member Make(chatId: int64, action: ChatAction, ?messageThreadId: int64) = 
    SendChatAction.Make(ChatId.Int chatId, action, ?messageThreadId = messageThreadId)
  static member Make(chatId: string, action: ChatAction, ?messageThreadId: int64) = 
    SendChatAction.Make(ChatId.String chatId, action, ?messageThreadId = messageThreadId)
  interface IRequestBase<bool> with
    member _.MethodName = "sendChatAction"
    
type SetMessageReaction =
  {
    ChatId: ChatId
    MessageId: int64
    Reaction: ReactionType[] option
    IsBig: bool option
  }
  static member Make(chatId: ChatId, messageId: int64, ?reaction: ReactionType[], ?isBig: bool) = 
    {
      ChatId = chatId
      MessageId = messageId
      Reaction = reaction
      IsBig = isBig
    }
  static member Make(chatId: int64, messageId: int64, ?reaction: ReactionType[], ?isBig: bool) = 
    SetMessageReaction.Make(ChatId.Int chatId, messageId, ?reaction = reaction, ?isBig = isBig)
  static member Make(chatId: string, messageId: int64, ?reaction: ReactionType[], ?isBig: bool) = 
    SetMessageReaction.Make(ChatId.String chatId, messageId, ?reaction = reaction, ?isBig = isBig)
  interface IRequestBase<bool> with
    member _.MethodName = "setMessageReaction"
    
type GetUserProfilePhotos =
  {
    UserId: int64
    Offset: int64 option
    Limit: int64 option
  }
  static member Make(userId: int64, ?offset: int64, ?limit: int64) = 
    {
      UserId = userId
      Offset = offset
      Limit = limit
    }
  interface IRequestBase<UserProfilePhotos> with
    member _.MethodName = "getUserProfilePhotos"
    
type GetFile =
  {
    FileId: string
  }
  static member Make(fileId: string) = 
    {
      FileId = fileId
    }
  interface IRequestBase<File> with
    member _.MethodName = "getFile"
    
type BanChatMember =
  {
    ChatId: ChatId
    UserId: int64
    UntilDate: int64 option
    RevokeMessages: bool option
  }
  static member Make(chatId: ChatId, userId: int64, ?untilDate: int64, ?revokeMessages: bool) = 
    {
      ChatId = chatId
      UserId = userId
      UntilDate = untilDate
      RevokeMessages = revokeMessages
    }
  static member Make(chatId: int64, userId: int64, ?untilDate: int64, ?revokeMessages: bool) = 
    BanChatMember.Make(ChatId.Int chatId, userId, ?untilDate = untilDate, ?revokeMessages = revokeMessages)
  static member Make(chatId: string, userId: int64, ?untilDate: int64, ?revokeMessages: bool) = 
    BanChatMember.Make(ChatId.String chatId, userId, ?untilDate = untilDate, ?revokeMessages = revokeMessages)
  interface IRequestBase<bool> with
    member _.MethodName = "banChatMember"
    
type UnbanChatMember =
  {
    ChatId: ChatId
    UserId: int64
    OnlyIfBanned: bool option
  }
  static member Make(chatId: ChatId, userId: int64, ?onlyIfBanned: bool) = 
    {
      ChatId = chatId
      UserId = userId
      OnlyIfBanned = onlyIfBanned
    }
  static member Make(chatId: int64, userId: int64, ?onlyIfBanned: bool) = 
    UnbanChatMember.Make(ChatId.Int chatId, userId, ?onlyIfBanned = onlyIfBanned)
  static member Make(chatId: string, userId: int64, ?onlyIfBanned: bool) = 
    UnbanChatMember.Make(ChatId.String chatId, userId, ?onlyIfBanned = onlyIfBanned)
  interface IRequestBase<bool> with
    member _.MethodName = "unbanChatMember"
    
type RestrictChatMember =
  {
    ChatId: ChatId
    UserId: int64
    Permissions: ChatPermissions
    UseIndependentChatPermissions: bool option
    UntilDate: int64 option
  }
  static member Make(chatId: ChatId, userId: int64, permissions: ChatPermissions, ?useIndependentChatPermissions: bool, ?untilDate: int64) = 
    {
      ChatId = chatId
      UserId = userId
      Permissions = permissions
      UseIndependentChatPermissions = useIndependentChatPermissions
      UntilDate = untilDate
    }
  static member Make(chatId: int64, userId: int64, permissions: ChatPermissions, ?useIndependentChatPermissions: bool, ?untilDate: int64) = 
    RestrictChatMember.Make(ChatId.Int chatId, userId, permissions, ?useIndependentChatPermissions = useIndependentChatPermissions, ?untilDate = untilDate)
  static member Make(chatId: string, userId: int64, permissions: ChatPermissions, ?useIndependentChatPermissions: bool, ?untilDate: int64) = 
    RestrictChatMember.Make(ChatId.String chatId, userId, permissions, ?useIndependentChatPermissions = useIndependentChatPermissions, ?untilDate = untilDate)
  interface IRequestBase<bool> with
    member _.MethodName = "restrictChatMember"
    
type PromoteChatMember =
  {
    ChatId: ChatId
    UserId: int64
    IsAnonymous: bool option
    CanManageChat: bool option
    CanDeleteMessages: bool option
    CanManageVideoChats: bool option
    CanRestrictMembers: bool option
    CanPromoteMembers: bool option
    CanChangeInfo: bool option
    CanInviteUsers: bool option
    CanPostMessages: bool option
    CanEditMessages: bool option
    CanPinMessages: bool option
    CanPostStories: bool option
    CanEditStories: bool option
    CanDeleteStories: bool option
    CanManageTopics: bool option
  }
  static member Make(chatId: ChatId, userId: int64, ?canEditStories: bool, ?canPostStories: bool, ?canPinMessages: bool, ?canEditMessages: bool, ?canPostMessages: bool, ?canInviteUsers: bool, ?canChangeInfo: bool, ?canPromoteMembers: bool, ?canRestrictMembers: bool, ?canManageVideoChats: bool, ?canDeleteMessages: bool, ?canManageChat: bool, ?isAnonymous: bool, ?canDeleteStories: bool, ?canManageTopics: bool) = 
    {
      ChatId = chatId
      UserId = userId
      IsAnonymous = isAnonymous
      CanManageChat = canManageChat
      CanDeleteMessages = canDeleteMessages
      CanManageVideoChats = canManageVideoChats
      CanRestrictMembers = canRestrictMembers
      CanPromoteMembers = canPromoteMembers
      CanChangeInfo = canChangeInfo
      CanInviteUsers = canInviteUsers
      CanPostMessages = canPostMessages
      CanEditMessages = canEditMessages
      CanPinMessages = canPinMessages
      CanPostStories = canPostStories
      CanEditStories = canEditStories
      CanDeleteStories = canDeleteStories
      CanManageTopics = canManageTopics
    }
  static member Make(chatId: int64, userId: int64, ?canEditStories: bool, ?canPostStories: bool, ?canPinMessages: bool, ?canEditMessages: bool, ?canPostMessages: bool, ?canInviteUsers: bool, ?canChangeInfo: bool, ?canPromoteMembers: bool, ?canRestrictMembers: bool, ?canManageVideoChats: bool, ?canDeleteMessages: bool, ?canManageChat: bool, ?isAnonymous: bool, ?canDeleteStories: bool, ?canManageTopics: bool) = 
    PromoteChatMember.Make(ChatId.Int chatId, userId, ?canEditStories = canEditStories, ?canPostStories = canPostStories, ?canPinMessages = canPinMessages, ?canEditMessages = canEditMessages, ?canPostMessages = canPostMessages, ?canInviteUsers = canInviteUsers, ?canChangeInfo = canChangeInfo, ?canPromoteMembers = canPromoteMembers, ?canRestrictMembers = canRestrictMembers, ?canManageVideoChats = canManageVideoChats, ?canDeleteMessages = canDeleteMessages, ?canManageChat = canManageChat, ?isAnonymous = isAnonymous, ?canDeleteStories = canDeleteStories, ?canManageTopics = canManageTopics)
  static member Make(chatId: string, userId: int64, ?canEditStories: bool, ?canPostStories: bool, ?canPinMessages: bool, ?canEditMessages: bool, ?canPostMessages: bool, ?canInviteUsers: bool, ?canChangeInfo: bool, ?canPromoteMembers: bool, ?canRestrictMembers: bool, ?canManageVideoChats: bool, ?canDeleteMessages: bool, ?canManageChat: bool, ?isAnonymous: bool, ?canDeleteStories: bool, ?canManageTopics: bool) = 
    PromoteChatMember.Make(ChatId.String chatId, userId, ?canEditStories = canEditStories, ?canPostStories = canPostStories, ?canPinMessages = canPinMessages, ?canEditMessages = canEditMessages, ?canPostMessages = canPostMessages, ?canInviteUsers = canInviteUsers, ?canChangeInfo = canChangeInfo, ?canPromoteMembers = canPromoteMembers, ?canRestrictMembers = canRestrictMembers, ?canManageVideoChats = canManageVideoChats, ?canDeleteMessages = canDeleteMessages, ?canManageChat = canManageChat, ?isAnonymous = isAnonymous, ?canDeleteStories = canDeleteStories, ?canManageTopics = canManageTopics)
  interface IRequestBase<bool> with
    member _.MethodName = "promoteChatMember"
    
type SetChatAdministratorCustomTitle =
  {
    ChatId: ChatId
    UserId: int64
    CustomTitle: string
  }
  static member Make(chatId: ChatId, userId: int64, customTitle: string) = 
    {
      ChatId = chatId
      UserId = userId
      CustomTitle = customTitle
    }
  static member Make(chatId: int64, userId: int64, customTitle: string) = 
    SetChatAdministratorCustomTitle.Make(ChatId.Int chatId, userId, customTitle)
  static member Make(chatId: string, userId: int64, customTitle: string) = 
    SetChatAdministratorCustomTitle.Make(ChatId.String chatId, userId, customTitle)
  interface IRequestBase<bool> with
    member _.MethodName = "setChatAdministratorCustomTitle"
    
type BanChatSenderChat =
  {
    ChatId: ChatId
    SenderChatId: int64
  }
  static member Make(chatId: ChatId, senderChatId: int64) = 
    {
      ChatId = chatId
      SenderChatId = senderChatId
    }
  static member Make(chatId: int64, senderChatId: int64) = 
    BanChatSenderChat.Make(ChatId.Int chatId, senderChatId)
  static member Make(chatId: string, senderChatId: int64) = 
    BanChatSenderChat.Make(ChatId.String chatId, senderChatId)
  interface IRequestBase<bool> with
    member _.MethodName = "banChatSenderChat"
    
type UnbanChatSenderChat =
  {
    ChatId: ChatId
    SenderChatId: int64
  }
  static member Make(chatId: ChatId, senderChatId: int64) = 
    {
      ChatId = chatId
      SenderChatId = senderChatId
    }
  static member Make(chatId: int64, senderChatId: int64) = 
    UnbanChatSenderChat.Make(ChatId.Int chatId, senderChatId)
  static member Make(chatId: string, senderChatId: int64) = 
    UnbanChatSenderChat.Make(ChatId.String chatId, senderChatId)
  interface IRequestBase<bool> with
    member _.MethodName = "unbanChatSenderChat"
    
type SetChatPermissions =
  {
    ChatId: ChatId
    Permissions: ChatPermissions
    UseIndependentChatPermissions: bool option
  }
  static member Make(chatId: ChatId, permissions: ChatPermissions, ?useIndependentChatPermissions: bool) = 
    {
      ChatId = chatId
      Permissions = permissions
      UseIndependentChatPermissions = useIndependentChatPermissions
    }
  static member Make(chatId: int64, permissions: ChatPermissions, ?useIndependentChatPermissions: bool) = 
    SetChatPermissions.Make(ChatId.Int chatId, permissions, ?useIndependentChatPermissions = useIndependentChatPermissions)
  static member Make(chatId: string, permissions: ChatPermissions, ?useIndependentChatPermissions: bool) = 
    SetChatPermissions.Make(ChatId.String chatId, permissions, ?useIndependentChatPermissions = useIndependentChatPermissions)
  interface IRequestBase<bool> with
    member _.MethodName = "setChatPermissions"
    
type ExportChatInviteLink =
  {
    ChatId: ChatId
  }
  static member Make(chatId: ChatId) = 
    {
      ChatId = chatId
    }
  static member Make(chatId: int64) = 
    ExportChatInviteLink.Make(ChatId.Int chatId)
  static member Make(chatId: string) = 
    ExportChatInviteLink.Make(ChatId.String chatId)
  interface IRequestBase<string> with
    member _.MethodName = "exportChatInviteLink"
    
type CreateChatInviteLink =
  {
    ChatId: ChatId
    Name: string option
    ExpireDate: int64 option
    MemberLimit: int64 option
    CreatesJoinRequest: bool option
  }
  static member Make(chatId: ChatId, ?name: string, ?expireDate: int64, ?memberLimit: int64, ?createsJoinRequest: bool) = 
    {
      ChatId = chatId
      Name = name
      ExpireDate = expireDate
      MemberLimit = memberLimit
      CreatesJoinRequest = createsJoinRequest
    }
  static member Make(chatId: int64, ?name: string, ?expireDate: int64, ?memberLimit: int64, ?createsJoinRequest: bool) = 
    CreateChatInviteLink.Make(ChatId.Int chatId, ?name = name, ?expireDate = expireDate, ?memberLimit = memberLimit, ?createsJoinRequest = createsJoinRequest)
  static member Make(chatId: string, ?name: string, ?expireDate: int64, ?memberLimit: int64, ?createsJoinRequest: bool) = 
    CreateChatInviteLink.Make(ChatId.String chatId, ?name = name, ?expireDate = expireDate, ?memberLimit = memberLimit, ?createsJoinRequest = createsJoinRequest)
  interface IRequestBase<ChatInviteLink> with
    member _.MethodName = "createChatInviteLink"
    
type EditChatInviteLink =
  {
    ChatId: ChatId
    InviteLink: string
    Name: string option
    ExpireDate: int64 option
    MemberLimit: int64 option
    CreatesJoinRequest: bool option
  }
  static member Make(chatId: ChatId, inviteLink: string, ?name: string, ?expireDate: int64, ?memberLimit: int64, ?createsJoinRequest: bool) = 
    {
      ChatId = chatId
      InviteLink = inviteLink
      Name = name
      ExpireDate = expireDate
      MemberLimit = memberLimit
      CreatesJoinRequest = createsJoinRequest
    }
  static member Make(chatId: int64, inviteLink: string, ?name: string, ?expireDate: int64, ?memberLimit: int64, ?createsJoinRequest: bool) = 
    EditChatInviteLink.Make(ChatId.Int chatId, inviteLink, ?name = name, ?expireDate = expireDate, ?memberLimit = memberLimit, ?createsJoinRequest = createsJoinRequest)
  static member Make(chatId: string, inviteLink: string, ?name: string, ?expireDate: int64, ?memberLimit: int64, ?createsJoinRequest: bool) = 
    EditChatInviteLink.Make(ChatId.String chatId, inviteLink, ?name = name, ?expireDate = expireDate, ?memberLimit = memberLimit, ?createsJoinRequest = createsJoinRequest)
  interface IRequestBase<ChatInviteLink> with
    member _.MethodName = "editChatInviteLink"
    
type RevokeChatInviteLink =
  {
    ChatId: ChatId
    InviteLink: string
  }
  static member Make(chatId: ChatId, inviteLink: string) = 
    {
      ChatId = chatId
      InviteLink = inviteLink
    }
  static member Make(chatId: int64, inviteLink: string) = 
    RevokeChatInviteLink.Make(ChatId.Int chatId, inviteLink)
  static member Make(chatId: string, inviteLink: string) = 
    RevokeChatInviteLink.Make(ChatId.String chatId, inviteLink)
  interface IRequestBase<ChatInviteLink> with
    member _.MethodName = "revokeChatInviteLink"
    
type ApproveChatJoinRequest =
  {
    ChatId: ChatId
    UserId: int64
  }
  static member Make(chatId: ChatId, userId: int64) = 
    {
      ChatId = chatId
      UserId = userId
    }
  static member Make(chatId: int64, userId: int64) = 
    ApproveChatJoinRequest.Make(ChatId.Int chatId, userId)
  static member Make(chatId: string, userId: int64) = 
    ApproveChatJoinRequest.Make(ChatId.String chatId, userId)
  interface IRequestBase<bool> with
    member _.MethodName = "approveChatJoinRequest"
    
type DeclineChatJoinRequest =
  {
    ChatId: ChatId
    UserId: int64
  }
  static member Make(chatId: ChatId, userId: int64) = 
    {
      ChatId = chatId
      UserId = userId
    }
  static member Make(chatId: int64, userId: int64) = 
    DeclineChatJoinRequest.Make(ChatId.Int chatId, userId)
  static member Make(chatId: string, userId: int64) = 
    DeclineChatJoinRequest.Make(ChatId.String chatId, userId)
  interface IRequestBase<bool> with
    member _.MethodName = "declineChatJoinRequest"
    
type SetChatPhoto =
  {
    ChatId: ChatId
    Photo: InputFile
  }
  static member Make(chatId: ChatId, photo: InputFile) = 
    {
      ChatId = chatId
      Photo = photo
    }
  static member Make(chatId: int64, photo: InputFile) = 
    SetChatPhoto.Make(ChatId.Int chatId, photo)
  static member Make(chatId: string, photo: InputFile) = 
    SetChatPhoto.Make(ChatId.String chatId, photo)
  interface IRequestBase<bool> with
    member _.MethodName = "setChatPhoto"
    
type DeleteChatPhoto =
  {
    ChatId: ChatId
  }
  static member Make(chatId: ChatId) = 
    {
      ChatId = chatId
    }
  static member Make(chatId: int64) = 
    DeleteChatPhoto.Make(ChatId.Int chatId)
  static member Make(chatId: string) = 
    DeleteChatPhoto.Make(ChatId.String chatId)
  interface IRequestBase<bool> with
    member _.MethodName = "deleteChatPhoto"
    
type SetChatTitle =
  {
    ChatId: ChatId
    Title: string
  }
  static member Make(chatId: ChatId, title: string) = 
    {
      ChatId = chatId
      Title = title
    }
  static member Make(chatId: int64, title: string) = 
    SetChatTitle.Make(ChatId.Int chatId, title)
  static member Make(chatId: string, title: string) = 
    SetChatTitle.Make(ChatId.String chatId, title)
  interface IRequestBase<bool> with
    member _.MethodName = "setChatTitle"
    
type SetChatDescription =
  {
    ChatId: ChatId
    Description: string option
  }
  static member Make(chatId: ChatId, ?description: string) = 
    {
      ChatId = chatId
      Description = description
    }
  static member Make(chatId: int64, ?description: string) = 
    SetChatDescription.Make(ChatId.Int chatId, ?description = description)
  static member Make(chatId: string, ?description: string) = 
    SetChatDescription.Make(ChatId.String chatId, ?description = description)
  interface IRequestBase<bool> with
    member _.MethodName = "setChatDescription"
    
type PinChatMessage =
  {
    ChatId: ChatId
    MessageId: int64
    DisableNotification: bool option
  }
  static member Make(chatId: ChatId, messageId: int64, ?disableNotification: bool) = 
    {
      ChatId = chatId
      MessageId = messageId
      DisableNotification = disableNotification
    }
  static member Make(chatId: int64, messageId: int64, ?disableNotification: bool) = 
    PinChatMessage.Make(ChatId.Int chatId, messageId, ?disableNotification = disableNotification)
  static member Make(chatId: string, messageId: int64, ?disableNotification: bool) = 
    PinChatMessage.Make(ChatId.String chatId, messageId, ?disableNotification = disableNotification)
  interface IRequestBase<bool> with
    member _.MethodName = "pinChatMessage"
    
type UnpinChatMessage =
  {
    ChatId: ChatId
    MessageId: int64 option
  }
  static member Make(chatId: ChatId, ?messageId: int64) = 
    {
      ChatId = chatId
      MessageId = messageId
    }
  static member Make(chatId: int64, ?messageId: int64) = 
    UnpinChatMessage.Make(ChatId.Int chatId, ?messageId = messageId)
  static member Make(chatId: string, ?messageId: int64) = 
    UnpinChatMessage.Make(ChatId.String chatId, ?messageId = messageId)
  interface IRequestBase<bool> with
    member _.MethodName = "unpinChatMessage"
    
type UnpinAllChatMessages =
  {
    ChatId: ChatId
  }
  static member Make(chatId: ChatId) = 
    {
      ChatId = chatId
    }
  static member Make(chatId: int64) = 
    UnpinAllChatMessages.Make(ChatId.Int chatId)
  static member Make(chatId: string) = 
    UnpinAllChatMessages.Make(ChatId.String chatId)
  interface IRequestBase<bool> with
    member _.MethodName = "unpinAllChatMessages"
    
type LeaveChat =
  {
    ChatId: ChatId
  }
  static member Make(chatId: ChatId) = 
    {
      ChatId = chatId
    }
  static member Make(chatId: int64) = 
    LeaveChat.Make(ChatId.Int chatId)
  static member Make(chatId: string) = 
    LeaveChat.Make(ChatId.String chatId)
  interface IRequestBase<bool> with
    member _.MethodName = "leaveChat"
    
type GetChat =
  {
    ChatId: ChatId
  }
  static member Make(chatId: ChatId) = 
    {
      ChatId = chatId
    }
  static member Make(chatId: int64) = 
    GetChat.Make(ChatId.Int chatId)
  static member Make(chatId: string) = 
    GetChat.Make(ChatId.String chatId)
  interface IRequestBase<Chat> with
    member _.MethodName = "getChat"
    
type GetChatAdministrators =
  {
    ChatId: ChatId
  }
  static member Make(chatId: ChatId) = 
    {
      ChatId = chatId
    }
  static member Make(chatId: int64) = 
    GetChatAdministrators.Make(ChatId.Int chatId)
  static member Make(chatId: string) = 
    GetChatAdministrators.Make(ChatId.String chatId)
  interface IRequestBase<ChatMember[]> with
    member _.MethodName = "getChatAdministrators"
    
type GetChatMemberCount =
  {
    ChatId: ChatId
  }
  static member Make(chatId: ChatId) = 
    {
      ChatId = chatId
    }
  static member Make(chatId: int64) = 
    GetChatMemberCount.Make(ChatId.Int chatId)
  static member Make(chatId: string) = 
    GetChatMemberCount.Make(ChatId.String chatId)
  interface IRequestBase<int> with
    member _.MethodName = "getChatMemberCount"
    
type GetChatMember =
  {
    ChatId: ChatId
    UserId: int64
  }
  static member Make(chatId: ChatId, userId: int64) = 
    {
      ChatId = chatId
      UserId = userId
    }
  static member Make(chatId: int64, userId: int64) = 
    GetChatMember.Make(ChatId.Int chatId, userId)
  static member Make(chatId: string, userId: int64) = 
    GetChatMember.Make(ChatId.String chatId, userId)
  interface IRequestBase<ChatMember> with
    member _.MethodName = "getChatMember"
    
type SetChatStickerSet =
  {
    ChatId: ChatId
    StickerSetName: string
  }
  static member Make(chatId: ChatId, stickerSetName: string) = 
    {
      ChatId = chatId
      StickerSetName = stickerSetName
    }
  static member Make(chatId: int64, stickerSetName: string) = 
    SetChatStickerSet.Make(ChatId.Int chatId, stickerSetName)
  static member Make(chatId: string, stickerSetName: string) = 
    SetChatStickerSet.Make(ChatId.String chatId, stickerSetName)
  interface IRequestBase<bool> with
    member _.MethodName = "setChatStickerSet"
    
type DeleteChatStickerSet =
  {
    ChatId: ChatId
  }
  static member Make(chatId: ChatId) = 
    {
      ChatId = chatId
    }
  static member Make(chatId: int64) = 
    DeleteChatStickerSet.Make(ChatId.Int chatId)
  static member Make(chatId: string) = 
    DeleteChatStickerSet.Make(ChatId.String chatId)
  interface IRequestBase<bool> with
    member _.MethodName = "deleteChatStickerSet"
    
type GetForumTopicIconStickers() =
  static member Make() = GetForumTopicIconStickers()
  interface IRequestBase<Sticker[]> with
    member _.MethodName = "getForumTopicIconStickers"
    
type CreateForumTopic =
  {
    ChatId: ChatId
    Name: string
    IconColor: int64 option
    IconCustomEmojiId: string option
  }
  static member Make(chatId: ChatId, name: string, ?iconColor: int64, ?iconCustomEmojiId: string) = 
    {
      ChatId = chatId
      Name = name
      IconColor = iconColor
      IconCustomEmojiId = iconCustomEmojiId
    }
  static member Make(chatId: int64, name: string, ?iconColor: int64, ?iconCustomEmojiId: string) = 
    CreateForumTopic.Make(ChatId.Int chatId, name, ?iconColor = iconColor, ?iconCustomEmojiId = iconCustomEmojiId)
  static member Make(chatId: string, name: string, ?iconColor: int64, ?iconCustomEmojiId: string) = 
    CreateForumTopic.Make(ChatId.String chatId, name, ?iconColor = iconColor, ?iconCustomEmojiId = iconCustomEmojiId)
  interface IRequestBase<ForumTopic> with
    member _.MethodName = "createForumTopic"
    
type EditForumTopic =
  {
    ChatId: ChatId
    MessageThreadId: int64
    Name: string option
    IconCustomEmojiId: string option
  }
  static member Make(chatId: ChatId, messageThreadId: int64, ?name: string, ?iconCustomEmojiId: string) = 
    {
      ChatId = chatId
      MessageThreadId = messageThreadId
      Name = name
      IconCustomEmojiId = iconCustomEmojiId
    }
  static member Make(chatId: int64, messageThreadId: int64, ?name: string, ?iconCustomEmojiId: string) = 
    EditForumTopic.Make(ChatId.Int chatId, messageThreadId, ?name = name, ?iconCustomEmojiId = iconCustomEmojiId)
  static member Make(chatId: string, messageThreadId: int64, ?name: string, ?iconCustomEmojiId: string) = 
    EditForumTopic.Make(ChatId.String chatId, messageThreadId, ?name = name, ?iconCustomEmojiId = iconCustomEmojiId)
  interface IRequestBase<bool> with
    member _.MethodName = "editForumTopic"
    
type CloseForumTopic =
  {
    ChatId: ChatId
    MessageThreadId: int64
  }
  static member Make(chatId: ChatId, messageThreadId: int64) = 
    {
      ChatId = chatId
      MessageThreadId = messageThreadId
    }
  static member Make(chatId: int64, messageThreadId: int64) = 
    CloseForumTopic.Make(ChatId.Int chatId, messageThreadId)
  static member Make(chatId: string, messageThreadId: int64) = 
    CloseForumTopic.Make(ChatId.String chatId, messageThreadId)
  interface IRequestBase<bool> with
    member _.MethodName = "closeForumTopic"
    
type ReopenForumTopic =
  {
    ChatId: ChatId
    MessageThreadId: int64
  }
  static member Make(chatId: ChatId, messageThreadId: int64) = 
    {
      ChatId = chatId
      MessageThreadId = messageThreadId
    }
  static member Make(chatId: int64, messageThreadId: int64) = 
    ReopenForumTopic.Make(ChatId.Int chatId, messageThreadId)
  static member Make(chatId: string, messageThreadId: int64) = 
    ReopenForumTopic.Make(ChatId.String chatId, messageThreadId)
  interface IRequestBase<bool> with
    member _.MethodName = "reopenForumTopic"
    
type DeleteForumTopic =
  {
    ChatId: ChatId
    MessageThreadId: int64
  }
  static member Make(chatId: ChatId, messageThreadId: int64) = 
    {
      ChatId = chatId
      MessageThreadId = messageThreadId
    }
  static member Make(chatId: int64, messageThreadId: int64) = 
    DeleteForumTopic.Make(ChatId.Int chatId, messageThreadId)
  static member Make(chatId: string, messageThreadId: int64) = 
    DeleteForumTopic.Make(ChatId.String chatId, messageThreadId)
  interface IRequestBase<bool> with
    member _.MethodName = "deleteForumTopic"
    
type UnpinAllForumTopicMessages =
  {
    ChatId: ChatId
    MessageThreadId: int64
  }
  static member Make(chatId: ChatId, messageThreadId: int64) = 
    {
      ChatId = chatId
      MessageThreadId = messageThreadId
    }
  static member Make(chatId: int64, messageThreadId: int64) = 
    UnpinAllForumTopicMessages.Make(ChatId.Int chatId, messageThreadId)
  static member Make(chatId: string, messageThreadId: int64) = 
    UnpinAllForumTopicMessages.Make(ChatId.String chatId, messageThreadId)
  interface IRequestBase<bool> with
    member _.MethodName = "unpinAllForumTopicMessages"
    
type EditGeneralForumTopic =
  {
    ChatId: ChatId
    Name: string
  }
  static member Make(chatId: ChatId, name: string) = 
    {
      ChatId = chatId
      Name = name
    }
  static member Make(chatId: int64, name: string) = 
    EditGeneralForumTopic.Make(ChatId.Int chatId, name)
  static member Make(chatId: string, name: string) = 
    EditGeneralForumTopic.Make(ChatId.String chatId, name)
  interface IRequestBase<bool> with
    member _.MethodName = "editGeneralForumTopic"
    
type CloseGeneralForumTopic =
  {
    ChatId: ChatId
  }
  static member Make(chatId: ChatId) = 
    {
      ChatId = chatId
    }
  static member Make(chatId: int64) = 
    CloseGeneralForumTopic.Make(ChatId.Int chatId)
  static member Make(chatId: string) = 
    CloseGeneralForumTopic.Make(ChatId.String chatId)
  interface IRequestBase<bool> with
    member _.MethodName = "closeGeneralForumTopic"
    
type ReopenGeneralForumTopic =
  {
    ChatId: ChatId
  }
  static member Make(chatId: ChatId) = 
    {
      ChatId = chatId
    }
  static member Make(chatId: int64) = 
    ReopenGeneralForumTopic.Make(ChatId.Int chatId)
  static member Make(chatId: string) = 
    ReopenGeneralForumTopic.Make(ChatId.String chatId)
  interface IRequestBase<bool> with
    member _.MethodName = "reopenGeneralForumTopic"
    
type HideGeneralForumTopic =
  {
    ChatId: ChatId
  }
  static member Make(chatId: ChatId) = 
    {
      ChatId = chatId
    }
  static member Make(chatId: int64) = 
    HideGeneralForumTopic.Make(ChatId.Int chatId)
  static member Make(chatId: string) = 
    HideGeneralForumTopic.Make(ChatId.String chatId)
  interface IRequestBase<bool> with
    member _.MethodName = "hideGeneralForumTopic"
    
type UnhideGeneralForumTopic =
  {
    ChatId: ChatId
  }
  static member Make(chatId: ChatId) = 
    {
      ChatId = chatId
    }
  static member Make(chatId: int64) = 
    UnhideGeneralForumTopic.Make(ChatId.Int chatId)
  static member Make(chatId: string) = 
    UnhideGeneralForumTopic.Make(ChatId.String chatId)
  interface IRequestBase<bool> with
    member _.MethodName = "unhideGeneralForumTopic"
    
type UnpinAllGeneralForumTopicMessages =
  {
    ChatId: ChatId
  }
  static member Make(chatId: ChatId) = 
    {
      ChatId = chatId
    }
  static member Make(chatId: int64) = 
    UnpinAllGeneralForumTopicMessages.Make(ChatId.Int chatId)
  static member Make(chatId: string) = 
    UnpinAllGeneralForumTopicMessages.Make(ChatId.String chatId)
  interface IRequestBase<bool> with
    member _.MethodName = "unpinAllGeneralForumTopicMessages"
    
type AnswerCallbackQuery =
  {
    CallbackQueryId: string
    Text: string option
    ShowAlert: bool option
    Url: string option
    CacheTime: int64 option
  }
  static member Make(callbackQueryId: string, ?text: string, ?showAlert: bool, ?url: string, ?cacheTime: int64) = 
    {
      CallbackQueryId = callbackQueryId
      Text = text
      ShowAlert = showAlert
      Url = url
      CacheTime = cacheTime
    }
  interface IRequestBase<bool> with
    member _.MethodName = "answerCallbackQuery"
    
type GetUserChatBoosts =
  {
    ChatId: ChatId
    UserId: int64
  }
  static member Make(chatId: ChatId, userId: int64) = 
    {
      ChatId = chatId
      UserId = userId
    }
  static member Make(chatId: int64, userId: int64) = 
    GetUserChatBoosts.Make(ChatId.Int chatId, userId)
  static member Make(chatId: string, userId: int64) = 
    GetUserChatBoosts.Make(ChatId.String chatId, userId)
  interface IRequestBase<UserChatBoosts> with
    member _.MethodName = "getUserChatBoosts"
    
type SetMyCommands =
  {
    Commands: BotCommand[]
    Scope: BotCommandScope option
    LanguageCode: string option
  }
  static member Make(commands: BotCommand[], ?scope: BotCommandScope, ?languageCode: string) = 
    {
      Commands = commands
      Scope = scope
      LanguageCode = languageCode
    }
  interface IRequestBase<bool> with
    member _.MethodName = "setMyCommands"
    
type DeleteMyCommands =
  {
    Scope: BotCommandScope option
    LanguageCode: string option
  }
  static member Make(?scope: BotCommandScope, ?languageCode: string) = 
    {
      Scope = scope
      LanguageCode = languageCode
    }
  interface IRequestBase<bool> with
    member _.MethodName = "deleteMyCommands"
    
type GetMyCommands =
  {
    Scope: BotCommandScope option
    LanguageCode: string option
  }
  static member Make(?scope: BotCommandScope, ?languageCode: string) = 
    {
      Scope = scope
      LanguageCode = languageCode
    }
  interface IRequestBase<BotCommand[]> with
    member _.MethodName = "getMyCommands"
    
type SetMyName =
  {
    Name: string option
    LanguageCode: string option
  }
  static member Make(?name: string, ?languageCode: string) = 
    {
      Name = name
      LanguageCode = languageCode
    }
  interface IRequestBase<bool> with
    member _.MethodName = "setMyName"
    
type GetMyName =
  {
    LanguageCode: string option
  }
  static member Make(?languageCode: string) = 
    {
      LanguageCode = languageCode
    }
  interface IRequestBase<BotName> with
    member _.MethodName = "getMyName"
    
type SetMyDescription =
  {
    Description: string option
    LanguageCode: string option
  }
  static member Make(?description: string, ?languageCode: string) = 
    {
      Description = description
      LanguageCode = languageCode
    }
  interface IRequestBase<bool> with
    member _.MethodName = "setMyDescription"
    
type GetMyDescription =
  {
    LanguageCode: string option
  }
  static member Make(?languageCode: string) = 
    {
      LanguageCode = languageCode
    }
  interface IRequestBase<BotDescription> with
    member _.MethodName = "getMyDescription"
    
type SetMyShortDescription =
  {
    ShortDescription: string option
    LanguageCode: string option
  }
  static member Make(?shortDescription: string, ?languageCode: string) = 
    {
      ShortDescription = shortDescription
      LanguageCode = languageCode
    }
  interface IRequestBase<bool> with
    member _.MethodName = "setMyShortDescription"
    
type GetMyShortDescription =
  {
    LanguageCode: string option
  }
  static member Make(?languageCode: string) = 
    {
      LanguageCode = languageCode
    }
  interface IRequestBase<BotShortDescription> with
    member _.MethodName = "getMyShortDescription"
    
type SetChatMenuButton =
  {
    ChatId: int64 option
    MenuButton: MenuButton option
  }
  static member Make(?chatId: int64, ?menuButton: MenuButton) = 
    {
      ChatId = chatId
      MenuButton = menuButton
    }
  interface IRequestBase<bool> with
    member _.MethodName = "setChatMenuButton"
    
type GetChatMenuButton =
  {
    ChatId: int64 option
  }
  static member Make(?chatId: int64) = 
    {
      ChatId = chatId
    }
  interface IRequestBase<MenuButton> with
    member _.MethodName = "getChatMenuButton"
    
type SetMyDefaultAdministratorRights =
  {
    Rights: ChatAdministratorRights option
    ForChannels: bool option
  }
  static member Make(?rights: ChatAdministratorRights, ?forChannels: bool) = 
    {
      Rights = rights
      ForChannels = forChannels
    }
  interface IRequestBase<bool> with
    member _.MethodName = "setMyDefaultAdministratorRights"
    
type GetMyDefaultAdministratorRights =
  {
    ForChannels: bool option
  }
  static member Make(?forChannels: bool) = 
    {
      ForChannels = forChannels
    }
  interface IRequestBase<ChatAdministratorRights> with
    member _.MethodName = "getMyDefaultAdministratorRights"
    
type EditMessageText =
  {
    ChatId: ChatId option
    MessageId: int64 option
    InlineMessageId: string option
    Text: string
    ParseMode: ParseMode option
    Entities: MessageEntity[] option
    LinkPreviewOptions: LinkPreviewOptions option
    ReplyMarkup: InlineKeyboardMarkup option
  }
  static member Make(text: string, ?chatId: ChatId, ?messageId: int64, ?inlineMessageId: string, ?parseMode: ParseMode, ?entities: MessageEntity[], ?linkPreviewOptions: LinkPreviewOptions, ?replyMarkup: InlineKeyboardMarkup) = 
    {
      ChatId = chatId
      MessageId = messageId
      InlineMessageId = inlineMessageId
      Text = text
      ParseMode = parseMode
      Entities = entities
      LinkPreviewOptions = linkPreviewOptions
      ReplyMarkup = replyMarkup
    }
  interface IRequestBase<EditMessageResult> with
    member _.MethodName = "editMessageText"
    
type EditMessageCaption =
  {
    ChatId: ChatId option
    MessageId: int64 option
    InlineMessageId: string option
    Caption: string option
    ParseMode: ParseMode option
    CaptionEntities: MessageEntity[] option
    ReplyMarkup: InlineKeyboardMarkup option
  }
  static member Make(?chatId: ChatId, ?messageId: int64, ?inlineMessageId: string, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?replyMarkup: InlineKeyboardMarkup) = 
    {
      ChatId = chatId
      MessageId = messageId
      InlineMessageId = inlineMessageId
      Caption = caption
      ParseMode = parseMode
      CaptionEntities = captionEntities
      ReplyMarkup = replyMarkup
    }
  static member Make(?chatId: int64, ?messageId: int64, ?inlineMessageId: string, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?replyMarkup: InlineKeyboardMarkup) = 
    EditMessageCaption.Make(?chatId = (chatId |> Option.map ChatId.Int), ?messageId = messageId, ?inlineMessageId = inlineMessageId, ?caption = caption, ?parseMode = parseMode, ?captionEntities = captionEntities, ?replyMarkup = replyMarkup)
  static member Make(?chatId: string, ?messageId: int64, ?inlineMessageId: string, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?replyMarkup: InlineKeyboardMarkup) = 
    EditMessageCaption.Make(?chatId = (chatId |> Option.map ChatId.String), ?messageId = messageId, ?inlineMessageId = inlineMessageId, ?caption = caption, ?parseMode = parseMode, ?captionEntities = captionEntities, ?replyMarkup = replyMarkup)
  interface IRequestBase<EditMessageResult> with
    member _.MethodName = "editMessageCaption"
    
type EditMessageMedia =
  {
    ChatId: ChatId option
    MessageId: int64 option
    InlineMessageId: string option
    Media: InputMedia
    ReplyMarkup: InlineKeyboardMarkup option
  }
  static member Make(media: InputMedia, ?chatId: ChatId, ?messageId: int64, ?inlineMessageId: string, ?replyMarkup: InlineKeyboardMarkup) = 
    {
      ChatId = chatId
      MessageId = messageId
      InlineMessageId = inlineMessageId
      Media = media
      ReplyMarkup = replyMarkup
    }
  interface IRequestBase<EditMessageResult> with
    member _.MethodName = "editMessageMedia"
    
type EditMessageLiveLocation =
  {
    ChatId: ChatId option
    MessageId: int64 option
    InlineMessageId: string option
    Latitude: float
    Longitude: float
    HorizontalAccuracy: float option
    Heading: int64 option
    ProximityAlertRadius: int64 option
    ReplyMarkup: InlineKeyboardMarkup option
  }
  static member Make(latitude: float, longitude: float, ?chatId: ChatId, ?messageId: int64, ?inlineMessageId: string, ?horizontalAccuracy: float, ?heading: int64, ?proximityAlertRadius: int64, ?replyMarkup: InlineKeyboardMarkup) = 
    {
      ChatId = chatId
      MessageId = messageId
      InlineMessageId = inlineMessageId
      Latitude = latitude
      Longitude = longitude
      HorizontalAccuracy = horizontalAccuracy
      Heading = heading
      ProximityAlertRadius = proximityAlertRadius
      ReplyMarkup = replyMarkup
    }
  interface IRequestBase<EditMessageResult> with
    member _.MethodName = "editMessageLiveLocation"
    
type StopMessageLiveLocation =
  {
    ChatId: ChatId option
    MessageId: int64 option
    InlineMessageId: string option
    ReplyMarkup: InlineKeyboardMarkup option
  }
  static member Make(?chatId: ChatId, ?messageId: int64, ?inlineMessageId: string, ?replyMarkup: InlineKeyboardMarkup) = 
    {
      ChatId = chatId
      MessageId = messageId
      InlineMessageId = inlineMessageId
      ReplyMarkup = replyMarkup
    }
  static member Make(?chatId: int64, ?messageId: int64, ?inlineMessageId: string, ?replyMarkup: InlineKeyboardMarkup) = 
    StopMessageLiveLocation.Make(?chatId = (chatId |> Option.map ChatId.Int), ?messageId = messageId, ?inlineMessageId = inlineMessageId, ?replyMarkup = replyMarkup)
  static member Make(?chatId: string, ?messageId: int64, ?inlineMessageId: string, ?replyMarkup: InlineKeyboardMarkup) = 
    StopMessageLiveLocation.Make(?chatId = (chatId |> Option.map ChatId.String), ?messageId = messageId, ?inlineMessageId = inlineMessageId, ?replyMarkup = replyMarkup)
  interface IRequestBase<EditMessageResult> with
    member _.MethodName = "stopMessageLiveLocation"
    
type EditMessageReplyMarkup =
  {
    ChatId: ChatId option
    MessageId: int64 option
    InlineMessageId: string option
    ReplyMarkup: InlineKeyboardMarkup option
  }
  static member Make(?chatId: ChatId, ?messageId: int64, ?inlineMessageId: string, ?replyMarkup: InlineKeyboardMarkup) = 
    {
      ChatId = chatId
      MessageId = messageId
      InlineMessageId = inlineMessageId
      ReplyMarkup = replyMarkup
    }
  static member Make(?chatId: int64, ?messageId: int64, ?inlineMessageId: string, ?replyMarkup: InlineKeyboardMarkup) = 
    EditMessageReplyMarkup.Make(?chatId = (chatId |> Option.map ChatId.Int), ?messageId = messageId, ?inlineMessageId = inlineMessageId, ?replyMarkup = replyMarkup)
  static member Make(?chatId: string, ?messageId: int64, ?inlineMessageId: string, ?replyMarkup: InlineKeyboardMarkup) = 
    EditMessageReplyMarkup.Make(?chatId = (chatId |> Option.map ChatId.String), ?messageId = messageId, ?inlineMessageId = inlineMessageId, ?replyMarkup = replyMarkup)
  interface IRequestBase<EditMessageResult> with
    member _.MethodName = "editMessageReplyMarkup"
    
type StopPoll =
  {
    ChatId: ChatId
    MessageId: int64
    ReplyMarkup: InlineKeyboardMarkup option
  }
  static member Make(chatId: ChatId, messageId: int64, ?replyMarkup: InlineKeyboardMarkup) = 
    {
      ChatId = chatId
      MessageId = messageId
      ReplyMarkup = replyMarkup
    }
  static member Make(chatId: int64, messageId: int64, ?replyMarkup: InlineKeyboardMarkup) = 
    StopPoll.Make(ChatId.Int chatId, messageId, ?replyMarkup = replyMarkup)
  static member Make(chatId: string, messageId: int64, ?replyMarkup: InlineKeyboardMarkup) = 
    StopPoll.Make(ChatId.String chatId, messageId, ?replyMarkup = replyMarkup)
  interface IRequestBase<Poll> with
    member _.MethodName = "stopPoll"
    
type DeleteMessage =
  {
    ChatId: ChatId
    MessageId: int64
  }
  static member Make(chatId: ChatId, messageId: int64) = 
    {
      ChatId = chatId
      MessageId = messageId
    }
  static member Make(chatId: int64, messageId: int64) = 
    DeleteMessage.Make(ChatId.Int chatId, messageId)
  static member Make(chatId: string, messageId: int64) = 
    DeleteMessage.Make(ChatId.String chatId, messageId)
  interface IRequestBase<bool> with
    member _.MethodName = "deleteMessage"
    
type DeleteMessages =
  {
    ChatId: ChatId
    MessageIds: int64[]
  }
  static member Make(chatId: ChatId, messageIds: int64[]) = 
    {
      ChatId = chatId
      MessageIds = messageIds
    }
  static member Make(chatId: int64, messageIds: int64[]) = 
    DeleteMessages.Make(ChatId.Int chatId, messageIds)
  static member Make(chatId: string, messageIds: int64[]) = 
    DeleteMessages.Make(ChatId.String chatId, messageIds)
  interface IRequestBase<bool> with
    member _.MethodName = "deleteMessages"
    
type SendSticker =
  {
    ChatId: ChatId
    MessageThreadId: int64 option
    Sticker: InputFile
    Emoji: string option
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyParameters: ReplyParameters option
    ReplyMarkup: Markup option
  }
  static member Make(chatId: ChatId, sticker: InputFile, ?messageThreadId: int64, ?emoji: string, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    {
      ChatId = chatId
      MessageThreadId = messageThreadId
      Sticker = sticker
      Emoji = emoji
      DisableNotification = disableNotification
      ProtectContent = protectContent
      ReplyParameters = replyParameters
      ReplyMarkup = replyMarkup
    }
  static member Make(chatId: int64, sticker: InputFile, ?messageThreadId: int64, ?emoji: string, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    SendSticker.Make(ChatId.Int chatId, sticker, ?messageThreadId = messageThreadId, ?emoji = emoji, ?disableNotification = disableNotification, ?protectContent = protectContent, ?replyParameters = replyParameters, ?replyMarkup = replyMarkup)
  static member Make(chatId: string, sticker: InputFile, ?messageThreadId: int64, ?emoji: string, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: Markup) = 
    SendSticker.Make(ChatId.String chatId, sticker, ?messageThreadId = messageThreadId, ?emoji = emoji, ?disableNotification = disableNotification, ?protectContent = protectContent, ?replyParameters = replyParameters, ?replyMarkup = replyMarkup)
  interface IRequestBase<Message> with
    member _.MethodName = "sendSticker"
    
type GetStickerSet =
  {
    Name: string
  }
  static member Make(name: string) = 
    {
      Name = name
    }
  interface IRequestBase<StickerSet> with
    member _.MethodName = "getStickerSet"
    
type GetCustomEmojiStickers =
  {
    CustomEmojiIds: string[]
  }
  static member Make(customEmojiIds: string[]) = 
    {
      CustomEmojiIds = customEmojiIds
    }
  interface IRequestBase<Sticker[]> with
    member _.MethodName = "getCustomEmojiStickers"
    
type UploadStickerFile =
  {
    UserId: int64
    Sticker: InputFile
    StickerFormat: string
  }
  static member Make(userId: int64, sticker: InputFile, stickerFormat: string) = 
    {
      UserId = userId
      Sticker = sticker
      StickerFormat = stickerFormat
    }
  interface IRequestBase<File> with
    member _.MethodName = "uploadStickerFile"
    
type CreateNewStickerSet =
  {
    UserId: int64
    Name: string
    Title: string
    Stickers: InputSticker[]
    StickerFormat: string
    StickerType: string option
    NeedsRepainting: bool option
  }
  static member Make(userId: int64, name: string, title: string, stickers: InputSticker[], stickerFormat: string, ?stickerType: string, ?needsRepainting: bool) = 
    {
      UserId = userId
      Name = name
      Title = title
      Stickers = stickers
      StickerFormat = stickerFormat
      StickerType = stickerType
      NeedsRepainting = needsRepainting
    }
  interface IRequestBase<bool> with
    member _.MethodName = "createNewStickerSet"
    
type AddStickerToSet =
  {
    UserId: int64
    Name: string
    Sticker: InputSticker
  }
  static member Make(userId: int64, name: string, sticker: InputSticker) = 
    {
      UserId = userId
      Name = name
      Sticker = sticker
    }
  interface IRequestBase<bool> with
    member _.MethodName = "addStickerToSet"
    
type SetStickerPositionInSet =
  {
    Sticker: string
    Position: int64
  }
  static member Make(sticker: string, position: int64) = 
    {
      Sticker = sticker
      Position = position
    }
  interface IRequestBase<bool> with
    member _.MethodName = "setStickerPositionInSet"
    
type DeleteStickerFromSet =
  {
    Sticker: string
  }
  static member Make(sticker: string) = 
    {
      Sticker = sticker
    }
  interface IRequestBase<bool> with
    member _.MethodName = "deleteStickerFromSet"
    
type SetStickerEmojiList =
  {
    Sticker: string
    EmojiList: string[]
  }
  static member Make(sticker: string, emojiList: string[]) = 
    {
      Sticker = sticker
      EmojiList = emojiList
    }
  interface IRequestBase<bool> with
    member _.MethodName = "setStickerEmojiList"
    
type SetStickerKeywords =
  {
    Sticker: string
    Keywords: string[] option
  }
  static member Make(sticker: string, ?keywords: string[]) = 
    {
      Sticker = sticker
      Keywords = keywords
    }
  interface IRequestBase<bool> with
    member _.MethodName = "setStickerKeywords"
    
type SetStickerMaskPosition =
  {
    Sticker: string
    MaskPosition: MaskPosition option
  }
  static member Make(sticker: string, ?maskPosition: MaskPosition) = 
    {
      Sticker = sticker
      MaskPosition = maskPosition
    }
  interface IRequestBase<bool> with
    member _.MethodName = "setStickerMaskPosition"
    
type SetStickerSetTitle =
  {
    Name: string
    Title: string
  }
  static member Make(name: string, title: string) = 
    {
      Name = name
      Title = title
    }
  interface IRequestBase<bool> with
    member _.MethodName = "setStickerSetTitle"
    
type SetStickerSetThumbnail =
  {
    Name: string
    UserId: int64
    Thumbnail: InputFile option
  }
  static member Make(name: string, userId: int64, ?thumbnail: InputFile) = 
    {
      Name = name
      UserId = userId
      Thumbnail = thumbnail
    }
  interface IRequestBase<bool> with
    member _.MethodName = "setStickerSetThumbnail"
    
type SetCustomEmojiStickerSetThumbnail =
  {
    Name: string
    CustomEmojiId: string option
  }
  static member Make(name: string, ?customEmojiId: string) = 
    {
      Name = name
      CustomEmojiId = customEmojiId
    }
  interface IRequestBase<bool> with
    member _.MethodName = "setCustomEmojiStickerSetThumbnail"
    
type DeleteStickerSet =
  {
    Name: string
  }
  static member Make(name: string) = 
    {
      Name = name
    }
  interface IRequestBase<bool> with
    member _.MethodName = "deleteStickerSet"
    
type AnswerInlineQuery =
  {
    InlineQueryId: string
    Results: InlineQueryResult[]
    CacheTime: int64 option
    IsPersonal: bool option
    NextOffset: string option
    Button: InlineQueryResultsButton option
  }
  static member Make(inlineQueryId: string, results: InlineQueryResult[], ?cacheTime: int64, ?isPersonal: bool, ?nextOffset: string, ?button: InlineQueryResultsButton) = 
    {
      InlineQueryId = inlineQueryId
      Results = results
      CacheTime = cacheTime
      IsPersonal = isPersonal
      NextOffset = nextOffset
      Button = button
    }
  interface IRequestBase<bool> with
    member _.MethodName = "answerInlineQuery"
    
type AnswerWebAppQuery =
  {
    WebAppQueryId: string
    Result: InlineQueryResult
  }
  static member Make(webAppQueryId: string, result: InlineQueryResult) = 
    {
      WebAppQueryId = webAppQueryId
      Result = result
    }
  interface IRequestBase<SentWebAppMessage> with
    member _.MethodName = "answerWebAppQuery"
    
type SendInvoice =
  {
    ChatId: ChatId
    MessageThreadId: int64 option
    Title: string
    Description: string
    Payload: string
    ProviderToken: string
    Currency: string
    Prices: LabeledPrice[]
    MaxTipAmount: int64 option
    SuggestedTipAmounts: int64[] option
    StartParameter: string option
    ProviderData: string option
    PhotoUrl: string option
    PhotoSize: int64 option
    PhotoWidth: int64 option
    PhotoHeight: int64 option
    NeedName: bool option
    NeedPhoneNumber: bool option
    NeedEmail: bool option
    NeedShippingAddress: bool option
    SendPhoneNumberToProvider: bool option
    SendEmailToProvider: bool option
    IsFlexible: bool option
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyParameters: ReplyParameters option
    ReplyMarkup: InlineKeyboardMarkup option
  }
  static member Make(chatId: ChatId, title: string, description: string, payload: string, providerToken: string, currency: string, prices: LabeledPrice[], ?protectContent: bool, ?disableNotification: bool, ?isFlexible: bool, ?sendEmailToProvider: bool, ?sendPhoneNumberToProvider: bool, ?needShippingAddress: bool, ?needEmail: bool, ?needPhoneNumber: bool, ?needName: bool, ?photoSize: int64, ?photoWidth: int64, ?replyParameters: ReplyParameters, ?photoUrl: string, ?providerData: string, ?startParameter: string, ?suggestedTipAmounts: int64[], ?maxTipAmount: int64, ?messageThreadId: int64, ?photoHeight: int64, ?replyMarkup: InlineKeyboardMarkup) = 
    {
      ChatId = chatId
      MessageThreadId = messageThreadId
      Title = title
      Description = description
      Payload = payload
      ProviderToken = providerToken
      Currency = currency
      Prices = prices
      MaxTipAmount = maxTipAmount
      SuggestedTipAmounts = suggestedTipAmounts
      StartParameter = startParameter
      ProviderData = providerData
      PhotoUrl = photoUrl
      PhotoSize = photoSize
      PhotoWidth = photoWidth
      PhotoHeight = photoHeight
      NeedName = needName
      NeedPhoneNumber = needPhoneNumber
      NeedEmail = needEmail
      NeedShippingAddress = needShippingAddress
      SendPhoneNumberToProvider = sendPhoneNumberToProvider
      SendEmailToProvider = sendEmailToProvider
      IsFlexible = isFlexible
      DisableNotification = disableNotification
      ProtectContent = protectContent
      ReplyParameters = replyParameters
      ReplyMarkup = replyMarkup
    }
  static member Make(chatId: int64, title: string, description: string, payload: string, providerToken: string, currency: string, prices: LabeledPrice[], ?protectContent: bool, ?disableNotification: bool, ?isFlexible: bool, ?sendEmailToProvider: bool, ?sendPhoneNumberToProvider: bool, ?needShippingAddress: bool, ?needEmail: bool, ?needPhoneNumber: bool, ?needName: bool, ?photoSize: int64, ?photoWidth: int64, ?replyParameters: ReplyParameters, ?photoUrl: string, ?providerData: string, ?startParameter: string, ?suggestedTipAmounts: int64[], ?maxTipAmount: int64, ?messageThreadId: int64, ?photoHeight: int64, ?replyMarkup: InlineKeyboardMarkup) = 
    SendInvoice.Make(ChatId.Int chatId, title, description, payload, providerToken, currency, prices, ?protectContent = protectContent, ?disableNotification = disableNotification, ?isFlexible = isFlexible, ?sendEmailToProvider = sendEmailToProvider, ?sendPhoneNumberToProvider = sendPhoneNumberToProvider, ?needShippingAddress = needShippingAddress, ?needEmail = needEmail, ?needPhoneNumber = needPhoneNumber, ?needName = needName, ?photoSize = photoSize, ?photoWidth = photoWidth, ?replyParameters = replyParameters, ?photoUrl = photoUrl, ?providerData = providerData, ?startParameter = startParameter, ?suggestedTipAmounts = suggestedTipAmounts, ?maxTipAmount = maxTipAmount, ?messageThreadId = messageThreadId, ?photoHeight = photoHeight, ?replyMarkup = replyMarkup)
  static member Make(chatId: string, title: string, description: string, payload: string, providerToken: string, currency: string, prices: LabeledPrice[], ?protectContent: bool, ?disableNotification: bool, ?isFlexible: bool, ?sendEmailToProvider: bool, ?sendPhoneNumberToProvider: bool, ?needShippingAddress: bool, ?needEmail: bool, ?needPhoneNumber: bool, ?needName: bool, ?photoSize: int64, ?photoWidth: int64, ?replyParameters: ReplyParameters, ?photoUrl: string, ?providerData: string, ?startParameter: string, ?suggestedTipAmounts: int64[], ?maxTipAmount: int64, ?messageThreadId: int64, ?photoHeight: int64, ?replyMarkup: InlineKeyboardMarkup) = 
    SendInvoice.Make(ChatId.String chatId, title, description, payload, providerToken, currency, prices, ?protectContent = protectContent, ?disableNotification = disableNotification, ?isFlexible = isFlexible, ?sendEmailToProvider = sendEmailToProvider, ?sendPhoneNumberToProvider = sendPhoneNumberToProvider, ?needShippingAddress = needShippingAddress, ?needEmail = needEmail, ?needPhoneNumber = needPhoneNumber, ?needName = needName, ?photoSize = photoSize, ?photoWidth = photoWidth, ?replyParameters = replyParameters, ?photoUrl = photoUrl, ?providerData = providerData, ?startParameter = startParameter, ?suggestedTipAmounts = suggestedTipAmounts, ?maxTipAmount = maxTipAmount, ?messageThreadId = messageThreadId, ?photoHeight = photoHeight, ?replyMarkup = replyMarkup)
  interface IRequestBase<Message> with
    member _.MethodName = "sendInvoice"
    
type CreateInvoiceLink =
  {
    Title: string
    Description: string
    Payload: string
    ProviderToken: string
    Currency: string
    Prices: LabeledPrice[]
    MaxTipAmount: int64 option
    SuggestedTipAmounts: int64[] option
    ProviderData: string option
    PhotoUrl: string option
    PhotoSize: int64 option
    PhotoWidth: int64 option
    PhotoHeight: int64 option
    NeedName: bool option
    NeedPhoneNumber: bool option
    NeedEmail: bool option
    NeedShippingAddress: bool option
    SendPhoneNumberToProvider: bool option
    SendEmailToProvider: bool option
    IsFlexible: bool option
  }
  static member Make(title: string, description: string, payload: string, providerToken: string, currency: string, prices: LabeledPrice[], ?sendPhoneNumberToProvider: bool, ?needShippingAddress: bool, ?needEmail: bool, ?needPhoneNumber: bool, ?needName: bool, ?photoHeight: int64, ?photoUrl: string, ?photoSize: int64, ?sendEmailToProvider: bool, ?providerData: string, ?suggestedTipAmounts: int64[], ?maxTipAmount: int64, ?photoWidth: int64, ?isFlexible: bool) = 
    {
      Title = title
      Description = description
      Payload = payload
      ProviderToken = providerToken
      Currency = currency
      Prices = prices
      MaxTipAmount = maxTipAmount
      SuggestedTipAmounts = suggestedTipAmounts
      ProviderData = providerData
      PhotoUrl = photoUrl
      PhotoSize = photoSize
      PhotoWidth = photoWidth
      PhotoHeight = photoHeight
      NeedName = needName
      NeedPhoneNumber = needPhoneNumber
      NeedEmail = needEmail
      NeedShippingAddress = needShippingAddress
      SendPhoneNumberToProvider = sendPhoneNumberToProvider
      SendEmailToProvider = sendEmailToProvider
      IsFlexible = isFlexible
    }
  interface IRequestBase<string> with
    member _.MethodName = "createInvoiceLink"
    
type AnswerShippingQuery =
  {
    ShippingQueryId: string
    Ok: bool
    ShippingOptions: ShippingOption[] option
    ErrorMessage: string option
  }
  static member Make(shippingQueryId: string, ok: bool, ?shippingOptions: ShippingOption[], ?errorMessage: string) = 
    {
      ShippingQueryId = shippingQueryId
      Ok = ok
      ShippingOptions = shippingOptions
      ErrorMessage = errorMessage
    }
  interface IRequestBase<bool> with
    member _.MethodName = "answerShippingQuery"
    
type AnswerPreCheckoutQuery =
  {
    PreCheckoutQueryId: string
    Ok: bool
    ErrorMessage: string option
  }
  static member Make(preCheckoutQueryId: string, ok: bool, ?errorMessage: string) = 
    {
      PreCheckoutQueryId = preCheckoutQueryId
      Ok = ok
      ErrorMessage = errorMessage
    }
  interface IRequestBase<bool> with
    member _.MethodName = "answerPreCheckoutQuery"
    
type SetPassportDataErrors =
  {
    UserId: int64
    Errors: PassportElementError[]
  }
  static member Make(userId: int64, errors: PassportElementError[]) = 
    {
      UserId = userId
      Errors = errors
    }
  interface IRequestBase<bool> with
    member _.MethodName = "setPassportDataErrors"
    
type SendGame =
  {
    ChatId: int64
    MessageThreadId: int64 option
    GameShortName: string
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyParameters: ReplyParameters option
    ReplyMarkup: InlineKeyboardMarkup option
  }
  static member Make(chatId: int64, gameShortName: string, ?messageThreadId: int64, ?disableNotification: bool, ?protectContent: bool, ?replyParameters: ReplyParameters, ?replyMarkup: InlineKeyboardMarkup) = 
    {
      ChatId = chatId
      MessageThreadId = messageThreadId
      GameShortName = gameShortName
      DisableNotification = disableNotification
      ProtectContent = protectContent
      ReplyParameters = replyParameters
      ReplyMarkup = replyMarkup
    }
  interface IRequestBase<Message> with
    member _.MethodName = "sendGame"
    
type SetGameScore =
  {
    UserId: int64
    Score: int64
    Force: bool option
    DisableEditMessage: bool option
    ChatId: int64 option
    MessageId: int64 option
    InlineMessageId: string option
  }
  static member Make(userId: int64, score: int64, ?force: bool, ?disableEditMessage: bool, ?chatId: int64, ?messageId: int64, ?inlineMessageId: string) = 
    {
      UserId = userId
      Score = score
      Force = force
      DisableEditMessage = disableEditMessage
      ChatId = chatId
      MessageId = messageId
      InlineMessageId = inlineMessageId
    }
  interface IRequestBase<EditMessageResult> with
    member _.MethodName = "setGameScore"
    
type GetGameHighScores =
  {
    UserId: int64
    ChatId: int64 option
    MessageId: int64 option
    InlineMessageId: string option
  }
  static member Make(userId: int64, ?chatId: int64, ?messageId: int64, ?inlineMessageId: string) = 
    {
      UserId = userId
      ChatId = chatId
      MessageId = messageId
      InlineMessageId = inlineMessageId
    }
  interface IRequestBase<GameHighScore[]> with
    member _.MethodName = "getGameHighScores"
    