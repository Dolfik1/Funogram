module Funogram.Telegram.RequestsTypes

open Funogram.Types
open Types
open System

type GetUpdatesReq = 
  { Offset: int64 option
    Limit: int option
    Timeout: int option
    AllowedUpdates: string seq }
  interface IRequestBase<Update seq> with
    member __.MethodName = "getUpdates"

type SetWebhookReq = 
  { Url: string
    Certificate: FileToSend option
    MaxConnections: int option
    AllowedUpdates: (string seq) option }
  interface IRequestBase<bool> with
    member __.MethodName = "setWebhook"

type DeleteWebhookReq() =
  interface IRequestBase<bool> with
    member __.MethodName = "deleteWebhook"


type GetWebhookInfoReq() =
  interface IRequestBase<WebhookInfo> with
    member __.MethodName = "getWebhookInfo"

type GetMeReq() = 
  interface IRequestBase<User> with
    member __.MethodName = "getMe"

type SendMessageReq = 
  { ChatId: ChatId
    Text: string
    ParseMode: ParseMode option
    DisableWebPagePreview: bool option
    DisableNotification: bool option
    ReplyToMessageId: int64 option
    ReplyMarkup: Markup option }
  interface IRequestBase<Message> with
    member __.MethodName = "sendMessage"

let sendMessageReqBase = 
  { ChatId = ChatId.Int 0L
    Text = ""
    ParseMode = None
    DisableWebPagePreview = None
    DisableNotification = None
    ReplyToMessageId = None
    ReplyMarkup = None }

type ForwardMessageReq = 
  { ChatId: ChatId
    FromChatId: ChatId
    MessageId: int64
    DisableNotification: bool option }
  interface IRequestBase<Message> with
    member __.MethodName = "forwardMessage"

type SendPhotoReq = 
  { ChatId: ChatId
    Photo: FileToSend
    Caption: string option
    ParseMode: ParseMode option
    DisableNotification: bool option
    ReplyToMessageId: int64 option
    ReplyMarkup: Markup option }
  interface IRequestBase<Message> with
    member __.MethodName = "sendPhoto"

type SendAudioReq = 
  { ChatId: ChatId
    Audio: FileToSend
    Caption: string option
    ParseMode: ParseMode option
    Duration: int option
    Performer: string option
    Title: string option
    Thumb: FileToSend option
    DisableNotification: bool option
    ReplyToMessageId: int64 option
    ReplyMarkup: Markup option }
  interface IRequestBase<Message> with
    member __.MethodName = "sendAudio"

type SendDocumentReq = 
  { ChatId: ChatId
    Document: FileToSend
    Thumb: FileToSend  option
    Caption: string option
    ParseMode: ParseMode option
    DisableNotification: bool option
    ReplyToMessageId: int64 option
    ReplyMarkup: Markup option }
  interface IRequestBase<Message> with
    member __.MethodName = "sendDocument"

type SendStickerReq = 
  { ChatId: ChatId
    Sticker: FileToSend
    DisableNotification: bool option
    ReplyToMessageId: int64 option
    ReplyMarkup: Markup option }
  interface IRequestBase<Message> with
    member __.MethodName = "sendSticker"

type SendVideoReq = 
  { ChatId: ChatId
    Video: FileToSend
    Duration: int option
    Width: int option
    Height: int option
    Thumb: FileToSend option
    Caption: string option
    ParseMode: ParseMode option
    DisableNotification: bool option
    ReplyToMessageId: int64 option
    ReplyMarkup: Markup option }
  interface IRequestBase<Message> with
    member __.MethodName = "sendVideo"

type SendVoiceReq = 
  { ChatId: ChatId
    Voice: FileToSend
    Caption: string option
    ParseMode: ParseMode option
    Duration: int option
    DisableNotification: bool option
    ReplyToMessageId: int64 option
    ReplyMarkup: Markup option }
  interface IRequestBase<Message> with
    member __.MethodName = "sendVoice"

type SendMediaGroupReq = 
  { ChatId: ChatId
    Media: InputMedia[]
    DisableNotification: bool option
    ReplyToMessageId: int64 option }
  interface IRequestBase<Message[]> with
    member __.MethodName = "sendMediaGroup"


type SendVideoNoteReq = 
  { ChatId: ChatId
    VideoNote: FileToSend
    Duration: int option
    Length: int option
    Thumb: FileToSend option
    DisableNotification: bool option
    ReplyToMessageId: int64 option
    ReplyMarkup: Markup option }
  interface IRequestBase<Message> with
    member __.MethodName = "sendVideoNote"

