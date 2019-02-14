module Funogram.Types

open Funogram.JsonHelpers

open System.IO
open System.Runtime.CompilerServices

open System
open Newtonsoft.Json

// Allow construct types to Funogram.Tests
[<assembly: InternalsVisibleTo("Newtonsoft.Json")>]
do()

type ChatId = 
  | Int of int64
  | String of string

[<CLIMutable>]
/// This object represents a Telegram user or bot.
type User =
  { /// Unique identifier for this user or bot
    Id: int64
    /// True, if this user is a bot
    IsBot: bool
    /// User‘s or bot’s first name
    FirstName: string
    /// User‘s or bot’s last name
    LastName: string option
    /// User‘s or bot’s username
    Username: string option
    /// IETF language tag of the user's language
    LanguageCode: string option }

[<CLIMutable>]
/// This object represents a chat photo
type ChatPhoto =
  { /// Unique file identifier of small (160x160) chat photo. This file_id can be used only for photo download.
    SmallFileId: string
    /// Unique file identifier of big (640x640) chat photo. This file_id can be used only for photo download.
    BigFileId: string }

[<CLIMutable>]
/// This object represents a chat.
type Chat =
  {  /// Unique identifier for this chat.
    Id: int64
    /// Type of chat, can be either "private", "group", "supergroup" or "channel"
    Type: string
    /// Title, for supergroups, channels and group chats
    Title: string option
    /// Username, for private chats, supergroups and channels if available
    Username: string option
    /// First name of the other party in a private chat
    FirstName: string option
    /// Last name of the other party in a private chat
    LastName: string option
    /// True if a group has ‘All Members Are Admins’ enabled.
    AllMembersAreAdministrators: bool option
    /// Chat photo. Returned only in getChat
    Photo: ChatPhoto option
    /// Description, for supergroups and channel chats. Returned only in getChat.
    Description: string option
    /// Chat invite link, for supergroups and channel chats. Returned only in getChat.
    InviteLink: string option
    /// Pinned message, for supergroups. Returned only in getChat.
    PinnedMessage: Message option }

/// This object represents one special entity in a text message. For example, hashtags, usernames, URLs, etc.
and [<CLIMutable>] MessageEntity =
  {  /// Type of the entity. Can be mention (@username), hashtag, bot_command, url, email, bold (bold text), 
    /// italic (italic text), code (monowidth string), pre (monowidth block), text_link (for clickable text URLs), 
    /// text_mention (for users without usernames)    
    Type: string
    /// Offset in UTF-16 code units to the start of the entity
    Offset: int64
    /// Length of the entity in UTF-16 code units
    Length: int64
    /// For “text_link” only, url that will be opened after user taps on the text
    Url: string option
    /// For “text_mention” only, the mentioned user
    User: User option }

/// This object represents an audio file to be treated as music by the Telegram clients
and [<CLIMutable>] Audio =
  {  /// Unique identifier for this file
    FileId: string
    /// Duration of the audio in seconds as defined by sender
    Duration: int
    /// Performer of the audio as defined by sender or by audio tags
    Performer: string option
    /// Title of the audio as defined by sender or by audio tags
    Title: string option
    /// MIME type of the file as defined by sender
    MimeType: string option
    /// File size
    FileSize: int option }

/// This object represents one size of a photo or a file / sticker thumbnail.
and [<CLIMutable>] PhotoSize =
  {  /// Unique identifier for this file
    FileId: string
    /// Photo width
    Width: int
    /// Photo height
    Height: int
    /// File size
    FileSize: int option }

/// This object represents a general file (as opposed to photos, voice messages and audio files).
and [<CLIMutable>] Document = 
  {  /// Unique file identifier
    FileId: string
    /// Document thumbnail as defined by sender
    Thumb: PhotoSize option
    /// Original filename as defined by sender
    FileName: string option
    /// MIME type of the file as defined by sender
    MimeType: string option
    /// File size
    FileSize: int option }

and [<InSnakeCase>] MaskPoint =
  | Forehead
  | Eyes
  | Mouth
  | Chin

/// This object describes the position on faces where a mask should be placed by default.
and [<CLIMutable>] MaskPosition =
  { /// The part of the face relative to which the mask should be placed. One of “forehead”, “eyes”, “mouth”, or “chin”.
    Point: MaskPoint
    /// Shift by X-axis measured in widths of the mask scaled to the face size, from left to right. For example, choosing -1.0 will place mask just to the left of the default mask position.
    XShift: float
    /// Shift by Y-axis measured in heights of the mask scaled to the face size, from top to bottom. For example, 1.0 will place the mask just below the default mask position.
    YShift: float
    /// Mask scaling coefficient. For example, 2.0 means double size.
    Scale: float
  }

/// This object represents a sticker.
and [<CLIMutable>] Sticker =
  {  /// Unique identifier for this file
    FileId: string
    /// Sticker width
    Width: int
    /// Sticker height
    Height: int
    /// Sticker thumbnail in .webp or .jpg format
    Thumb: PhotoSize option
    /// Emoji associated with the sticker
    Emoji: string option
    /// Name of the sticker set to which the sticker belongs
    SetName: string option
    /// For mask stickers, the position where the mask should be placed
    MaskPosition: MaskPosition option
    /// File size
    FileSize: int option }

/// This object represents a sticker set.
and [<CLIMutable>] StickerSet =
  { /// Sticker set name
    Name: string
    /// Sticker set title
    Title: string
    /// True, if the sticker set contains masks
    ContainsMasks: bool
    /// List of all set stickers
    Stickers: Sticker seq }

/// This object represents a video file.
and [<CLIMutable>] Video =
  {  /// Unique identifier for this file
    FileId: string
    /// Video width as defined by sender
    Width: int
    /// Video height as defined by sender
    Height: int
    /// Duration of the video in seconds as defined by sender
    Duration: int
    /// Video thumbnail
    Thumb: PhotoSize option
    /// Mime type of a file as defined by sender
    MimeType: string option
    /// File size
    FileSize: int option }

