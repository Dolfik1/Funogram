module Funogram.Telegram.RequestsTypes

open Funogram.Types
open Types
open System
    
type GetUpdatesReq =
  {
    Offset: int64 option
    Limit: int64 option
    Timeout: int64 option
    AllowedUpdates: string[] option
  }
  static member Make(?offset: int64, ?limit: int64, ?timeout: int64, ?allowedUpdates: string[]) = ()
  interface IRequestBase<Update[]> with
    member _.MethodName = "getUpdates"
    
type SetWebhookReq =
  {
    Url: string
    Certificate: InputFile option
    IpAddress: string option
    MaxConnections: int64 option
    AllowedUpdates: string[] option
    DropPendingUpdates: bool option
  }
  static member Make(url: string, ?certificate: InputFile, ?ipAddress: string, ?maxConnections: int64, ?allowedUpdates: string[], ?dropPendingUpdates: bool) = ()
  interface IRequestBase<bool> with
    member _.MethodName = "setWebhook"
    
type DeleteWebhookReq =
  {
    DropPendingUpdates: bool option
  }
  static member Make(?dropPendingUpdates: bool) = ()
  interface IRequestBase<bool> with
    member _.MethodName = "deleteWebhook"
    
type GetWebhookInfoReq =
  {
    Url: string
    HasCustomCertificate: bool
    PendingUpdateCount: int64
    IpAddress: string option
    LastErrorDate: int64 option
    LastErrorMessage: string option
    LastSynchronizationErrorDate: int64 option
    MaxConnections: int64 option
    AllowedUpdates: string[] option
  }
  static member Make(url: string, hasCustomCertificate: bool, pendingUpdateCount: int64, ?ipAddress: string, ?lastErrorDate: int64, ?lastErrorMessage: string, ?lastSynchronizationErrorDate: int64, ?maxConnections: int64, ?allowedUpdates: string[]) = ()
  interface IRequestBase<WebhookInfo> with
    member _.MethodName = "getWebhookInfo"
    
type GetMeReq() =
  static member Make() = GetMeReq()
  interface IRequestBase<User> with
    member _.MethodName = "getMe"
    
type LogOutReq() =
  static member Make() = LogOutReq()
  interface IRequestBase<bool> with
    member _.MethodName = "logOut"
    
type CloseReq() =
  static member Make() = CloseReq()
  interface IRequestBase<bool> with
    member _.MethodName = "close"
    
type SendMessageReq =
  {
    ChatId: ChatId
    Text: string
    ParseMode: string option
    Entities: MessageEntity[] option
    DisableWebPagePreview: bool option
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyToMessageId: int64 option
    AllowSendingWithoutReply: bool option
    ReplyMarkup: Markup option
  }
  static member Make(chatId: ChatId, text: string, ?parseMode: string, ?entities: MessageEntity[], ?disableWebPagePreview: bool, ?disableNotification: bool, ?protectContent: bool, ?replyToMessageId: int64, ?allowSendingWithoutReply: bool, ?replyMarkup: Markup) = ()
  interface IRequestBase<Message> with
    member _.MethodName = "sendMessage"
    
type ForwardMessageReq =
  {
    ChatId: ChatId
    FromChatId: ChatId
    DisableNotification: bool option
    ProtectContent: bool option
    MessageId: int64
  }
  static member Make(chatId: ChatId, fromChatId: ChatId, messageId: int64, ?disableNotification: bool, ?protectContent: bool) = ()
  interface IRequestBase<Message> with
    member _.MethodName = "forwardMessage"
    
type CopyMessageReq =
  {
    ChatId: ChatId
    FromChatId: ChatId
    MessageId: int64
    Caption: string option
    ParseMode: string option
    CaptionEntities: MessageEntity[] option
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyToMessageId: int64 option
    AllowSendingWithoutReply: bool option
    ReplyMarkup: Markup option
  }
  static member Make(chatId: ChatId, fromChatId: ChatId, messageId: int64, ?caption: string, ?parseMode: string, ?captionEntities: MessageEntity[], ?disableNotification: bool, ?protectContent: bool, ?replyToMessageId: int64, ?allowSendingWithoutReply: bool, ?replyMarkup: Markup) = ()
  interface IRequestBase<MessageId> with
    member _.MethodName = "copyMessage"
    
