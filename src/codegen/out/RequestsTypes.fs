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
  interface IRequestBase<Update[]> with
    member _.MethodName = "Array of Update"
    
type SetWebhookReq =
  {
    Url: string
    Certificate: InputFile option
    IpAddress: string option
    MaxConnections: int64 option
    AllowedUpdates: string[] option
    DropPendingUpdates: bool option
  }
  interface IRequestBase<bool> with
    member _.MethodName = "True"
    
type DeleteWebhookReq =
  {
    DropPendingUpdates: bool option
  }
  interface IRequestBase<bool> with
    member _.MethodName = "True"
    
type GetWebhookInfoReq =
  {
    Url: string
    HasCustomCertificate: bool
    PendingUpdateCount: int64
    IpAddress: string option
    LastErrorDate: int64 option
    LastErrorMessage: string option
    MaxConnections: int64 option
    AllowedUpdates: string[] option
  }
  interface IRequestBase<WebhookInfo> with
    member _.MethodName = "WebhookInfo"
    
type GetMeReq() =
  interface IRequestBase<User> with
    member _.MethodName = "User"
    
type LogOutReq() =
  interface IRequestBase<bool> with
    member _.MethodName = "True"
    
type CloseReq() =
  interface IRequestBase<bool> with
    member _.MethodName = "True"
    
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
    ReplyMarkup: InlineKeyboardMarkup or ReplyKeyboardMarkup or ReplyKeyboardRemove or ForceReply option
  }
  interface IRequestBase<Message> with
    member _.MethodName = "Message"
    
type ForwardMessageReq =
  {
    ChatId: ChatId
    FromChatId: ChatId
    DisableNotification: bool option
    ProtectContent: bool option
    MessageId: int64
  }
  interface IRequestBase<Message> with
    member _.MethodName = "Message"
    
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
    ReplyMarkup: InlineKeyboardMarkup or ReplyKeyboardMarkup or ReplyKeyboardRemove or ForceReply option
  }
  interface IRequestBase<MessageId> with
    member _.MethodName = "MessageId"
    
type SendPhotoReq =
  {
    ChatId: ChatId
    Photo: FileToSend
    Caption: string option
    ParseMode: string option
    CaptionEntities: MessageEntity[] option
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyToMessageId: int64 option
    AllowSendingWithoutReply: bool option
    ReplyMarkup: InlineKeyboardMarkup or ReplyKeyboardMarkup or ReplyKeyboardRemove or ForceReply option
  }
  interface IRequestBase<Message> with
    member _.MethodName = "Message"
    
type SendAudioReq =
  {
    ChatId: ChatId
    Audio: FileToSend
    Caption: string option
    ParseMode: string option
    CaptionEntities: MessageEntity[] option
    Duration: int64 option
    Performer: string option
    Title: string option
    Thumb: FileToSend option
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyToMessageId: int64 option
    AllowSendingWithoutReply: bool option
    ReplyMarkup: InlineKeyboardMarkup or ReplyKeyboardMarkup or ReplyKeyboardRemove or ForceReply option
  }
  interface IRequestBase<Message> with
    member _.MethodName = "Message"
    
type SendDocumentReq =
  {
    ChatId: ChatId
    Document: FileToSend
    Thumb: FileToSend option
    Caption: string option
    ParseMode: string option
    CaptionEntities: MessageEntity[] option
    DisableContentTypeDetection: bool option
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyToMessageId: int64 option
    AllowSendingWithoutReply: bool option
    ReplyMarkup: InlineKeyboardMarkup or ReplyKeyboardMarkup or ReplyKeyboardRemove or ForceReply option
  }
  interface IRequestBase<Message> with
    member _.MethodName = "Message"
    