/// This object represents a voice note
and [<CLIMutable>] Voice = 
  {  /// Unique identifier for this file
    FileId: string
    /// Duration of the audio in seconds as defined by sender
    Duration: int
    /// MIME type of the file as defined by sender
    MimeType: string option
    /// File size
    FileSize: int option }

/// Contact's phone number
and [<CLIMutable>] Contact =
  {  /// 
    PhoneNumber: string
    /// Contact's first name
    FirstName: string
    /// Contact's last name
    LastName: string option
    /// Contact's user identifier in Telegram
    UserId: int option }

/// This object represents a point on the map
and [<CLIMutable>] Location =
  {  /// Longitude as defined by sender
    Longitude: double
    /// Latitude as defined by sender
    Latitude: double }
    
/// This object represents a venue.
and [<CLIMutable>] Venue =
  {  /// Venue location
    Location: Location
    /// Name of the venue
    Title: string
    /// Address of the venue
    Address: string
    /// Foursquare identifier of the venue
    FoursquareId: string option }
    
/// This object represent a user's profile pictures.
and [<CLIMutable>] UserProfilePhotos =
  { /// Total number of profile pictures the target user has
    TotalCount: int
    /// Requested profile pictures (in up to 4 sizes each)
    Photos: seq<seq<PhotoSize>> }

/// This object represents a file ready to be downloaded. The file can be downloaded via the link https://api.telegram.org/file/bot<token>/<file_path>. 
/// It is guaranteed that the link will be valid for at least 1 hour. When the link expires, a new one can be requested by calling getFile.
and [<CLIMutable>] File =
  { /// Unique identifier for this file
    FileId: string
    /// File size, if known
    FileSize: int option
    /// File path. Use https://api.telegram.org/file/bot<token>/<file_path> to get the file.
    FilePath: string option }
      
/// You can provide an animation for your game so that it looks stylish in chats (check out Lumberjack for an example). This object represents an animation file to be displayed in the message containing a game.
and [<CLIMutable>] Animation =
  {  /// Unique file identifier
    FileId: string
    /// Animation thumbnail as defined by sender
    Thumb: PhotoSize option
    /// Original animation filename as defined by sender
    FileName: string option
    /// MIME type of the file as defined by sender
    MimeType: string option
    /// File size
    FileSize: string option }

/// This object represents a game. Use BotFather to create and edit games, their short names will act as unique identifiers
and [<CLIMutable>] Game = 
  {  /// Title of the game
    Title: string
    /// Description of the game
    Description: string
    /// Photo that will be displayed in the game message in chats
    Photo: PhotoSize
    /// Brief description of the game or high scores included in the game message. 
    /// Can be automatically edited to include current high scores for the game when the bot 
    /// calls setGameScore, or manually edited using editMessageText. 0-4096 characters.
    Text: string option
    /// Special entities that appear in text, such as usernames, URLs, bot commands, etc.
    TextEntities: MessageEntity seq option
    /// Animation that will be displayed in the game message in chats. Upload via BotFather
    Animation: Animation option }
    
/// This object represents a video message (available in Telegram apps as of v.4.0).
and [<CLIMutable>] VideoNote =
  { /// Unique identifier for this file
    FileId: string
    /// Video width and height as defined by sender
    Length: int
    /// Duration of the video in seconds as defined by sender
    Duration: int
    /// Video thumbnail
    Thumb: PhotoSize option
    /// File size
    FileSize: int option }

/// This object contains basic information about an invoice
and [<CLIMutable>] Invoice = 
  { /// Product name
    Title: string
    /// Product description
    Description: string
    /// Unique bot deep-linking parameter that can be used to generate this invoice
    StartParameter: string
    /// Three-letter ISO 4217 currency code
    Currency: string
    /// Total price in the smallest units of the currency (integer, not float/double). For example, for a price of US$ 1.45 pass amount = 145. See the exp parameter in currencies.json, it shows the number of digits past the decimal point for each currency (2 for the majority of currencies).
    TotalAmount: int }
    
/// This object represents a shipping address
and [<CLIMutable>] ShippingAddress =
  { /// ISO 3166-1 alpha-2 country code
    CountryCode: string
    /// State, if applicable
    State: string
    /// City
    City: string
    /// First line for the address
    StreetLine1: string
    /// Second line for the address
    StreetLine2: string
    /// Address post code
    PostCode: string }

/// This object represents information about an order.
and [<CLIMutable>] OrderInfo = 
  { /// User name
    Name: string option
    /// Optional. User's phone number
    PhoneNumber: string option
    /// User email
    Email: string option
    /// User shipping address
    ShippingAddress: ShippingAddress option }

/// This object represents a portion of the price for goods or services.
and [<CLIMutable>] LabeledPrice =
  { /// Portion label
    Label: string
    /// Price of the product in the smallest units of the currency (integer, not float/double). For example, for a price of US$ 1.45 pass amount = 145. See the exp parameter in currencies.json, it shows the number of digits past the decimal point for each currency (2 for the majority of currencies).
    Amount: int }

/// This object represents one shipping option
and [<CLIMutable>] ShippingOption =
  { /// Shipping option identifier
    Id: string
    /// Option title
    Title: string
    /// List of price portions
    Prices: LabeledPrice seq }

/// This object contains basic information about a successful payment
and [<CLIMutable>] SuccessfulPayment =
  { /// Three-letter ISO 4217 currency code
    Currency: string
    /// Total price in the smallest units of the currency (integer, not float/double). For example, for a price of US$ 1.45 pass amount = 145. See the exp parameter in currencies.json, it shows the number of digits past the decimal point for each currency (2 for the majority of currencies).
    TotalAmount: int
    /// Bot specified invoice payload
    InvoicePayload: string
    /// Identifier of the shipping option chosen by the user
    ShippingOptionId: string option
    /// Order info provided by the user
    OrderInfo: OrderInfo option
    /// Telegram payment identifier
    TelegramPaymentChargeId: string
    /// Provider payment identifier
    ProviderPaymentChargeId: string }
    
/// This object contains information about an incoming shipping query.
and [<CLIMutable>] ShippingQuery = 
  { /// Unique query identifier
    Id: string
    /// User who sent the query
    From: User
    /// Bot specified invoice payload
    InvoicePayload: string
    /// User specified shipping address
    ShippingAddress: ShippingAddress }