type SendPhotoReq =
  {
    ChatId: ChatId
    Photo: InputFile
    Caption: string option
    ParseMode: string option
    CaptionEntities: MessageEntity[] option
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyToMessageId: int64 option
    AllowSendingWithoutReply: bool option
    ReplyMarkup: Markup option
  }
  static member Make(chatId: ChatId, photo: InputFile, ?caption: string, ?parseMode: string, ?captionEntities: MessageEntity[], ?disableNotification: bool, ?protectContent: bool, ?replyToMessageId: int64, ?allowSendingWithoutReply: bool, ?replyMarkup: Markup) = ()
  interface IRequestBase<Message> with
    member _.MethodName = "sendPhoto"
    
type SendAudioReq =
  {
    ChatId: ChatId
    Audio: InputFile
    Caption: string option
    ParseMode: string option
    CaptionEntities: MessageEntity[] option
    Duration: int64 option
    Performer: string option
    Title: string option
    Thumb: InputFile option
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyToMessageId: int64 option
    AllowSendingWithoutReply: bool option
    ReplyMarkup: Markup option
  }
  static member Make(chatId: ChatId, audio: InputFile, ?caption: string, ?parseMode: string, ?captionEntities: MessageEntity[], ?duration: int64, ?performer: string, ?title: string, ?thumb: InputFile, ?disableNotification: bool, ?protectContent: bool, ?replyToMessageId: int64, ?allowSendingWithoutReply: bool, ?replyMarkup: Markup) = ()
  interface IRequestBase<Message> with
    member _.MethodName = "sendAudio"
    
type SendDocumentReq =
  {
    ChatId: ChatId
    Document: InputFile
    Thumb: InputFile option
    Caption: string option
    ParseMode: string option
    CaptionEntities: MessageEntity[] option
    DisableContentTypeDetection: bool option
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyToMessageId: int64 option
    AllowSendingWithoutReply: bool option
    ReplyMarkup: Markup option
  }
  static member Make(chatId: ChatId, document: InputFile, ?thumb: InputFile, ?caption: string, ?parseMode: string, ?captionEntities: MessageEntity[], ?disableContentTypeDetection: bool, ?disableNotification: bool, ?protectContent: bool, ?replyToMessageId: int64, ?allowSendingWithoutReply: bool, ?replyMarkup: Markup) = ()
  interface IRequestBase<Message> with
    member _.MethodName = "sendDocument"
    
type SendVideoReq =
  {
    ChatId: ChatId
    Video: InputFile
    Duration: int64 option
    Width: int64 option
    Height: int64 option
    Thumb: InputFile option
    Caption: string option
    ParseMode: string option
    CaptionEntities: MessageEntity[] option
    SupportsStreaming: bool option
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyToMessageId: int64 option
    AllowSendingWithoutReply: bool option
    ReplyMarkup: Markup option
  }
  static member Make(chatId: ChatId, video: InputFile, ?duration: int64, ?width: int64, ?height: int64, ?thumb: InputFile, ?caption: string, ?parseMode: string, ?captionEntities: MessageEntity[], ?supportsStreaming: bool, ?disableNotification: bool, ?protectContent: bool, ?replyToMessageId: int64, ?allowSendingWithoutReply: bool, ?replyMarkup: Markup) = ()
  interface IRequestBase<Message> with
    member _.MethodName = "sendVideo"
    
type SendAnimationReq =
  {
    ChatId: ChatId
    Animation: InputFile
    Duration: int64 option
    Width: int64 option
    Height: int64 option
    Thumb: InputFile option
    Caption: string option
    ParseMode: string option
    CaptionEntities: MessageEntity[] option
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyToMessageId: int64 option
    AllowSendingWithoutReply: bool option
    ReplyMarkup: Markup option
  }
  static member Make(chatId: ChatId, animation: InputFile, ?duration: int64, ?width: int64, ?height: int64, ?thumb: InputFile, ?caption: string, ?parseMode: string, ?captionEntities: MessageEntity[], ?disableNotification: bool, ?protectContent: bool, ?replyToMessageId: int64, ?allowSendingWithoutReply: bool, ?replyMarkup: Markup) = ()
  interface IRequestBase<Message> with
    member _.MethodName = "sendAnimation"
    
type SendVoiceReq =
  {
    ChatId: ChatId
    Voice: InputFile
    Caption: string option
    ParseMode: string option
    CaptionEntities: MessageEntity[] option
    Duration: int64 option
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyToMessageId: int64 option
    AllowSendingWithoutReply: bool option
    ReplyMarkup: Markup option
  }
  static member Make(chatId: ChatId, voice: InputFile, ?caption: string, ?parseMode: string, ?captionEntities: MessageEntity[], ?duration: int64, ?disableNotification: bool, ?protectContent: bool, ?replyToMessageId: int64, ?allowSendingWithoutReply: bool, ?replyMarkup: Markup) = ()
  interface IRequestBase<Message> with
    member _.MethodName = "sendVoice"
    