type SendAnimationReq = 
  { ChatId: ChatId
    Animation: FileToSend
    Duration: int option
    Width: int option
    Height: int option
    Thumb: FileToSend option
    Caption: string option
    ParseMode: ParseMode option
    DisableNotification: bool option
    ReplyToMessageId: int64 option
    ReplyMarkup: Markup option }
  interface IRequestBase<Message> with
    member __.MethodName = "sendAnimation"

type SendLocationReq = 
  { ChatId: ChatId
    Latitude: Double
    Longitude: Double
    LivePeriod: int option
    DisableNotification: bool option
    ReplyToMessageId: int64 option
    ReplyMarkup: Markup option }
  interface IRequestBase<Message> with
    member __.MethodName = "sendLocation"

type EditMessageLiveLocationReq = 
  { ChatId: ChatId option
    MessageId: int option
    InlineMessageId: string option
    Latitude: Double
    Longitude: Double
    ReplyMarkup: Markup option }
  interface IRequestBase<EditMessageResult> with
    member __.MethodName = "editMessageLiveLocation"

type StopMessageLiveLocationReq = 
  { ChatId: ChatId option
    MessageId: int option
    InlineMessageId: string option
    ReplyMarkup: Markup option }
  interface IRequestBase<EditMessageResult> with
    member __.MethodName = "stopMessageLiveLocation"

type SendVenueReq = 
  { ChatId: ChatId
    Latitude: Double
    Longitude: Double
    Title: string
    Address: string
    FoursquareId: string option
    FoursquareType: string option
    DisableNotification: bool option
    ReplyToMessageId: int64 option
    ReplyMarkup: Markup option }
  interface IRequestBase<Message> with
    member __.MethodName = "sendVenue"

type SendContactReq = 
  { ChatId: ChatId
    PhoneNumber: string
    FirstName: string
    LastName: string option
    VCard: string option
    DisableNotification: bool option
    ReplyToMessageId: int64 option
    ReplyMarkup: Markup option }
  interface IRequestBase<Message> with
    member __.MethodName = "sendContact"

type SendPollReq = 
  { ChatId: ChatId
    Question: string
    Options: string[]
    DisableNotification: bool option
    ReplyToMessageId: int64 option
    ReplyMarkup: Markup option }
  interface IRequestBase<Message> with
    member __.MethodName = "sendPoll"

type SendChatActionReq = 
  { ChatId: ChatId
    Action: ChatAction }
  interface IRequestBase<bool> with
    member __.MethodName = "sendChatAction"

type GetFileReq = 
  { FileId: string }
  interface IRequestBase<File> with
    member __.MethodName = "getFile"

type GetUserProfilePhotosReq = 
  { UserId: int64
    Offset: int option
    Limit: int option }
  interface IRequestBase<UserProfilePhotos> with
    member __.MethodName = "getUserProfilePhotos"

type UnbanChatMemberReq = 
  { ChatId: ChatId
    UserId: int64 }
  interface IRequestBase<bool> with
    member __.MethodName = "unbanChatMember"

type LeaveChatReq = 
  { ChatId: ChatId }
  interface IRequestBase<bool> with
    member __.MethodName = "leaveChat"

type GetChatReq = 
  { ChatId: ChatId }
  interface IRequestBase<Chat> with
    member __.MethodName = "getChat"

type GetChatAdministratorsReq = 
  { ChatId: ChatId }
  interface IRequestBase<ChatMember seq> with
    member __.MethodName = "getChatAdministrators"

type GetChatMembersCountReq = 
  { ChatId: ChatId }
  interface IRequestBase<int> with
    member __.MethodName = "getChatMembersCount"

type GetChatMemberReq = 
  { ChatId: ChatId
    UserId: int64 }
  interface IRequestBase<ChatMember> with
    member __.MethodName = "getChatMember"

type RestrictChatMemberReq = 
  { ChatId: ChatId
    UserId: int64
    Permissions: ChatPermissions
    UntilDate: DateTime option }
  interface IRequestBase<bool> with
    member __.MethodName = "restrictChatMember"

type PromoteChatMemberReq = 
  { ChatId: ChatId
    UserId: int64
    CanChangeInfo: bool option
    CanPostMessages: bool option
    CanEditMessages: bool option
    CanDeleteMessages: bool option
    CanInviteUsers: bool option
    CanRestrictMembers: bool option
    CanPinMessages: bool option
    CanPromoteMembers: bool option }
  interface IRequestBase<bool> with
    member __.MethodName = "promoteChatMember"