/// This object contains information about an incoming pre-checkout query
and [<CLIMutable>] PreCheckoutQuery =
  { /// Unique query identifier
    Id: string
    /// User who sent the query
    From: User
    /// Three-letter ISO 4217 currency code
    Currency: string
    /// Total price in the smallest units of the currency (integer, not float/double). For example, for a price of US$ 1.45 pass amount = 145. See the exp parameter in currencies.json, it shows the number of digits past the decimal point for each currency (2 for the majority of currencies).
    TotalAmount: int
    /// Bot specified invoice payload
    InvoicePayload: string
    /// Identifier of the shipping option chosen by the user
    ShippingOptionId: string option
    /// Order info provided by the user
    OrderInfo: OrderInfo option }

/// This object represents a message
and [<CLIMutable>] Message = 
  {  /// Unique message identifier inside this chat
    MessageId: int64
    /// Sender, can be empty for messages sent to channels
    From: User option
    [<JsonConverter(typeof<JsonHelpers.UnixDateTimeConverter>)>]
    /// Date the message was sent in Unix time
    Date: DateTime
    /// Conversation the message belongs to
    Chat: Chat
    /// For forwarded messages, sender of the original message
    ForwardFrom: User option
    /// For messages forwarded from a channel, information about the original channel
    ForwardFromChat: Chat option
    /// For forwarded channel posts, identifier of the original message in the channel
    ForwardFromMessageId: int64 option
    /// For messages forwarded from channels, signature of the post author if present
    ForwardSignature: string option

    [<JsonConverter(typeof<JsonHelpers.UnixDateTimeConverter>)>]
    /// For forwarded messages, date the original message
    ForwardDate: DateTime option
    /// For replies, the original message. 
    /// Note that the Message object in this field will not contain further 
    /// ReplyToMessage fields even if it itself is a reply.
    ReplyToMessage: Message option;
    [<JsonConverter(typeof<JsonHelpers.UnixDateTimeConverter>)>]
    /// Date the message was last edited
    EditDate: DateTime option
    /// Signature of the post author for messages in channels
    AuthorSignature: string option
    /// For text messages, the actual UTF-8 text of the message, 0-4096 characters.
    Text: string option
    /// For text messages, special entities like usernames, URLs, bot commands, etc. that appear in the text
    Entities: MessageEntity seq option
    /// Message is an audio file, information about the file
    Audio: Audio option
    /// Message is a general file, information about the file
    Document: Document option
    /// Message is a game, information about the game
    Game: Game option
    /// Message is a photo, available sizes of the photo
    Photo: PhotoSize seq option
    /// Message is a sticker, information about the sticker
    Sticker: Sticker option
    /// Message is a video, information about the video
    Video: Video option
    /// Message is a voice message, information about the file
    Voice: Voice option
    /// Message is a video note, information about the video message
    VideoNote: VideoNote option
    /// New members that were added to the group or supergroup and information about them (the bot itself may be one of these members)
    NewChatMembers: User seq option
    /// Caption for the document, photo or video, 0-200 characters
    Caption: string option
    /// Message is a shared contact, information about the contact
    Contact: Contact option
    /// Message is a shared location, information about the location
    Location: Location option
    /// Message is a venue, information about the venue
    Venue: Venue option
    /// A new member was added to the group, information about them (this member may be the bot itself)
    NewChatMember: User option
    /// A member was removed from the group, information about them (this member may be the bot itself)
    LeftChatMember: User option
    /// A chat title was changed to this value
    NewChatTitle: string option
    /// A chat photo was change to this value
    NewChatPhoto: PhotoSize seq option
    /// Service message: the chat photo was deleted
    DeleteChatPhoto: bool option
    /// Service message: the group has been created
    GroupChatCreated: bool option
    /// Service message: the supergroup has been created. This field can‘t be received 
    /// in a message coming through updates, because bot can’t be a member of a supergroup 
    /// when it is created. It can only be found in ReplyToMessage if someone replies to a 
    /// very first message in a directly created supergroup.
    SupergroupChatCreated: bool option
    /// Service message: the channel has been created. This field can‘t be received 
    /// in a message coming through updates, because bot can’t be a member of a channel 
    /// when it is created. It can only be found in ReplyToMessage if someone replies 
    /// to a very first message in a channel.
    ChannelChatCreated: bool option
    /// The group has been migrated to a supergroup with the specified identifier
    MigrateToChatId: int64 option
    /// The supergroup has been migrated from a group with the specified identifier
    MigrateFromChatId: int64 option
    /// Specified message was pinned. Note that the Message object in this field will not contain further ReplyToMessage fields even if it is itself a reply.
    PinnedMessage: Message option
    /// Message is an invoice for a payment, information about the invoice
    Invoice: Invoice option
    /// Message is a service message about a successful payment, information about the payment
    SuccessfulPayment: SuccessfulPayment option }


let defaultChat = { Id = 0L; Type = ""; Chat.Title = None; Username = None; FirstName = None; LastName = None; AllMembersAreAdministrators = None; Photo = None; Description = None; InviteLink = None; PinnedMessage = None }

let defaultMessage = { MessageId = 1L; From = None; Date = DateTime.MinValue; Chat = defaultChat; ForwardFrom = None; ForwardFromChat = None; 
  ForwardDate = None; ForwardFromMessageId = None; ReplyToMessage = None; EditDate = None; Text = None; Entities = None; Audio = None; 
  Document = None; Game = None; Photo = None; Sticker = None; Video = None; Voice = None; VideoNote = None; NewChatMembers = None; Caption = None; 
  Contact = None; Location = None; Venue = None; NewChatMember = None; LeftChatMember = None; NewChatTitle = None; NewChatPhoto = None; 
  DeleteChatPhoto = None; GroupChatCreated = None; SupergroupChatCreated = None; ChannelChatCreated = None; MigrateToChatId = None; 
  MigrateFromChatId = None; ForwardSignature = None; AuthorSignature = None;
  PinnedMessage = None; Invoice = None; SuccessfulPayment = None; }