type SendVideoNoteReq =
  {
    ChatId: ChatId
    VideoNote: InputFile
    Duration: int64 option
    Length: int64 option
    Thumb: InputFile option
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyToMessageId: int64 option
    AllowSendingWithoutReply: bool option
    ReplyMarkup: Markup option
  }
  static member Make(chatId: ChatId, videoNote: InputFile, ?duration: int64, ?length: int64, ?thumb: InputFile, ?disableNotification: bool, ?protectContent: bool, ?replyToMessageId: int64, ?allowSendingWithoutReply: bool, ?replyMarkup: Markup) = ()
  interface IRequestBase<Message> with
    member _.MethodName = "sendVideoNote"
    
type SendMediaGroupReq =
  {
    ChatId: ChatId
    Media: InputMedia[]
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyToMessageId: int64 option
    AllowSendingWithoutReply: bool option
  }
  static member Make(chatId: ChatId, media: InputMedia[], ?disableNotification: bool, ?protectContent: bool, ?replyToMessageId: int64, ?allowSendingWithoutReply: bool) = ()
  interface IRequestBase<Message[]> with
    member _.MethodName = "sendMediaGroup"
    
type SendLocationReq =
  {
    ChatId: ChatId
    Latitude: float
    Longitude: float
    HorizontalAccuracy: float option
    LivePeriod: int64 option
    Heading: int64 option
    ProximityAlertRadius: int64 option
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyToMessageId: int64 option
    AllowSendingWithoutReply: bool option
    ReplyMarkup: Markup option
  }
  static member Make(chatId: ChatId, latitude: float, longitude: float, ?horizontalAccuracy: float, ?livePeriod: int64, ?heading: int64, ?proximityAlertRadius: int64, ?disableNotification: bool, ?protectContent: bool, ?replyToMessageId: int64, ?allowSendingWithoutReply: bool, ?replyMarkup: Markup) = ()
  interface IRequestBase<Message> with
    member _.MethodName = "sendLocation"
    
type EditMessageLiveLocationReq =
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
  static member Make(latitude: float, longitude: float, ?chatId: ChatId, ?messageId: int64, ?inlineMessageId: string, ?horizontalAccuracy: float, ?heading: int64, ?proximityAlertRadius: int64, ?replyMarkup: InlineKeyboardMarkup) = ()
  interface IRequestBase<EditMessageResult> with
    member _.MethodName = "editMessageLiveLocation"
    
type StopMessageLiveLocationReq =
  {
    ChatId: ChatId option
    MessageId: int64 option
    InlineMessageId: string option
    ReplyMarkup: InlineKeyboardMarkup option
  }
  static member Make(?chatId: ChatId, ?messageId: int64, ?inlineMessageId: string, ?replyMarkup: InlineKeyboardMarkup) = ()
  interface IRequestBase<EditMessageResult> with
    member _.MethodName = "stopMessageLiveLocation"
    
type SendVenueReq =
  {
    ChatId: ChatId
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
    ReplyToMessageId: int64 option
    AllowSendingWithoutReply: bool option
    ReplyMarkup: Markup option
  }
  static member Make(chatId: ChatId, latitude: float, longitude: float, title: string, address: string, ?foursquareId: string, ?foursquareType: string, ?googlePlaceId: string, ?googlePlaceType: string, ?disableNotification: bool, ?protectContent: bool, ?replyToMessageId: int64, ?allowSendingWithoutReply: bool, ?replyMarkup: Markup) = ()
  interface IRequestBase<Message> with
    member _.MethodName = "sendVenue"
    
type SendContactReq =
  {
    ChatId: ChatId
    PhoneNumber: string
    FirstName: string
    LastName: string option
    Vcard: string option
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyToMessageId: int64 option
    AllowSendingWithoutReply: bool option
    ReplyMarkup: Markup option
  }
  static member Make(chatId: ChatId, phoneNumber: string, firstName: string, ?lastName: string, ?vcard: string, ?disableNotification: bool, ?protectContent: bool, ?replyToMessageId: int64, ?allowSendingWithoutReply: bool, ?replyMarkup: Markup) = ()
  interface IRequestBase<Message> with
    member _.MethodName = "sendContact"
    