type SetChatPermissionsReq = 
  { ChatId: ChatId
    Permissions: ChatPermissions }
  interface IRequestBase<bool> with
    member __.MethodName = "setChatPermissions"

type KickChatMemberReq = 
  { ChatId: ChatId
    UserId: int64
    UntilDate: DateTime option }
  interface IRequestBase<bool> with
    member __.MethodName = "kickChatMember"

type ExportChatInviteLinkReq = 
  { ChatId: ChatId }
  interface IRequestBase<string> with
    member __.MethodName = "exportChatInviteLink"

type SetChatPhotoReq = 
  { ChatId: ChatId
    Photo: FileToSend }
  interface IRequestBase<string> with
    member __.MethodName = "setChatPhoto"

type DeleteChatPhotoReq = 
  { ChatId: ChatId }
  interface IRequestBase<string> with
    member __.MethodName = "deleteChatPhoto"

type SetChatTitleReq = 
  { ChatId: ChatId
    Title: string }
  interface IRequestBase<string> with
    member __.MethodName = "setChatTitle"

type SetChatDescriptionReq = 
  { ChatId: ChatId
    Description: string }
  interface IRequestBase<string> with
    member __.MethodName = "setChatDescription"

type PinChatMessageReq = 
  { ChatId: ChatId
    MessageId: int64
    DisableNotification: bool option }
  interface IRequestBase<bool> with
    member __.MethodName = "pinChatMessage"

type UnpinChatMessageReq = 
  { ChatId: ChatId }
  interface IRequestBase<bool> with
    member __.MethodName = "unpinChatMessage"

type SetChatStickerSetReq = 
  { ChatId: ChatId
    StickerSetName: string }
  interface IRequestBase<bool> with
    member __.MethodName = "setChatStickerSet"

type DeleteChatStickerSet = 
  { ChatId: ChatId }
  interface IRequestBase<bool> with
    member __.MethodName = "deleteChatStickerSet"

type AnswerCallbackQueryReq = 
  { CallbackQueryId: string option
    Text: string option
    ShowAlert: bool option
    Url: string option
    CacheTime: int option }
  interface IRequestBase<bool> with
    member __.MethodName = "answerCallbackQuery"

type EditMessageTextReq = 
  { Text: string
    ChatId: ChatId option
    MessageId: int64 option
    InlineMessageId: string option
    ParseMode: ParseMode option
    DisableWebPagePreview: bool option
    ReplyMarkup: InlineKeyboardMarkup option }
  interface IRequestBase<EditMessageResult> with
    member __.MethodName = "editMessageText"

type EditMessageCaptionReq = 
  { ChatId: ChatId option
    MessageId: int64 option
    InlineMessageId: string option
    Caption: string option
    ReplyMarkup: InlineKeyboardMarkup option }
  interface IRequestBase<EditMessageResult> with
    member __.MethodName = "editMessageCaption"

type EditMessageMediaReq = 
  { ChatId: ChatId option
    MessageId: int64 option
    InlineMessageId: string option
    Media: InputMedia
    ReplyMarkup: InlineKeyboardMarkup option }
  interface IRequestBase<EditMessageResult> with
    member __.MethodName = "editMessageMedia"

type EditMessageReplyMarkupReq = 
  { ChatId: ChatId option
    MessageId: int64 option
    InlineMessageId: string option
    ReplyMarkup: InlineKeyboardMarkup option }
  interface IRequestBase<EditMessageResult> with
    member __.MethodName = "editMessageReplyMarkup"

type StopPollReq = 
  { ChatId: ChatId option
    MessageId: int64 option
    ReplyMarkup: InlineKeyboardMarkup option }
  interface IRequestBase<Poll> with
    member __.MethodName = "stopPoll"

type DeleteMessageReq = 
  { ChatId: ChatId
    MessageId: int64 }
  interface IRequestBase<EditMessageResult> with
    member __.MethodName = "deleteMessage"

type AnswerInlineQueryReq = 
  { InlineQueryId: string
    Results: InlineQueryResult[]
    CacheTime: int option
    IsPersonal: bool option
    NextOffset: string option
    SwitchPmText: string option
    SwitchPmParameter: string option }
  interface IRequestBase<EditMessageResult> with
    member __.MethodName = "answerInlineQuery"