[<CLIMutable>]
/// This object represents an incoming inline query. 
/// When the user sends an empty query, your bot could return some default or trending results.
type InlineQuery =
  {  /// Unique identifier for this query
    Id: string
    /// Sender
    From: User
    /// Sender location, only for bots that request user location
    Location: Location option
    /// Text of the query (up to 512 characters)
    Query: string
    /// Offset of the results to be returned, can be controlled by the bot
    Offset: string }

[<CLIMutable>]
/// Represents a result of an inline query that was chosen by the user and sent to their chat partner. 
type ChosenInlineResult =
  {  /// The unique identifier for the result that was chosen
    ResultId: string
    /// The user that chose the result
    From: User
    /// Sender location, only for bots that require user location
    Location: Location option
    /// Identifier of the sent inline message. Available only if there is an inline keyboard attached to the message. 
    /// Will be also received in callback queries and can be used to edit the message.
    InlineMessageId: string option
    /// The query that was used to obtain the result
    Query: string }

[<CLIMutable>]
/// This object represents an incoming callback query from a callback button in an inline keyboard. If the button that originated the query was attached to a message sent by the bot, the field message will be present.  If the button was attached to a message sent via the bot (in inline mode), the field InlineMessageId will be present. Exactly one of the fields data or GameShortName will be present.
type CallbackQuery =
  {  /// Unique identifier for this query
    Id: string
    /// Sender
    From: User
    /// Message with the callback button that originated the query. 
    /// Note that message content and message date will not be available if the message is too old
    Message: Message option
    /// Identifier of the message sent via the bot in inline mode, that originated the query.
    InlineMessageId: string option
    /// Global identifier, uniquely corresponding to the chat to which the message 
    /// with the callback button was sent. Useful for high scores in games.
    ChatInstance: string
    /// Data associated with the callback button. Be aware that a bad client can send arbitrary data in this field.
    Data: string option
    /// Short name of a Game to be returned, serves as the unique identifier for the game
    GameShortName: string option }

[<CLIMutable>]
/// This object represents an incoming update. At most one of the optional parameters can be present in any given update
type Update = 
  { /// The update‘s unique identifier. Update identifiers start from a certain positive number and increase sequentially. This ID becomes especially handy if you’re using Webhooks, since it allows you to ignore repeated updates or to restore the correct update sequence, should they get out of order.
    UpdateId: int64
    /// New incoming message of any kind — text, photo, sticker, etc.
    Message: Message option
    /// New version of a message that is known to the bot and was edited
    EditedMessage: Message option
    /// New incoming channel post of any kind — text, photo, sticker, etc.
    ChannelPost: Message option
    /// New version of a channel post that is known to the bot and was edited
    EditedChannelPost: Message option
    /// New incoming inline query
    InlineQuery: InlineQuery option
    /// The result of an inline query that was chosen by a user and sent to their chat partner.
    ChosenInlineResult: ChosenInlineResult option
    /// New incoming callback query
    CallbackQuery: CallbackQuery option }

/// Message text parsing mode
type ParseMode = 
  /// Markdown parse syntax
  | Markdown
  /// Html parse syntax
  | HTML

/// Type of action to broadcast
[<InSnakeCase>]
type ChatAction =
  | Typing
  | UploadPhoto
  | RecordVideo
  | UploadVideo
  | RecordAudio
  | UploadAudio
  | UploadDocument
  | FindLocation
  | RecordVideoNote
  | UploadVideoNote

/// A placeholder, currently holds no information
type CallbackGame() = class end

/// This object represents one button of an inline keyboard. You must use exactly one of the optional fields.
type InlineKeyboardButton = 
  { /// Label text on the button
    Text: string
    /// HTTP url to be opened when button is pressed
    Url: string option
    /// Data to be sent in a callback query to the bot when button is pressed, 1-64 bytes
    CallbackData: string option
    /// If set, pressing the button will prompt the user to select one of their chats, open that chat and 
    /// insert the bot‘s username and the specified inline query in the input field. Can be empty, in which 
    /// case just the bot’s username will be inserted.
    SwitchInlineQuery: string option
    /// If set, pressing the button will insert the bot‘s username and the specified inline query in the current 
    /// chat's input field. Can be empty, in which case only the bot’s username will be inserted.
    SwitchInlineQueryCurrentChat: string option
    /// Description of the game that will be launched when the user presses the button.
    CallbackGame: CallbackGame option }

/// This object represents one button of the reply keyboard. For simple text buttons String can be used instead of this object to specify text of the button. Optional fields are mutually exclusive.
type KeyboardButton = 
  { /// Text of the button. If none of the optional fields are used, it will be sent to the bot as a message when the button is pressed
    Text: string
    /// If True, the user's phone number will be sent as a contact when the button is pressed. Available in private chats only
    RequestContact: bool option
    /// If True, the user's current location will be sent when the button is pressed. Available in private chats only
    RequestLocation: bool option }

/// This object represents an inline keyboard that appears right next to the message it belongs to
type InlineKeyboardMarkup =
  { /// Array of button rows, each represented by an Array of InlineKeyboardButton objects
    InlineKeyboard: (InlineKeyboardButton seq) seq }

/// This object represents a custom keyboard with reply options (see Introduction to bots for details and examples).
type ReplyKeyboardMarkup =
  {
    /// Array of button rows, each represented by an Array of KeyboardButton objects
    Keyboard: (KeyboardButton seq) seq
    /// Requests clients to resize the keyboard vertically for optimal fit (e.g., make the keyboard smaller if there are just two rows of buttons). Defaults to false, in which case the custom keyboard is always of the same height as the app's standard keyboard.
    ResizeKeyboard: bool option
    /// Requests clients to hide the keyboard as soon as it's been used. The keyboard will still be available, but clients will automatically display the usual letter-keyboard in the chat – the user can press a special button in the input field to see the custom keyboard again. Defaults to false.
    OneTimeKeyboard: bool option
    /// Use this parameter if you want to show the keyboard to specific users only. Targets: 1) users that are @mentioned in the text of the Message object; 2) if the bot's message is a reply (has reply_to_message_id), sender of the original message
    Selective: bool option }
      