type SendPollReq =
  {
    ChatId: ChatId
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
    ReplyToMessageId: int64 option
    AllowSendingWithoutReply: bool option
    ReplyMarkup: Markup option
  }
  static member Make(chatId: ChatId, question: string, options: string[], ?replyToMessageId: int64, ?protectContent: bool, ?disableNotification: bool, ?isClosed: bool, ?closeDate: int64, ?openPeriod: int64, ?explanationParseMode: string, ?allowSendingWithoutReply: bool, ?explanation: string, ?correctOptionId: int64, ?allowsMultipleAnswers: bool, ?``type``: string, ?isAnonymous: bool, ?explanationEntities: MessageEntity[], ?replyMarkup: Markup) = ()
  interface IRequestBase<Message> with
    member _.MethodName = "sendPoll"
    
type SendDiceReq =
  {
    ChatId: ChatId
    Emoji: string option
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyToMessageId: int64 option
    AllowSendingWithoutReply: bool option
    ReplyMarkup: Markup option
  }
  static member Make(chatId: ChatId, ?emoji: string, ?disableNotification: bool, ?protectContent: bool, ?replyToMessageId: int64, ?allowSendingWithoutReply: bool, ?replyMarkup: Markup) = ()
  interface IRequestBase<Message> with
    member _.MethodName = "sendDice"
    
type SendChatActionReq =
  {
    ChatId: ChatId
    Action: string
  }
  static member Make(chatId: ChatId, action: string) = ()
  interface IRequestBase<bool> with
    member _.MethodName = "sendChatAction"
    
type GetUserProfilePhotosReq =
  {
    UserId: int64
    Offset: int64 option
    Limit: int64 option
  }
  static member Make(userId: int64, ?offset: int64, ?limit: int64) = ()
  interface IRequestBase<UserProfilePhotos> with
    member _.MethodName = "getUserProfilePhotos"
    
type GetFileReq =
  {
    FileId: string
  }
  static member Make(fileId: string) = ()
  interface IRequestBase<File> with
    member _.MethodName = "getFile"
    
type BanChatMemberReq =
  {
    ChatId: ChatId
    UserId: int64
    UntilDate: int64 option
    RevokeMessages: bool option
  }
  static member Make(chatId: ChatId, userId: int64, ?untilDate: int64, ?revokeMessages: bool) = ()
  interface IRequestBase<bool> with
    member _.MethodName = "banChatMember"
    
type UnbanChatMemberReq =
  {
    ChatId: ChatId
    UserId: int64
    OnlyIfBanned: bool option
  }
  static member Make(chatId: ChatId, userId: int64, ?onlyIfBanned: bool) = ()
  interface IRequestBase<bool> with
    member _.MethodName = "unbanChatMember"
    
type RestrictChatMemberReq =
  {
    ChatId: ChatId
    UserId: int64
    Permissions: ChatPermissions
    UntilDate: int64 option
  }
  static member Make(chatId: ChatId, userId: int64, permissions: ChatPermissions, ?untilDate: int64) = ()
  interface IRequestBase<bool> with
    member _.MethodName = "restrictChatMember"
    
type PromoteChatMemberReq =
  {
    ChatId: ChatId
    UserId: int64
    IsAnonymous: bool option
    CanManageChat: bool option
    CanPostMessages: bool option
    CanEditMessages: bool option
    CanDeleteMessages: bool option
    CanManageVideoChats: bool option
    CanRestrictMembers: bool option
    CanPromoteMembers: bool option
    CanChangeInfo: bool option
    CanInviteUsers: bool option
    CanPinMessages: bool option
  }
  static member Make(chatId: ChatId, userId: int64, ?isAnonymous: bool, ?canManageChat: bool, ?canPostMessages: bool, ?canEditMessages: bool, ?canDeleteMessages: bool, ?canManageVideoChats: bool, ?canRestrictMembers: bool, ?canPromoteMembers: bool, ?canChangeInfo: bool, ?canInviteUsers: bool, ?canPinMessages: bool) = ()
  interface IRequestBase<bool> with
    member _.MethodName = "promoteChatMember"
    
type SetChatAdministratorCustomTitleReq =
  {
    ChatId: ChatId
    UserId: int64
    CustomTitle: string
  }
  static member Make(chatId: ChatId, userId: int64, customTitle: string) = ()
  interface IRequestBase<bool> with
    member _.MethodName = "setChatAdministratorCustomTitle"
    
type BanChatSenderChatReq =
  {
    ChatId: ChatId
    SenderChatId: int64
  }
  static member Make(chatId: ChatId, senderChatId: int64) = ()
  interface IRequestBase<bool> with
    member _.MethodName = "banChatSenderChat"
    
type UnbanChatSenderChatReq =
  {
    ChatId: ChatId
    SenderChatId: int64
  }
  static member Make(chatId: ChatId, senderChatId: int64) = ()
  interface IRequestBase<bool> with
    member _.MethodName = "unbanChatSenderChat"
    