type SendInvoiceReq = 
  { ChatId: int64
    Title: string
    Description: string option
    Payload: string
    ProviderToken: string
    StartParameter: string
    Currency: string
    Prices: LabeledPrice seq
    ProviderData: string option
    PhotoUrl: string option
    PhotoSize: int option
    PhotoWidth: int option
    PhotoHeight: int option
    NeedName: bool option
    NeedPhoneNumber: bool option
    NeedEmail: bool option
    NeedShippingAddress: bool option
    IsFlexible: bool option
    ReplyToMessageId: int option
    ReplyMarkup: InlineKeyboardMarkup option }
  interface IRequestBase<Message> with
    member __.MethodName = "sendInvoice"

type SendInvoiceArgs = 
  { PhotoUrl: string option
    Description: string option
    PhotoSize: int option
    PhotoWidth: int option
    PhotoHeight: int option
    NeedName: bool option
    NeedPhoneNumber: bool option
    NeedEmail: bool option
    NeedShippingAddress: bool option
    SendPhoneNumberToProvider: bool option
    SendEmailToProvider: bool option
    IsFlexible: bool option
    ReplyToMessageId: int option
    ReplyMarkup: InlineKeyboardMarkup option }

let defaultInvoiceArgs = 
  { SendInvoiceArgs.PhotoUrl = None
    Description = None
    PhotoSize = None
    PhotoWidth = None
    PhotoHeight = None
    NeedName = None
    NeedPhoneNumber = None
    NeedEmail = None
    NeedShippingAddress = None
    IsFlexible = None
    ReplyToMessageId = None
    ReplyMarkup = None
    SendPhoneNumberToProvider = None
    SendEmailToProvider = None }

type AnswerShippingQueryReq = 
  { ShippingQueryId: string
    Ok: bool
    ShippingOptions: ShippingOption seq option
    ErrorMessage: string option }
  interface IRequestBase<bool> with
    member __.MethodName = "answerShippingQuery"

type AnswerPreCheckoutQueryReq = 
  { PreCheckoutQueryId: string
    Ok: bool
    ErrorMessage: string option }
  interface IRequestBase<bool> with
    member __.MethodName = "answerPreCheckoutQuery"

type SetPassportDataErrorsReq = 
  { UserId: int64
    Errors: PassportElementError[] }
  interface IRequestBase<bool> with
    member __.MethodName = "setPassportDataErrors"

type SendGameReq = 
  { ChatId: int64
    GameShortName: string
    DisableNotification: bool option
    ReplyToMessageId: int64 option
    ReplyMarkup: Markup option }
  interface IRequestBase<Message> with
    member __.MethodName = "sendGame"
type SetGameScoreReq = 
  { UserId: int64
    Score: uint32
    Force: bool option
    DisableEditMessage: bool option
    ChatId: int option
    MessageId: int64 option
    InlineMessageId: string option }
  interface IRequestBase<EditMessageResult> with
    member __.MethodName = "setGameScore"

type GetGameHighScoresReq = 
  { UserId: int64
    ChatId: int64 option
    MessageId: int64 option
    InlineMessageId: string option }
  interface IRequestBase<GameHighScore seq> with
    member __.MethodName = "getGameHighScores"

// Stickers
type GetStickerSetReq = 
  { Name: string }
  interface IRequestBase<StickerSet> with
    member __.MethodName = "getStickerSet"

type UploadStickerFileReq = 
  { UserId: int64
    PngSticker: File }
  interface IRequestBase<File> with
    member __.MethodName = "uploadStickerFile"

type CreateNewStickerSetReq = 
  { UserId: int64
    Name: string
    Title: string
    PngSticker: File
    Emojis: string
    ContainsMasks: bool option
    MaskPosition: MaskPosition option }
  interface IRequestBase<bool> with
    member __.MethodName = "createNewStickerSet"

type AddStickerToSetReq = 
  { UserId: int64
    Name: string
    PngSticker: File
    Emojis: string
    MaskPosition: MaskPosition option }
  interface IRequestBase<bool> with
    member __.MethodName = "addStickerToSet"

type SetStickerPositionInSetReq = 
  { Sticker: string
    Position: int }
  interface IRequestBase<bool> with
    member __.MethodName = "setStickerPositionInSet"

type DeleteStickerFromSet = 
  { Sticker: string }
  interface IRequestBase<bool> with
    member __.MethodName = "deleteStickerFromSet"

type SetMyCommandsReq =
  { Commands: BotCommand array
    LaungeageCode: string option }
  interface IRequestBase<bool> with
    member __.MethodName = "setMyCommands"

type DeleteMyCommandsReq =
  { LaungeageCode: string option }
  interface IRequestBase<bool> with
    member __.MethodName = "deleteMyCommands"

type GetMyCommandsReq =
  { LaungeageCode: string option }
  interface IRequestBase<BotCommand array> with
    member __.MethodName = "getMyCommands"