/// Upon receiving a message with this object, Telegram clients will remove the current custom keyboard and display the default letter-keyboard. By default, custom keyboards are displayed until a new keyboard is sent by a bot. An exception is made for one-time keyboards that are hidden immediately after the user presses a button (see ReplyKeyboardMarkup).
type ReplyKeyboardRemove =
  { /// Requests clients to remove the custom keyboard (user will not be able to summon this keyboard; if you want to hide the keyboard from sight but keep it accessible, use one_time_keyboard in ReplyKeyboardMarkup)
    RemoveKeyboard: bool
    /// Use this parameter if you want to remove the keyboard for specific users only. Targets: 1) users that are @mentioned in the text of the Message object; 2) if the bot's message is a reply (has reply_to_message_id), sender of the original message.
    Selective: bool option }

/// Upon receiving a message with this object, Telegram clients will display a reply interface to the user (act as if the user has selected the bot‘s message and tapped ’Reply'). This can be extremely useful if you want to create user-friendly step-by-step interfaces without having to sacrifice privacy mode.
type ForceReply = 
  { /// Shows reply interface to the user, as if they manually selected the bot‘s message and tapped ’Reply'
    ForceReply: bool
    /// Use this parameter if you want to force reply from specific users only. Targets: 1) users that are @mentioned in the 
    /// text of the Message object; 2) if the bot's message is a reply (has reply_to_message_id), sender of the original message.
    Selective: bool }
    
/// This object contains information about one member of the chat.
type ChatMember = 
  { /// Information about the user
    User: User
    /// The member's status in the chat. Can be “creator”, “administrator”, “member”, “left” or “kicked”
    Status: string 
    [<JsonConverter(typeof<JsonHelpers.UnixDateTimeConverter>)>]
    /// Restictred and kicked only. Date when restrictions will be lifted for this user, unix time
    UntilDate: DateTime option
    /// Administrators only. True, if the bot is allowed to edit administrator privileges of that user
    CanBeEdited: bool option
    /// Administrators only. True, if the administrator can change the chat title, photo and other settings
    CanChangeInfo: bool option
    /// Administrators only. True, if the administrator can post in the channel, channels only
    CanPostMessages: bool option
    /// Administrators only. True, if the administrator can edit messages of other users, channels only
    CanEditMessages: bool option
    /// Administrators only. True, if the administrator can delete messages of other users
    CanDeleteMessages: bool option
    /// Administrators only. True, if the administrator can invite new users to the chat
    CanInviteUsers: bool option
    /// Administrators only. True, if the administrator can restrict, ban or unban chat members
    CanRestrictMembers: bool option
    /// Administrators only. True, if the administrator can pin messages, supergroups only
    CanPinMessages: bool option
    /// Administrators only. True, if the administrator can add new administrators with a subset of his own privileges or demote administrators that he has promoted, directly or indirectly (promoted by administrators that were appointed by the user)
    CanPromoteMembers: bool option
    /// Restricted only. True, if the user can send text messages, contacts, locations and venues
    CanSendMessages: bool option
    /// Restricted only. True, if the user can send audios, documents, photos, videos, video notes and voice notes, implies can_send_messages
    CanSendMediaMessages: bool option
    /// Restricted only. True, if the user can send animations, games, stickers and use inline bots, implies can_send_media_messages
    CanSendOtherMessages: bool option
    /// Optional. Restricted only. True, if user may add web page previews to his messages, implies can_send_media_messages
    CanAddWebPagePreviews: bool option }
      
type Markup = 
  | InlineKeyboardMarkup of InlineKeyboardMarkup 
  | ReplyKeyboardMarkup of ReplyKeyboardMarkup
  | ReplyKeyboardRemove of ReplyKeyboardRemove
  | ForceReply of ForceReply

/// Telegram Bot Api Response
type ApiResponse<'a> = 
  { /// True if request success
    Ok: bool
    /// Result of request
    Result: 'a option
    Description: string option
    ErrorCode: int option }

/// Telegram Bot Api Response Error
type ApiResponseError = 
  { Description: string
    ErrorCode: int }
      
/// Provides available types of an update.
module UpdateType = 
    /// Indicates that the received update is a Message
    let (|Message|_|) (update: Update) = update.Message
    /// New version of a message that is known to the bot and was edited
    let (|EditedMessage|_|) (update: Update) = update.EditedMessage
    /// New incoming channel post of any kind — text, photo, sticker, etc.
    let (|ChannelPost|_|) (update: Update) = update.ChannelPost
    /// New version of a incoming channel post of any kind — text, photo, sticker, etc.
    let (|EditedChannelPost|_|) (update: Update) = update.EditedChannelPost
    /// Indicates that the received update is a type of InlineQuery
    let (|InlineQuery|_|) (update: Update) = update.InlineQuery
    /// Indicates that the received update is a result of an inline query that was chosen by a user and sent to their chat partner
    let (|ChosenInlineResult|_|) (update: Update) = update.ChosenInlineResult
    /// New incoming callback query
    let (|CallbackQuery|_|) (update: Update) = update.CallbackQuery

/// If edited message is sent by the bot, used Message, otherwise Success.
type EditMessageResult = 
  /// Message sent by the bot
  | Message of Message
  /// Message sent via the bot or another...
  | Success of bool
    
/// Represents the content of a text message to be sent as the result of an inline query. 
type InputTextMessageContent =
  { /// Text of the message to be sent, 1-4096 characters
    MessageText: string
    /// Send Markdown or HTML, if you want Telegram apps to show bold, italic, fixed-width text or inline URLs in your bot's message.
    ParseMode: ParseMode option
    /// Disables link previews for links in the sent message
    DisableWebPagePreview: bool option }

/// Represents the content of a location message to be sent as the result of an inline query. 
type InputLocationMessageContent =
  { /// Latitude of the location in degrees
    Latitude: float
    /// Longitude of the location in degrees
    Longitude: float }

/// Represents the content of a venue message to be sent as the result of an inline query. 
type InputVenueMessageContent =
  { /// Latitude of the venue in degrees
    Latitude: float
    /// Longitude of the venue in degrees
    Longitude: float
    /// Name of the venue
    Title: string
    /// Address of the venue
    Address: string
    /// Foursquare identifier of the venue, if known
    FoursquareId: string option }