type SetChatPermissionsReq =
  {
    ChatId: ChatId
    Permissions: ChatPermissions
  }
  static member Make(chatId: ChatId, permissions: ChatPermissions) = ()
  interface IRequestBase<bool> with
    member _.MethodName = "setChatPermissions"
    
type ExportChatInviteLinkReq =
  {
    ChatId: ChatId
  }
  static member Make(chatId: ChatId) = ()
  interface IRequestBase<string> with
    member _.MethodName = "exportChatInviteLink"
    
type CreateChatInviteLinkReq =
  {
    ChatId: ChatId
    Name: string option
    ExpireDate: int64 option
    MemberLimit: int64 option
    CreatesJoinRequest: bool option
  }
  static member Make(chatId: ChatId, ?name: string, ?expireDate: int64, ?memberLimit: int64, ?createsJoinRequest: bool) = ()
  interface IRequestBase<ChatInviteLink> with
    member _.MethodName = "createChatInviteLink"
    
type EditChatInviteLinkReq =
  {
    ChatId: ChatId
    InviteLink: string
    Name: string option
    ExpireDate: int64 option
    MemberLimit: int64 option
    CreatesJoinRequest: bool option
  }
  static member Make(chatId: ChatId, inviteLink: string, ?name: string, ?expireDate: int64, ?memberLimit: int64, ?createsJoinRequest: bool) = ()
  interface IRequestBase<ChatInviteLink> with
    member _.MethodName = "editChatInviteLink"
    
type RevokeChatInviteLinkReq =
  {
    ChatId: ChatId
    InviteLink: string
  }
  static member Make(chatId: ChatId, inviteLink: string) = ()
  interface IRequestBase<ChatInviteLink> with
    member _.MethodName = "revokeChatInviteLink"
    
type ApproveChatJoinRequestReq =
  {
    ChatId: ChatId
    UserId: int64
  }
  static member Make(chatId: ChatId, userId: int64) = ()
  interface IRequestBase<bool> with
    member _.MethodName = "approveChatJoinRequest"
    
type DeclineChatJoinRequestReq =
  {
    ChatId: ChatId
    UserId: int64
  }
  static member Make(chatId: ChatId, userId: int64) = ()
  interface IRequestBase<bool> with
    member _.MethodName = "declineChatJoinRequest"
    
type SetChatPhotoReq =
  {
    ChatId: ChatId
    Photo: InputFile
  }
  static member Make(chatId: ChatId, photo: InputFile) = ()
  interface IRequestBase<bool> with
    member _.MethodName = "setChatPhoto"
    
type DeleteChatPhotoReq =
  {
    ChatId: ChatId
  }
  static member Make(chatId: ChatId) = ()
  interface IRequestBase<bool> with
    member _.MethodName = "deleteChatPhoto"
    
type SetChatTitleReq =
  {
    ChatId: ChatId
    Title: string
  }
  static member Make(chatId: ChatId, title: string) = ()
  interface IRequestBase<bool> with
    member _.MethodName = "setChatTitle"
    
type SetChatDescriptionReq =
  {
    ChatId: ChatId
    Description: string option
  }
  static member Make(chatId: ChatId, ?description: string) = ()
  interface IRequestBase<bool> with
    member _.MethodName = "setChatDescription"
    
type PinChatMessageReq =
  {
    ChatId: ChatId
    MessageId: int64
    DisableNotification: bool option
  }
  static member Make(chatId: ChatId, messageId: int64, ?disableNotification: bool) = ()
  interface IRequestBase<bool> with
    member _.MethodName = "pinChatMessage"
    
type UnpinChatMessageReq =
  {
    ChatId: ChatId
    MessageId: int64 option
  }
  static member Make(chatId: ChatId, ?messageId: int64) = ()
  interface IRequestBase<bool> with
    member _.MethodName = "unpinChatMessage"
    
type UnpinAllChatMessagesReq =
  {
    ChatId: ChatId
  }
  static member Make(chatId: ChatId) = ()
  interface IRequestBase<bool> with
    member _.MethodName = "unpinAllChatMessages"
    
type LeaveChatReq =
  {
    ChatId: ChatId
  }
  static member Make(chatId: ChatId) = ()
  interface IRequestBase<bool> with
    member _.MethodName = "leaveChat"
    
type GetChatReq =
  {
    ChatId: ChatId
  }
  static member Make(chatId: ChatId) = ()
  interface IRequestBase<Chat> with
    member _.MethodName = "getChat"
    