type SendVideoReq =
  {
    ChatId: ChatId
    Video: FileToSend
    Duration: int64 option
    Width: int64 option
    Height: int64 option
    Thumb: FileToSend option
    Caption: string option
    ParseMode: string option
    CaptionEntities: MessageEntity[] option
    SupportsStreaming: bool option
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyToMessageId: int64 option
    AllowSendingWithoutReply: bool option
    ReplyMarkup: InlineKeyboardMarkup or ReplyKeyboardMarkup or ReplyKeyboardRemove or ForceReply option
  }
  interface IRequestBase<Message> with
    member _.MethodName = "Message"
    
type SendAnimationReq =
  {
    ChatId: ChatId
    Animation: FileToSend
    Duration: int64 option
    Width: int64 option
    Height: int64 option
    Thumb: FileToSend option
    Caption: string option
    ParseMode: string option
    CaptionEntities: MessageEntity[] option
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyToMessageId: int64 option
    AllowSendingWithoutReply: bool option
    ReplyMarkup: InlineKeyboardMarkup or ReplyKeyboardMarkup or ReplyKeyboardRemove or ForceReply option
  }
  interface IRequestBase<Message> with
    member _.MethodName = "Message"
    
type SendVoiceReq =
  {
    ChatId: ChatId
    Voice: FileToSend
    Caption: string option
    ParseMode: string option
    CaptionEntities: MessageEntity[] option
    Duration: int64 option
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyToMessageId: int64 option
    AllowSendingWithoutReply: bool option
    ReplyMarkup: InlineKeyboardMarkup or ReplyKeyboardMarkup or ReplyKeyboardRemove or ForceReply option
  }
  interface IRequestBase<Message> with
    member _.MethodName = "Message"
    
type SendVideoNoteReq =
  {
    ChatId: ChatId
    VideoNote: FileToSend
    Duration: int64 option
    Length: int64 option
    Thumb: FileToSend option
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyToMessageId: int64 option
    AllowSendingWithoutReply: bool option
    ReplyMarkup: InlineKeyboardMarkup or ReplyKeyboardMarkup or ReplyKeyboardRemove or ForceReply option
  }
  interface IRequestBase<Message> with
    member _.MethodName = "Message"
    
type SendMediaGroupReq =
  {
    ChatId: ChatId
    Media: InputMediaAudio, InputMediaDocument, InputMediaPhoto and InputMediaVideo[]
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyToMessageId: int64 option
    AllowSendingWithoutReply: bool option
  }
  interface IRequestBase<array of Messages> with
    member _.MethodName = "array of Messages"
    
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
    ReplyMarkup: InlineKeyboardMarkup or ReplyKeyboardMarkup or ReplyKeyboardRemove or ForceReply option
  }
  interface IRequestBase<Message> with
    member _.MethodName = "Message"
    
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
  interface IRequestBase<Message or True> with
    member _.MethodName = "Message or True"
    
type StopMessageLiveLocationReq =
  {
    ChatId: ChatId option
    MessageId: int64 option
    InlineMessageId: string option
    ReplyMarkup: InlineKeyboardMarkup option
  }
  interface IRequestBase<Message or True> with
    member _.MethodName = "Message or True"
    
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
    ReplyMarkup: InlineKeyboardMarkup or ReplyKeyboardMarkup or ReplyKeyboardRemove or ForceReply option
  }
  interface IRequestBase<Message> with
    member _.MethodName = "Message"
    
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
    ReplyMarkup: InlineKeyboardMarkup or ReplyKeyboardMarkup or ReplyKeyboardRemove or ForceReply option
  }
  interface IRequestBase<Message> with
    member _.MethodName = "Message"
    
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
    ReplyMarkup: InlineKeyboardMarkup or ReplyKeyboardMarkup or ReplyKeyboardRemove or ForceReply option
  }
  interface IRequestBase<Message> with
    member _.MethodName = "Message"
    
type SendDiceReq =
  {
    ChatId: ChatId
    Emoji: string option
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyToMessageId: int64 option
    AllowSendingWithoutReply: bool option
    ReplyMarkup: InlineKeyboardMarkup or ReplyKeyboardMarkup or ReplyKeyboardRemove or ForceReply option
  }
  interface IRequestBase<Message> with
    member _.MethodName = "Message"
    