/// Represents the content of a contact message to be sent as the result of an inline query. 
type InputContactMessageContent =
  { /// Contact's phone number
    PhoneNumber: string
    /// Contact's first name
    FirstName: string
    /// Contact's last name
    LastName: string option }

/// This object represents the content of a message to be sent as a result of an inline query. Telegram clients currently support the following 4 types:
type InputMessageContent =
  | TextMessage of InputTextMessageContent
  | LocationMessage of InputLocationMessageContent
  | VenueMessage of InputVenueMessageContent
  | ContactMessage of InputContactMessageContent


/// Represents a link to an mp3 audio file stored on the Telegram servers. By default, this audio file will be sent by the user. Alternatively, you can use input_message_content to send a message with the specified content instead of the audio.
type InlineQueryResultCachedAudio = 
  { /// Type of the result, must be audio
    Type: string
    /// Unique identifier for this result, 1-64 bytes
    Id: string
    /// A valid file identifier for the audio file
    AudioFileId: string
    /// Caption, 0-200 characters
    Caption: string option
    /// Inline keyboard attached to the message
    ReplyMarkup: InlineKeyboardMarkup option
    /// Content of the message to be sent instead of the audio
    InputMessageContent: InputMessageContent option }

/// Represents a link to a file stored on the Telegram servers. By default, this file will be sent by the user with an optional caption. Alternatively, you can use input_message_content to send a message with the specified content instead of the file.
type InlineQueryResultCachedDocument = 
  { /// Type of the result, must be document
    Type: string
    /// Unique identifier for this result, 1-64 bytes
    Id: string
    /// Title for the result
    Title: string
    /// A valid file identifier for the file
    DocumentFileId: string
    /// Short description of the result
    Description: string option
    /// Caption of the document to be sent, 0-200 characters
    Caption: string option
    /// Inline keyboard attached to the message
    ReplyMarkup: InlineKeyboardMarkup option
    /// Content of the message to be sent instead of the file
    InputMessageContent: InputMessageContent option }
      
/// Represents a link to an animated GIF file stored on the Telegram servers. By default, this animated GIF file will be sent by the user with an optional caption. Alternatively, you can use input_message_content to send a message with specified content instead of the animation.
type InlineQueryResultCachedGif =
  { /// Type of the result, must be gif
    Type: string
    /// Unique identifier for this result, 1-64 bytes
    Id: string
    /// A valid file identifier for the GIF file
    GifFileId: string
    /// Title for the result
    Title: string option
    /// Caption of the GIF file to be sent, 0-200 characters
    Caption: string option
    /// Inline keyboard attached to the message
    ReplyMarkup: InlineKeyboardMarkup option
    /// Content of the message to be sent instead of the GIF animation
    InputMessageContent: InputMessageContent option }

/// Represents a link to a video animation (H.264/MPEG-4 AVC video without sound) stored on the Telegram servers. By default, this animated MPEG-4 file will be sent by the user with an optional caption. Alternatively, you can use input_message_content to send a message with the specified content instead of the animation.
type InlineQueryResultCachedMpeg4Gif =
  { /// Type of the result, must be mpeg4_gif
    Type: string
    /// Unique identifier for this result, 1-64 bytes
    Id: string
    /// A valid file identifier for the MP4 file
    Mpeg4FileId: string
    /// Title for the result
    Title: string
    /// Caption of the MPEG-4 file to be sent, 0-200 characters
    Caption: string
    /// Inline keyboard attached to the message
    ReplyMarkup: InlineKeyboardMarkup
    /// Content of the message to be sent instead of the video animation
    InputMessageContent: InputMessageContent }

/// Represents a link to a photo stored on the Telegram servers. By default, this photo will be sent by the user with an optional caption. Alternatively, you can use input_message_content to send a message with the specified content instead of the photo.
type InlineQueryResultCachedPhoto =
  { /// Type of the result, must be photo
    Type: string
    /// Unique identifier for this result, 1-64 bytes
    Id: string
    /// A valid file identifier of the photo
    PhotoFileId: string
    /// Title for the result
    Title: string option
    /// Short description of the result
    Description: string option
    /// Caption of the photo to be sent, 0-200 characters
    Caption: string option
    /// Inline keyboard attached to the message
    ReplyMarkup: InlineKeyboardMarkup option
    /// Content of the message to be sent instead of the photo
    InputMessageContent: InputMessageContent option }

/// Represents a link to a sticker stored on the Telegram servers. By default, this sticker will be sent by the user. Alternatively, you can use input_message_content to send a message with the specified content instead of the sticker.
type InlineQueryResultCachedSticker =
  { /// Type of the result, must be sticker
    Type: string
    /// Unique identifier for this result, 1-64 bytes
    Id: string
    /// A valid file identifier of the sticker
    StickerFileId: string
    /// Inline keyboard attached to the message
    ReplyMarkup: InlineKeyboardMarkup option
    /// Content of the message to be sent instead of the sticker
    InputMessageContent: InputMessageContent option }

/// Represents a link to a video file stored on the Telegram servers. By default, this video file will be sent by the user with an optional caption. Alternatively, you can use input_message_content to send a message with the specified content instead of the video.
type InlineQueryResultCachedVideo = 
  { /// Type of the result, must be video
    Type: string
    /// Unique identifier for this result, 1-64 bytes
    Id: string
    /// A valid file identifier for the video file
    VideoFileId: string
    /// Title for the result
    Title: string
    /// Short description of the result
    Description: string option
    /// Caption of the video to be sent, 0-200 characters
    Caption: string option
    /// Inline keyboard attached to the message
    ReplyMarkup: InlineKeyboardMarkup option
    /// Content of the message to be sent instead of the video
    InputMessageContent: InputMessageContent option }

