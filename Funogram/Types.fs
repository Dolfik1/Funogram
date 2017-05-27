namespace Funogram

open System.Runtime.CompilerServices

// Allow construct types to Funogram.Tests
[<assembly: InternalsVisibleTo("Newtonsoft.Json")>]
do()

module Types =
    open System
    open Newtonsoft.Json

    type ChatId = 
      | ChatIdInt of int
      | ChatIdLong of int64
      | ChatIdString of string


    [<CLIMutable>]
    /// This object represents a Telegram user or bot.
    type User =
        { /// Unique identifier for this user or bot
          Id: int64
          /// User‘s or bot’s first name
          FirstName: string
          /// User‘s or bot’s last name
          LastName: string option
          /// User‘s or bot’s username
          Username: string option
          /// IETF language tag of the user's language
          LanguageCode: string option }

    [<CLIMutable>]
    /// This object represents a chat.
    type Chat =
        { /// Unique identifier for this chat.
          Id: int64
          /// Type of chat, can be either "private", "group", "supergroup" or "channel"
          Type: string
          /// Title, for supergroups, channels and group chats
          Title: string
          /// Username, for private chats, supergroups and channels if available
          Username: string
          /// First name of the other party in a private chat
          FirstName: string
          /// Last name of the other party in a private chat
          LastName: string
          /// True if a group has ‘All Members Are Admins’ enabled.
          AllMembersAreAdministrators: bool }

    [<CLIMutable>]
    /// This object represents one special entity in a text message. For example, hashtags, usernames, URLs, etc.
    type MessageEntity =
        { /// Type of the entity. Can be mention (@username), hashtag, bot_command, url, email, bold (bold text), 
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

    [<CLIMutable>]
    /// This object represents an audio file to be treated as music by the Telegram clients
    type Audio =
        { /// Unique identifier for this file
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

    [<CLIMutable>]
    /// This object represents one size of a photo or a file / sticker thumbnail.
    type PhotoSize =
        { /// Unique identifier for this file
          FileId: string
          /// Photo width
          Width: int
          /// Photo height
          Height: int
          /// File size
          FileSize: int option }

    [<CLIMutable>]
    /// This object represents a general file (as opposed to photos, voice messages and audio files).
    type Document = 
        { /// Unique file identifier
          FileId: string
          /// Document thumbnail as defined by sender
          Thumb: PhotoSize option
          /// Original filename as defined by sender
          FileName: string option
          /// MIME type of the file as defined by sender
          MimeType: string option
          /// File size
          FileSize: int option }

    [<CLIMutable>]
    /// This object represents a sticker.
    type Sticker =
        { /// Unique identifier for this file
          FileId: string
          /// Sticker width
          Width: int
          /// Sticker height
          Height: int
          /// Sticker thumbnail in .webp or .jpg format
          Thumb: PhotoSize option
          /// Emoji associated with the sticker
          Emoji: string option
          /// File size
          FileSize: int option }

    [<CLIMutable>]
    /// This object represents a video file.
    type Video =
        { /// Unique identifier for this file
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

    [<CLIMutable>]
    /// This object represents a voice note
    type Voice = 
        { /// Unique identifier for this file
          FileId: string
          /// Duration of the audio in seconds as defined by sender
          Duration: int
          /// MIME type of the file as defined by sender
          MimeType: string option
          /// File size
          FileSize: int option }

    [<CLIMutable>]
    /// Contact's phone number
    type Contact =
        { /// 
          PhoneNumber: string
          /// Contact's first name
          FirstName: string
          /// Contact's last name
          LastName: string option
          /// Contact's user identifier in Telegram
          UserId: int option }

    [<CLIMutable>]
    /// This object represents a point on the map
    type Location =
        { /// Longitude as defined by sender
          Longitude: double
          /// Latitude as defined by sender
          Latitude: double }
    
    [<CLIMutable>]
    /// This object represents a venue.
    type Venue =
        { /// Venue location
          Location: Location
          /// Name of the venue
          Title: string
          /// Address of the venue
          Address: string
          /// Foursquare identifier of the venue
          FoursquareId: string option }
    
    [<CLIMutable>]
    // This object represent a user's profile pictures.
    type UserProfilePhotos =
      { // Total number of profile pictures the target user has
        TotalCount: int
        // Requested profile pictures (in up to 4 sizes each)
        Photos: seq<seq<PhotoSize>> }

    [<CLIMutable>]
    /// You can provide an animation for your game so that it looks stylish in chats 
    /// (check out Lumberjack for an example). This object represents an animation file 
    // to be displayed in the message containing a game.
    type Animation =
        { /// Unique file identifier
          FileId: string
          /// Animation thumbnail as defined by sender
          Thumb: PhotoSize option
          /// Original animation filename as defined by sender
          FileName: string option
          /// MIME type of the file as defined by sender
          MimeType: string option
          /// File size
          FileSize: string option }

    [<CLIMutable>]
    /// This object represents a game. Use BotFather to create and edit games, their short names will act as unique identifiers
    type Game = 
        { /// Title of the game
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
          
    [<CLIMutable>]
    /// This object represents a message
    type Message = 
        { /// Unique message identifier inside this chat
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
          ForwardFromMessageId: int64
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
          /// Specified message was pinned. Note that the Message object in this field 
          /// will not contain further ReplyToMessage fields even if it is itself a reply.
          PinnedMessage: Message option }

    [<CLIMutable>]
    /// This object represents an incoming inline query. 
    /// When the user sends an empty query, your bot could return some default or trending results.
    type InlineQuery =
        { /// Unique identifier for this query
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
        { /// The unique identifier for the result that was chosen
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
    /// This object represents an incoming callback query from a callback button 
    /// in an inline keyboard. If the button that originated the query was attached 
    /// to a message sent by the bot, the field message will be present. 
    /// If the button was attached to a message sent via the bot (in inline mode), 
    /// the field InlineMessageId will be present. Exactly one of the fields data 
    /// or GameShortName will be present.
    type CallbackQuery =
        { /// Unique identifier for this query
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
    /// This object represents an incoming update.
    /// At most one of the optional parameters can be present in any given update
    type Update = 
        { /// The update‘s unique identifier. Update identifiers start from a certain positive 
          /// number and increase sequentially. This ID becomes especially handy if you’re using 
          /// Webhooks, since it allows you to ignore repeated updates or to restore the correct
          /// update sequence, should they get out of order.
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
      /// HTML parse syntax
      | HTML

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
        CallbackGame: CallbackGame option
      }

    /// This object represents one button of the reply keyboard. For simple text buttons String can be used 
    /// instead of this object to specify text of the button. Optional fields are mutually exclusive.
    type KeyboardButton = 
      { /// Text of the button. If none of the optional fields are used, it will be sent to the bot as a message when the button is pressed
        Text: string
        /// If True, the user's phone number will be sent as a contact when the button is pressed. Available in private chats only
        RequestContact: bool option
        /// If True, the user's current location will be sent when the button is pressed. Available in private chats only
        RequestLocation: bool option
      }

    /// This object represents an inline keyboard that appears right next to the message it belongs to
    type InlineKeyboardMarkup =
      { /// Array of button rows, each represented by an Array of InlineKeyboardButton objects
        InlineKeyboard: (InlineKeyboardButton list * InlineKeyboardButton list) list
      }

    /// This object represents a custom keyboard with reply options (see Introduction to bots for details and examples).
    type ReplyKeyboardMarkup =
      {
        /// Array of button rows, each represented by an Array of KeyboardButton objects
        Keyboard: (KeyboardButton list * KeyboardButton list) list
        /// Requests clients to resize the keyboard vertically for optimal fit (e.g., make the keyboard smaller if there are 
        /// just two rows of buttons). Defaults to false, in which case the custom keyboard is always of the same height 
        /// as the app's standard keyboard.
        ResizeKeyboard: bool option
        /// Requests clients to hide the keyboard as soon as it's been used. The keyboard will still be available, 
        /// but clients will automatically display the usual letter-keyboard in the chat – the user can press a special 
        /// button in the input field to see the custom keyboard again. Defaults to false.
        OneTimeKeyboard: bool option
        /// Use this parameter if you want to show the keyboard to specific users only. Targets: 1) users that are @mentioned in 
        /// the text of the Message object; 2) if the bot's message is a reply (has reply_to_message_id), sender of the original message
        Selective: bool option
      }
      
    /// Upon receiving a message with this object, Telegram clients will remove the current custom keyboard and display 
    /// the default letter-keyboard. By default, custom keyboards are displayed until a new keyboard is sent by a bot. 
    /// An exception is made for one-time keyboards that are hidden immediately after the user presses a button (see ReplyKeyboardMarkup).
    type ReplyKeyboardRemove =
      { /// Requests clients to remove the custom keyboard (user will not be able to summon this keyboard; if you want to hide the keyboard 
        /// from sight but keep it accessible, use one_time_keyboard in ReplyKeyboardMarkup)
        RemoveKeyboard: bool
        /// Use this parameter if you want to remove the keyboard for specific users only. Targets: 1) users that are @mentioned in the text 
        /// of the Message object; 2) if the bot's message is a reply (has reply_to_message_id), sender of the original message.
        Selective: bool option
      }

    /// Upon receiving a message with this object, Telegram clients will display a reply interface to the user 
    /// (act as if the user has selected the bot‘s message and tapped ’Reply'). This can be extremely useful if 
    /// you want to create user-friendly step-by-step interfaces without having to sacrifice privacy mode.
    type ForceReply = 
      { /// Shows reply interface to the user, as if they manually selected the bot‘s message and tapped ’Reply'
        ForceReply: bool
        /// Use this parameter if you want to force reply from specific users only. Targets: 1) users that are @mentioned in the 
        /// text of the Message object; 2) if the bot's message is a reply (has reply_to_message_id), sender of the original message.
        Selective: bool
      }
      
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