type SendChatActionReq =
  {
    ChatId: ChatId
    Action: string
  }
  interface IRequestBase<bool> with
    member _.MethodName = "True"
    
type GetUserProfilePhotosReq =
  {
    UserId: int64
    Offset: int64 option
    Limit: int64 option
  }
  interface IRequestBase<UserProfilePhotos> with
    member _.MethodName = "UserProfilePhotos"
    
type GetFileReq =
  {
    FileId: string
  }
  interface IRequestBase<File> with
    member _.MethodName = "File"
    
type BanChatMemberReq =
  {
    ChatId: ChatId
    UserId: int64
    UntilDate: int64 option
    RevokeMessages: bool option
  }
  interface IRequestBase<bool> with
    member _.MethodName = "True"
    
type UnbanChatMemberReq =
  {
    ChatId: ChatId
    UserId: int64
    OnlyIfBanned: bool option
  }
  interface IRequestBase<bool> with
    member _.MethodName = "True"
    
type RestrictChatMemberReq =
  {
    ChatId: ChatId
    UserId: int64
    Permissions: ChatPermissions
    UntilDate: int64 option
  }
  interface IRequestBase<bool> with
    member _.MethodName = "True"
    
type PromoteChatMemberReq =
  {
    ChatId: ChatId
    UserId: int64
    IsAnonymous: bool option
    CanManageChat: bool option
    CanPostMessages: bool option
    CanEditMessages: bool option
    CanDeleteMessages: bool option
    CanManageVoiceChats: bool option
    CanRestrictMembers: bool option
    CanPromoteMembers: bool option
    CanChangeInfo: bool option
    CanInviteUsers: bool option
    CanPinMessages: bool option
  }
  interface IRequestBase<bool> with
    member _.MethodName = "True"
    
type SetChatAdministratorCustomTitleReq =
  {
    ChatId: ChatId
    UserId: int64
    CustomTitle: string
  }
  interface IRequestBase<bool> with
    member _.MethodName = "True"
    
type BanChatSenderChatReq =
  {
    ChatId: ChatId
    SenderChatId: int64
  }
  interface IRequestBase<bool> with
    member _.MethodName = "True"
    
type UnbanChatSenderChatReq =
  {
    ChatId: ChatId
    SenderChatId: int64
  }
  interface IRequestBase<bool> with
    member _.MethodName = "True"
    
type SetChatPermissionsReq =
  {
    ChatId: ChatId
    Permissions: ChatPermissions
  }
  interface IRequestBase<bool> with
    member _.MethodName = "True"
    
type ExportChatInviteLinkReq =
  {
    ChatId: ChatId
  }
  interface IRequestBase<string> with
    member _.MethodName = "String"
    
type CreateChatInviteLinkReq =
  {
    ChatId: ChatId
    Name: string option
    ExpireDate: int64 option
    MemberLimit: int64 option
    CreatesJoinRequest: bool option
  }
  interface IRequestBase<ChatInviteLink> with
    member _.MethodName = "ChatInviteLink"
    
type EditChatInviteLinkReq =
  {
    ChatId: ChatId
    InviteLink: string
    Name: string option
    ExpireDate: int64 option
    MemberLimit: int64 option
    CreatesJoinRequest: bool option
  }
  interface IRequestBase<ChatInviteLink> with
    member _.MethodName = "ChatInviteLink"
    
type RevokeChatInviteLinkReq =
  {
    ChatId: ChatId
    InviteLink: string
  }
  interface IRequestBase<ChatInviteLink> with
    member _.MethodName = "ChatInviteLink"
    
type ApproveChatJoinRequestReq =
  {
    ChatId: ChatId
    UserId: int64
  }
  interface IRequestBase<bool> with
    member _.MethodName = "True"
    