/// Represents a link to a voice message stored on the Telegram servers. By default, this voice message will be sent by the user. Alternatively, you can use input_message_content to send a message with the specified content instead of the voice message.
type InlineQueryResultCachedVoice =
  { /// Type of the result, must be voice
    Type: string
    /// Unique identifier for this result, 1-64 bytes
    Id: string
    /// A valid file identifier for the voice message
    VoiceFileId: string
    /// Voice message title
    Title: string
    /// Caption, 0-200 characters
    Caption: string option
    /// Inline keyboard attached to the message
    ReplyMarkup: InlineKeyboardMarkup option
    /// Content of the message to be sent instead of the voice message
    InputMessageContent: InputMessageContent option }

/// Represents a link to an article or web page.
type InlineQueryResultArticle =
  { /// Type of the result, must be article
    Type: string
    /// Unique identifier for this result, 1-64 Bytes
    Id: string
    /// Title of the result
    Title: string
    /// Content of the message to be sent
    InputMessageContent: InputMessageContent option
    /// Inline keyboard attached to the message
    ReplyMarkup: InlineKeyboardMarkup option
    /// URL of the result
    Url: string option
    /// Pass True, if you don't want the URL to be shown in the message
    HideUrl: bool option
    /// Short description of the result
    Description: string option
    /// Url of the thumbnail for the result
    ThumbUrl: string option
    /// Thumbnail width
    ThumbWidth: int option
    /// Thumbnail height
    ThumbHeight: int option }
    
/// Represents a link to an mp3 audio file. By default, this audio file will be sent by the user. Alternatively, you can use input_message_content to send a message with the specified content instead of the audio.
type InlineQueryResultAudio = 
  { /// Type of the result, must be audio
    Type: string
    /// Unique identifier for this result, 1-64 bytes
    Id: string
    /// A valid URL for the audio file
    AudioUrl: string
    /// Title
    Title: string
    /// Caption, 0-200 characters
    Caption: string option
    /// Performer
    Performer: string option
    /// Audio duration in seconds
    AudioDuration: int option
    /// Inline keyboard attached to the message
    ReplyMarkup: InlineKeyboardMarkup option
    /// Content of the message to be sent instead of the audio
    InputMessageContent: InputMessageContent option }

/// Represents a contact with a phone number. By default, this contact will be sent by the user. Alternatively, you can use input_message_content to send a message with the specified content instead of the contact.
type InlineQueryResultContact =
  { /// Type of the result, must be contact
    Type: string
    /// Unique identifier for this result, 1-64 Bytes
    Id: string
    /// Contact's phone number
    PhoneNumber: string
    /// Contact's first name
    FirstName: string
    /// Contact's last name
    LastName: string option
    /// Inline keyboard attached to the message
    ReplyMarkup: InlineKeyboardMarkup option
    /// Content of the message to be sent instead of the contact
    InputMessageContent: InputMessageContent option
    /// Url of the thumbnail for the result
    ThumbUrl: string option
    /// Thumbnail width
    ThumbWidth: int option
    /// Thumbnail height
    ThumbHeight: int option }
    
/// Represents a Game.
type InlineQueryResultGame = 
  { /// Type of the result, must be game
    Type: string
    /// Unique identifier for this result, 1-64 bytes
    Id: string
    /// Short name of the game
    GameShortName: string
    /// Inline keyboard attached to the message
    ReplyMarkup: InlineKeyboardMarkup option }
    
/// Represents a link to a file. By default, this file will be sent by the user with an optional caption. Alternatively, you can use input_message_content to send a message with the specified content instead of the file. Currently, only .PDF and .ZIP files can be sent using this method.
type InlineQueryResultDocument =
  { /// Type of the result, must be document
    Type: string
    /// Unique identifier for this result, 1-64 bytes
    Id: string
    /// Title for the result
    Title: string
    /// Caption of the document to be sent, 0-200 characters
    Caption: string
    /// A valid URL for the file
    DocumentUrl: string
    /// Mime type of the content of the file, either “application/pdf” or “application/zip”
    MimeType: string
    /// Short description of the result
    Description: string option
    /// Inline keyboard attached to the message
    ReplyMarkup: InlineKeyboardMarkup option
    /// Content of the message to be sent instead of the file
    InputMessageContent: InputMessageContent option
    /// URL of the thumbnail (jpeg only) for the file
    ThumbUrl: string option
    /// Thumbnail width
    ThumbWidth: int option
    /// Thumbnail height
    ThumbHeight: int option }

/// Represents a link to an animated GIF file. By default, this animated GIF file will be sent by the user with optional caption. Alternatively, you can use input_message_content to send a message with the specified content instead of the animation.
type InlineQueryResultGif =
  { /// Type of the result, must be gif
    Type: string
    /// Unique identifier for this result, 1-64 bytes
    Id: string
    /// A valid URL for the GIF file. File size must not exceed 1MB
    GifUrl: string
    /// Width of the GIF
    GifWidth: int option
    /// Height of the GIF
    GifHeight: int option
    /// Duration of the GIF
    GifDuration: int option
    /// URL of the static thumbnail for the result (jpeg or gif)
    ThumbUrl: string
    /// Title for the result
    Title: string option
    /// Caption of the GIF file to be sent, 0-200 characters
    Caption: string option
    /// Inline keyboard attached to the message
    ReplyMarkup: InlineKeyboardMarkup option
    /// Content of the message to be sent instead of the GIF animation
    InputMessageContent: InputMessageContent option }

/// Represents a location on a map. By default, the location will be sent by the user. Alternatively, you can use input_message_content to send a message with the specified content instead of the location
type InlineQueryResultLocation =
  { /// Type of the result, must be location
    Type: string
    /// Unique identifier for this result, 1-64 Bytes
    Id: string
    /// Location latitude in degrees
    Latitude: float
    /// Location longitude in degrees
    Longitude: float
    /// Location title
    Title: string
    /// Inline keyboard attached to the message
    ReplyMarkup: InlineKeyboardMarkup option
    /// Content of the message to be sent instead of the location
    InputMessageContent: InputMessageContent option
    /// Url of the thumbnail for the result
    ThumbUrl: string option
    /// Thumbnail width
    ThumbWidth: int option
    /// Thumbnail height
    ThumbHeight: int option }
    