type GetChatAdministratorsReq =
  {
    ChatId: ChatId
  }
  static member Make(chatId: ChatId) = ()
  interface IRequestBase<ChatMember[]> with
    member _.MethodName = "getChatAdministrators"
    
type GetChatMemberCountReq =
  {
    ChatId: ChatId
  }
  static member Make(chatId: ChatId) = ()
  interface IRequestBase<int> with
    member _.MethodName = "getChatMemberCount"
    
type GetChatMemberReq =
  {
    ChatId: ChatId
    UserId: int64
  }
  static member Make(chatId: ChatId, userId: int64) = ()
  interface IRequestBase<ChatMember> with
    member _.MethodName = "getChatMember"
    
type SetChatStickerSetReq =
  {
    ChatId: ChatId
    StickerSetName: string
  }
  static member Make(chatId: ChatId, stickerSetName: string) = ()
  interface IRequestBase<bool> with
    member _.MethodName = "setChatStickerSet"
    
type DeleteChatStickerSetReq =
  {
    ChatId: ChatId
  }
  static member Make(chatId: ChatId) = ()
  interface IRequestBase<bool> with
    member _.MethodName = "deleteChatStickerSet"
    
type AnswerCallbackQueryReq =
  {
    CallbackQueryId: string
    Text: string option
    ShowAlert: bool option
    Url: string option
    CacheTime: int64 option
  }
  static member Make(callbackQueryId: string, ?text: string, ?showAlert: bool, ?url: string, ?cacheTime: int64) = ()
  interface IRequestBase<bool> with
    member _.MethodName = "answerCallbackQuery"
    
type SetMyCommandsReq =
  {
    Commands: BotCommand[]
    Scope: BotCommandScope option
    LanguageCode: string option
  }
  static member Make(commands: BotCommand[], ?scope: BotCommandScope, ?languageCode: string) = ()
  interface IRequestBase<bool> with
    member _.MethodName = "setMyCommands"
    
type DeleteMyCommandsReq =
  {
    Scope: BotCommandScope option
    LanguageCode: string option
  }
  static member Make(?scope: BotCommandScope, ?languageCode: string) = ()
  interface IRequestBase<bool> with
    member _.MethodName = "deleteMyCommands"
    
type GetMyCommandsReq =
  {
    Scope: BotCommandScope option
    LanguageCode: string option
  }
  static member Make(?scope: BotCommandScope, ?languageCode: string) = ()
  interface IRequestBase<BotCommand[]> with
    member _.MethodName = "getMyCommands"
    
type SetChatMenuButtonReq =
  {
    ChatId: int64 option
    MenuButton: MenuButton option
  }
  static member Make(?chatId: int64, ?menuButton: MenuButton) = ()
  interface IRequestBase<bool> with
    member _.MethodName = "setChatMenuButton"
    
type GetChatMenuButtonReq =
  {
    ChatId: int64 option
  }
  static member Make(?chatId: int64) = ()
  interface IRequestBase<MenuButton> with
    member _.MethodName = "getChatMenuButton"
    
type SetMyDefaultAdministratorRightsReq =
  {
    Rights: ChatAdministratorRights option
    ForChannels: bool option
  }
  static member Make(?rights: ChatAdministratorRights, ?forChannels: bool) = ()
  interface IRequestBase<bool> with
    member _.MethodName = "setMyDefaultAdministratorRights"
    
type GetMyDefaultAdministratorRightsReq =
  {
    ForChannels: bool option
  }
  static member Make(?forChannels: bool) = ()
  interface IRequestBase<ChatAdministratorRights> with
    member _.MethodName = "getMyDefaultAdministratorRights"
    
type EditMessageTextReq =
  {
    ChatId: ChatId option
    MessageId: int64 option
    InlineMessageId: string option
    Text: string
    ParseMode: string option
    Entities: MessageEntity[] option
    DisableWebPagePreview: bool option
    ReplyMarkup: InlineKeyboardMarkup option
  }
  static member Make(text: string, ?chatId: ChatId, ?messageId: int64, ?inlineMessageId: string, ?parseMode: string, ?entities: MessageEntity[], ?disableWebPagePreview: bool, ?replyMarkup: InlineKeyboardMarkup) = ()
  interface IRequestBase<EditMessageResult> with
    member _.MethodName = "editMessageText"
    
type EditMessageCaptionReq =
  {
    ChatId: ChatId option
    MessageId: int64 option
    InlineMessageId: string option
    Caption: string option
    ParseMode: string option
    CaptionEntities: MessageEntity[] option
    ReplyMarkup: InlineKeyboardMarkup option
  }
  static member Make(?chatId: ChatId, ?messageId: int64, ?inlineMessageId: string, ?caption: string, ?parseMode: string, ?captionEntities: MessageEntity[], ?replyMarkup: InlineKeyboardMarkup) = ()
  interface IRequestBase<EditMessageResult> with
    member _.MethodName = "editMessageCaption"
    