type DeclineChatJoinRequestReq =
  {
    ChatId: ChatId
    UserId: int64
  }
  interface IRequestBase<bool> with
    member _.MethodName = "True"
    
type SetChatPhotoReq =
  {
    ChatId: ChatId
    Photo: InputFile
  }
  interface IRequestBase<bool> with
    member _.MethodName = "True"
    
type DeleteChatPhotoReq =
  {
    ChatId: ChatId
  }
  interface IRequestBase<bool> with
    member _.MethodName = "True"
    
type SetChatTitleReq =
  {
    ChatId: ChatId
    Title: string
  }
  interface IRequestBase<bool> with
    member _.MethodName = "True"
    
type SetChatDescriptionReq =
  {
    ChatId: ChatId
    Description: string option
  }
  interface IRequestBase<bool> with
    member _.MethodName = "True"
    
type PinChatMessageReq =
  {
    ChatId: ChatId
    MessageId: int64
    DisableNotification: bool option
  }
  interface IRequestBase<bool> with
    member _.MethodName = "True"
    
type UnpinChatMessageReq =
  {
    ChatId: ChatId
    MessageId: int64 option
  }
  interface IRequestBase<bool> with
    member _.MethodName = "True"
    
type UnpinAllChatMessagesReq =
  {
    ChatId: ChatId
  }
  interface IRequestBase<bool> with
    member _.MethodName = "True"
    
type LeaveChatReq =
  {
    ChatId: ChatId
  }
  interface IRequestBase<bool> with
    member _.MethodName = "True"
    
type GetChatReq =
  {
    ChatId: ChatId
  }
  interface IRequestBase<Chat> with
    member _.MethodName = "Chat"
    
type GetChatAdministratorsReq =
  {
    ChatId: ChatId
  }
  interface IRequestBase<ChatMember[]> with
    member _.MethodName = "Array of ChatMember"
    
type GetChatMemberCountReq =
  {
    ChatId: ChatId
  }
  interface IRequestBase<Int> with
    member _.MethodName = "Int"
    
type GetChatMemberReq =
  {
    ChatId: ChatId
    UserId: int64
  }
  interface IRequestBase<ChatMember> with
    member _.MethodName = "ChatMember"
    
type SetChatStickerSetReq =
  {
    ChatId: ChatId
    StickerSetName: string
  }
  interface IRequestBase<bool> with
    member _.MethodName = "True"
    
type DeleteChatStickerSetReq =
  {
    ChatId: ChatId
  }
  interface IRequestBase<bool> with
    member _.MethodName = "True"
    
type AnswerCallbackQueryReq =
  {
    CallbackQueryId: string
    Text: string option
    ShowAlert: bool option
    Url: string option
    CacheTime: int64 option
  }
  interface IRequestBase<bool> with
    member _.MethodName = "True"
    
type SetMyCommandsReq =
  {
    Commands: BotCommand[]
    Scope: BotCommandScope option
    LanguageCode: string option
  }
  interface IRequestBase<bool> with
    member _.MethodName = "True"
    
type DeleteMyCommandsReq =
  {
    Scope: BotCommandScope option
    LanguageCode: string option
  }
  interface IRequestBase<bool> with
    member _.MethodName = "True"
    
type GetMyCommandsReq =
  {
    Scope: BotCommandScope option
    LanguageCode: string option
  }
  interface IRequestBase<BotCommand[]> with
    member _.MethodName = "Array of BotCommand"
    
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
  interface IRequestBase<Message or True> with
    member _.MethodName = "Message or True"
    
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
  interface IRequestBase<Message or True> with
    member _.MethodName = "Message or True"
    
type EditMessageMediaReq =
  {
    ChatId: ChatId option
    MessageId: int64 option
    InlineMessageId: string option
    Media: InputMedia
    ReplyMarkup: InlineKeyboardMarkup option
  }
  interface IRequestBase<Message or True> with
    member _.MethodName = "Message or True"
    