/// Represents a link to a video animation (H.264/MPEG-4 AVC video without sound). By default, this animated MPEG-4 file will be sent by the user with optional caption. Alternatively, you can use input_message_content to send a message with the specified content instead of the animation
type InlineQueryResultMpeg4Gif =
  { /// Type of the result, must be mpeg4_gif
    Type: string
    /// Unique identifier for this result, 1-64 bytes
    Id: string
    /// A valid URL for the MP4 file. File size must not exceed 1MB
    Mpeg4Url: string
    /// Video width
    Mpeg4Width: int option
    /// Video height
    Mpeg4Height: int option
    /// Video duration
    Mpeg4Duration: int option
    /// URL of the static thumbnail (jpeg or gif) for the result
    ThumbUrl: string
    /// Title for the result
    Title: string option
    /// Caption of the MPEG-4 file to be sent, 0-200 characters
    Caption: string option
    /// Inline keyboard attached to the message
    ReplyMarkup: InlineKeyboardMarkup option
    /// Content of the message to be sent instead of the video animation
    InputMessageContent: InputMessageContent option }

/// Represents a link to a photo. By default, this photo will be sent by the user with optional caption. Alternatively, you can use input_message_content to send a message with the specified content instead of the photo.
type InlineQueryResultPhoto =
  { /// Type of the result, must be photo
    Type: string
    /// Unique identifier for this result, 1-64 bytes
    Id: string
    /// A valid URL of the photo. Photo must be in jpeg format. Photo size must not exceed 5MB
    PhotoUrl: string
    /// URL of the thumbnail for the photo
    ThumbUrl: string
    /// Width of the photo
    PhotoWidth: int option
    /// Height of the photo
    PhotoHeight: int option
    /// Title for the result
    Title: string option
    /// Short description of the result
    Description: string option
    /// Caption of the photo to be sent, 0-200 characters
    Caption: string option
    /// Inline keyboard attached to the message
    ReplyMarkup: InlineKeyboardMarkup option
    /// Content of the message to be sent instead of the photo
    InputMessageContent: InputMessageContent option }
    
/// Represents a venue. By default, the venue will be sent by the user. Alternatively, you can use input_message_content to send a message with the specified content instead of the venue
type InlineQueryResultVenue =
  { /// Type of the result, must be venue
    Type: string
    /// Unique identifier for this result, 1-64 Bytes
    Id: string
    /// Latitude of the venue location in degrees
    Latitude: float
    /// Longitude of the venue location in degrees
    Longitude: float
    /// Title of the venue
    Title: string
    /// Address of the venue
    Address: string
    /// Foursquare identifier of the venue if known
    FoursquareId: string option
    /// Inline keyboard attached to the message
    ReplyMarkup: InlineKeyboardMarkup option
    /// Content of the message to be sent instead of the venue
    InputMessageContent: InputMessageContent option
    /// Url of the thumbnail for the result
    ThumbUrl: string option
    /// Thumbnail width
    ThumbWidth: int option
    /// Thumbnail height
    ThumbHeight: int option }

/// Represents a link to a page containing an embedded video player or a video file. By default, this video file will be sent by the user with an optional caption. Alternatively, you can use input_message_content to send a message with the specified content instead of the video.
type InlineQueryResultVideo =
  { /// Type of the result, must be video
    Type: string
    /// Unique identifier for this result, 1-64 bytes
    Id: string
    /// A valid URL for the embedded video player or video file
    VideoUrl: string
    /// Mime type of the content of video url, “text/html” or “video/mp4”
    MimeType: string
    /// URL of the thumbnail (jpeg only) for the video
    ThumbUrl: string
    /// Title for the result
    Title: string
    /// Caption of the video to be sent, 0-200 characters
    Caption: string option
    /// Video width
    VideoWidth: int option
    /// Video height
    VideoHeight: int option
    /// Video duration in seconds
    VideoDuration: int option
    /// Short description of the result
    Description: string option
    /// Inline keyboard attached to the message
    ReplyMarkup: InlineKeyboardMarkup option
    /// Content of the message to be sent instead of the video
    InputMessageContent: InputMessageContent option }

/// Represents a link to a voice recording in an .ogg container encoded with OPUS. By default, this voice recording will be sent by the user. Alternatively, you can use input_message_content to send a message with the specified content instead of the the voice message.
type InlineQueryResultVoice =
  { /// Type of the result, must be voice
    Type: string
    /// Unique identifier for this result, 1-64 bytes
    Id: string
    /// A valid URL for the voice recording
    VoiceUrl: string
    /// Recording title
    Title: string
    /// Caption, 0-200 characters
    Caption: string option
    /// Recording duration in seconds
    VoiceDuration: int option
    /// Inline keyboard attached to the message
    ReplyMarkup: InlineKeyboardMarkup option
    /// Content of the message to be sent instead of the voice recording
    InputMessageContent: InputMessageContent option }
      
/// This object represents one result of an inline query. Telegram clients currently support results of the 20 types
type InlineQueryResult = 
  | CachedAudio of InlineQueryResultCachedAudio
  | CachedDocument of InlineQueryResultCachedDocument
  | CachedGif of InlineQueryResultCachedGif
  | CachedMpeg4Gif of InlineQueryResultCachedMpeg4Gif
  | CachedPhoto of InlineQueryResultCachedPhoto
  | CachedSticker of InlineQueryResultCachedSticker
  | CachedVideo of InlineQueryResultCachedVideo
  | CachedVoice of InlineQueryResultCachedVoice
  | Article of InlineQueryResultArticle
  | Audio of InlineQueryResultAudio
  | Contact of InlineQueryResultContact
  | Game of InlineQueryResultGame
  | Document of InlineQueryResultDocument
  | Gif of InlineQueryResultGif
  | Location of InlineQueryResultLocation
  | Mpeg4Gif of InlineQueryResultMpeg4Gif
  | Photo of InlineQueryResultPhoto
  | Venue of InlineQueryResultVenue
  | Video of InlineQueryResultVideo
  | Voice of InlineQueryResultVoice

type FileToSend = 
  | Url of Uri 
  | File of string * Stream
  | FileId of string

/// This object represents one row of the high scores table for a game
type GameHighScore =
  { /// Position in high score table for the game
    Position: int
    /// User
    User: User
    /// Score
    Score: int }