type EditMessageMediaReq =
  {
    ChatId: ChatId option
    MessageId: int64 option
    InlineMessageId: string option
    Media: InputMedia
    ReplyMarkup: InlineKeyboardMarkup option
  }
  static member Make(media: InputMedia, ?chatId: ChatId, ?messageId: int64, ?inlineMessageId: string, ?replyMarkup: InlineKeyboardMarkup) = ()
  interface IRequestBase<EditMessageResult> with
    member _.MethodName = "editMessageMedia"
    
type EditMessageReplyMarkupReq =
  {
    ChatId: ChatId option
    MessageId: int64 option
    InlineMessageId: string option
    ReplyMarkup: InlineKeyboardMarkup option
  }
  static member Make(?chatId: ChatId, ?messageId: int64, ?inlineMessageId: string, ?replyMarkup: InlineKeyboardMarkup) = ()
  interface IRequestBase<EditMessageResult> with
    member _.MethodName = "editMessageReplyMarkup"
    
type StopPollReq =
  {
    ChatId: ChatId
    MessageId: int64
    ReplyMarkup: InlineKeyboardMarkup option
  }
  static member Make(chatId: ChatId, messageId: int64, ?replyMarkup: InlineKeyboardMarkup) = ()
  interface IRequestBase<Poll> with
    member _.MethodName = "stopPoll"
    
type DeleteMessageReq =
  {
    ChatId: ChatId
    MessageId: int64
  }
  static member Make(chatId: ChatId, messageId: int64) = ()
  interface IRequestBase<bool> with
    member _.MethodName = "deleteMessage"
    
type SendStickerReq =
  {
    ChatId: ChatId
    Sticker: InputFile
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyToMessageId: int64 option
    AllowSendingWithoutReply: bool option
    ReplyMarkup: Markup option
  }
  static member Make(chatId: ChatId, sticker: InputFile, ?disableNotification: bool, ?protectContent: bool, ?replyToMessageId: int64, ?allowSendingWithoutReply: bool, ?replyMarkup: Markup) = ()
  interface IRequestBase<Message> with
    member _.MethodName = "sendSticker"
    
type GetStickerSetReq =
  {
    Name: string
  }
  static member Make(name: string) = ()
  interface IRequestBase<StickerSet> with
    member _.MethodName = "getStickerSet"
    
type UploadStickerFileReq =
  {
    UserId: int64
    PngSticker: InputFile
  }
  static member Make(userId: int64, pngSticker: InputFile) = ()
  interface IRequestBase<File> with
    member _.MethodName = "uploadStickerFile"
    
type CreateNewStickerSetReq =
  {
    UserId: int64
    Name: string
    Title: string
    PngSticker: InputFile option
    TgsSticker: InputFile option
    WebmSticker: InputFile option
    Emojis: string
    ContainsMasks: bool option
    MaskPosition: MaskPosition option
  }
  static member Make(userId: int64, name: string, title: string, emojis: string, ?pngSticker: InputFile, ?tgsSticker: InputFile, ?webmSticker: InputFile, ?containsMasks: bool, ?maskPosition: MaskPosition) = ()
  interface IRequestBase<bool> with
    member _.MethodName = "createNewStickerSet"
    
type AddStickerToSetReq =
  {
    UserId: int64
    Name: string
    PngSticker: InputFile option
    TgsSticker: InputFile option
    WebmSticker: InputFile option
    Emojis: string
    MaskPosition: MaskPosition option
  }
  static member Make(userId: int64, name: string, emojis: string, ?pngSticker: InputFile, ?tgsSticker: InputFile, ?webmSticker: InputFile, ?maskPosition: MaskPosition) = ()
  interface IRequestBase<bool> with
    member _.MethodName = "addStickerToSet"
    
type SetStickerPositionInSetReq =
  {
    Sticker: string
    Position: int64
  }
  static member Make(sticker: string, position: int64) = ()
  interface IRequestBase<bool> with
    member _.MethodName = "setStickerPositionInSet"
    
type DeleteStickerFromSetReq =
  {
    Sticker: string
  }
  static member Make(sticker: string) = ()
  interface IRequestBase<bool> with
    member _.MethodName = "deleteStickerFromSet"
    