type EditMessageReplyMarkupReq =
  {
    ChatId: ChatId option
    MessageId: int64 option
    InlineMessageId: string option
    ReplyMarkup: InlineKeyboardMarkup option
  }
  interface IRequestBase<Message or True> with
    member _.MethodName = "Message or True"
    
type StopPollReq =
  {
    ChatId: ChatId
    MessageId: int64
    ReplyMarkup: InlineKeyboardMarkup option
  }
  interface IRequestBase<Poll> with
    member _.MethodName = "Poll"
    
type DeleteMessageReq =
  {
    ChatId: ChatId
    MessageId: int64
  }
  interface IRequestBase<bool> with
    member _.MethodName = "True"
    
type SendStickerReq =
  {
    ChatId: ChatId
    Sticker: FileToSend
    DisableNotification: bool option
    ProtectContent: bool option
    ReplyToMessageId: int64 option
    AllowSendingWithoutReply: bool option
    ReplyMarkup: InlineKeyboardMarkup or ReplyKeyboardMarkup or ReplyKeyboardRemove or ForceReply option
  }
  interface IRequestBase<Message> with
    member _.MethodName = "Message"
    
type GetStickerSetReq =
  {
    Name: string
  }
  interface IRequestBase<StickerSet> with
    member _.MethodName = "StickerSet"
    
type UploadStickerFileReq =
  {
    UserId: int64
    PngSticker: InputFile
  }
  interface IRequestBase<File> with
    member _.MethodName = "File"
    
type CreateNewStickerSetReq =
  {
    UserId: int64
    Name: string
    Title: string
    PngSticker: FileToSend option
    TgsSticker: InputFile option
    WebmSticker: InputFile option
    Emojis: string
    ContainsMasks: bool option
    MaskPosition: MaskPosition option
  }
  interface IRequestBase<bool> with
    member _.MethodName = "True"
    
type AddStickerToSetReq =
  {
    UserId: int64
    Name: string
    PngSticker: FileToSend option
    TgsSticker: InputFile option
    WebmSticker: InputFile option
    Emojis: string
    MaskPosition: MaskPosition option
  }
  interface IRequestBase<bool> with
    member _.MethodName = "True"
    
type SetStickerPositionInSetReq =
  {
    Sticker: string
    Position: int64
  }
  interface IRequestBase<bool> with
    member _.MethodName = "True"
    
type DeleteStickerFromSetReq =
  {
    Sticker: string
  }
  interface IRequestBase<bool> with
    member _.MethodName = "True"
    
type SetStickerSetThumbReq =
  {
    Name: string
    UserId: int64
    Thumb: FileToSend option
  }
  interface IRequestBase<bool> with
    member _.MethodName = "True"
    
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
  interface IRequestBase<bool> with
    member _.MethodName = "True"
    
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
  interface IRequestBase<Message> with
    member _.MethodName = "Message"
    
type AnswerShippingQueryReq =
  {
    ShippingQueryId: string
    Ok: bool
    ShippingOptions: ShippingOption[] option
    ErrorMessage: string option
  }
  interface IRequestBase<bool> with
    member _.MethodName = "True"
    
type AnswerPreCheckoutQueryReq =
  {
    PreCheckoutQueryId: string
    Ok: bool
    ErrorMessage: string option
  }
  interface IRequestBase<bool> with
    member _.MethodName = "True"
    
type SetPassportDataErrorsReq =
  {
    UserId: int64
    Errors: PassportElementError[]
  }
  interface IRequestBase<bool> with
    member _.MethodName = "True"
    
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
  interface IRequestBase<Message> with
    member _.MethodName = "Message"
    
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
  interface IRequestBase<Message or True> with
    member _.MethodName = "Message or True"
    
type GetGameHighScoresReq =
  {
    UserId: int64
    ChatId: int64 option
    MessageId: int64 option
    InlineMessageId: string option
  }
  interface IRequestBase<GameHighScore[]> with
    member _.MethodName = "Array of GameHighScore"
    