type SetStickerSetThumbReq =
  {
    Name: string
    UserId: int64
    Thumb: InputFile option
  }
  static member Make(name: string, userId: int64, ?thumb: InputFile) = ()
  interface IRequestBase<bool> with
    member _.MethodName = "setStickerSetThumb"
    
type AnswerInlineQueryReq =
  {
    InlineQueryId: string
    Results: InlineQueryResult[]
    CacheTime: int64 option
    IsPersonal: bool option
    NextOffset: string option
    SwitchPmText: string option
    SwitchPmParameter: string option
  }
  static member Make(inlineQueryId: string, results: InlineQueryResult[], ?cacheTime: int64, ?isPersonal: bool, ?nextOffset: string, ?switchPmText: string, ?switchPmParameter: string) = ()
  interface IRequestBase<bool> with
    member _.MethodName = "answerInlineQuery"
    
type AnswerWebAppQueryReq =
  {
    WebAppQueryId: string
    Result: InlineQueryResult
  }
  static member Make(webAppQueryId: string, result: InlineQueryResult) = ()
  interface IRequestBase<SentWebAppMessage> with
    member _.MethodName = "answerWebAppQuery"
    
type SendInvoiceReq =
  {
    ChatId: ChatId
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
    ReplyToMessageId: int64 option
    AllowSendingWithoutReply: bool option
    ReplyMarkup: InlineKeyboardMarkup option
  }
  static member Make(chatId: ChatId, title: string, description: string, payload: string, providerToken: string, currency: string, prices: LabeledPrice[], ?replyToMessageId: int64, ?protectContent: bool, ?disableNotification: bool, ?isFlexible: bool, ?sendEmailToProvider: bool, ?sendPhoneNumberToProvider: bool, ?needShippingAddress: bool, ?needEmail: bool, ?needPhoneNumber: bool, ?photoWidth: int64, ?photoHeight: int64, ?allowSendingWithoutReply: bool, ?photoSize: int64, ?photoUrl: string, ?providerData: string, ?startParameter: string, ?suggestedTipAmounts: int64[], ?maxTipAmount: int64, ?needName: bool, ?replyMarkup: InlineKeyboardMarkup) = ()
  interface IRequestBase<Message> with
    member _.MethodName = "sendInvoice"
    
type AnswerShippingQueryReq =
  {
    ShippingQueryId: string
    Ok: bool
    ShippingOptions: ShippingOption[] option
    ErrorMessage: string option
  }
  static member Make(shippingQueryId: string, ok: bool, ?shippingOptions: ShippingOption[], ?errorMessage: string) = ()
  interface IRequestBase<bool> with
    member _.MethodName = "answerShippingQuery"
    
type AnswerPreCheckoutQueryReq =
  {
    PreCheckoutQueryId: string
    Ok: bool
    ErrorMessage: string option
  }
  static member Make(preCheckoutQueryId: string, ok: bool, ?errorMessage: string) = ()
  interface IRequestBase<bool> with
    member _.MethodName = "answerPreCheckoutQuery"
    
type SetPassportDataErrorsReq =
  {
    UserId: int64
    Errors: PassportElementError[]
  }
  static member Make(userId: int64, errors: PassportElementError[]) = ()
  interface IRequestBase<bool> with
    member _.MethodName = "setPassportDataErrors"
    
type SendGameReq =
  {
    ChatId: int64
    GameShortName: string
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyToMessageId: int64 option
    AllowSendingWithoutReply: bool option
    ReplyMarkup: InlineKeyboardMarkup option
  }
  static member Make(chatId: int64, gameShortName: string, ?disableNotification: bool, ?protectContent: bool, ?replyToMessageId: int64, ?allowSendingWithoutReply: bool, ?replyMarkup: InlineKeyboardMarkup) = ()
  interface IRequestBase<Message> with
    member _.MethodName = "sendGame"
    
type SetGameScoreReq =
  {
    UserId: int64
    Score: int64
    Force: bool option
    DisableEditMessage: bool option
    ChatId: int64 option
    MessageId: int64 option
    InlineMessageId: string option
  }
  static member Make(userId: int64, score: int64, ?force: bool, ?disableEditMessage: bool, ?chatId: int64, ?messageId: int64, ?inlineMessageId: string) = ()
  interface IRequestBase<EditMessageResult> with
    member _.MethodName = "setGameScore"
    
type GetGameHighScoresReq =
  {
    UserId: int64
    ChatId: int64 option
    MessageId: int64 option
    InlineMessageId: string option
  }
  static member Make(userId: int64, ?chatId: int64, ?messageId: int64, ?inlineMessageId: string) = ()
  interface IRequestBase<GameHighScore[]> with
    member _.MethodName = "getGameHighScores"
    