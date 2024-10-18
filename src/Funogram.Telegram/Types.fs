module Funogram.Telegram.Types

open System
open System.IO
open System.Runtime.Serialization

type ChatId = 
  | Int of int64
  | String of string
    
type InputFile = 
  | Url of Uri 
  | File of string * Stream
  | FileBytes of string * byte[]
  | FileId of string

type ChatType =
  | Private
  | Group
  | [<DataMember(Name = "supergroup")>] SuperGroup
  | Channel
  | Sender
  | Unknown

/// Message text parsing mode
type ParseMode = 
  /// Markdown parse syntax
  | Markdown
  /// Html parse syntax
  | HTML
  /// MarkdownV2 parse syntax
  | [<DataMember(Name = "MarkdownV2")>] MarkdownV2

/// Type of action to broadcast
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
  | ChooseSticker

type ChatMemberStatus =
  | Creator
  | Administrator
  | Member
  | Restricted
  | Left
  | Kicked
  | Unknown

type MaskPoint =
  | Forehead
  | Eyes
  | Mouth
  | Chin

type Markup = 
  | InlineKeyboardMarkup of InlineKeyboardMarkup 
  | ReplyKeyboardMarkup of ReplyKeyboardMarkup
  | ReplyKeyboardRemove of ReplyKeyboardRemove
  | ForceReply of ForceReply

/// If edited message is sent by the bot, used Message, otherwise Success.
and EditMessageResult = 
  /// Message sent by the bot
  | Message of Message
  /// Message sent via the bot or another...
  | Success of bool

/// This object represents an incoming update.
/// At most one of the optional parameters can be present in any given update.
and [<CLIMutable>] Update =
  {
    /// The update's unique identifier. Update identifiers start from a certain positive number and increase sequentially. This identifier becomes especially handy if you're using webhooks, since it allows you to ignore repeated updates or to restore the correct update sequence, should they get out of order. If there are no new updates for at least a week, then identifier of the next update will be chosen randomly instead of sequentially.
    [<DataMember(Name = "update_id")>]
    UpdateId: int64
    /// New incoming message of any kind - text, photo, sticker, etc.
    [<DataMember(Name = "message")>]
    Message: Message option
    /// New version of a message that is known to the bot and was edited. This update may at times be triggered by changes to message fields that are either unavailable or not actively used by your bot.
    [<DataMember(Name = "edited_message")>]
    EditedMessage: Message option
    /// New incoming channel post of any kind - text, photo, sticker, etc.
    [<DataMember(Name = "channel_post")>]
    ChannelPost: Message option
    /// New version of a channel post that is known to the bot and was edited. This update may at times be triggered by changes to message fields that are either unavailable or not actively used by your bot.
    [<DataMember(Name = "edited_channel_post")>]
    EditedChannelPost: Message option
    /// The bot was connected to or disconnected from a business account, or a user edited an existing connection with the bot
    [<DataMember(Name = "business_connection")>]
    BusinessConnection: BusinessConnection option
    /// New message from a connected business account
    [<DataMember(Name = "business_message")>]
    BusinessMessage: Message option
    /// New version of a message from a connected business account
    [<DataMember(Name = "edited_business_message")>]
    EditedBusinessMessage: Message option
    /// Messages were deleted from a connected business account
    [<DataMember(Name = "deleted_business_messages")>]
    DeletedBusinessMessages: BusinessMessagesDeleted option
    /// A reaction to a message was changed by a user. The bot must be an administrator in the chat and must explicitly specify "message_reaction" in the list of allowed_updates to receive these updates. The update isn't received for reactions set by bots.
    [<DataMember(Name = "message_reaction")>]
    MessageReaction: MessageReactionUpdated option
    /// Reactions to a message with anonymous reactions were changed. The bot must be an administrator in the chat and must explicitly specify "message_reaction_count" in the list of allowed_updates to receive these updates. The updates are grouped and can be sent with delay up to a few minutes.
    [<DataMember(Name = "message_reaction_count")>]
    MessageReactionCount: MessageReactionCountUpdated option
    /// New incoming inline query
    [<DataMember(Name = "inline_query")>]
    InlineQuery: InlineQuery option
    /// The result of an inline query that was chosen by a user and sent to their chat partner. Please see our documentation on the feedback collecting for details on how to enable these updates for your bot.
    [<DataMember(Name = "chosen_inline_result")>]
    ChosenInlineResult: ChosenInlineResult option
    /// New incoming callback query
    [<DataMember(Name = "callback_query")>]
    CallbackQuery: CallbackQuery option
    /// New incoming shipping query. Only for invoices with flexible price
    [<DataMember(Name = "shipping_query")>]
    ShippingQuery: ShippingQuery option
    /// New incoming pre-checkout query. Contains full information about checkout
    [<DataMember(Name = "pre_checkout_query")>]
    PreCheckoutQuery: PreCheckoutQuery option
    /// A user purchased paid media with a non-empty payload sent by the bot in a non-channel chat
    [<DataMember(Name = "purchased_paid_media")>]
    PurchasedPaidMedia: PaidMediaPurchased option
    /// New poll state. Bots receive only updates about manually stopped polls and polls, which are sent by the bot
    [<DataMember(Name = "poll")>]
    Poll: Poll option
    /// A user changed their answer in a non-anonymous poll. Bots receive new votes only in polls that were sent by the bot itself.
    [<DataMember(Name = "poll_answer")>]
    PollAnswer: PollAnswer option
    /// The bot's chat member status was updated in a chat. For private chats, this update is received only when the bot is blocked or unblocked by the user.
    [<DataMember(Name = "my_chat_member")>]
    MyChatMember: ChatMemberUpdated option
    /// A chat member's status was updated in a chat. The bot must be an administrator in the chat and must explicitly specify "chat_member" in the list of allowed_updates to receive these updates.
    [<DataMember(Name = "chat_member")>]
    ChatMember: ChatMemberUpdated option
    /// A request to join the chat has been sent. The bot must have the can_invite_users administrator right in the chat to receive these updates.
    [<DataMember(Name = "chat_join_request")>]
    ChatJoinRequest: ChatJoinRequest option
    /// A chat boost was added or changed. The bot must be an administrator in the chat to receive these updates.
    [<DataMember(Name = "chat_boost")>]
    ChatBoost: ChatBoostUpdated option
    /// A boost was removed from a chat. The bot must be an administrator in the chat to receive these updates.
    [<DataMember(Name = "removed_chat_boost")>]
    RemovedChatBoost: ChatBoostRemoved option
  }
  static member Create(updateId: int64, ?chatJoinRequest: ChatJoinRequest, ?chatMember: ChatMemberUpdated, ?myChatMember: ChatMemberUpdated, ?pollAnswer: PollAnswer, ?poll: Poll, ?purchasedPaidMedia: PaidMediaPurchased, ?preCheckoutQuery: PreCheckoutQuery, ?shippingQuery: ShippingQuery, ?callbackQuery: CallbackQuery, ?chosenInlineResult: ChosenInlineResult, ?inlineQuery: InlineQuery, ?messageReactionCount: MessageReactionCountUpdated, ?messageReaction: MessageReactionUpdated, ?deletedBusinessMessages: BusinessMessagesDeleted, ?editedBusinessMessage: Message, ?businessMessage: Message, ?businessConnection: BusinessConnection, ?editedChannelPost: Message, ?channelPost: Message, ?editedMessage: Message, ?message: Message, ?chatBoost: ChatBoostUpdated, ?removedChatBoost: ChatBoostRemoved) = 
    {
      UpdateId = updateId
      ChatJoinRequest = chatJoinRequest
      ChatMember = chatMember
      MyChatMember = myChatMember
      PollAnswer = pollAnswer
      Poll = poll
      PurchasedPaidMedia = purchasedPaidMedia
      PreCheckoutQuery = preCheckoutQuery
      ShippingQuery = shippingQuery
      CallbackQuery = callbackQuery
      ChosenInlineResult = chosenInlineResult
      InlineQuery = inlineQuery
      MessageReactionCount = messageReactionCount
      MessageReaction = messageReaction
      DeletedBusinessMessages = deletedBusinessMessages
      EditedBusinessMessage = editedBusinessMessage
      BusinessMessage = businessMessage
      BusinessConnection = businessConnection
      EditedChannelPost = editedChannelPost
      ChannelPost = channelPost
      EditedMessage = editedMessage
      Message = message
      ChatBoost = chatBoost
      RemovedChatBoost = removedChatBoost
    }

/// Describes the current status of a webhook.
/// All types used in the Bot API responses are represented as JSON-objects.
/// It is safe to use 32-bit signed integers for storing all Integer fields unless otherwise noted.
and [<CLIMutable>] WebhookInfo =
  {
    /// Webhook URL, may be empty if webhook is not set up
    [<DataMember(Name = "url")>]
    Url: string
    /// True, if a custom certificate was provided for webhook certificate checks
    [<DataMember(Name = "has_custom_certificate")>]
    HasCustomCertificate: bool
    /// Number of updates awaiting delivery
    [<DataMember(Name = "pending_update_count")>]
    PendingUpdateCount: int64
    /// Currently used webhook IP address
    [<DataMember(Name = "ip_address")>]
    IpAddress: string option
    /// Unix time for the most recent error that happened when trying to deliver an update via webhook
    [<DataMember(Name = "last_error_date")>]
    LastErrorDate: int64 option
    /// Error message in human-readable format for the most recent error that happened when trying to deliver an update via webhook
    [<DataMember(Name = "last_error_message")>]
    LastErrorMessage: string option
    /// Unix time of the most recent error that happened when trying to synchronize available updates with Telegram datacenters
    [<DataMember(Name = "last_synchronization_error_date")>]
    LastSynchronizationErrorDate: int64 option
    /// The maximum allowed number of simultaneous HTTPS connections to the webhook for update delivery
    [<DataMember(Name = "max_connections")>]
    MaxConnections: int64 option
    /// A list of update types the bot is subscribed to. Defaults to all update types except chat_member
    [<DataMember(Name = "allowed_updates")>]
    AllowedUpdates: string[] option
  }
  static member Create(url: string, hasCustomCertificate: bool, pendingUpdateCount: int64, ?ipAddress: string, ?lastErrorDate: int64, ?lastErrorMessage: string, ?lastSynchronizationErrorDate: int64, ?maxConnections: int64, ?allowedUpdates: string[]) = 
    {
      Url = url
      HasCustomCertificate = hasCustomCertificate
      PendingUpdateCount = pendingUpdateCount
      IpAddress = ipAddress
      LastErrorDate = lastErrorDate
      LastErrorMessage = lastErrorMessage
      LastSynchronizationErrorDate = lastSynchronizationErrorDate
      MaxConnections = maxConnections
      AllowedUpdates = allowedUpdates
    }

/// This object represents a Telegram user or bot.
and [<CLIMutable>] User =
  {
    /// Unique identifier for this user or bot. This number may have more than 32 significant bits and some programming languages may have difficulty/silent defects in interpreting it. But it has at most 52 significant bits, so a 64-bit integer or double-precision float type are safe for storing this identifier.
    [<DataMember(Name = "id")>]
    Id: int64
    /// True, if this user is a bot
    [<DataMember(Name = "is_bot")>]
    IsBot: bool
    /// User's or bot's first name
    [<DataMember(Name = "first_name")>]
    FirstName: string
    /// User's or bot's last name
    [<DataMember(Name = "last_name")>]
    LastName: string option
    /// User's or bot's username
    [<DataMember(Name = "username")>]
    Username: string option
    /// IETF language tag of the user's language
    [<DataMember(Name = "language_code")>]
    LanguageCode: string option
    /// True, if this user is a Telegram Premium user
    [<DataMember(Name = "is_premium")>]
    IsPremium: bool option
    /// True, if this user added the bot to the attachment menu
    [<DataMember(Name = "added_to_attachment_menu")>]
    AddedToAttachmentMenu: bool option
    /// True, if the bot can be invited to groups. Returned only in getMe.
    [<DataMember(Name = "can_join_groups")>]
    CanJoinGroups: bool option
    /// True, if privacy mode is disabled for the bot. Returned only in getMe.
    [<DataMember(Name = "can_read_all_group_messages")>]
    CanReadAllGroupMessages: bool option
    /// True, if the bot supports inline queries. Returned only in getMe.
    [<DataMember(Name = "supports_inline_queries")>]
    SupportsInlineQueries: bool option
    /// True, if the bot can be connected to a Telegram Business account to receive its messages. Returned only in getMe.
    [<DataMember(Name = "can_connect_to_business")>]
    CanConnectToBusiness: bool option
    /// True, if the bot has a main Web App. Returned only in getMe.
    [<DataMember(Name = "has_main_web_app")>]
    HasMainWebApp: bool option
  }
  static member Create(id: int64, isBot: bool, firstName: string, ?lastName: string, ?username: string, ?languageCode: string, ?isPremium: bool, ?addedToAttachmentMenu: bool, ?canJoinGroups: bool, ?canReadAllGroupMessages: bool, ?supportsInlineQueries: bool, ?canConnectToBusiness: bool, ?hasMainWebApp: bool) = 
    {
      Id = id
      IsBot = isBot
      FirstName = firstName
      LastName = lastName
      Username = username
      LanguageCode = languageCode
      IsPremium = isPremium
      AddedToAttachmentMenu = addedToAttachmentMenu
      CanJoinGroups = canJoinGroups
      CanReadAllGroupMessages = canReadAllGroupMessages
      SupportsInlineQueries = supportsInlineQueries
      CanConnectToBusiness = canConnectToBusiness
      HasMainWebApp = hasMainWebApp
    }

/// This object represents a chat.
and [<CLIMutable>] Chat =
  {
    /// Unique identifier for this chat. This number may have more than 32 significant bits and some programming languages may have difficulty/silent defects in interpreting it. But it has at most 52 significant bits, so a signed 64-bit integer or double-precision float type are safe for storing this identifier.
    [<DataMember(Name = "id")>]
    Id: int64
    /// Type of the chat, can be either “private”, “group”, “supergroup” or “channel”
    [<DataMember(Name = "type")>]
    Type: ChatType
    /// Title, for supergroups, channels and group chats
    [<DataMember(Name = "title")>]
    Title: string option
    /// Username, for private chats, supergroups and channels if available
    [<DataMember(Name = "username")>]
    Username: string option
    /// First name of the other party in a private chat
    [<DataMember(Name = "first_name")>]
    FirstName: string option
    /// Last name of the other party in a private chat
    [<DataMember(Name = "last_name")>]
    LastName: string option
    /// True, if the supergroup chat is a forum (has topics enabled)
    [<DataMember(Name = "is_forum")>]
    IsForum: bool option
  }
  static member Create(id: int64, ``type``: ChatType, ?title: string, ?username: string, ?firstName: string, ?lastName: string, ?isForum: bool) = 
    {
      Id = id
      Type = ``type``
      Title = title
      Username = username
      FirstName = firstName
      LastName = lastName
      IsForum = isForum
    }

/// This object contains full information about a chat.
and [<CLIMutable>] ChatFullInfo =
  {
    /// Unique identifier for this chat. This number may have more than 32 significant bits and some programming languages may have difficulty/silent defects in interpreting it. But it has at most 52 significant bits, so a signed 64-bit integer or double-precision float type are safe for storing this identifier.
    [<DataMember(Name = "id")>]
    Id: int64
    /// Type of the chat, can be either “private”, “group”, “supergroup” or “channel”
    [<DataMember(Name = "type")>]
    Type: string
    /// Title, for supergroups, channels and group chats
    [<DataMember(Name = "title")>]
    Title: string option
    /// Username, for private chats, supergroups and channels if available
    [<DataMember(Name = "username")>]
    Username: string option
    /// First name of the other party in a private chat
    [<DataMember(Name = "first_name")>]
    FirstName: string option
    /// Last name of the other party in a private chat
    [<DataMember(Name = "last_name")>]
    LastName: string option
    /// True, if the supergroup chat is a forum (has topics enabled)
    [<DataMember(Name = "is_forum")>]
    IsForum: bool option
    /// Identifier of the accent color for the chat name and backgrounds of the chat photo, reply header, and link preview. See accent colors for more details.
    [<DataMember(Name = "accent_color_id")>]
    AccentColorId: int64
    /// The maximum number of reactions that can be set on a message in the chat
    [<DataMember(Name = "max_reaction_count")>]
    MaxReactionCount: int64
    /// Chat photo
    [<DataMember(Name = "photo")>]
    Photo: ChatPhoto option
    /// If non-empty, the list of all active chat usernames; for private chats, supergroups and channels
    [<DataMember(Name = "active_usernames")>]
    ActiveUsernames: string[] option
    /// For private chats, the date of birth of the user
    [<DataMember(Name = "birthdate")>]
    Birthdate: Birthdate option
    /// For private chats with business accounts, the intro of the business
    [<DataMember(Name = "business_intro")>]
    BusinessIntro: BusinessIntro option
    /// For private chats with business accounts, the location of the business
    [<DataMember(Name = "business_location")>]
    BusinessLocation: BusinessLocation option
    /// For private chats with business accounts, the opening hours of the business
    [<DataMember(Name = "business_opening_hours")>]
    BusinessOpeningHours: BusinessOpeningHours option
    /// For private chats, the personal channel of the user
    [<DataMember(Name = "personal_chat")>]
    PersonalChat: Chat option
    /// List of available reactions allowed in the chat. If omitted, then all emoji reactions are allowed.
    [<DataMember(Name = "available_reactions")>]
    AvailableReactions: ReactionType[] option
    /// Custom emoji identifier of the emoji chosen by the chat for the reply header and link preview background
    [<DataMember(Name = "background_custom_emoji_id")>]
    BackgroundCustomEmojiId: string option
    /// Identifier of the accent color for the chat's profile background. See profile accent colors for more details.
    [<DataMember(Name = "profile_accent_color_id")>]
    ProfileAccentColorId: int64 option
    /// Custom emoji identifier of the emoji chosen by the chat for its profile background
    [<DataMember(Name = "profile_background_custom_emoji_id")>]
    ProfileBackgroundCustomEmojiId: string option
    /// Custom emoji identifier of the emoji status of the chat or the other party in a private chat
    [<DataMember(Name = "emoji_status_custom_emoji_id")>]
    EmojiStatusCustomEmojiId: string option
    /// Expiration date of the emoji status of the chat or the other party in a private chat, in Unix time, if any
    [<DataMember(Name = "emoji_status_expiration_date")>]
    EmojiStatusExpirationDate: int64 option
    /// Bio of the other party in a private chat
    [<DataMember(Name = "bio")>]
    Bio: string option
    /// True, if privacy settings of the other party in the private chat allows to use tg://user?id=<user_id> links only in chats with the user
    [<DataMember(Name = "has_private_forwards")>]
    HasPrivateForwards: bool option
    /// True, if the privacy settings of the other party restrict sending voice and video note messages in the private chat
    [<DataMember(Name = "has_restricted_voice_and_video_messages")>]
    HasRestrictedVoiceAndVideoMessages: bool option
    /// True, if users need to join the supergroup before they can send messages
    [<DataMember(Name = "join_to_send_messages")>]
    JoinToSendMessages: bool option
    /// True, if all users directly joining the supergroup without using an invite link need to be approved by supergroup administrators
    [<DataMember(Name = "join_by_request")>]
    JoinByRequest: bool option
    /// Description, for groups, supergroups and channel chats
    [<DataMember(Name = "description")>]
    Description: string option
    /// Primary invite link, for groups, supergroups and channel chats
    [<DataMember(Name = "invite_link")>]
    InviteLink: string option
    /// The most recent pinned message (by sending date)
    [<DataMember(Name = "pinned_message")>]
    PinnedMessage: Message option
    /// Default chat member permissions, for groups and supergroups
    [<DataMember(Name = "permissions")>]
    Permissions: ChatPermissions option
    /// True, if paid media messages can be sent or forwarded to the channel chat. The field is available only for channel chats.
    [<DataMember(Name = "can_send_paid_media")>]
    CanSendPaidMedia: bool option
    /// For supergroups, the minimum allowed delay between consecutive messages sent by each unprivileged user; in seconds
    [<DataMember(Name = "slow_mode_delay")>]
    SlowModeDelay: int64 option
    /// For supergroups, the minimum number of boosts that a non-administrator user needs to add in order to ignore slow mode and chat permissions
    [<DataMember(Name = "unrestrict_boost_count")>]
    UnrestrictBoostCount: int64 option
    /// The time after which all messages sent to the chat will be automatically deleted; in seconds
    [<DataMember(Name = "message_auto_delete_time")>]
    MessageAutoDeleteTime: int64 option
    /// True, if aggressive anti-spam checks are enabled in the supergroup. The field is only available to chat administrators.
    [<DataMember(Name = "has_aggressive_anti_spam_enabled")>]
    HasAggressiveAntiSpamEnabled: bool option
    /// True, if non-administrators can only get the list of bots and administrators in the chat
    [<DataMember(Name = "has_hidden_members")>]
    HasHiddenMembers: bool option
    /// True, if messages from the chat can't be forwarded to other chats
    [<DataMember(Name = "has_protected_content")>]
    HasProtectedContent: bool option
    /// True, if new chat members will have access to old messages; available only to chat administrators
    [<DataMember(Name = "has_visible_history")>]
    HasVisibleHistory: bool option
    /// For supergroups, name of the group sticker set
    [<DataMember(Name = "sticker_set_name")>]
    StickerSetName: string option
    /// True, if the bot can change the group sticker set
    [<DataMember(Name = "can_set_sticker_set")>]
    CanSetStickerSet: bool option
    /// For supergroups, the name of the group's custom emoji sticker set. Custom emoji from this set can be used by all users and bots in the group.
    [<DataMember(Name = "custom_emoji_sticker_set_name")>]
    CustomEmojiStickerSetName: string option
    /// Unique identifier for the linked chat, i.e. the discussion group identifier for a channel and vice versa; for supergroups and channel chats. This identifier may be greater than 32 bits and some programming languages may have difficulty/silent defects in interpreting it. But it is smaller than 52 bits, so a signed 64 bit integer or double-precision float type are safe for storing this identifier.
    [<DataMember(Name = "linked_chat_id")>]
    LinkedChatId: int64 option
    /// For supergroups, the location to which the supergroup is connected
    [<DataMember(Name = "location")>]
    Location: ChatLocation option
  }
  static member Create(id: int64, ``type``: string, accentColorId: int64, maxReactionCount: int64, ?joinToSendMessages: bool, ?joinByRequest: bool, ?description: string, ?inviteLink: string, ?pinnedMessage: Message, ?permissions: ChatPermissions, ?canSendPaidMedia: bool, ?slowModeDelay: int64, ?messageAutoDeleteTime: int64, ?hasRestrictedVoiceAndVideoMessages: bool, ?hasAggressiveAntiSpamEnabled: bool, ?hasHiddenMembers: bool, ?hasProtectedContent: bool, ?hasVisibleHistory: bool, ?stickerSetName: string, ?canSetStickerSet: bool, ?customEmojiStickerSetName: string, ?unrestrictBoostCount: int64, ?hasPrivateForwards: bool, ?emojiStatusExpirationDate: int64, ?linkedChatId: int64, ?title: string, ?username: string, ?firstName: string, ?lastName: string, ?isForum: bool, ?photo: ChatPhoto, ?activeUsernames: string[], ?birthdate: Birthdate, ?businessIntro: BusinessIntro, ?businessLocation: BusinessLocation, ?businessOpeningHours: BusinessOpeningHours, ?personalChat: Chat, ?availableReactions: ReactionType[], ?backgroundCustomEmojiId: string, ?profileAccentColorId: int64, ?profileBackgroundCustomEmojiId: string, ?emojiStatusCustomEmojiId: string, ?bio: string, ?location: ChatLocation) = 
    {
      Id = id
      Type = ``type``
      AccentColorId = accentColorId
      MaxReactionCount = maxReactionCount
      JoinToSendMessages = joinToSendMessages
      JoinByRequest = joinByRequest
      Description = description
      InviteLink = inviteLink
      PinnedMessage = pinnedMessage
      Permissions = permissions
      CanSendPaidMedia = canSendPaidMedia
      SlowModeDelay = slowModeDelay
      MessageAutoDeleteTime = messageAutoDeleteTime
      HasRestrictedVoiceAndVideoMessages = hasRestrictedVoiceAndVideoMessages
      HasAggressiveAntiSpamEnabled = hasAggressiveAntiSpamEnabled
      HasHiddenMembers = hasHiddenMembers
      HasProtectedContent = hasProtectedContent
      HasVisibleHistory = hasVisibleHistory
      StickerSetName = stickerSetName
      CanSetStickerSet = canSetStickerSet
      CustomEmojiStickerSetName = customEmojiStickerSetName
      UnrestrictBoostCount = unrestrictBoostCount
      HasPrivateForwards = hasPrivateForwards
      EmojiStatusExpirationDate = emojiStatusExpirationDate
      LinkedChatId = linkedChatId
      Title = title
      Username = username
      FirstName = firstName
      LastName = lastName
      IsForum = isForum
      Photo = photo
      ActiveUsernames = activeUsernames
      Birthdate = birthdate
      BusinessIntro = businessIntro
      BusinessLocation = businessLocation
      BusinessOpeningHours = businessOpeningHours
      PersonalChat = personalChat
      AvailableReactions = availableReactions
      BackgroundCustomEmojiId = backgroundCustomEmojiId
      ProfileAccentColorId = profileAccentColorId
      ProfileBackgroundCustomEmojiId = profileBackgroundCustomEmojiId
      EmojiStatusCustomEmojiId = emojiStatusCustomEmojiId
      Bio = bio
      Location = location
    }

/// This object represents a message.
and [<CLIMutable>] Message =
  {
    /// Unique message identifier inside this chat
    [<DataMember(Name = "message_id")>]
    MessageId: int64
    /// Unique identifier of a message thread to which the message belongs; for supergroups only
    [<DataMember(Name = "message_thread_id")>]
    MessageThreadId: int64 option
    /// Sender of the message; may be empty for messages sent to channels. For backward compatibility, if the message was sent on behalf of a chat, the field contains a fake sender user in non-channel chats
    [<DataMember(Name = "from")>]
    From: User option
    /// Sender of the message when sent on behalf of a chat. For example, the supergroup itself for messages sent by its anonymous administrators or a linked channel for messages automatically forwarded to the channel's discussion group. For backward compatibility, if the message was sent on behalf of a chat, the field from contains a fake sender user in non-channel chats.
    [<DataMember(Name = "sender_chat")>]
    SenderChat: Chat option
    /// If the sender of the message boosted the chat, the number of boosts added by the user
    [<DataMember(Name = "sender_boost_count")>]
    SenderBoostCount: int64 option
    /// The bot that actually sent the message on behalf of the business account. Available only for outgoing messages sent on behalf of the connected business account.
    [<DataMember(Name = "sender_business_bot")>]
    SenderBusinessBot: User option
    /// Date the message was sent in Unix time. It is always a positive number, representing a valid date.
    [<DataMember(Name = "date")>]
    Date: DateTime
    /// Unique identifier of the business connection from which the message was received. If non-empty, the message belongs to a chat of the corresponding business account that is independent from any potential bot chat which might share the same identifier.
    [<DataMember(Name = "business_connection_id")>]
    BusinessConnectionId: string option
    /// Chat the message belongs to
    [<DataMember(Name = "chat")>]
    Chat: Chat
    /// Information about the original message for forwarded messages
    [<DataMember(Name = "forward_origin")>]
    ForwardOrigin: MessageOrigin option
    /// True, if the message is sent to a forum topic
    [<DataMember(Name = "is_topic_message")>]
    IsTopicMessage: bool option
    /// True, if the message is a channel post that was automatically forwarded to the connected discussion group
    [<DataMember(Name = "is_automatic_forward")>]
    IsAutomaticForward: bool option
    /// For replies in the same chat and message thread, the original message. Note that the Message object in this field will not contain further reply_to_message fields even if it itself is a reply.
    [<DataMember(Name = "reply_to_message")>]
    ReplyToMessage: Message option
    /// Information about the message that is being replied to, which may come from another chat or forum topic
    [<DataMember(Name = "external_reply")>]
    ExternalReply: ExternalReplyInfo option
    /// For replies that quote part of the original message, the quoted part of the message
    [<DataMember(Name = "quote")>]
    Quote: TextQuote option
    /// For replies to a story, the original story
    [<DataMember(Name = "reply_to_story")>]
    ReplyToStory: Story option
    /// Bot through which the message was sent
    [<DataMember(Name = "via_bot")>]
    ViaBot: User option
    /// Date the message was last edited in Unix time
    [<DataMember(Name = "edit_date")>]
    EditDate: int64 option
    /// True, if the message can't be forwarded
    [<DataMember(Name = "has_protected_content")>]
    HasProtectedContent: bool option
    /// True, if the message was sent by an implicit action, for example, as an away or a greeting business message, or as a scheduled message
    [<DataMember(Name = "is_from_offline")>]
    IsFromOffline: bool option
    /// The unique identifier of a media message group this message belongs to
    [<DataMember(Name = "media_group_id")>]
    MediaGroupId: string option
    /// Signature of the post author for messages in channels, or the custom title of an anonymous group administrator
    [<DataMember(Name = "author_signature")>]
    AuthorSignature: string option
    /// For text messages, the actual UTF-8 text of the message
    [<DataMember(Name = "text")>]
    Text: string option
    /// For text messages, special entities like usernames, URLs, bot commands, etc. that appear in the text
    [<DataMember(Name = "entities")>]
    Entities: MessageEntity[] option
    /// Options used for link preview generation for the message, if it is a text message and link preview options were changed
    [<DataMember(Name = "link_preview_options")>]
    LinkPreviewOptions: LinkPreviewOptions option
    /// Unique identifier of the message effect added to the message
    [<DataMember(Name = "effect_id")>]
    EffectId: string option
    /// Message is an animation, information about the animation. For backward compatibility, when this field is set, the document field will also be set
    [<DataMember(Name = "animation")>]
    Animation: Animation option
    /// Message is an audio file, information about the file
    [<DataMember(Name = "audio")>]
    Audio: Audio option
    /// Message is a general file, information about the file
    [<DataMember(Name = "document")>]
    Document: Document option
    /// Message contains paid media; information about the paid media
    [<DataMember(Name = "paid_media")>]
    PaidMedia: PaidMediaInfo option
    /// Message is a photo, available sizes of the photo
    [<DataMember(Name = "photo")>]
    Photo: PhotoSize[] option
    /// Message is a sticker, information about the sticker
    [<DataMember(Name = "sticker")>]
    Sticker: Sticker option
    /// Message is a forwarded story
    [<DataMember(Name = "story")>]
    Story: Story option
    /// Message is a video, information about the video
    [<DataMember(Name = "video")>]
    Video: Video option
    /// Message is a video note, information about the video message
    [<DataMember(Name = "video_note")>]
    VideoNote: VideoNote option
    /// Message is a voice message, information about the file
    [<DataMember(Name = "voice")>]
    Voice: Voice option
    /// Caption for the animation, audio, document, paid media, photo, video or voice
    [<DataMember(Name = "caption")>]
    Caption: string option
    /// For messages with a caption, special entities like usernames, URLs, bot commands, etc. that appear in the caption
    [<DataMember(Name = "caption_entities")>]
    CaptionEntities: MessageEntity[] option
    /// True, if the caption must be shown above the message media
    [<DataMember(Name = "show_caption_above_media")>]
    ShowCaptionAboveMedia: bool option
    /// True, if the message media is covered by a spoiler animation
    [<DataMember(Name = "has_media_spoiler")>]
    HasMediaSpoiler: bool option
    /// Message is a shared contact, information about the contact
    [<DataMember(Name = "contact")>]
    Contact: Contact option
    /// Message is a dice with random value
    [<DataMember(Name = "dice")>]
    Dice: Dice option
    /// Message is a game, information about the game. More about games »
    [<DataMember(Name = "game")>]
    Game: Game option
    /// Message is a native poll, information about the poll
    [<DataMember(Name = "poll")>]
    Poll: Poll option
    /// Message is a venue, information about the venue. For backward compatibility, when this field is set, the location field will also be set
    [<DataMember(Name = "venue")>]
    Venue: Venue option
    /// Message is a shared location, information about the location
    [<DataMember(Name = "location")>]
    Location: Location option
    /// New members that were added to the group or supergroup and information about them (the bot itself may be one of these members)
    [<DataMember(Name = "new_chat_members")>]
    NewChatMembers: User[] option
    /// A member was removed from the group, information about them (this member may be the bot itself)
    [<DataMember(Name = "left_chat_member")>]
    LeftChatMember: User option
    /// A chat title was changed to this value
    [<DataMember(Name = "new_chat_title")>]
    NewChatTitle: string option
    /// A chat photo was change to this value
    [<DataMember(Name = "new_chat_photo")>]
    NewChatPhoto: PhotoSize[] option
    /// Service message: the chat photo was deleted
    [<DataMember(Name = "delete_chat_photo")>]
    DeleteChatPhoto: bool option
    /// Service message: the group has been created
    [<DataMember(Name = "group_chat_created")>]
    GroupChatCreated: bool option
    /// Service message: the supergroup has been created. This field can't be received in a message coming through updates, because bot can't be a member of a supergroup when it is created. It can only be found in reply_to_message if someone replies to a very first message in a directly created supergroup.
    [<DataMember(Name = "supergroup_chat_created")>]
    SupergroupChatCreated: bool option
    /// Service message: the channel has been created. This field can't be received in a message coming through updates, because bot can't be a member of a channel when it is created. It can only be found in reply_to_message if someone replies to a very first message in a channel.
    [<DataMember(Name = "channel_chat_created")>]
    ChannelChatCreated: bool option
    /// Service message: auto-delete timer settings changed in the chat
    [<DataMember(Name = "message_auto_delete_timer_changed")>]
    MessageAutoDeleteTimerChanged: MessageAutoDeleteTimerChanged option
    /// The group has been migrated to a supergroup with the specified identifier. This number may have more than 32 significant bits and some programming languages may have difficulty/silent defects in interpreting it. But it has at most 52 significant bits, so a signed 64-bit integer or double-precision float type are safe for storing this identifier.
    [<DataMember(Name = "migrate_to_chat_id")>]
    MigrateToChatId: int64 option
    /// The supergroup has been migrated from a group with the specified identifier. This number may have more than 32 significant bits and some programming languages may have difficulty/silent defects in interpreting it. But it has at most 52 significant bits, so a signed 64-bit integer or double-precision float type are safe for storing this identifier.
    [<DataMember(Name = "migrate_from_chat_id")>]
    MigrateFromChatId: int64 option
    /// Specified message was pinned. Note that the Message object in this field will not contain further reply_to_message fields even if it itself is a reply.
    [<DataMember(Name = "pinned_message")>]
    PinnedMessage: MaybeInaccessibleMessage option
    /// Message is an invoice for a payment, information about the invoice. More about payments »
    [<DataMember(Name = "invoice")>]
    Invoice: Invoice option
    /// Message is a service message about a successful payment, information about the payment. More about payments »
    [<DataMember(Name = "successful_payment")>]
    SuccessfulPayment: SuccessfulPayment option
    /// Message is a service message about a refunded payment, information about the payment. More about payments »
    [<DataMember(Name = "refunded_payment")>]
    RefundedPayment: RefundedPayment option
    /// Service message: users were shared with the bot
    [<DataMember(Name = "users_shared")>]
    UsersShared: UsersShared option
    /// Service message: a chat was shared with the bot
    [<DataMember(Name = "chat_shared")>]
    ChatShared: ChatShared option
    /// The domain name of the website on which the user has logged in. More about Telegram Login »
    [<DataMember(Name = "connected_website")>]
    ConnectedWebsite: string option
    /// Service message: the user allowed the bot to write messages after adding it to the attachment or side menu, launching a Web App from a link, or accepting an explicit request from a Web App sent by the method requestWriteAccess
    [<DataMember(Name = "write_access_allowed")>]
    WriteAccessAllowed: WriteAccessAllowed option
    /// Telegram Passport data
    [<DataMember(Name = "passport_data")>]
    PassportData: PassportData option
    /// Service message. A user in the chat triggered another user's proximity alert while sharing Live Location.
    [<DataMember(Name = "proximity_alert_triggered")>]
    ProximityAlertTriggered: ProximityAlertTriggered option
    /// Service message: user boosted the chat
    [<DataMember(Name = "boost_added")>]
    BoostAdded: ChatBoostAdded option
    /// Service message: chat background set
    [<DataMember(Name = "chat_background_set")>]
    ChatBackgroundSet: ChatBackground option
    /// Service message: forum topic created
    [<DataMember(Name = "forum_topic_created")>]
    ForumTopicCreated: ForumTopicCreated option
    /// Service message: forum topic edited
    [<DataMember(Name = "forum_topic_edited")>]
    ForumTopicEdited: ForumTopicEdited option
    /// Service message: forum topic closed
    [<DataMember(Name = "forum_topic_closed")>]
    ForumTopicClosed: ForumTopicClosed option
    /// Service message: forum topic reopened
    [<DataMember(Name = "forum_topic_reopened")>]
    ForumTopicReopened: ForumTopicReopened option
    /// Service message: the 'General' forum topic hidden
    [<DataMember(Name = "general_forum_topic_hidden")>]
    GeneralForumTopicHidden: GeneralForumTopicHidden option
    /// Service message: the 'General' forum topic unhidden
    [<DataMember(Name = "general_forum_topic_unhidden")>]
    GeneralForumTopicUnhidden: GeneralForumTopicUnhidden option
    /// Service message: a scheduled giveaway was created
    [<DataMember(Name = "giveaway_created")>]
    GiveawayCreated: GiveawayCreated option
    /// The message is a scheduled giveaway message
    [<DataMember(Name = "giveaway")>]
    Giveaway: Giveaway option
    /// A giveaway with public winners was completed
    [<DataMember(Name = "giveaway_winners")>]
    GiveawayWinners: GiveawayWinners option
    /// Service message: a giveaway without public winners was completed
    [<DataMember(Name = "giveaway_completed")>]
    GiveawayCompleted: GiveawayCompleted option
    /// Service message: video chat scheduled
    [<DataMember(Name = "video_chat_scheduled")>]
    VideoChatScheduled: VideoChatScheduled option
    /// Service message: video chat started
    [<DataMember(Name = "video_chat_started")>]
    VideoChatStarted: VideoChatStarted option
    /// Service message: video chat ended
    [<DataMember(Name = "video_chat_ended")>]
    VideoChatEnded: VideoChatEnded option
    /// Service message: new participants invited to a video chat
    [<DataMember(Name = "video_chat_participants_invited")>]
    VideoChatParticipantsInvited: VideoChatParticipantsInvited option
    /// Service message: data sent by a Web App
    [<DataMember(Name = "web_app_data")>]
    WebAppData: WebAppData option
    /// Inline keyboard attached to the message. login_url buttons are represented as ordinary url buttons.
    [<DataMember(Name = "reply_markup")>]
    ReplyMarkup: InlineKeyboardMarkup option
  }
  static member Create(messageId: int64, date: DateTime, chat: Chat, ?usersShared: UsersShared, ?refundedPayment: RefundedPayment, ?successfulPayment: SuccessfulPayment, ?invoice: Invoice, ?pinnedMessage: MaybeInaccessibleMessage, ?migrateFromChatId: int64, ?migrateToChatId: int64, ?messageAutoDeleteTimerChanged: MessageAutoDeleteTimerChanged, ?supergroupChatCreated: bool, ?chatShared: ChatShared, ?groupChatCreated: bool, ?deleteChatPhoto: bool, ?newChatPhoto: PhotoSize[], ?newChatTitle: string, ?leftChatMember: User, ?newChatMembers: User[], ?location: Location, ?channelChatCreated: bool, ?connectedWebsite: string, ?writeAccessAllowed: WriteAccessAllowed, ?passportData: PassportData, ?videoChatParticipantsInvited: VideoChatParticipantsInvited, ?videoChatEnded: VideoChatEnded, ?videoChatStarted: VideoChatStarted, ?videoChatScheduled: VideoChatScheduled, ?giveawayCompleted: GiveawayCompleted, ?giveawayWinners: GiveawayWinners, ?giveaway: Giveaway, ?giveawayCreated: GiveawayCreated, ?generalForumTopicUnhidden: GeneralForumTopicUnhidden, ?generalForumTopicHidden: GeneralForumTopicHidden, ?forumTopicReopened: ForumTopicReopened, ?forumTopicClosed: ForumTopicClosed, ?forumTopicEdited: ForumTopicEdited, ?forumTopicCreated: ForumTopicCreated, ?chatBackgroundSet: ChatBackground, ?boostAdded: ChatBoostAdded, ?proximityAlertTriggered: ProximityAlertTriggered, ?venue: Venue, ?poll: Poll, ?game: Game, ?dice: Dice, ?isFromOffline: bool, ?hasProtectedContent: bool, ?editDate: int64, ?viaBot: User, ?replyToStory: Story, ?quote: TextQuote, ?externalReply: ExternalReplyInfo, ?replyToMessage: Message, ?isAutomaticForward: bool, ?isTopicMessage: bool, ?forwardOrigin: MessageOrigin, ?businessConnectionId: string, ?senderBusinessBot: User, ?senderBoostCount: int64, ?senderChat: Chat, ?from: User, ?messageThreadId: int64, ?mediaGroupId: string, ?webAppData: WebAppData, ?authorSignature: string, ?entities: MessageEntity[], ?contact: Contact, ?hasMediaSpoiler: bool, ?showCaptionAboveMedia: bool, ?captionEntities: MessageEntity[], ?caption: string, ?voice: Voice, ?videoNote: VideoNote, ?video: Video, ?story: Story, ?sticker: Sticker, ?photo: PhotoSize[], ?paidMedia: PaidMediaInfo, ?document: Document, ?audio: Audio, ?animation: Animation, ?effectId: string, ?linkPreviewOptions: LinkPreviewOptions, ?text: string, ?replyMarkup: InlineKeyboardMarkup) = 
    {
      MessageId = messageId
      Date = date
      Chat = chat
      UsersShared = usersShared
      RefundedPayment = refundedPayment
      SuccessfulPayment = successfulPayment
      Invoice = invoice
      PinnedMessage = pinnedMessage
      MigrateFromChatId = migrateFromChatId
      MigrateToChatId = migrateToChatId
      MessageAutoDeleteTimerChanged = messageAutoDeleteTimerChanged
      SupergroupChatCreated = supergroupChatCreated
      ChatShared = chatShared
      GroupChatCreated = groupChatCreated
      DeleteChatPhoto = deleteChatPhoto
      NewChatPhoto = newChatPhoto
      NewChatTitle = newChatTitle
      LeftChatMember = leftChatMember
      NewChatMembers = newChatMembers
      Location = location
      ChannelChatCreated = channelChatCreated
      ConnectedWebsite = connectedWebsite
      WriteAccessAllowed = writeAccessAllowed
      PassportData = passportData
      VideoChatParticipantsInvited = videoChatParticipantsInvited
      VideoChatEnded = videoChatEnded
      VideoChatStarted = videoChatStarted
      VideoChatScheduled = videoChatScheduled
      GiveawayCompleted = giveawayCompleted
      GiveawayWinners = giveawayWinners
      Giveaway = giveaway
      GiveawayCreated = giveawayCreated
      GeneralForumTopicUnhidden = generalForumTopicUnhidden
      GeneralForumTopicHidden = generalForumTopicHidden
      ForumTopicReopened = forumTopicReopened
      ForumTopicClosed = forumTopicClosed
      ForumTopicEdited = forumTopicEdited
      ForumTopicCreated = forumTopicCreated
      ChatBackgroundSet = chatBackgroundSet
      BoostAdded = boostAdded
      ProximityAlertTriggered = proximityAlertTriggered
      Venue = venue
      Poll = poll
      Game = game
      Dice = dice
      IsFromOffline = isFromOffline
      HasProtectedContent = hasProtectedContent
      EditDate = editDate
      ViaBot = viaBot
      ReplyToStory = replyToStory
      Quote = quote
      ExternalReply = externalReply
      ReplyToMessage = replyToMessage
      IsAutomaticForward = isAutomaticForward
      IsTopicMessage = isTopicMessage
      ForwardOrigin = forwardOrigin
      BusinessConnectionId = businessConnectionId
      SenderBusinessBot = senderBusinessBot
      SenderBoostCount = senderBoostCount
      SenderChat = senderChat
      From = from
      MessageThreadId = messageThreadId
      MediaGroupId = mediaGroupId
      WebAppData = webAppData
      AuthorSignature = authorSignature
      Entities = entities
      Contact = contact
      HasMediaSpoiler = hasMediaSpoiler
      ShowCaptionAboveMedia = showCaptionAboveMedia
      CaptionEntities = captionEntities
      Caption = caption
      Voice = voice
      VideoNote = videoNote
      Video = video
      Story = story
      Sticker = sticker
      Photo = photo
      PaidMedia = paidMedia
      Document = document
      Audio = audio
      Animation = animation
      EffectId = effectId
      LinkPreviewOptions = linkPreviewOptions
      Text = text
      ReplyMarkup = replyMarkup
    }

/// This object represents a unique message identifier.
and [<CLIMutable>] MessageId =
  {
    /// Unique message identifier
    [<DataMember(Name = "message_id")>]
    MessageId: int64
  }
  static member Create(messageId: int64) = 
    {
      MessageId = messageId
    }

/// This object describes a message that was deleted or is otherwise inaccessible to the bot.
and [<CLIMutable>] InaccessibleMessage =
  {
    /// Chat the message belonged to
    [<DataMember(Name = "chat")>]
    Chat: Chat
    /// Unique message identifier inside the chat
    [<DataMember(Name = "message_id")>]
    MessageId: int64
    /// Always 0. The field can be used to differentiate regular and inaccessible messages.
    [<DataMember(Name = "date")>]
    Date: DateTime
  }
  static member Create(chat: Chat, messageId: int64, date: DateTime) = 
    {
      Chat = chat
      MessageId = messageId
      Date = date
    }

/// This object describes a message that can be inaccessible to the bot. It can be one of
and MaybeInaccessibleMessage =
  | Message of Message
  | InaccessibleMessage of InaccessibleMessage

/// This object represents one special entity in a text message. For example, hashtags, usernames, URLs, etc.
and [<CLIMutable>] MessageEntity =
  {
    /// Type of the entity. Currently, can be “mention” (@username), “hashtag” (#hashtag), “cashtag” ($USD), “bot_command” (/start@jobs_bot), “url” (https://telegram.org), “email” (do-not-reply@telegram.org), “phone_number” (+1-212-555-0123), “bold” (bold text), “italic” (italic text), “underline” (underlined text), “strikethrough” (strikethrough text), “spoiler” (spoiler message), “blockquote” (block quotation), “expandable_blockquote” (collapsed-by-default block quotation), “code” (monowidth string), “pre” (monowidth block), “text_link” (for clickable text URLs), “text_mention” (for users without usernames), “custom_emoji” (for inline custom emoji stickers)
    [<DataMember(Name = "type")>]
    Type: string
    /// Offset in UTF-16 code units to the start of the entity
    [<DataMember(Name = "offset")>]
    Offset: int64
    /// Length of the entity in UTF-16 code units
    [<DataMember(Name = "length")>]
    Length: int64
    /// For “text_link” only, URL that will be opened after user taps on the text
    [<DataMember(Name = "url")>]
    Url: string option
    /// For “text_mention” only, the mentioned user
    [<DataMember(Name = "user")>]
    User: User option
    /// For “pre” only, the programming language of the entity text
    [<DataMember(Name = "language")>]
    Language: string option
    /// For “custom_emoji” only, unique identifier of the custom emoji. Use getCustomEmojiStickers to get full information about the sticker
    [<DataMember(Name = "custom_emoji_id")>]
    CustomEmojiId: string option
  }
  static member Create(``type``: string, offset: int64, length: int64, ?url: string, ?user: User, ?language: string, ?customEmojiId: string) = 
    {
      Type = ``type``
      Offset = offset
      Length = length
      Url = url
      User = user
      Language = language
      CustomEmojiId = customEmojiId
    }

/// This object contains information about the quoted part of a message that is replied to by the given message.
and [<CLIMutable>] TextQuote =
  {
    /// Text of the quoted part of a message that is replied to by the given message
    [<DataMember(Name = "text")>]
    Text: string
    /// Special entities that appear in the quote. Currently, only bold, italic, underline, strikethrough, spoiler, and custom_emoji entities are kept in quotes.
    [<DataMember(Name = "entities")>]
    Entities: MessageEntity[] option
    /// Approximate quote position in the original message in UTF-16 code units as specified by the sender
    [<DataMember(Name = "position")>]
    Position: int64
    /// True, if the quote was chosen manually by the message sender. Otherwise, the quote was added automatically by the server.
    [<DataMember(Name = "is_manual")>]
    IsManual: bool option
  }
  static member Create(text: string, position: int64, ?entities: MessageEntity[], ?isManual: bool) = 
    {
      Text = text
      Position = position
      Entities = entities
      IsManual = isManual
    }

/// This object contains information about a message that is being replied to, which may come from another chat or forum topic.
and [<CLIMutable>] ExternalReplyInfo =
  {
    /// Origin of the message replied to by the given message
    [<DataMember(Name = "origin")>]
    Origin: MessageOrigin
    /// Chat the original message belongs to. Available only if the chat is a supergroup or a channel.
    [<DataMember(Name = "chat")>]
    Chat: Chat option
    /// Unique message identifier inside the original chat. Available only if the original chat is a supergroup or a channel.
    [<DataMember(Name = "message_id")>]
    MessageId: int64 option
    /// Options used for link preview generation for the original message, if it is a text message
    [<DataMember(Name = "link_preview_options")>]
    LinkPreviewOptions: LinkPreviewOptions option
    /// Message is an animation, information about the animation
    [<DataMember(Name = "animation")>]
    Animation: Animation option
    /// Message is an audio file, information about the file
    [<DataMember(Name = "audio")>]
    Audio: Audio option
    /// Message is a general file, information about the file
    [<DataMember(Name = "document")>]
    Document: Document option
    /// Message contains paid media; information about the paid media
    [<DataMember(Name = "paid_media")>]
    PaidMedia: PaidMediaInfo option
    /// Message is a photo, available sizes of the photo
    [<DataMember(Name = "photo")>]
    Photo: PhotoSize[] option
    /// Message is a sticker, information about the sticker
    [<DataMember(Name = "sticker")>]
    Sticker: Sticker option
    /// Message is a forwarded story
    [<DataMember(Name = "story")>]
    Story: Story option
    /// Message is a video, information about the video
    [<DataMember(Name = "video")>]
    Video: Video option
    /// Message is a video note, information about the video message
    [<DataMember(Name = "video_note")>]
    VideoNote: VideoNote option
    /// Message is a voice message, information about the file
    [<DataMember(Name = "voice")>]
    Voice: Voice option
    /// True, if the message media is covered by a spoiler animation
    [<DataMember(Name = "has_media_spoiler")>]
    HasMediaSpoiler: bool option
    /// Message is a shared contact, information about the contact
    [<DataMember(Name = "contact")>]
    Contact: Contact option
    /// Message is a dice with random value
    [<DataMember(Name = "dice")>]
    Dice: Dice option
    /// Message is a game, information about the game. More about games »
    [<DataMember(Name = "game")>]
    Game: Game option
    /// Message is a scheduled giveaway, information about the giveaway
    [<DataMember(Name = "giveaway")>]
    Giveaway: Giveaway option
    /// A giveaway with public winners was completed
    [<DataMember(Name = "giveaway_winners")>]
    GiveawayWinners: GiveawayWinners option
    /// Message is an invoice for a payment, information about the invoice. More about payments »
    [<DataMember(Name = "invoice")>]
    Invoice: Invoice option
    /// Message is a shared location, information about the location
    [<DataMember(Name = "location")>]
    Location: Location option
    /// Message is a native poll, information about the poll
    [<DataMember(Name = "poll")>]
    Poll: Poll option
    /// Message is a venue, information about the venue
    [<DataMember(Name = "venue")>]
    Venue: Venue option
  }
  static member Create(origin: MessageOrigin, ?location: Location, ?invoice: Invoice, ?giveawayWinners: GiveawayWinners, ?giveaway: Giveaway, ?game: Game, ?dice: Dice, ?contact: Contact, ?hasMediaSpoiler: bool, ?voice: Voice, ?videoNote: VideoNote, ?video: Video, ?story: Story, ?sticker: Sticker, ?photo: PhotoSize[], ?paidMedia: PaidMediaInfo, ?document: Document, ?audio: Audio, ?animation: Animation, ?linkPreviewOptions: LinkPreviewOptions, ?messageId: int64, ?chat: Chat, ?poll: Poll, ?venue: Venue) = 
    {
      Origin = origin
      Location = location
      Invoice = invoice
      GiveawayWinners = giveawayWinners
      Giveaway = giveaway
      Game = game
      Dice = dice
      Contact = contact
      HasMediaSpoiler = hasMediaSpoiler
      Voice = voice
      VideoNote = videoNote
      Video = video
      Story = story
      Sticker = sticker
      Photo = photo
      PaidMedia = paidMedia
      Document = document
      Audio = audio
      Animation = animation
      LinkPreviewOptions = linkPreviewOptions
      MessageId = messageId
      Chat = chat
      Poll = poll
      Venue = venue
    }

/// Describes reply parameters for the message that is being sent.
and [<CLIMutable>] ReplyParameters =
  {
    /// Identifier of the message that will be replied to in the current chat, or in the chat chat_id if it is specified
    [<DataMember(Name = "message_id")>]
    MessageId: int64
    /// If the message to be replied to is from a different chat, unique identifier for the chat or username of the channel (in the format @channelusername). Not supported for messages sent on behalf of a business account.
    [<DataMember(Name = "chat_id")>]
    ChatId: ChatId option
    /// Pass True if the message should be sent even if the specified message to be replied to is not found. Always False for replies in another chat or forum topic. Always True for messages sent on behalf of a business account.
    [<DataMember(Name = "allow_sending_without_reply")>]
    AllowSendingWithoutReply: bool option
    /// Quoted part of the message to be replied to; 0-1024 characters after entities parsing. The quote must be an exact substring of the message to be replied to, including bold, italic, underline, strikethrough, spoiler, and custom_emoji entities. The message will fail to send if the quote isn't found in the original message.
    [<DataMember(Name = "quote")>]
    Quote: string option
    /// Mode for parsing entities in the quote. See formatting options for more details.
    [<DataMember(Name = "quote_parse_mode")>]
    QuoteParseMode: string option
    /// A JSON-serialized list of special entities that appear in the quote. It can be specified instead of quote_parse_mode.
    [<DataMember(Name = "quote_entities")>]
    QuoteEntities: MessageEntity[] option
    /// Position of the quote in the original message in UTF-16 code units
    [<DataMember(Name = "quote_position")>]
    QuotePosition: int64 option
  }
  static member Create(messageId: int64, ?chatId: ChatId, ?allowSendingWithoutReply: bool, ?quote: string, ?quoteParseMode: string, ?quoteEntities: MessageEntity[], ?quotePosition: int64) = 
    {
      MessageId = messageId
      ChatId = chatId
      AllowSendingWithoutReply = allowSendingWithoutReply
      Quote = quote
      QuoteParseMode = quoteParseMode
      QuoteEntities = quoteEntities
      QuotePosition = quotePosition
    }

/// This object describes the origin of a message. It can be one of
and MessageOrigin =
  | User of MessageOriginUser
  | HiddenUser of MessageOriginHiddenUser
  | Chat of MessageOriginChat
  | Channel of MessageOriginChannel

/// The message was originally sent by a known user.
and [<CLIMutable>] MessageOriginUser =
  {
    /// Type of the message origin, always “user”
    [<DataMember(Name = "type")>]
    Type: string
    /// Date the message was sent originally in Unix time
    [<DataMember(Name = "date")>]
    Date: DateTime
    /// User that sent the message originally
    [<DataMember(Name = "sender_user")>]
    SenderUser: User
  }
  static member Create(``type``: string, date: DateTime, senderUser: User) = 
    {
      Type = ``type``
      Date = date
      SenderUser = senderUser
    }

/// The message was originally sent by an unknown user.
and [<CLIMutable>] MessageOriginHiddenUser =
  {
    /// Type of the message origin, always “hidden_user”
    [<DataMember(Name = "type")>]
    Type: string
    /// Date the message was sent originally in Unix time
    [<DataMember(Name = "date")>]
    Date: DateTime
    /// Name of the user that sent the message originally
    [<DataMember(Name = "sender_user_name")>]
    SenderUserName: string
  }
  static member Create(``type``: string, date: DateTime, senderUserName: string) = 
    {
      Type = ``type``
      Date = date
      SenderUserName = senderUserName
    }

/// The message was originally sent on behalf of a chat to a group chat.
and [<CLIMutable>] MessageOriginChat =
  {
    /// Type of the message origin, always “chat”
    [<DataMember(Name = "type")>]
    Type: string
    /// Date the message was sent originally in Unix time
    [<DataMember(Name = "date")>]
    Date: DateTime
    /// Chat that sent the message originally
    [<DataMember(Name = "sender_chat")>]
    SenderChat: Chat
    /// For messages originally sent by an anonymous chat administrator, original message author signature
    [<DataMember(Name = "author_signature")>]
    AuthorSignature: string option
  }
  static member Create(``type``: string, date: DateTime, senderChat: Chat, ?authorSignature: string) = 
    {
      Type = ``type``
      Date = date
      SenderChat = senderChat
      AuthorSignature = authorSignature
    }

/// The message was originally sent to a channel chat.
and [<CLIMutable>] MessageOriginChannel =
  {
    /// Type of the message origin, always “channel”
    [<DataMember(Name = "type")>]
    Type: string
    /// Date the message was sent originally in Unix time
    [<DataMember(Name = "date")>]
    Date: DateTime
    /// Channel chat to which the message was originally sent
    [<DataMember(Name = "chat")>]
    Chat: Chat
    /// Unique message identifier inside the chat
    [<DataMember(Name = "message_id")>]
    MessageId: int64
    /// Signature of the original post author
    [<DataMember(Name = "author_signature")>]
    AuthorSignature: string option
  }
  static member Create(``type``: string, date: DateTime, chat: Chat, messageId: int64, ?authorSignature: string) = 
    {
      Type = ``type``
      Date = date
      Chat = chat
      MessageId = messageId
      AuthorSignature = authorSignature
    }

/// This object represents one size of a photo or a file / sticker thumbnail.
and [<CLIMutable>] PhotoSize =
  {
    /// Identifier for this file, which can be used to download or reuse the file
    [<DataMember(Name = "file_id")>]
    FileId: string
    /// Unique identifier for this file, which is supposed to be the same over time and for different bots. Can't be used to download or reuse the file.
    [<DataMember(Name = "file_unique_id")>]
    FileUniqueId: string
    /// Photo width
    [<DataMember(Name = "width")>]
    Width: int64
    /// Photo height
    [<DataMember(Name = "height")>]
    Height: int64
    /// File size in bytes
    [<DataMember(Name = "file_size")>]
    FileSize: int64 option
  }
  static member Create(fileId: string, fileUniqueId: string, width: int64, height: int64, ?fileSize: int64) = 
    {
      FileId = fileId
      FileUniqueId = fileUniqueId
      Width = width
      Height = height
      FileSize = fileSize
    }

/// This object represents an animation file (GIF or H.264/MPEG-4 AVC video without sound).
and [<CLIMutable>] Animation =
  {
    /// Identifier for this file, which can be used to download or reuse the file
    [<DataMember(Name = "file_id")>]
    FileId: string
    /// Unique identifier for this file, which is supposed to be the same over time and for different bots. Can't be used to download or reuse the file.
    [<DataMember(Name = "file_unique_id")>]
    FileUniqueId: string
    /// Video width as defined by the sender
    [<DataMember(Name = "width")>]
    Width: int64
    /// Video height as defined by the sender
    [<DataMember(Name = "height")>]
    Height: int64
    /// Duration of the video in seconds as defined by the sender
    [<DataMember(Name = "duration")>]
    Duration: int64
    /// Animation thumbnail as defined by the sender
    [<DataMember(Name = "thumbnail")>]
    Thumbnail: PhotoSize option
    /// Original animation filename as defined by the sender
    [<DataMember(Name = "file_name")>]
    FileName: string option
    /// MIME type of the file as defined by the sender
    [<DataMember(Name = "mime_type")>]
    MimeType: string option
    /// File size in bytes. It can be bigger than 2^31 and some programming languages may have difficulty/silent defects in interpreting it. But it has at most 52 significant bits, so a signed 64-bit integer or double-precision float type are safe for storing this value.
    [<DataMember(Name = "file_size")>]
    FileSize: int64 option
  }
  static member Create(fileId: string, fileUniqueId: string, width: int64, height: int64, duration: int64, ?thumbnail: PhotoSize, ?fileName: string, ?mimeType: string, ?fileSize: int64) = 
    {
      FileId = fileId
      FileUniqueId = fileUniqueId
      Width = width
      Height = height
      Duration = duration
      Thumbnail = thumbnail
      FileName = fileName
      MimeType = mimeType
      FileSize = fileSize
    }

/// This object represents an audio file to be treated as music by the Telegram clients.
and [<CLIMutable>] Audio =
  {
    /// Identifier for this file, which can be used to download or reuse the file
    [<DataMember(Name = "file_id")>]
    FileId: string
    /// Unique identifier for this file, which is supposed to be the same over time and for different bots. Can't be used to download or reuse the file.
    [<DataMember(Name = "file_unique_id")>]
    FileUniqueId: string
    /// Duration of the audio in seconds as defined by the sender
    [<DataMember(Name = "duration")>]
    Duration: int64
    /// Performer of the audio as defined by the sender or by audio tags
    [<DataMember(Name = "performer")>]
    Performer: string option
    /// Title of the audio as defined by the sender or by audio tags
    [<DataMember(Name = "title")>]
    Title: string option
    /// Original filename as defined by the sender
    [<DataMember(Name = "file_name")>]
    FileName: string option
    /// MIME type of the file as defined by the sender
    [<DataMember(Name = "mime_type")>]
    MimeType: string option
    /// File size in bytes. It can be bigger than 2^31 and some programming languages may have difficulty/silent defects in interpreting it. But it has at most 52 significant bits, so a signed 64-bit integer or double-precision float type are safe for storing this value.
    [<DataMember(Name = "file_size")>]
    FileSize: int64 option
    /// Thumbnail of the album cover to which the music file belongs
    [<DataMember(Name = "thumbnail")>]
    Thumbnail: PhotoSize option
  }
  static member Create(fileId: string, fileUniqueId: string, duration: int64, ?performer: string, ?title: string, ?fileName: string, ?mimeType: string, ?fileSize: int64, ?thumbnail: PhotoSize) = 
    {
      FileId = fileId
      FileUniqueId = fileUniqueId
      Duration = duration
      Performer = performer
      Title = title
      FileName = fileName
      MimeType = mimeType
      FileSize = fileSize
      Thumbnail = thumbnail
    }

/// This object represents a general file (as opposed to photos, voice messages and audio files).
and [<CLIMutable>] Document =
  {
    /// Identifier for this file, which can be used to download or reuse the file
    [<DataMember(Name = "file_id")>]
    FileId: string
    /// Unique identifier for this file, which is supposed to be the same over time and for different bots. Can't be used to download or reuse the file.
    [<DataMember(Name = "file_unique_id")>]
    FileUniqueId: string
    /// Document thumbnail as defined by the sender
    [<DataMember(Name = "thumbnail")>]
    Thumbnail: PhotoSize option
    /// Original filename as defined by the sender
    [<DataMember(Name = "file_name")>]
    FileName: string option
    /// MIME type of the file as defined by the sender
    [<DataMember(Name = "mime_type")>]
    MimeType: string option
    /// File size in bytes. It can be bigger than 2^31 and some programming languages may have difficulty/silent defects in interpreting it. But it has at most 52 significant bits, so a signed 64-bit integer or double-precision float type are safe for storing this value.
    [<DataMember(Name = "file_size")>]
    FileSize: int64 option
  }
  static member Create(fileId: string, fileUniqueId: string, ?thumbnail: PhotoSize, ?fileName: string, ?mimeType: string, ?fileSize: int64) = 
    {
      FileId = fileId
      FileUniqueId = fileUniqueId
      Thumbnail = thumbnail
      FileName = fileName
      MimeType = mimeType
      FileSize = fileSize
    }

/// This object represents a story.
and [<CLIMutable>] Story =
  {
    /// Chat that posted the story
    [<DataMember(Name = "chat")>]
    Chat: Chat
    /// Unique identifier for the story in the chat
    [<DataMember(Name = "id")>]
    Id: int64
  }
  static member Create(chat: Chat, id: int64) = 
    {
      Chat = chat
      Id = id
    }

/// This object represents a video file.
and [<CLIMutable>] Video =
  {
    /// Identifier for this file, which can be used to download or reuse the file
    [<DataMember(Name = "file_id")>]
    FileId: string
    /// Unique identifier for this file, which is supposed to be the same over time and for different bots. Can't be used to download or reuse the file.
    [<DataMember(Name = "file_unique_id")>]
    FileUniqueId: string
    /// Video width as defined by the sender
    [<DataMember(Name = "width")>]
    Width: int64
    /// Video height as defined by the sender
    [<DataMember(Name = "height")>]
    Height: int64
    /// Duration of the video in seconds as defined by the sender
    [<DataMember(Name = "duration")>]
    Duration: int64
    /// Video thumbnail
    [<DataMember(Name = "thumbnail")>]
    Thumbnail: PhotoSize option
    /// Original filename as defined by the sender
    [<DataMember(Name = "file_name")>]
    FileName: string option
    /// MIME type of the file as defined by the sender
    [<DataMember(Name = "mime_type")>]
    MimeType: string option
    /// File size in bytes. It can be bigger than 2^31 and some programming languages may have difficulty/silent defects in interpreting it. But it has at most 52 significant bits, so a signed 64-bit integer or double-precision float type are safe for storing this value.
    [<DataMember(Name = "file_size")>]
    FileSize: int64 option
  }
  static member Create(fileId: string, fileUniqueId: string, width: int64, height: int64, duration: int64, ?thumbnail: PhotoSize, ?fileName: string, ?mimeType: string, ?fileSize: int64) = 
    {
      FileId = fileId
      FileUniqueId = fileUniqueId
      Width = width
      Height = height
      Duration = duration
      Thumbnail = thumbnail
      FileName = fileName
      MimeType = mimeType
      FileSize = fileSize
    }

/// This object represents a video message (available in Telegram apps as of v.4.0).
and [<CLIMutable>] VideoNote =
  {
    /// Identifier for this file, which can be used to download or reuse the file
    [<DataMember(Name = "file_id")>]
    FileId: string
    /// Unique identifier for this file, which is supposed to be the same over time and for different bots. Can't be used to download or reuse the file.
    [<DataMember(Name = "file_unique_id")>]
    FileUniqueId: string
    /// Video width and height (diameter of the video message) as defined by the sender
    [<DataMember(Name = "length")>]
    Length: int64
    /// Duration of the video in seconds as defined by the sender
    [<DataMember(Name = "duration")>]
    Duration: int64
    /// Video thumbnail
    [<DataMember(Name = "thumbnail")>]
    Thumbnail: PhotoSize option
    /// File size in bytes
    [<DataMember(Name = "file_size")>]
    FileSize: int64 option
  }
  static member Create(fileId: string, fileUniqueId: string, length: int64, duration: int64, ?thumbnail: PhotoSize, ?fileSize: int64) = 
    {
      FileId = fileId
      FileUniqueId = fileUniqueId
      Length = length
      Duration = duration
      Thumbnail = thumbnail
      FileSize = fileSize
    }

/// This object represents a voice note.
and [<CLIMutable>] Voice =
  {
    /// Identifier for this file, which can be used to download or reuse the file
    [<DataMember(Name = "file_id")>]
    FileId: string
    /// Unique identifier for this file, which is supposed to be the same over time and for different bots. Can't be used to download or reuse the file.
    [<DataMember(Name = "file_unique_id")>]
    FileUniqueId: string
    /// Duration of the audio in seconds as defined by the sender
    [<DataMember(Name = "duration")>]
    Duration: int64
    /// MIME type of the file as defined by the sender
    [<DataMember(Name = "mime_type")>]
    MimeType: string option
    /// File size in bytes. It can be bigger than 2^31 and some programming languages may have difficulty/silent defects in interpreting it. But it has at most 52 significant bits, so a signed 64-bit integer or double-precision float type are safe for storing this value.
    [<DataMember(Name = "file_size")>]
    FileSize: int64 option
  }
  static member Create(fileId: string, fileUniqueId: string, duration: int64, ?mimeType: string, ?fileSize: int64) = 
    {
      FileId = fileId
      FileUniqueId = fileUniqueId
      Duration = duration
      MimeType = mimeType
      FileSize = fileSize
    }

/// Describes the paid media added to a message.
and [<CLIMutable>] PaidMediaInfo =
  {
    /// The number of Telegram Stars that must be paid to buy access to the media
    [<DataMember(Name = "star_count")>]
    StarCount: int64
    /// Information about the paid media
    [<DataMember(Name = "paid_media")>]
    PaidMedia: PaidMedia[]
  }
  static member Create(starCount: int64, paidMedia: PaidMedia[]) = 
    {
      StarCount = starCount
      PaidMedia = paidMedia
    }

/// This object describes paid media. Currently, it can be one of
and PaidMedia =
  | Preview of PaidMediaPreview
  | Photo of PaidMediaPhoto
  | Video of PaidMediaVideo

/// The paid media isn't available before the payment.
and [<CLIMutable>] PaidMediaPreview =
  {
    /// Type of the paid media, always “preview”
    [<DataMember(Name = "type")>]
    Type: string
    /// Media width as defined by the sender
    [<DataMember(Name = "width")>]
    Width: int64 option
    /// Media height as defined by the sender
    [<DataMember(Name = "height")>]
    Height: int64 option
    /// Duration of the media in seconds as defined by the sender
    [<DataMember(Name = "duration")>]
    Duration: int64 option
  }
  static member Create(``type``: string, ?width: int64, ?height: int64, ?duration: int64) = 
    {
      Type = ``type``
      Width = width
      Height = height
      Duration = duration
    }

/// The paid media is a photo.
and [<CLIMutable>] PaidMediaPhoto =
  {
    /// Type of the paid media, always “photo”
    [<DataMember(Name = "type")>]
    Type: string
    /// The photo
    [<DataMember(Name = "photo")>]
    Photo: PhotoSize[]
  }
  static member Create(``type``: string, photo: PhotoSize[]) = 
    {
      Type = ``type``
      Photo = photo
    }

/// The paid media is a video.
and [<CLIMutable>] PaidMediaVideo =
  {
    /// Type of the paid media, always “video”
    [<DataMember(Name = "type")>]
    Type: string
    /// The video
    [<DataMember(Name = "video")>]
    Video: Video
  }
  static member Create(``type``: string, video: Video) = 
    {
      Type = ``type``
      Video = video
    }

/// This object represents a phone contact.
and [<CLIMutable>] Contact =
  {
    /// Contact's phone number
    [<DataMember(Name = "phone_number")>]
    PhoneNumber: string
    /// Contact's first name
    [<DataMember(Name = "first_name")>]
    FirstName: string
    /// Contact's last name
    [<DataMember(Name = "last_name")>]
    LastName: string option
    /// Contact's user identifier in Telegram. This number may have more than 32 significant bits and some programming languages may have difficulty/silent defects in interpreting it. But it has at most 52 significant bits, so a 64-bit integer or double-precision float type are safe for storing this identifier.
    [<DataMember(Name = "user_id")>]
    UserId: int64 option
    /// Additional data about the contact in the form of a vCard
    [<DataMember(Name = "vcard")>]
    Vcard: string option
  }
  static member Create(phoneNumber: string, firstName: string, ?lastName: string, ?userId: int64, ?vcard: string) = 
    {
      PhoneNumber = phoneNumber
      FirstName = firstName
      LastName = lastName
      UserId = userId
      Vcard = vcard
    }

/// This object represents an animated emoji that displays a random value.
and [<CLIMutable>] Dice =
  {
    /// Emoji on which the dice throw animation is based
    [<DataMember(Name = "emoji")>]
    Emoji: string
    /// Value of the dice, 1-6 for “”, “” and “” base emoji, 1-5 for “” and “” base emoji, 1-64 for “” base emoji
    [<DataMember(Name = "value")>]
    Value: int64
  }
  static member Create(emoji: string, value: int64) = 
    {
      Emoji = emoji
      Value = value
    }

/// This object contains information about one answer option in a poll.
and [<CLIMutable>] PollOption =
  {
    /// Option text, 1-100 characters
    [<DataMember(Name = "text")>]
    Text: string
    /// Special entities that appear in the option text. Currently, only custom emoji entities are allowed in poll option texts
    [<DataMember(Name = "text_entities")>]
    TextEntities: MessageEntity[] option
    /// Number of users that voted for this option
    [<DataMember(Name = "voter_count")>]
    VoterCount: int64
  }
  static member Create(text: string, voterCount: int64, ?textEntities: MessageEntity[]) = 
    {
      Text = text
      VoterCount = voterCount
      TextEntities = textEntities
    }

/// This object contains information about one answer option in a poll to be sent.
and [<CLIMutable>] InputPollOption =
  {
    /// Option text, 1-100 characters
    [<DataMember(Name = "text")>]
    Text: string
    /// Mode for parsing entities in the text. See formatting options for more details. Currently, only custom emoji entities are allowed
    [<DataMember(Name = "text_parse_mode")>]
    TextParseMode: string option
    /// A JSON-serialized list of special entities that appear in the poll option text. It can be specified instead of text_parse_mode
    [<DataMember(Name = "text_entities")>]
    TextEntities: MessageEntity[] option
  }
  static member Create(text: string, ?textParseMode: string, ?textEntities: MessageEntity[]) = 
    {
      Text = text
      TextParseMode = textParseMode
      TextEntities = textEntities
    }

/// This object represents an answer of a user in a non-anonymous poll.
and [<CLIMutable>] PollAnswer =
  {
    /// Unique poll identifier
    [<DataMember(Name = "poll_id")>]
    PollId: string
    /// The chat that changed the answer to the poll, if the voter is anonymous
    [<DataMember(Name = "voter_chat")>]
    VoterChat: Chat option
    /// The user that changed the answer to the poll, if the voter isn't anonymous
    [<DataMember(Name = "user")>]
    User: User option
    /// 0-based identifiers of chosen answer options. May be empty if the vote was retracted.
    [<DataMember(Name = "option_ids")>]
    OptionIds: int64[]
  }
  static member Create(pollId: string, optionIds: int64[], ?voterChat: Chat, ?user: User) = 
    {
      PollId = pollId
      OptionIds = optionIds
      VoterChat = voterChat
      User = user
    }

/// This object contains information about a poll.
and [<CLIMutable>] Poll =
  {
    /// Unique poll identifier
    [<DataMember(Name = "id")>]
    Id: string
    /// Poll question, 1-300 characters
    [<DataMember(Name = "question")>]
    Question: string
    /// Special entities that appear in the question. Currently, only custom emoji entities are allowed in poll questions
    [<DataMember(Name = "question_entities")>]
    QuestionEntities: MessageEntity[] option
    /// List of poll options
    [<DataMember(Name = "options")>]
    Options: PollOption[]
    /// Total number of users that voted in the poll
    [<DataMember(Name = "total_voter_count")>]
    TotalVoterCount: int64
    /// True, if the poll is closed
    [<DataMember(Name = "is_closed")>]
    IsClosed: bool
    /// True, if the poll is anonymous
    [<DataMember(Name = "is_anonymous")>]
    IsAnonymous: bool
    /// Poll type, currently can be “regular” or “quiz”
    [<DataMember(Name = "type")>]
    Type: string
    /// True, if the poll allows multiple answers
    [<DataMember(Name = "allows_multiple_answers")>]
    AllowsMultipleAnswers: bool
    /// 0-based identifier of the correct answer option. Available only for polls in the quiz mode, which are closed, or was sent (not forwarded) by the bot or to the private chat with the bot.
    [<DataMember(Name = "correct_option_id")>]
    CorrectOptionId: int64 option
    /// Text that is shown when a user chooses an incorrect answer or taps on the lamp icon in a quiz-style poll, 0-200 characters
    [<DataMember(Name = "explanation")>]
    Explanation: string option
    /// Special entities like usernames, URLs, bot commands, etc. that appear in the explanation
    [<DataMember(Name = "explanation_entities")>]
    ExplanationEntities: MessageEntity[] option
    /// Amount of time in seconds the poll will be active after creation
    [<DataMember(Name = "open_period")>]
    OpenPeriod: int64 option
    /// Point in time (Unix timestamp) when the poll will be automatically closed
    [<DataMember(Name = "close_date")>]
    CloseDate: int64 option
  }
  static member Create(id: string, question: string, options: PollOption[], totalVoterCount: int64, isClosed: bool, isAnonymous: bool, ``type``: string, allowsMultipleAnswers: bool, ?questionEntities: MessageEntity[], ?correctOptionId: int64, ?explanation: string, ?explanationEntities: MessageEntity[], ?openPeriod: int64, ?closeDate: int64) = 
    {
      Id = id
      Question = question
      Options = options
      TotalVoterCount = totalVoterCount
      IsClosed = isClosed
      IsAnonymous = isAnonymous
      Type = ``type``
      AllowsMultipleAnswers = allowsMultipleAnswers
      QuestionEntities = questionEntities
      CorrectOptionId = correctOptionId
      Explanation = explanation
      ExplanationEntities = explanationEntities
      OpenPeriod = openPeriod
      CloseDate = closeDate
    }

/// This object represents a point on the map.
and [<CLIMutable>] Location =
  {
    /// Latitude as defined by the sender
    [<DataMember(Name = "latitude")>]
    Latitude: float
    /// Longitude as defined by the sender
    [<DataMember(Name = "longitude")>]
    Longitude: float
    /// The radius of uncertainty for the location, measured in meters; 0-1500
    [<DataMember(Name = "horizontal_accuracy")>]
    HorizontalAccuracy: float option
    /// Time relative to the message sending date, during which the location can be updated; in seconds. For active live locations only.
    [<DataMember(Name = "live_period")>]
    LivePeriod: int64 option
    /// The direction in which user is moving, in degrees; 1-360. For active live locations only.
    [<DataMember(Name = "heading")>]
    Heading: int64 option
    /// The maximum distance for proximity alerts about approaching another chat member, in meters. For sent live locations only.
    [<DataMember(Name = "proximity_alert_radius")>]
    ProximityAlertRadius: int64 option
  }
  static member Create(latitude: float, longitude: float, ?horizontalAccuracy: float, ?livePeriod: int64, ?heading: int64, ?proximityAlertRadius: int64) = 
    {
      Latitude = latitude
      Longitude = longitude
      HorizontalAccuracy = horizontalAccuracy
      LivePeriod = livePeriod
      Heading = heading
      ProximityAlertRadius = proximityAlertRadius
    }

/// This object represents a venue.
and [<CLIMutable>] Venue =
  {
    /// Venue location. Can't be a live location
    [<DataMember(Name = "location")>]
    Location: Location
    /// Name of the venue
    [<DataMember(Name = "title")>]
    Title: string
    /// Address of the venue
    [<DataMember(Name = "address")>]
    Address: string
    /// Foursquare identifier of the venue
    [<DataMember(Name = "foursquare_id")>]
    FoursquareId: string option
    /// Foursquare type of the venue. (For example, “arts_entertainment/default”, “arts_entertainment/aquarium” or “food/icecream”.)
    [<DataMember(Name = "foursquare_type")>]
    FoursquareType: string option
    /// Google Places identifier of the venue
    [<DataMember(Name = "google_place_id")>]
    GooglePlaceId: string option
    /// Google Places type of the venue. (See supported types.)
    [<DataMember(Name = "google_place_type")>]
    GooglePlaceType: string option
  }
  static member Create(location: Location, title: string, address: string, ?foursquareId: string, ?foursquareType: string, ?googlePlaceId: string, ?googlePlaceType: string) = 
    {
      Location = location
      Title = title
      Address = address
      FoursquareId = foursquareId
      FoursquareType = foursquareType
      GooglePlaceId = googlePlaceId
      GooglePlaceType = googlePlaceType
    }

/// Describes data sent from a Web App to the bot.
and [<CLIMutable>] WebAppData =
  {
    /// The data. Be aware that a bad client can send arbitrary data in this field.
    [<DataMember(Name = "data")>]
    Data: string
    /// Text of the web_app keyboard button from which the Web App was opened. Be aware that a bad client can send arbitrary data in this field.
    [<DataMember(Name = "button_text")>]
    ButtonText: string
  }
  static member Create(data: string, buttonText: string) = 
    {
      Data = data
      ButtonText = buttonText
    }

/// This object represents the content of a service message, sent whenever a user in the chat triggers a proximity alert set by another user.
and [<CLIMutable>] ProximityAlertTriggered =
  {
    /// User that triggered the alert
    [<DataMember(Name = "traveler")>]
    Traveler: User
    /// User that set the alert
    [<DataMember(Name = "watcher")>]
    Watcher: User
    /// The distance between the users
    [<DataMember(Name = "distance")>]
    Distance: int64
  }
  static member Create(traveler: User, watcher: User, distance: int64) = 
    {
      Traveler = traveler
      Watcher = watcher
      Distance = distance
    }

/// This object represents a service message about a change in auto-delete timer settings.
and [<CLIMutable>] MessageAutoDeleteTimerChanged =
  {
    /// New auto-delete time for messages in the chat; in seconds
    [<DataMember(Name = "message_auto_delete_time")>]
    MessageAutoDeleteTime: int64
  }
  static member Create(messageAutoDeleteTime: int64) = 
    {
      MessageAutoDeleteTime = messageAutoDeleteTime
    }

/// This object represents a service message about a user boosting a chat.
and [<CLIMutable>] ChatBoostAdded =
  {
    /// Number of boosts added by the user
    [<DataMember(Name = "boost_count")>]
    BoostCount: int64
  }
  static member Create(boostCount: int64) = 
    {
      BoostCount = boostCount
    }

/// This object describes the way a background is filled based on the selected colors. Currently, it can be one of
and BackgroundFill =
  | Solid of BackgroundFillSolid
  | Gradient of BackgroundFillGradient
  | FreeformGradient of BackgroundFillFreeformGradient

/// The background is filled using the selected color.
and [<CLIMutable>] BackgroundFillSolid =
  {
    /// Type of the background fill, always “solid”
    [<DataMember(Name = "type")>]
    Type: string
    /// The color of the background fill in the RGB24 format
    [<DataMember(Name = "color")>]
    Color: int64
  }
  static member Create(``type``: string, color: int64) = 
    {
      Type = ``type``
      Color = color
    }

/// The background is a gradient fill.
and [<CLIMutable>] BackgroundFillGradient =
  {
    /// Type of the background fill, always “gradient”
    [<DataMember(Name = "type")>]
    Type: string
    /// Top color of the gradient in the RGB24 format
    [<DataMember(Name = "top_color")>]
    TopColor: int64
    /// Bottom color of the gradient in the RGB24 format
    [<DataMember(Name = "bottom_color")>]
    BottomColor: int64
    /// Clockwise rotation angle of the background fill in degrees; 0-359
    [<DataMember(Name = "rotation_angle")>]
    RotationAngle: int64
  }
  static member Create(``type``: string, topColor: int64, bottomColor: int64, rotationAngle: int64) = 
    {
      Type = ``type``
      TopColor = topColor
      BottomColor = bottomColor
      RotationAngle = rotationAngle
    }

/// The background is a freeform gradient that rotates after every message in the chat.
and [<CLIMutable>] BackgroundFillFreeformGradient =
  {
    /// Type of the background fill, always “freeform_gradient”
    [<DataMember(Name = "type")>]
    Type: string
    /// A list of the 3 or 4 base colors that are used to generate the freeform gradient in the RGB24 format
    [<DataMember(Name = "colors")>]
    Colors: int64[]
  }
  static member Create(``type``: string, colors: int64[]) = 
    {
      Type = ``type``
      Colors = colors
    }

/// This object describes the type of a background. Currently, it can be one of
and BackgroundType =
  | Fill of BackgroundTypeFill
  | Wallpaper of BackgroundTypeWallpaper
  | Pattern of BackgroundTypePattern
  | ChatTheme of BackgroundTypeChatTheme

/// The background is automatically filled based on the selected colors.
and [<CLIMutable>] BackgroundTypeFill =
  {
    /// Type of the background, always “fill”
    [<DataMember(Name = "type")>]
    Type: string
    /// The background fill
    [<DataMember(Name = "fill")>]
    Fill: BackgroundFill
    /// Dimming of the background in dark themes, as a percentage; 0-100
    [<DataMember(Name = "dark_theme_dimming")>]
    DarkThemeDimming: int64
  }
  static member Create(``type``: string, fill: BackgroundFill, darkThemeDimming: int64) = 
    {
      Type = ``type``
      Fill = fill
      DarkThemeDimming = darkThemeDimming
    }

/// The background is a wallpaper in the JPEG format.
and [<CLIMutable>] BackgroundTypeWallpaper =
  {
    /// Type of the background, always “wallpaper”
    [<DataMember(Name = "type")>]
    Type: string
    /// Document with the wallpaper
    [<DataMember(Name = "document")>]
    Document: Document
    /// Dimming of the background in dark themes, as a percentage; 0-100
    [<DataMember(Name = "dark_theme_dimming")>]
    DarkThemeDimming: int64
    /// True, if the wallpaper is downscaled to fit in a 450x450 square and then box-blurred with radius 12
    [<DataMember(Name = "is_blurred")>]
    IsBlurred: bool option
    /// True, if the background moves slightly when the device is tilted
    [<DataMember(Name = "is_moving")>]
    IsMoving: bool option
  }
  static member Create(``type``: string, document: Document, darkThemeDimming: int64, ?isBlurred: bool, ?isMoving: bool) = 
    {
      Type = ``type``
      Document = document
      DarkThemeDimming = darkThemeDimming
      IsBlurred = isBlurred
      IsMoving = isMoving
    }

/// The background is a PNG or TGV (gzipped subset of SVG with MIME type “application/x-tgwallpattern”) pattern to be combined with the background fill chosen by the user.
and [<CLIMutable>] BackgroundTypePattern =
  {
    /// Type of the background, always “pattern”
    [<DataMember(Name = "type")>]
    Type: string
    /// Document with the pattern
    [<DataMember(Name = "document")>]
    Document: Document
    /// The background fill that is combined with the pattern
    [<DataMember(Name = "fill")>]
    Fill: BackgroundFill
    /// Intensity of the pattern when it is shown above the filled background; 0-100
    [<DataMember(Name = "intensity")>]
    Intensity: int64
    /// True, if the background fill must be applied only to the pattern itself. All other pixels are black in this case. For dark themes only
    [<DataMember(Name = "is_inverted")>]
    IsInverted: bool option
    /// True, if the background moves slightly when the device is tilted
    [<DataMember(Name = "is_moving")>]
    IsMoving: bool option
  }
  static member Create(``type``: string, document: Document, fill: BackgroundFill, intensity: int64, ?isInverted: bool, ?isMoving: bool) = 
    {
      Type = ``type``
      Document = document
      Fill = fill
      Intensity = intensity
      IsInverted = isInverted
      IsMoving = isMoving
    }

/// The background is taken directly from a built-in chat theme.
and [<CLIMutable>] BackgroundTypeChatTheme =
  {
    /// Type of the background, always “chat_theme”
    [<DataMember(Name = "type")>]
    Type: string
    /// Name of the chat theme, which is usually an emoji
    [<DataMember(Name = "theme_name")>]
    ThemeName: string
  }
  static member Create(``type``: string, themeName: string) = 
    {
      Type = ``type``
      ThemeName = themeName
    }

/// This object represents a chat background.
and [<CLIMutable>] ChatBackground =
  {
    /// Type of the background
    [<DataMember(Name = "type")>]
    Type: BackgroundType
  }
  static member Create(``type``: BackgroundType) = 
    {
      Type = ``type``
    }

/// This object represents a service message about a new forum topic created in the chat.
and [<CLIMutable>] ForumTopicCreated =
  {
    /// Name of the topic
    [<DataMember(Name = "name")>]
    Name: string
    /// Color of the topic icon in RGB format
    [<DataMember(Name = "icon_color")>]
    IconColor: int64
    /// Unique identifier of the custom emoji shown as the topic icon
    [<DataMember(Name = "icon_custom_emoji_id")>]
    IconCustomEmojiId: string option
  }
  static member Create(name: string, iconColor: int64, ?iconCustomEmojiId: string) = 
    {
      Name = name
      IconColor = iconColor
      IconCustomEmojiId = iconCustomEmojiId
    }

/// This object represents a service message about a forum topic closed in the chat. Currently holds no information.
and ForumTopicClosed =
  new() = {}

/// This object represents a service message about an edited forum topic.
and [<CLIMutable>] ForumTopicEdited =
  {
    /// New name of the topic, if it was edited
    [<DataMember(Name = "name")>]
    Name: string option
    /// New identifier of the custom emoji shown as the topic icon, if it was edited; an empty string if the icon was removed
    [<DataMember(Name = "icon_custom_emoji_id")>]
    IconCustomEmojiId: string option
  }
  static member Create(?name: string, ?iconCustomEmojiId: string) = 
    {
      Name = name
      IconCustomEmojiId = iconCustomEmojiId
    }

/// This object represents a service message about a forum topic reopened in the chat. Currently holds no information.
and ForumTopicReopened =
  new() = {}

/// This object represents a service message about General forum topic hidden in the chat. Currently holds no information.
and GeneralForumTopicHidden =
  new() = {}

/// This object represents a service message about General forum topic unhidden in the chat. Currently holds no information.
and GeneralForumTopicUnhidden =
  new() = {}

/// This object contains information about a user that was shared with the bot using a KeyboardButtonRequestUsers button.
and [<CLIMutable>] SharedUser =
  {
    /// Identifier of the shared user. This number may have more than 32 significant bits and some programming languages may have difficulty/silent defects in interpreting it. But it has at most 52 significant bits, so 64-bit integers or double-precision float types are safe for storing these identifiers. The bot may not have access to the user and could be unable to use this identifier, unless the user is already known to the bot by some other means.
    [<DataMember(Name = "user_id")>]
    UserId: int64
    /// First name of the user, if the name was requested by the bot
    [<DataMember(Name = "first_name")>]
    FirstName: string option
    /// Last name of the user, if the name was requested by the bot
    [<DataMember(Name = "last_name")>]
    LastName: string option
    /// Username of the user, if the username was requested by the bot
    [<DataMember(Name = "username")>]
    Username: string option
    /// Available sizes of the chat photo, if the photo was requested by the bot
    [<DataMember(Name = "photo")>]
    Photo: PhotoSize[] option
  }
  static member Create(userId: int64, ?firstName: string, ?lastName: string, ?username: string, ?photo: PhotoSize[]) = 
    {
      UserId = userId
      FirstName = firstName
      LastName = lastName
      Username = username
      Photo = photo
    }

/// This object contains information about the users whose identifiers were shared with the bot using a KeyboardButtonRequestUsers button.
and [<CLIMutable>] UsersShared =
  {
    /// Identifier of the request
    [<DataMember(Name = "request_id")>]
    RequestId: int64
    /// Information about users shared with the bot.
    [<DataMember(Name = "users")>]
    Users: SharedUser[]
  }
  static member Create(requestId: int64, users: SharedUser[]) = 
    {
      RequestId = requestId
      Users = users
    }

/// This object contains information about a chat that was shared with the bot using a KeyboardButtonRequestChat button.
and [<CLIMutable>] ChatShared =
  {
    /// Identifier of the request
    [<DataMember(Name = "request_id")>]
    RequestId: int64
    /// Identifier of the shared chat. This number may have more than 32 significant bits and some programming languages may have difficulty/silent defects in interpreting it. But it has at most 52 significant bits, so a 64-bit integer or double-precision float type are safe for storing this identifier. The bot may not have access to the chat and could be unable to use this identifier, unless the chat is already known to the bot by some other means.
    [<DataMember(Name = "chat_id")>]
    ChatId: int64
    /// Title of the chat, if the title was requested by the bot.
    [<DataMember(Name = "title")>]
    Title: string option
    /// Username of the chat, if the username was requested by the bot and available.
    [<DataMember(Name = "username")>]
    Username: string option
    /// Available sizes of the chat photo, if the photo was requested by the bot
    [<DataMember(Name = "photo")>]
    Photo: PhotoSize[] option
  }
  static member Create(requestId: int64, chatId: int64, ?title: string, ?username: string, ?photo: PhotoSize[]) = 
    {
      RequestId = requestId
      ChatId = chatId
      Title = title
      Username = username
      Photo = photo
    }

/// This object represents a service message about a user allowing a bot to write messages after adding it to the attachment menu, launching a Web App from a link, or accepting an explicit request from a Web App sent by the method requestWriteAccess.
and [<CLIMutable>] WriteAccessAllowed =
  {
    /// True, if the access was granted after the user accepted an explicit request from a Web App sent by the method requestWriteAccess
    [<DataMember(Name = "from_request")>]
    FromRequest: bool option
    /// Name of the Web App, if the access was granted when the Web App was launched from a link
    [<DataMember(Name = "web_app_name")>]
    WebAppName: string option
    /// True, if the access was granted when the bot was added to the attachment or side menu
    [<DataMember(Name = "from_attachment_menu")>]
    FromAttachmentMenu: bool option
  }
  static member Create(?fromRequest: bool, ?webAppName: string, ?fromAttachmentMenu: bool) = 
    {
      FromRequest = fromRequest
      WebAppName = webAppName
      FromAttachmentMenu = fromAttachmentMenu
    }

/// This object represents a service message about a video chat scheduled in the chat.
and [<CLIMutable>] VideoChatScheduled =
  {
    /// Point in time (Unix timestamp) when the video chat is supposed to be started by a chat administrator
    [<DataMember(Name = "start_date")>]
    StartDate: int64
  }
  static member Create(startDate: int64) = 
    {
      StartDate = startDate
    }

/// This object represents a service message about a video chat started in the chat. Currently holds no information.
and [<CLIMutable>] VideoChatStarted =
  {
    /// Unique identifier of video chat (not present in Bot API at the moment, for internal use)
    [<DataMember(Name = "id")>]
    Id: int64 option
  }
  static member Create(?id: int64) = 
    {
      Id = id
    }

/// This object represents a service message about a video chat ended in the chat.
and [<CLIMutable>] VideoChatEnded =
  {
    /// Video chat duration in seconds
    [<DataMember(Name = "duration")>]
    Duration: int64
    /// Unique identifier of video chat (not present in Bot API at the moment, for internal use)
    [<DataMember(Name = "id")>]
    Id: int64 option
  }
  static member Create(duration: int64, ?id: int64) = 
    {
      Duration = duration
      Id = id
    }

/// This object represents a service message about new members invited to a video chat.
and [<CLIMutable>] VideoChatParticipantsInvited =
  {
    /// New members that were invited to the video chat
    [<DataMember(Name = "users")>]
    Users: User[]
  }
  static member Create(users: User[]) = 
    {
      Users = users
    }

/// This object represents a service message about the creation of a scheduled giveaway.
and [<CLIMutable>] GiveawayCreated =
  {
    /// The number of Telegram Stars to be split between giveaway winners; for Telegram Star giveaways only
    [<DataMember(Name = "prize_star_count")>]
    PrizeStarCount: int64 option
  }
  static member Create(?prizeStarCount: int64) = 
    {
      PrizeStarCount = prizeStarCount
    }

/// This object represents a message about a scheduled giveaway.
and [<CLIMutable>] Giveaway =
  {
    /// The list of chats which the user must join to participate in the giveaway
    [<DataMember(Name = "chats")>]
    Chats: Chat[]
    /// Point in time (Unix timestamp) when winners of the giveaway will be selected
    [<DataMember(Name = "winners_selection_date")>]
    WinnersSelectionDate: int64
    /// The number of users which are supposed to be selected as winners of the giveaway
    [<DataMember(Name = "winner_count")>]
    WinnerCount: int64
    /// True, if only users who join the chats after the giveaway started should be eligible to win
    [<DataMember(Name = "only_new_members")>]
    OnlyNewMembers: bool option
    /// True, if the list of giveaway winners will be visible to everyone
    [<DataMember(Name = "has_public_winners")>]
    HasPublicWinners: bool option
    /// Description of additional giveaway prize
    [<DataMember(Name = "prize_description")>]
    PrizeDescription: string option
    /// A list of two-letter ISO 3166-1 alpha-2 country codes indicating the countries from which eligible users for the giveaway must come. If empty, then all users can participate in the giveaway. Users with a phone number that was bought on Fragment can always participate in giveaways.
    [<DataMember(Name = "country_codes")>]
    CountryCodes: string[] option
    /// The number of Telegram Stars to be split between giveaway winners; for Telegram Star giveaways only
    [<DataMember(Name = "prize_star_count")>]
    PrizeStarCount: int64 option
    /// The number of months the Telegram Premium subscription won from the giveaway will be active for; for Telegram Premium giveaways only
    [<DataMember(Name = "premium_subscription_month_count")>]
    PremiumSubscriptionMonthCount: int64 option
  }
  static member Create(chats: Chat[], winnersSelectionDate: int64, winnerCount: int64, ?onlyNewMembers: bool, ?hasPublicWinners: bool, ?prizeDescription: string, ?countryCodes: string[], ?prizeStarCount: int64, ?premiumSubscriptionMonthCount: int64) = 
    {
      Chats = chats
      WinnersSelectionDate = winnersSelectionDate
      WinnerCount = winnerCount
      OnlyNewMembers = onlyNewMembers
      HasPublicWinners = hasPublicWinners
      PrizeDescription = prizeDescription
      CountryCodes = countryCodes
      PrizeStarCount = prizeStarCount
      PremiumSubscriptionMonthCount = premiumSubscriptionMonthCount
    }

/// This object represents a message about the completion of a giveaway with public winners.
and [<CLIMutable>] GiveawayWinners =
  {
    /// The chat that created the giveaway
    [<DataMember(Name = "chat")>]
    Chat: Chat
    /// Identifier of the message with the giveaway in the chat
    [<DataMember(Name = "giveaway_message_id")>]
    GiveawayMessageId: int64
    /// Point in time (Unix timestamp) when winners of the giveaway were selected
    [<DataMember(Name = "winners_selection_date")>]
    WinnersSelectionDate: int64
    /// Total number of winners in the giveaway
    [<DataMember(Name = "winner_count")>]
    WinnerCount: int64
    /// List of up to 100 winners of the giveaway
    [<DataMember(Name = "winners")>]
    Winners: User[]
    /// The number of other chats the user had to join in order to be eligible for the giveaway
    [<DataMember(Name = "additional_chat_count")>]
    AdditionalChatCount: int64 option
    /// The number of Telegram Stars that were split between giveaway winners; for Telegram Star giveaways only
    [<DataMember(Name = "prize_star_count")>]
    PrizeStarCount: int64 option
    /// The number of months the Telegram Premium subscription won from the giveaway will be active for; for Telegram Premium giveaways only
    [<DataMember(Name = "premium_subscription_month_count")>]
    PremiumSubscriptionMonthCount: int64 option
    /// Number of undistributed prizes
    [<DataMember(Name = "unclaimed_prize_count")>]
    UnclaimedPrizeCount: int64 option
    /// True, if only users who had joined the chats after the giveaway started were eligible to win
    [<DataMember(Name = "only_new_members")>]
    OnlyNewMembers: bool option
    /// True, if the giveaway was canceled because the payment for it was refunded
    [<DataMember(Name = "was_refunded")>]
    WasRefunded: bool option
    /// Description of additional giveaway prize
    [<DataMember(Name = "prize_description")>]
    PrizeDescription: string option
  }
  static member Create(chat: Chat, giveawayMessageId: int64, winnersSelectionDate: int64, winnerCount: int64, winners: User[], ?additionalChatCount: int64, ?prizeStarCount: int64, ?premiumSubscriptionMonthCount: int64, ?unclaimedPrizeCount: int64, ?onlyNewMembers: bool, ?wasRefunded: bool, ?prizeDescription: string) = 
    {
      Chat = chat
      GiveawayMessageId = giveawayMessageId
      WinnersSelectionDate = winnersSelectionDate
      WinnerCount = winnerCount
      Winners = winners
      AdditionalChatCount = additionalChatCount
      PrizeStarCount = prizeStarCount
      PremiumSubscriptionMonthCount = premiumSubscriptionMonthCount
      UnclaimedPrizeCount = unclaimedPrizeCount
      OnlyNewMembers = onlyNewMembers
      WasRefunded = wasRefunded
      PrizeDescription = prizeDescription
    }

/// This object represents a service message about the completion of a giveaway without public winners.
and [<CLIMutable>] GiveawayCompleted =
  {
    /// Number of winners in the giveaway
    [<DataMember(Name = "winner_count")>]
    WinnerCount: int64
    /// Number of undistributed prizes
    [<DataMember(Name = "unclaimed_prize_count")>]
    UnclaimedPrizeCount: int64 option
    /// Message with the giveaway that was completed, if it wasn't deleted
    [<DataMember(Name = "giveaway_message")>]
    GiveawayMessage: Message option
    /// True, if the giveaway is a Telegram Star giveaway. Otherwise, currently, the giveaway is a Telegram Premium giveaway.
    [<DataMember(Name = "is_star_giveaway")>]
    IsStarGiveaway: bool option
  }
  static member Create(winnerCount: int64, ?unclaimedPrizeCount: int64, ?giveawayMessage: Message, ?isStarGiveaway: bool) = 
    {
      WinnerCount = winnerCount
      UnclaimedPrizeCount = unclaimedPrizeCount
      GiveawayMessage = giveawayMessage
      IsStarGiveaway = isStarGiveaway
    }

/// Describes the options used for link preview generation.
and [<CLIMutable>] LinkPreviewOptions =
  {
    /// True, if the link preview is disabled
    [<DataMember(Name = "is_disabled")>]
    IsDisabled: bool option
    /// URL to use for the link preview. If empty, then the first URL found in the message text will be used
    [<DataMember(Name = "url")>]
    Url: string option
    /// True, if the media in the link preview is supposed to be shrunk; ignored if the URL isn't explicitly specified or media size change isn't supported for the preview
    [<DataMember(Name = "prefer_small_media")>]
    PreferSmallMedia: bool option
    /// True, if the media in the link preview is supposed to be enlarged; ignored if the URL isn't explicitly specified or media size change isn't supported for the preview
    [<DataMember(Name = "prefer_large_media")>]
    PreferLargeMedia: bool option
    /// True, if the link preview must be shown above the message text; otherwise, the link preview will be shown below the message text
    [<DataMember(Name = "show_above_text")>]
    ShowAboveText: bool option
  }
  static member Create(?isDisabled: bool, ?url: string, ?preferSmallMedia: bool, ?preferLargeMedia: bool, ?showAboveText: bool) = 
    {
      IsDisabled = isDisabled
      Url = url
      PreferSmallMedia = preferSmallMedia
      PreferLargeMedia = preferLargeMedia
      ShowAboveText = showAboveText
    }

/// This object represent a user's profile pictures.
and [<CLIMutable>] UserProfilePhotos =
  {
    /// Total number of profile pictures the target user has
    [<DataMember(Name = "total_count")>]
    TotalCount: int64
    /// Requested profile pictures (in up to 4 sizes each)
    [<DataMember(Name = "photos")>]
    Photos: PhotoSize[][]
  }
  static member Create(totalCount: int64, photos: PhotoSize[][]) = 
    {
      TotalCount = totalCount
      Photos = photos
    }

/// This object represents a file ready to be downloaded. The file can be downloaded via the link https://api.telegram.org/file/bot<token>/<file_path>. It is guaranteed that the link will be valid for at least 1 hour. When the link expires, a new one can be requested by calling getFile.
and [<CLIMutable>] File =
  {
    /// Identifier for this file, which can be used to download or reuse the file
    [<DataMember(Name = "file_id")>]
    FileId: string
    /// Unique identifier for this file, which is supposed to be the same over time and for different bots. Can't be used to download or reuse the file.
    [<DataMember(Name = "file_unique_id")>]
    FileUniqueId: string
    /// File size in bytes. It can be bigger than 2^31 and some programming languages may have difficulty/silent defects in interpreting it. But it has at most 52 significant bits, so a signed 64-bit integer or double-precision float type are safe for storing this value.
    [<DataMember(Name = "file_size")>]
    FileSize: int64 option
    /// File path. Use https://api.telegram.org/file/bot<token>/<file_path> to get the file.
    [<DataMember(Name = "file_path")>]
    FilePath: string option
  }
  static member Create(fileId: string, fileUniqueId: string, ?fileSize: int64, ?filePath: string) = 
    {
      FileId = fileId
      FileUniqueId = fileUniqueId
      FileSize = fileSize
      FilePath = filePath
    }

/// Describes a Web App.
and [<CLIMutable>] WebAppInfo =
  {
    /// An HTTPS URL of a Web App to be opened with additional data as specified in Initializing Web Apps
    [<DataMember(Name = "url")>]
    Url: string
  }
  static member Create(url: string) = 
    {
      Url = url
    }

/// This object represents a custom keyboard with reply options (see Introduction to bots for details and examples). Not supported in channels and for messages sent on behalf of a Telegram Business account.
and [<CLIMutable>] ReplyKeyboardMarkup =
  {
    /// Array of button rows, each represented by an Array of KeyboardButton objects
    [<DataMember(Name = "keyboard")>]
    Keyboard: KeyboardButton[][]
    /// Requests clients to always show the keyboard when the regular keyboard is hidden. Defaults to false, in which case the custom keyboard can be hidden and opened with a keyboard icon.
    [<DataMember(Name = "is_persistent")>]
    IsPersistent: bool option
    /// Requests clients to resize the keyboard vertically for optimal fit (e.g., make the keyboard smaller if there are just two rows of buttons). Defaults to false, in which case the custom keyboard is always of the same height as the app's standard keyboard.
    [<DataMember(Name = "resize_keyboard")>]
    ResizeKeyboard: bool option
    /// Requests clients to hide the keyboard as soon as it's been used. The keyboard will still be available, but clients will automatically display the usual letter-keyboard in the chat - the user can press a special button in the input field to see the custom keyboard again. Defaults to false.
    [<DataMember(Name = "one_time_keyboard")>]
    OneTimeKeyboard: bool option
    /// The placeholder to be shown in the input field when the keyboard is active; 1-64 characters
    [<DataMember(Name = "input_field_placeholder")>]
    InputFieldPlaceholder: string option
    /// Use this parameter if you want to show the keyboard to specific users only. Targets: 1) users that are @mentioned in the text of the Message object; 2) if the bot's message is a reply to a message in the same chat and forum topic, sender of the original message.
    /// 
    /// Example: A user requests to change the bot's language, bot replies to the request with a keyboard to select the new language. Other users in the group don't see the keyboard.
    [<DataMember(Name = "selective")>]
    Selective: bool option
  }
  static member Create(keyboard: KeyboardButton[][], ?isPersistent: bool, ?resizeKeyboard: bool, ?oneTimeKeyboard: bool, ?inputFieldPlaceholder: string, ?selective: bool) = 
    {
      Keyboard = keyboard
      IsPersistent = isPersistent
      ResizeKeyboard = resizeKeyboard
      OneTimeKeyboard = oneTimeKeyboard
      InputFieldPlaceholder = inputFieldPlaceholder
      Selective = selective
    }

/// This object represents one button of the reply keyboard. At most one of the optional fields must be used to specify type of the button. For simple text buttons, String can be used instead of this object to specify the button text.
/// Note:request_users and request_chat options will only work in Telegram versions released after 3 February, 2023. Older clients will display unsupported message.
and [<CLIMutable>] KeyboardButton =
  {
    /// Text of the button. If none of the optional fields are used, it will be sent as a message when the button is pressed
    [<DataMember(Name = "text")>]
    Text: string
    /// If specified, pressing the button will open a list of suitable users. Identifiers of selected users will be sent to the bot in a “users_shared” service message. Available in private chats only.
    [<DataMember(Name = "request_users")>]
    RequestUsers: KeyboardButtonRequestUsers option
    /// If specified, pressing the button will open a list of suitable chats. Tapping on a chat will send its identifier to the bot in a “chat_shared” service message. Available in private chats only.
    [<DataMember(Name = "request_chat")>]
    RequestChat: KeyboardButtonRequestChat option
    /// If True, the user's phone number will be sent as a contact when the button is pressed. Available in private chats only.
    [<DataMember(Name = "request_contact")>]
    RequestContact: bool option
    /// If True, the user's current location will be sent when the button is pressed. Available in private chats only.
    [<DataMember(Name = "request_location")>]
    RequestLocation: bool option
    /// If specified, the user will be asked to create a poll and send it to the bot when the button is pressed. Available in private chats only.
    [<DataMember(Name = "request_poll")>]
    RequestPoll: KeyboardButtonPollType option
    /// If specified, the described Web App will be launched when the button is pressed. The Web App will be able to send a “web_app_data” service message. Available in private chats only.
    [<DataMember(Name = "web_app")>]
    WebApp: WebAppInfo option
  }
  static member Create(text: string, ?requestUsers: KeyboardButtonRequestUsers, ?requestChat: KeyboardButtonRequestChat, ?requestContact: bool, ?requestLocation: bool, ?requestPoll: KeyboardButtonPollType, ?webApp: WebAppInfo) = 
    {
      Text = text
      RequestUsers = requestUsers
      RequestChat = requestChat
      RequestContact = requestContact
      RequestLocation = requestLocation
      RequestPoll = requestPoll
      WebApp = webApp
    }

/// This object defines the criteria used to request suitable users. Information about the selected users will be shared with the bot when the corresponding button is pressed. More about requesting users »
and [<CLIMutable>] KeyboardButtonRequestUsers =
  {
    /// Signed 32-bit identifier of the request that will be received back in the UsersShared object. Must be unique within the message
    [<DataMember(Name = "request_id")>]
    RequestId: int64
    /// Pass True to request bots, pass False to request regular users. If not specified, no additional restrictions are applied.
    [<DataMember(Name = "user_is_bot")>]
    UserIsBot: bool option
    /// Pass True to request premium users, pass False to request non-premium users. If not specified, no additional restrictions are applied.
    [<DataMember(Name = "user_is_premium")>]
    UserIsPremium: bool option
    /// The maximum number of users to be selected; 1-10. Defaults to 1.
    [<DataMember(Name = "max_quantity")>]
    MaxQuantity: int64 option
    /// Pass True to request the users' first and last names
    [<DataMember(Name = "request_name")>]
    RequestName: bool option
    /// Pass True to request the users' usernames
    [<DataMember(Name = "request_username")>]
    RequestUsername: bool option
    /// Pass True to request the users' photos
    [<DataMember(Name = "request_photo")>]
    RequestPhoto: bool option
  }
  static member Create(requestId: int64, ?userIsBot: bool, ?userIsPremium: bool, ?maxQuantity: int64, ?requestName: bool, ?requestUsername: bool, ?requestPhoto: bool) = 
    {
      RequestId = requestId
      UserIsBot = userIsBot
      UserIsPremium = userIsPremium
      MaxQuantity = maxQuantity
      RequestName = requestName
      RequestUsername = requestUsername
      RequestPhoto = requestPhoto
    }

/// This object defines the criteria used to request a suitable chat. Information about the selected chat will be shared with the bot when the corresponding button is pressed. The bot will be granted requested rights in the chat if appropriate. More about requesting chats ».
and [<CLIMutable>] KeyboardButtonRequestChat =
  {
    /// Signed 32-bit identifier of the request, which will be received back in the ChatShared object. Must be unique within the message
    [<DataMember(Name = "request_id")>]
    RequestId: int64
    /// Pass True to request a channel chat, pass False to request a group or a supergroup chat.
    [<DataMember(Name = "chat_is_channel")>]
    ChatIsChannel: bool
    /// Pass True to request a forum supergroup, pass False to request a non-forum chat. If not specified, no additional restrictions are applied.
    [<DataMember(Name = "chat_is_forum")>]
    ChatIsForum: bool option
    /// Pass True to request a supergroup or a channel with a username, pass False to request a chat without a username. If not specified, no additional restrictions are applied.
    [<DataMember(Name = "chat_has_username")>]
    ChatHasUsername: bool option
    /// Pass True to request a chat owned by the user. Otherwise, no additional restrictions are applied.
    [<DataMember(Name = "chat_is_created")>]
    ChatIsCreated: bool option
    /// A JSON-serialized object listing the required administrator rights of the user in the chat. The rights must be a superset of bot_administrator_rights. If not specified, no additional restrictions are applied.
    [<DataMember(Name = "user_administrator_rights")>]
    UserAdministratorRights: ChatAdministratorRights option
    /// A JSON-serialized object listing the required administrator rights of the bot in the chat. The rights must be a subset of user_administrator_rights. If not specified, no additional restrictions are applied.
    [<DataMember(Name = "bot_administrator_rights")>]
    BotAdministratorRights: ChatAdministratorRights option
    /// Pass True to request a chat with the bot as a member. Otherwise, no additional restrictions are applied.
    [<DataMember(Name = "bot_is_member")>]
    BotIsMember: bool option
    /// Pass True to request the chat's title
    [<DataMember(Name = "request_title")>]
    RequestTitle: bool option
    /// Pass True to request the chat's username
    [<DataMember(Name = "request_username")>]
    RequestUsername: bool option
    /// Pass True to request the chat's photo
    [<DataMember(Name = "request_photo")>]
    RequestPhoto: bool option
  }
  static member Create(requestId: int64, chatIsChannel: bool, ?chatIsForum: bool, ?chatHasUsername: bool, ?chatIsCreated: bool, ?userAdministratorRights: ChatAdministratorRights, ?botAdministratorRights: ChatAdministratorRights, ?botIsMember: bool, ?requestTitle: bool, ?requestUsername: bool, ?requestPhoto: bool) = 
    {
      RequestId = requestId
      ChatIsChannel = chatIsChannel
      ChatIsForum = chatIsForum
      ChatHasUsername = chatHasUsername
      ChatIsCreated = chatIsCreated
      UserAdministratorRights = userAdministratorRights
      BotAdministratorRights = botAdministratorRights
      BotIsMember = botIsMember
      RequestTitle = requestTitle
      RequestUsername = requestUsername
      RequestPhoto = requestPhoto
    }

/// This object represents type of a poll, which is allowed to be created and sent when the corresponding button is pressed.
and [<CLIMutable>] KeyboardButtonPollType =
  {
    /// If quiz is passed, the user will be allowed to create only polls in the quiz mode. If regular is passed, only regular polls will be allowed. Otherwise, the user will be allowed to create a poll of any type.
    [<DataMember(Name = "type")>]
    Type: string option
  }
  static member Create(?``type``: string) = 
    {
      Type = ``type``
    }

/// Upon receiving a message with this object, Telegram clients will remove the current custom keyboard and display the default letter-keyboard. By default, custom keyboards are displayed until a new keyboard is sent by a bot. An exception is made for one-time keyboards that are hidden immediately after the user presses a button (see ReplyKeyboardMarkup). Not supported in channels and for messages sent on behalf of a Telegram Business account.
and [<CLIMutable>] ReplyKeyboardRemove =
  {
    /// Requests clients to remove the custom keyboard (user will not be able to summon this keyboard; if you want to hide the keyboard from sight but keep it accessible, use one_time_keyboard in ReplyKeyboardMarkup)
    [<DataMember(Name = "remove_keyboard")>]
    RemoveKeyboard: bool
    /// Use this parameter if you want to remove the keyboard for specific users only. Targets: 1) users that are @mentioned in the text of the Message object; 2) if the bot's message is a reply to a message in the same chat and forum topic, sender of the original message.
    /// 
    /// Example: A user votes in a poll, bot returns confirmation message in reply to the vote and removes the keyboard for that user, while still showing the keyboard with poll options to users who haven't voted yet.
    [<DataMember(Name = "selective")>]
    Selective: bool option
  }
  static member Create(removeKeyboard: bool, ?selective: bool) = 
    {
      RemoveKeyboard = removeKeyboard
      Selective = selective
    }

/// This object represents an inline keyboard that appears right next to the message it belongs to.
and [<CLIMutable>] InlineKeyboardMarkup =
  {
    /// Array of button rows, each represented by an Array of InlineKeyboardButton objects
    [<DataMember(Name = "inline_keyboard")>]
    InlineKeyboard: InlineKeyboardButton[][]
  }
  static member Create(inlineKeyboard: InlineKeyboardButton[][]) = 
    {
      InlineKeyboard = inlineKeyboard
    }

/// This object represents one button of an inline keyboard. Exactly one of the optional fields must be used to specify type of the button.
and [<CLIMutable>] InlineKeyboardButton =
  {
    /// Label text on the button
    [<DataMember(Name = "text")>]
    Text: string
    /// HTTP or tg:// URL to be opened when the button is pressed. Links tg://user?id=<user_id> can be used to mention a user by their identifier without using a username, if this is allowed by their privacy settings.
    [<DataMember(Name = "url")>]
    Url: string option
    /// Data to be sent in a callback query to the bot when the button is pressed, 1-64 bytes
    [<DataMember(Name = "callback_data")>]
    CallbackData: string option
    /// Description of the Web App that will be launched when the user presses the button. The Web App will be able to send an arbitrary message on behalf of the user using the method answerWebAppQuery. Available only in private chats between a user and the bot. Not supported for messages sent on behalf of a Telegram Business account.
    [<DataMember(Name = "web_app")>]
    WebApp: WebAppInfo option
    /// An HTTPS URL used to automatically authorize the user. Can be used as a replacement for the Telegram Login Widget.
    [<DataMember(Name = "login_url")>]
    LoginUrl: LoginUrl option
    /// If set, pressing the button will prompt the user to select one of their chats, open that chat and insert the bot's username and the specified inline query in the input field. May be empty, in which case just the bot's username will be inserted. Not supported for messages sent on behalf of a Telegram Business account.
    [<DataMember(Name = "switch_inline_query")>]
    SwitchInlineQuery: string option
    /// If set, pressing the button will insert the bot's username and the specified inline query in the current chat's input field. May be empty, in which case only the bot's username will be inserted.
    /// 
    /// This offers a quick way for the user to open your bot in inline mode in the same chat - good for selecting something from multiple options. Not supported in channels and for messages sent on behalf of a Telegram Business account.
    [<DataMember(Name = "switch_inline_query_current_chat")>]
    SwitchInlineQueryCurrentChat: string option
    /// If set, pressing the button will prompt the user to select one of their chats of the specified type, open that chat and insert the bot's username and the specified inline query in the input field. Not supported for messages sent on behalf of a Telegram Business account.
    [<DataMember(Name = "switch_inline_query_chosen_chat")>]
    SwitchInlineQueryChosenChat: SwitchInlineQueryChosenChat option
    /// Description of the game that will be launched when the user presses the button.
    /// 
    /// NOTE: This type of button must always be the first button in the first row.
    [<DataMember(Name = "callback_game")>]
    CallbackGame: CallbackGame option
    /// Specify True, to send a Pay button. Substrings “” and “XTR” in the buttons's text will be replaced with a Telegram Star icon.
    /// 
    /// NOTE: This type of button must always be the first button in the first row and can only be used in invoice messages.
    [<DataMember(Name = "pay")>]
    Pay: bool option
  }
  static member Create(text: string, ?url: string, ?callbackData: string, ?webApp: WebAppInfo, ?loginUrl: LoginUrl, ?switchInlineQuery: string, ?switchInlineQueryCurrentChat: string, ?switchInlineQueryChosenChat: SwitchInlineQueryChosenChat, ?callbackGame: CallbackGame, ?pay: bool) = 
    {
      Text = text
      Url = url
      CallbackData = callbackData
      WebApp = webApp
      LoginUrl = loginUrl
      SwitchInlineQuery = switchInlineQuery
      SwitchInlineQueryCurrentChat = switchInlineQueryCurrentChat
      SwitchInlineQueryChosenChat = switchInlineQueryChosenChat
      CallbackGame = callbackGame
      Pay = pay
    }

/// This object represents a parameter of the inline keyboard button used to automatically authorize a user. Serves as a great replacement for the Telegram Login Widget when the user is coming from Telegram. All the user needs to do is tap/click a button and confirm that they want to log in:
/// Telegram apps support these buttons as of version 5.7.
and [<CLIMutable>] LoginUrl =
  {
    /// An HTTPS URL to be opened with user authorization data added to the query string when the button is pressed. If the user refuses to provide authorization data, the original URL without information about the user will be opened. The data added is the same as described in Receiving authorization data.
    /// 
    /// NOTE: You must always check the hash of the received data to verify the authentication and the integrity of the data as described in Checking authorization.
    [<DataMember(Name = "url")>]
    Url: string
    /// New text of the button in forwarded messages.
    [<DataMember(Name = "forward_text")>]
    ForwardText: string option
    /// Username of a bot, which will be used for user authorization. See Setting up a bot for more details. If not specified, the current bot's username will be assumed. The url's domain must be the same as the domain linked with the bot. See Linking your domain to the bot for more details.
    [<DataMember(Name = "bot_username")>]
    BotUsername: string option
    /// Pass True to request the permission for your bot to send messages to the user.
    [<DataMember(Name = "request_write_access")>]
    RequestWriteAccess: bool option
  }
  static member Create(url: string, ?forwardText: string, ?botUsername: string, ?requestWriteAccess: bool) = 
    {
      Url = url
      ForwardText = forwardText
      BotUsername = botUsername
      RequestWriteAccess = requestWriteAccess
    }

/// This object represents an inline button that switches the current user to inline mode in a chosen chat, with an optional default inline query.
and [<CLIMutable>] SwitchInlineQueryChosenChat =
  {
    /// The default inline query to be inserted in the input field. If left empty, only the bot's username will be inserted
    [<DataMember(Name = "query")>]
    Query: string option
    /// True, if private chats with users can be chosen
    [<DataMember(Name = "allow_user_chats")>]
    AllowUserChats: bool option
    /// True, if private chats with bots can be chosen
    [<DataMember(Name = "allow_bot_chats")>]
    AllowBotChats: bool option
    /// True, if group and supergroup chats can be chosen
    [<DataMember(Name = "allow_group_chats")>]
    AllowGroupChats: bool option
    /// True, if channel chats can be chosen
    [<DataMember(Name = "allow_channel_chats")>]
    AllowChannelChats: bool option
  }
  static member Create(?query: string, ?allowUserChats: bool, ?allowBotChats: bool, ?allowGroupChats: bool, ?allowChannelChats: bool) = 
    {
      Query = query
      AllowUserChats = allowUserChats
      AllowBotChats = allowBotChats
      AllowGroupChats = allowGroupChats
      AllowChannelChats = allowChannelChats
    }

/// This object represents an incoming callback query from a callback button in an inline keyboard. If the button that originated the query was attached to a message sent by the bot, the field message will be present. If the button was attached to a message sent via the bot (in inline mode), the field inline_message_id will be present. Exactly one of the fields data or game_short_name will be present.
and [<CLIMutable>] CallbackQuery =
  {
    /// Unique identifier for this query
    [<DataMember(Name = "id")>]
    Id: string
    /// Sender
    [<DataMember(Name = "from")>]
    From: User
    /// Message sent by the bot with the callback button that originated the query
    [<DataMember(Name = "message")>]
    Message: MaybeInaccessibleMessage option
    /// Identifier of the message sent via the bot in inline mode, that originated the query.
    [<DataMember(Name = "inline_message_id")>]
    InlineMessageId: string option
    /// Global identifier, uniquely corresponding to the chat to which the message with the callback button was sent. Useful for high scores in games.
    [<DataMember(Name = "chat_instance")>]
    ChatInstance: string
    /// Data associated with the callback button. Be aware that the message originated the query can contain no callback buttons with this data.
    [<DataMember(Name = "data")>]
    Data: string option
    /// Short name of a Game to be returned, serves as the unique identifier for the game
    [<DataMember(Name = "game_short_name")>]
    GameShortName: string option
  }
  static member Create(id: string, from: User, chatInstance: string, ?message: MaybeInaccessibleMessage, ?inlineMessageId: string, ?data: string, ?gameShortName: string) = 
    {
      Id = id
      From = from
      ChatInstance = chatInstance
      Message = message
      InlineMessageId = inlineMessageId
      Data = data
      GameShortName = gameShortName
    }

/// Upon receiving a message with this object, Telegram clients will display a reply interface to the user (act as if the user has selected the bot's message and tapped 'Reply'). This can be extremely useful if you want to create user-friendly step-by-step interfaces without having to sacrifice privacy mode. Not supported in channels and for messages sent on behalf of a Telegram Business account.
and [<CLIMutable>] ForceReply =
  {
    /// Shows reply interface to the user, as if they manually selected the bot's message and tapped 'Reply'
    [<DataMember(Name = "force_reply")>]
    ForceReply: bool
    /// The placeholder to be shown in the input field when the reply is active; 1-64 characters
    [<DataMember(Name = "input_field_placeholder")>]
    InputFieldPlaceholder: string option
    /// Use this parameter if you want to force reply from specific users only. Targets: 1) users that are @mentioned in the text of the Message object; 2) if the bot's message is a reply to a message in the same chat and forum topic, sender of the original message.
    [<DataMember(Name = "selective")>]
    Selective: bool option
  }
  static member Create(forceReply: bool, ?inputFieldPlaceholder: string, ?selective: bool) = 
    {
      ForceReply = forceReply
      InputFieldPlaceholder = inputFieldPlaceholder
      Selective = selective
    }

/// This object represents a chat photo.
and [<CLIMutable>] ChatPhoto =
  {
    /// File identifier of small (160x160) chat photo. This file_id can be used only for photo download and only for as long as the photo is not changed.
    [<DataMember(Name = "small_file_id")>]
    SmallFileId: string
    /// Unique file identifier of small (160x160) chat photo, which is supposed to be the same over time and for different bots. Can't be used to download or reuse the file.
    [<DataMember(Name = "small_file_unique_id")>]
    SmallFileUniqueId: string
    /// File identifier of big (640x640) chat photo. This file_id can be used only for photo download and only for as long as the photo is not changed.
    [<DataMember(Name = "big_file_id")>]
    BigFileId: string
    /// Unique file identifier of big (640x640) chat photo, which is supposed to be the same over time and for different bots. Can't be used to download or reuse the file.
    [<DataMember(Name = "big_file_unique_id")>]
    BigFileUniqueId: string
  }
  static member Create(smallFileId: string, smallFileUniqueId: string, bigFileId: string, bigFileUniqueId: string) = 
    {
      SmallFileId = smallFileId
      SmallFileUniqueId = smallFileUniqueId
      BigFileId = bigFileId
      BigFileUniqueId = bigFileUniqueId
    }

/// Represents an invite link for a chat.
and [<CLIMutable>] ChatInviteLink =
  {
    /// The invite link. If the link was created by another chat administrator, then the second part of the link will be replaced with “…”.
    [<DataMember(Name = "invite_link")>]
    InviteLink: string
    /// Creator of the link
    [<DataMember(Name = "creator")>]
    Creator: User
    /// True, if users joining the chat via the link need to be approved by chat administrators
    [<DataMember(Name = "creates_join_request")>]
    CreatesJoinRequest: bool
    /// True, if the link is primary
    [<DataMember(Name = "is_primary")>]
    IsPrimary: bool
    /// True, if the link is revoked
    [<DataMember(Name = "is_revoked")>]
    IsRevoked: bool
    /// Invite link name
    [<DataMember(Name = "name")>]
    Name: string option
    /// Point in time (Unix timestamp) when the link will expire or has been expired
    [<DataMember(Name = "expire_date")>]
    ExpireDate: int64 option
    /// The maximum number of users that can be members of the chat simultaneously after joining the chat via this invite link; 1-99999
    [<DataMember(Name = "member_limit")>]
    MemberLimit: int64 option
    /// Number of pending join requests created using this link
    [<DataMember(Name = "pending_join_request_count")>]
    PendingJoinRequestCount: int64 option
    /// The number of seconds the subscription will be active for before the next payment
    [<DataMember(Name = "subscription_period")>]
    SubscriptionPeriod: int64 option
    /// The amount of Telegram Stars a user must pay initially and after each subsequent subscription period to be a member of the chat using the link
    [<DataMember(Name = "subscription_price")>]
    SubscriptionPrice: int64 option
  }
  static member Create(inviteLink: string, creator: User, createsJoinRequest: bool, isPrimary: bool, isRevoked: bool, ?name: string, ?expireDate: int64, ?memberLimit: int64, ?pendingJoinRequestCount: int64, ?subscriptionPeriod: int64, ?subscriptionPrice: int64) = 
    {
      InviteLink = inviteLink
      Creator = creator
      CreatesJoinRequest = createsJoinRequest
      IsPrimary = isPrimary
      IsRevoked = isRevoked
      Name = name
      ExpireDate = expireDate
      MemberLimit = memberLimit
      PendingJoinRequestCount = pendingJoinRequestCount
      SubscriptionPeriod = subscriptionPeriod
      SubscriptionPrice = subscriptionPrice
    }

/// Represents the rights of an administrator in a chat.
and [<CLIMutable>] ChatAdministratorRights =
  {
    /// True, if the user's presence in the chat is hidden
    [<DataMember(Name = "is_anonymous")>]
    IsAnonymous: bool
    /// True, if the administrator can access the chat event log, get boost list, see hidden supergroup and channel members, report spam messages and ignore slow mode. Implied by any other administrator privilege.
    [<DataMember(Name = "can_manage_chat")>]
    CanManageChat: bool
    /// True, if the administrator can delete messages of other users
    [<DataMember(Name = "can_delete_messages")>]
    CanDeleteMessages: bool
    /// True, if the administrator can manage video chats
    [<DataMember(Name = "can_manage_video_chats")>]
    CanManageVideoChats: bool
    /// True, if the administrator can restrict, ban or unban chat members, or access supergroup statistics
    [<DataMember(Name = "can_restrict_members")>]
    CanRestrictMembers: bool
    /// True, if the administrator can add new administrators with a subset of their own privileges or demote administrators that they have promoted, directly or indirectly (promoted by administrators that were appointed by the user)
    [<DataMember(Name = "can_promote_members")>]
    CanPromoteMembers: bool
    /// True, if the user is allowed to change the chat title, photo and other settings
    [<DataMember(Name = "can_change_info")>]
    CanChangeInfo: bool
    /// True, if the user is allowed to invite new users to the chat
    [<DataMember(Name = "can_invite_users")>]
    CanInviteUsers: bool
    /// True, if the administrator can post stories to the chat
    [<DataMember(Name = "can_post_stories")>]
    CanPostStories: bool
    /// True, if the administrator can edit stories posted by other users, post stories to the chat page, pin chat stories, and access the chat's story archive
    [<DataMember(Name = "can_edit_stories")>]
    CanEditStories: bool
    /// True, if the administrator can delete stories posted by other users
    [<DataMember(Name = "can_delete_stories")>]
    CanDeleteStories: bool
    /// True, if the administrator can post messages in the channel, or access channel statistics; for channels only
    [<DataMember(Name = "can_post_messages")>]
    CanPostMessages: bool option
    /// True, if the administrator can edit messages of other users and can pin messages; for channels only
    [<DataMember(Name = "can_edit_messages")>]
    CanEditMessages: bool option
    /// True, if the user is allowed to pin messages; for groups and supergroups only
    [<DataMember(Name = "can_pin_messages")>]
    CanPinMessages: bool option
    /// True, if the user is allowed to create, rename, close, and reopen forum topics; for supergroups only
    [<DataMember(Name = "can_manage_topics")>]
    CanManageTopics: bool option
    /// DEPRECATED: use can_manage_video_chats instead
    [<DataMember(Name = "can_manage_voice_chats")>]
    CanManageVoiceChats: bool option
  }
  static member Create(isAnonymous: bool, canManageChat: bool, canDeleteMessages: bool, canManageVideoChats: bool, canRestrictMembers: bool, canPromoteMembers: bool, canChangeInfo: bool, canInviteUsers: bool, canPostStories: bool, canEditStories: bool, canDeleteStories: bool, ?canPostMessages: bool, ?canEditMessages: bool, ?canPinMessages: bool, ?canManageTopics: bool, ?canManageVoiceChats: bool) = 
    {
      IsAnonymous = isAnonymous
      CanManageChat = canManageChat
      CanDeleteMessages = canDeleteMessages
      CanManageVideoChats = canManageVideoChats
      CanRestrictMembers = canRestrictMembers
      CanPromoteMembers = canPromoteMembers
      CanChangeInfo = canChangeInfo
      CanInviteUsers = canInviteUsers
      CanPostStories = canPostStories
      CanEditStories = canEditStories
      CanDeleteStories = canDeleteStories
      CanPostMessages = canPostMessages
      CanEditMessages = canEditMessages
      CanPinMessages = canPinMessages
      CanManageTopics = canManageTopics
      CanManageVoiceChats = canManageVoiceChats
    }

/// This object represents changes in the status of a chat member.
and [<CLIMutable>] ChatMemberUpdated =
  {
    /// Chat the user belongs to
    [<DataMember(Name = "chat")>]
    Chat: Chat
    /// Performer of the action, which resulted in the change
    [<DataMember(Name = "from")>]
    From: User
    /// Date the change was done in Unix time
    [<DataMember(Name = "date")>]
    Date: DateTime
    /// Previous information about the chat member
    [<DataMember(Name = "old_chat_member")>]
    OldChatMember: ChatMember
    /// New information about the chat member
    [<DataMember(Name = "new_chat_member")>]
    NewChatMember: ChatMember
    /// Chat invite link, which was used by the user to join the chat; for joining by invite link events only.
    [<DataMember(Name = "invite_link")>]
    InviteLink: ChatInviteLink option
    /// True, if the user joined the chat after sending a direct join request without using an invite link and being approved by an administrator
    [<DataMember(Name = "via_join_request")>]
    ViaJoinRequest: bool option
    /// True, if the user joined the chat via a chat folder invite link
    [<DataMember(Name = "via_chat_folder_invite_link")>]
    ViaChatFolderInviteLink: bool option
  }
  static member Create(chat: Chat, from: User, date: DateTime, oldChatMember: ChatMember, newChatMember: ChatMember, ?inviteLink: ChatInviteLink, ?viaJoinRequest: bool, ?viaChatFolderInviteLink: bool) = 
    {
      Chat = chat
      From = from
      Date = date
      OldChatMember = oldChatMember
      NewChatMember = newChatMember
      InviteLink = inviteLink
      ViaJoinRequest = viaJoinRequest
      ViaChatFolderInviteLink = viaChatFolderInviteLink
    }

/// This object contains information about one member of a chat. Currently, the following 6 types of chat members are supported:
and ChatMember =
  | Owner of ChatMemberOwner
  | Administrator of ChatMemberAdministrator
  | Member of ChatMemberMember
  | Restricted of ChatMemberRestricted
  | Left of ChatMemberLeft
  | Banned of ChatMemberBanned

/// Represents a chat member that owns the chat and has all administrator privileges.
and [<CLIMutable>] ChatMemberOwner =
  {
    /// The member's status in the chat, always “creator”
    [<DataMember(Name = "status")>]
    Status: string
    /// Information about the user
    [<DataMember(Name = "user")>]
    User: User
    /// True, if the user's presence in the chat is hidden
    [<DataMember(Name = "is_anonymous")>]
    IsAnonymous: bool
    /// Custom title for this user
    [<DataMember(Name = "custom_title")>]
    CustomTitle: string option
  }
  static member Create(status: string, user: User, isAnonymous: bool, ?customTitle: string) = 
    {
      Status = status
      User = user
      IsAnonymous = isAnonymous
      CustomTitle = customTitle
    }

/// Represents a chat member that has some additional privileges.
and [<CLIMutable>] ChatMemberAdministrator =
  {
    /// The member's status in the chat, always “administrator”
    [<DataMember(Name = "status")>]
    Status: string
    /// Information about the user
    [<DataMember(Name = "user")>]
    User: User
    /// True, if the bot is allowed to edit administrator privileges of that user
    [<DataMember(Name = "can_be_edited")>]
    CanBeEdited: bool
    /// True, if the user's presence in the chat is hidden
    [<DataMember(Name = "is_anonymous")>]
    IsAnonymous: bool
    /// True, if the administrator can access the chat event log, get boost list, see hidden supergroup and channel members, report spam messages and ignore slow mode. Implied by any other administrator privilege.
    [<DataMember(Name = "can_manage_chat")>]
    CanManageChat: bool
    /// True, if the administrator can delete messages of other users
    [<DataMember(Name = "can_delete_messages")>]
    CanDeleteMessages: bool
    /// True, if the administrator can manage video chats
    [<DataMember(Name = "can_manage_video_chats")>]
    CanManageVideoChats: bool
    /// True, if the administrator can restrict, ban or unban chat members, or access supergroup statistics
    [<DataMember(Name = "can_restrict_members")>]
    CanRestrictMembers: bool
    /// True, if the administrator can add new administrators with a subset of their own privileges or demote administrators that they have promoted, directly or indirectly (promoted by administrators that were appointed by the user)
    [<DataMember(Name = "can_promote_members")>]
    CanPromoteMembers: bool
    /// True, if the user is allowed to change the chat title, photo and other settings
    [<DataMember(Name = "can_change_info")>]
    CanChangeInfo: bool
    /// True, if the user is allowed to invite new users to the chat
    [<DataMember(Name = "can_invite_users")>]
    CanInviteUsers: bool
    /// True, if the administrator can post stories to the chat
    [<DataMember(Name = "can_post_stories")>]
    CanPostStories: bool
    /// True, if the administrator can edit stories posted by other users, post stories to the chat page, pin chat stories, and access the chat's story archive
    [<DataMember(Name = "can_edit_stories")>]
    CanEditStories: bool
    /// True, if the administrator can delete stories posted by other users
    [<DataMember(Name = "can_delete_stories")>]
    CanDeleteStories: bool
    /// True, if the administrator can post messages in the channel, or access channel statistics; for channels only
    [<DataMember(Name = "can_post_messages")>]
    CanPostMessages: bool option
    /// True, if the administrator can edit messages of other users and can pin messages; for channels only
    [<DataMember(Name = "can_edit_messages")>]
    CanEditMessages: bool option
    /// True, if the user is allowed to pin messages; for groups and supergroups only
    [<DataMember(Name = "can_pin_messages")>]
    CanPinMessages: bool option
    /// True, if the user is allowed to create, rename, close, and reopen forum topics; for supergroups only
    [<DataMember(Name = "can_manage_topics")>]
    CanManageTopics: bool option
    /// Custom title for this user
    [<DataMember(Name = "custom_title")>]
    CustomTitle: string option
    /// DEPRECATED: use can_manage_video_chats instead
    [<DataMember(Name = "can_manage_voice_chats")>]
    CanManageVoiceChats: bool option
  }
  static member Create(status: string, canDeleteStories: bool, canEditStories: bool, canPostStories: bool, canInviteUsers: bool, canPromoteMembers: bool, canRestrictMembers: bool, canChangeInfo: bool, canDeleteMessages: bool, canManageChat: bool, isAnonymous: bool, canBeEdited: bool, user: User, canManageVideoChats: bool, ?customTitle: string, ?canPostMessages: bool, ?canEditMessages: bool, ?canPinMessages: bool, ?canManageTopics: bool, ?canManageVoiceChats: bool) = 
    {
      Status = status
      CanDeleteStories = canDeleteStories
      CanEditStories = canEditStories
      CanPostStories = canPostStories
      CanInviteUsers = canInviteUsers
      CanPromoteMembers = canPromoteMembers
      CanRestrictMembers = canRestrictMembers
      CanChangeInfo = canChangeInfo
      CanDeleteMessages = canDeleteMessages
      CanManageChat = canManageChat
      IsAnonymous = isAnonymous
      CanBeEdited = canBeEdited
      User = user
      CanManageVideoChats = canManageVideoChats
      CustomTitle = customTitle
      CanPostMessages = canPostMessages
      CanEditMessages = canEditMessages
      CanPinMessages = canPinMessages
      CanManageTopics = canManageTopics
      CanManageVoiceChats = canManageVoiceChats
    }

/// Represents a chat member that has no additional privileges or restrictions.
and [<CLIMutable>] ChatMemberMember =
  {
    /// The member's status in the chat, always “member”
    [<DataMember(Name = "status")>]
    Status: string
    /// Information about the user
    [<DataMember(Name = "user")>]
    User: User
    /// Date when the user's subscription will expire; Unix time
    [<DataMember(Name = "until_date")>]
    UntilDate: DateTime option
  }
  static member Create(status: string, user: User, ?untilDate: DateTime) = 
    {
      Status = status
      User = user
      UntilDate = untilDate
    }

/// Represents a chat member that is under certain restrictions in the chat. Supergroups only.
and [<CLIMutable>] ChatMemberRestricted =
  {
    /// The member's status in the chat, always “restricted”
    [<DataMember(Name = "status")>]
    Status: string
    /// Information about the user
    [<DataMember(Name = "user")>]
    User: User
    /// True, if the user is a member of the chat at the moment of the request
    [<DataMember(Name = "is_member")>]
    IsMember: bool
    /// True, if the user is allowed to send text messages, contacts, giveaways, giveaway winners, invoices, locations and venues
    [<DataMember(Name = "can_send_messages")>]
    CanSendMessages: bool
    /// True, if the user is allowed to send audios
    [<DataMember(Name = "can_send_audios")>]
    CanSendAudios: bool
    /// True, if the user is allowed to send documents
    [<DataMember(Name = "can_send_documents")>]
    CanSendDocuments: bool
    /// True, if the user is allowed to send photos
    [<DataMember(Name = "can_send_photos")>]
    CanSendPhotos: bool
    /// True, if the user is allowed to send videos
    [<DataMember(Name = "can_send_videos")>]
    CanSendVideos: bool
    /// True, if the user is allowed to send video notes
    [<DataMember(Name = "can_send_video_notes")>]
    CanSendVideoNotes: bool
    /// True, if the user is allowed to send voice notes
    [<DataMember(Name = "can_send_voice_notes")>]
    CanSendVoiceNotes: bool
    /// True, if the user is allowed to send polls
    [<DataMember(Name = "can_send_polls")>]
    CanSendPolls: bool
    /// True, if the user is allowed to send animations, games, stickers and use inline bots
    [<DataMember(Name = "can_send_other_messages")>]
    CanSendOtherMessages: bool
    /// True, if the user is allowed to add web page previews to their messages
    [<DataMember(Name = "can_add_web_page_previews")>]
    CanAddWebPagePreviews: bool
    /// True, if the user is allowed to change the chat title, photo and other settings
    [<DataMember(Name = "can_change_info")>]
    CanChangeInfo: bool
    /// True, if the user is allowed to invite new users to the chat
    [<DataMember(Name = "can_invite_users")>]
    CanInviteUsers: bool
    /// True, if the user is allowed to pin messages
    [<DataMember(Name = "can_pin_messages")>]
    CanPinMessages: bool
    /// True, if the user is allowed to create forum topics
    [<DataMember(Name = "can_manage_topics")>]
    CanManageTopics: bool
    /// Date when restrictions will be lifted for this user; Unix time. If 0, then the user is restricted forever
    [<DataMember(Name = "until_date")>]
    UntilDate: DateTime
  }
  static member Create(status: string, canPinMessages: bool, canInviteUsers: bool, canChangeInfo: bool, canAddWebPagePreviews: bool, canSendOtherMessages: bool, canSendPolls: bool, canSendVoiceNotes: bool, canSendVideoNotes: bool, canSendVideos: bool, canSendPhotos: bool, canSendDocuments: bool, canSendAudios: bool, canSendMessages: bool, isMember: bool, user: User, canManageTopics: bool, untilDate: DateTime) = 
    {
      Status = status
      CanPinMessages = canPinMessages
      CanInviteUsers = canInviteUsers
      CanChangeInfo = canChangeInfo
      CanAddWebPagePreviews = canAddWebPagePreviews
      CanSendOtherMessages = canSendOtherMessages
      CanSendPolls = canSendPolls
      CanSendVoiceNotes = canSendVoiceNotes
      CanSendVideoNotes = canSendVideoNotes
      CanSendVideos = canSendVideos
      CanSendPhotos = canSendPhotos
      CanSendDocuments = canSendDocuments
      CanSendAudios = canSendAudios
      CanSendMessages = canSendMessages
      IsMember = isMember
      User = user
      CanManageTopics = canManageTopics
      UntilDate = untilDate
    }

/// Represents a chat member that isn't currently a member of the chat, but may join it themselves.
and [<CLIMutable>] ChatMemberLeft =
  {
    /// The member's status in the chat, always “left”
    [<DataMember(Name = "status")>]
    Status: string
    /// Information about the user
    [<DataMember(Name = "user")>]
    User: User
  }
  static member Create(status: string, user: User) = 
    {
      Status = status
      User = user
    }

/// Represents a chat member that was banned in the chat and can't return to the chat or view chat messages.
and [<CLIMutable>] ChatMemberBanned =
  {
    /// The member's status in the chat, always “kicked”
    [<DataMember(Name = "status")>]
    Status: string
    /// Information about the user
    [<DataMember(Name = "user")>]
    User: User
    /// Date when restrictions will be lifted for this user; Unix time. If 0, then the user is banned forever
    [<DataMember(Name = "until_date")>]
    UntilDate: DateTime
  }
  static member Create(status: string, user: User, untilDate: DateTime) = 
    {
      Status = status
      User = user
      UntilDate = untilDate
    }

/// Represents a join request sent to a chat.
and [<CLIMutable>] ChatJoinRequest =
  {
    /// Chat to which the request was sent
    [<DataMember(Name = "chat")>]
    Chat: Chat
    /// User that sent the join request
    [<DataMember(Name = "from")>]
    From: User
    /// Identifier of a private chat with the user who sent the join request. This number may have more than 32 significant bits and some programming languages may have difficulty/silent defects in interpreting it. But it has at most 52 significant bits, so a 64-bit integer or double-precision float type are safe for storing this identifier. The bot can use this identifier for 5 minutes to send messages until the join request is processed, assuming no other administrator contacted the user.
    [<DataMember(Name = "user_chat_id")>]
    UserChatId: int64
    /// Date the request was sent in Unix time
    [<DataMember(Name = "date")>]
    Date: DateTime
    /// Bio of the user.
    [<DataMember(Name = "bio")>]
    Bio: string option
    /// Chat invite link that was used by the user to send the join request
    [<DataMember(Name = "invite_link")>]
    InviteLink: ChatInviteLink option
  }
  static member Create(chat: Chat, from: User, userChatId: int64, date: DateTime, ?bio: string, ?inviteLink: ChatInviteLink) = 
    {
      Chat = chat
      From = from
      UserChatId = userChatId
      Date = date
      Bio = bio
      InviteLink = inviteLink
    }

/// Describes actions that a non-administrator user is allowed to take in a chat.
and [<CLIMutable>] ChatPermissions =
  {
    /// True, if the user is allowed to send text messages, contacts, giveaways, giveaway winners, invoices, locations and venues
    [<DataMember(Name = "can_send_messages")>]
    CanSendMessages: bool option
    /// True, if the user is allowed to send audios
    [<DataMember(Name = "can_send_audios")>]
    CanSendAudios: bool option
    /// True, if the user is allowed to send documents
    [<DataMember(Name = "can_send_documents")>]
    CanSendDocuments: bool option
    /// True, if the user is allowed to send photos
    [<DataMember(Name = "can_send_photos")>]
    CanSendPhotos: bool option
    /// True, if the user is allowed to send videos
    [<DataMember(Name = "can_send_videos")>]
    CanSendVideos: bool option
    /// True, if the user is allowed to send video notes
    [<DataMember(Name = "can_send_video_notes")>]
    CanSendVideoNotes: bool option
    /// True, if the user is allowed to send voice notes
    [<DataMember(Name = "can_send_voice_notes")>]
    CanSendVoiceNotes: bool option
    /// True, if the user is allowed to send polls
    [<DataMember(Name = "can_send_polls")>]
    CanSendPolls: bool option
    /// True, if the user is allowed to send animations, games, stickers and use inline bots
    [<DataMember(Name = "can_send_other_messages")>]
    CanSendOtherMessages: bool option
    /// True, if the user is allowed to add web page previews to their messages
    [<DataMember(Name = "can_add_web_page_previews")>]
    CanAddWebPagePreviews: bool option
    /// True, if the user is allowed to change the chat title, photo and other settings. Ignored in public supergroups
    [<DataMember(Name = "can_change_info")>]
    CanChangeInfo: bool option
    /// True, if the user is allowed to invite new users to the chat
    [<DataMember(Name = "can_invite_users")>]
    CanInviteUsers: bool option
    /// True, if the user is allowed to pin messages. Ignored in public supergroups
    [<DataMember(Name = "can_pin_messages")>]
    CanPinMessages: bool option
    /// True, if the user is allowed to create forum topics. If omitted defaults to the value of can_pin_messages
    [<DataMember(Name = "can_manage_topics")>]
    CanManageTopics: bool option
  }
  static member Create(?canSendMessages: bool, ?canSendAudios: bool, ?canSendDocuments: bool, ?canSendPhotos: bool, ?canSendVideos: bool, ?canSendVideoNotes: bool, ?canSendVoiceNotes: bool, ?canSendPolls: bool, ?canSendOtherMessages: bool, ?canAddWebPagePreviews: bool, ?canChangeInfo: bool, ?canInviteUsers: bool, ?canPinMessages: bool, ?canManageTopics: bool) = 
    {
      CanSendMessages = canSendMessages
      CanSendAudios = canSendAudios
      CanSendDocuments = canSendDocuments
      CanSendPhotos = canSendPhotos
      CanSendVideos = canSendVideos
      CanSendVideoNotes = canSendVideoNotes
      CanSendVoiceNotes = canSendVoiceNotes
      CanSendPolls = canSendPolls
      CanSendOtherMessages = canSendOtherMessages
      CanAddWebPagePreviews = canAddWebPagePreviews
      CanChangeInfo = canChangeInfo
      CanInviteUsers = canInviteUsers
      CanPinMessages = canPinMessages
      CanManageTopics = canManageTopics
    }

/// Describes the birthdate of a user.
and [<CLIMutable>] Birthdate =
  {
    /// Day of the user's birth; 1-31
    [<DataMember(Name = "day")>]
    Day: int64
    /// Month of the user's birth; 1-12
    [<DataMember(Name = "month")>]
    Month: int64
    /// Year of the user's birth
    [<DataMember(Name = "year")>]
    Year: int64 option
  }
  static member Create(day: int64, month: int64, ?year: int64) = 
    {
      Day = day
      Month = month
      Year = year
    }

/// Contains information about the start page settings of a Telegram Business account.
and [<CLIMutable>] BusinessIntro =
  {
    /// Title text of the business intro
    [<DataMember(Name = "title")>]
    Title: string option
    /// Message text of the business intro
    [<DataMember(Name = "message")>]
    Message: string option
    /// Sticker of the business intro
    [<DataMember(Name = "sticker")>]
    Sticker: Sticker option
  }
  static member Create(?title: string, ?message: string, ?sticker: Sticker) = 
    {
      Title = title
      Message = message
      Sticker = sticker
    }

/// Contains information about the location of a Telegram Business account.
and [<CLIMutable>] BusinessLocation =
  {
    /// Address of the business
    [<DataMember(Name = "address")>]
    Address: string
    /// Location of the business
    [<DataMember(Name = "location")>]
    Location: Location option
  }
  static member Create(address: string, ?location: Location) = 
    {
      Address = address
      Location = location
    }

/// Describes an interval of time during which a business is open.
and [<CLIMutable>] BusinessOpeningHoursInterval =
  {
    /// The minute's sequence number in a week, starting on Monday, marking the start of the time interval during which the business is open; 0 - 7 * 24 * 60
    [<DataMember(Name = "opening_minute")>]
    OpeningMinute: int64
    /// The minute's sequence number in a week, starting on Monday, marking the end of the time interval during which the business is open; 0 - 8 * 24 * 60
    [<DataMember(Name = "closing_minute")>]
    ClosingMinute: int64
  }
  static member Create(openingMinute: int64, closingMinute: int64) = 
    {
      OpeningMinute = openingMinute
      ClosingMinute = closingMinute
    }

/// Describes the opening hours of a business.
and [<CLIMutable>] BusinessOpeningHours =
  {
    /// Unique name of the time zone for which the opening hours are defined
    [<DataMember(Name = "time_zone_name")>]
    TimeZoneName: string
    /// List of time intervals describing business opening hours
    [<DataMember(Name = "opening_hours")>]
    OpeningHours: BusinessOpeningHoursInterval[]
  }
  static member Create(timeZoneName: string, openingHours: BusinessOpeningHoursInterval[]) = 
    {
      TimeZoneName = timeZoneName
      OpeningHours = openingHours
    }

/// Represents a location to which a chat is connected.
and [<CLIMutable>] ChatLocation =
  {
    /// The location to which the supergroup is connected. Can't be a live location.
    [<DataMember(Name = "location")>]
    Location: Location
    /// Location address; 1-64 characters, as defined by the chat owner
    [<DataMember(Name = "address")>]
    Address: string
  }
  static member Create(location: Location, address: string) = 
    {
      Location = location
      Address = address
    }

/// This object describes the type of a reaction. Currently, it can be one of
and ReactionType =
  | Emoji of ReactionTypeEmoji
  | CustomEmoji of ReactionTypeCustomEmoji
  | Paid of ReactionTypePaid

/// The reaction is based on an emoji.
and [<CLIMutable>] ReactionTypeEmoji =
  {
    /// Type of the reaction, always “emoji”
    [<DataMember(Name = "type")>]
    Type: string
    /// Reaction emoji. Currently, it can be one of "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", "", ""
    [<DataMember(Name = "emoji")>]
    Emoji: string
  }
  static member Create(``type``: string, emoji: string) = 
    {
      Type = ``type``
      Emoji = emoji
    }

/// The reaction is based on a custom emoji.
and [<CLIMutable>] ReactionTypeCustomEmoji =
  {
    /// Type of the reaction, always “custom_emoji”
    [<DataMember(Name = "type")>]
    Type: string
    /// Custom emoji identifier
    [<DataMember(Name = "custom_emoji_id")>]
    CustomEmojiId: string
  }
  static member Create(``type``: string, customEmojiId: string) = 
    {
      Type = ``type``
      CustomEmojiId = customEmojiId
    }

/// The reaction is paid.
and [<CLIMutable>] ReactionTypePaid =
  {
    /// Type of the reaction, always “paid”
    [<DataMember(Name = "type")>]
    Type: string
  }
  static member Create(``type``: string) = 
    {
      Type = ``type``
    }

/// Represents a reaction added to a message along with the number of times it was added.
and [<CLIMutable>] ReactionCount =
  {
    /// Type of the reaction
    [<DataMember(Name = "type")>]
    Type: ReactionType
    /// Number of times the reaction was added
    [<DataMember(Name = "total_count")>]
    TotalCount: int64
  }
  static member Create(``type``: ReactionType, totalCount: int64) = 
    {
      Type = ``type``
      TotalCount = totalCount
    }

/// This object represents a change of a reaction on a message performed by a user.
and [<CLIMutable>] MessageReactionUpdated =
  {
    /// The chat containing the message the user reacted to
    [<DataMember(Name = "chat")>]
    Chat: Chat
    /// Unique identifier of the message inside the chat
    [<DataMember(Name = "message_id")>]
    MessageId: int64
    /// The user that changed the reaction, if the user isn't anonymous
    [<DataMember(Name = "user")>]
    User: User option
    /// The chat on behalf of which the reaction was changed, if the user is anonymous
    [<DataMember(Name = "actor_chat")>]
    ActorChat: Chat option
    /// Date of the change in Unix time
    [<DataMember(Name = "date")>]
    Date: DateTime
    /// Previous list of reaction types that were set by the user
    [<DataMember(Name = "old_reaction")>]
    OldReaction: ReactionType[]
    /// New list of reaction types that have been set by the user
    [<DataMember(Name = "new_reaction")>]
    NewReaction: ReactionType[]
  }
  static member Create(chat: Chat, messageId: int64, date: DateTime, oldReaction: ReactionType[], newReaction: ReactionType[], ?user: User, ?actorChat: Chat) = 
    {
      Chat = chat
      MessageId = messageId
      Date = date
      OldReaction = oldReaction
      NewReaction = newReaction
      User = user
      ActorChat = actorChat
    }

/// This object represents reaction changes on a message with anonymous reactions.
and [<CLIMutable>] MessageReactionCountUpdated =
  {
    /// The chat containing the message
    [<DataMember(Name = "chat")>]
    Chat: Chat
    /// Unique message identifier inside the chat
    [<DataMember(Name = "message_id")>]
    MessageId: int64
    /// Date of the change in Unix time
    [<DataMember(Name = "date")>]
    Date: DateTime
    /// List of reactions that are present on the message
    [<DataMember(Name = "reactions")>]
    Reactions: ReactionCount[]
  }
  static member Create(chat: Chat, messageId: int64, date: DateTime, reactions: ReactionCount[]) = 
    {
      Chat = chat
      MessageId = messageId
      Date = date
      Reactions = reactions
    }

/// This object represents a forum topic.
and [<CLIMutable>] ForumTopic =
  {
    /// Unique identifier of the forum topic
    [<DataMember(Name = "message_thread_id")>]
    MessageThreadId: int64
    /// Name of the topic
    [<DataMember(Name = "name")>]
    Name: string
    /// Color of the topic icon in RGB format
    [<DataMember(Name = "icon_color")>]
    IconColor: int64
    /// Unique identifier of the custom emoji shown as the topic icon
    [<DataMember(Name = "icon_custom_emoji_id")>]
    IconCustomEmojiId: string option
  }
  static member Create(messageThreadId: int64, name: string, iconColor: int64, ?iconCustomEmojiId: string) = 
    {
      MessageThreadId = messageThreadId
      Name = name
      IconColor = iconColor
      IconCustomEmojiId = iconCustomEmojiId
    }

/// This object represents a bot command.
and [<CLIMutable>] BotCommand =
  {
    /// Text of the command; 1-32 characters. Can contain only lowercase English letters, digits and underscores.
    [<DataMember(Name = "command")>]
    Command: string
    /// Description of the command; 1-256 characters.
    [<DataMember(Name = "description")>]
    Description: string
  }
  static member Create(command: string, description: string) = 
    {
      Command = command
      Description = description
    }

/// This object represents the scope to which bot commands are applied. Currently, the following 7 scopes are supported:
and BotCommandScope =
  | Default of BotCommandScopeDefault
  | AllPrivateChats of BotCommandScopeAllPrivateChats
  | AllGroupChats of BotCommandScopeAllGroupChats
  | AllChatAdministrators of BotCommandScopeAllChatAdministrators
  | Chat of BotCommandScopeChat
  | ChatAdministrators of BotCommandScopeChatAdministrators
  | ChatMember of BotCommandScopeChatMember

/// Represents the default scope of bot commands. Default commands are used if no commands with a narrower scope are specified for the user.
and [<CLIMutable>] BotCommandScopeDefault =
  {
    /// Scope type, must be default
    [<DataMember(Name = "type")>]
    Type: string
  }
  static member Create(``type``: string) = 
    {
      Type = ``type``
    }

/// Represents the scope of bot commands, covering all private chats.
and [<CLIMutable>] BotCommandScopeAllPrivateChats =
  {
    /// Scope type, must be all_private_chats
    [<DataMember(Name = "type")>]
    Type: string
  }
  static member Create(``type``: string) = 
    {
      Type = ``type``
    }

/// Represents the scope of bot commands, covering all group and supergroup chats.
and [<CLIMutable>] BotCommandScopeAllGroupChats =
  {
    /// Scope type, must be all_group_chats
    [<DataMember(Name = "type")>]
    Type: string
  }
  static member Create(``type``: string) = 
    {
      Type = ``type``
    }

/// Represents the scope of bot commands, covering all group and supergroup chat administrators.
and [<CLIMutable>] BotCommandScopeAllChatAdministrators =
  {
    /// Scope type, must be all_chat_administrators
    [<DataMember(Name = "type")>]
    Type: string
  }
  static member Create(``type``: string) = 
    {
      Type = ``type``
    }

/// Represents the scope of bot commands, covering a specific chat.
and [<CLIMutable>] BotCommandScopeChat =
  {
    /// Scope type, must be chat
    [<DataMember(Name = "type")>]
    Type: string
    /// Unique identifier for the target chat or username of the target supergroup (in the format @supergroupusername)
    [<DataMember(Name = "chat_id")>]
    ChatId: ChatId
  }
  static member Create(``type``: string, chatId: ChatId) = 
    {
      Type = ``type``
      ChatId = chatId
    }

/// Represents the scope of bot commands, covering all administrators of a specific group or supergroup chat.
and [<CLIMutable>] BotCommandScopeChatAdministrators =
  {
    /// Scope type, must be chat_administrators
    [<DataMember(Name = "type")>]
    Type: string
    /// Unique identifier for the target chat or username of the target supergroup (in the format @supergroupusername)
    [<DataMember(Name = "chat_id")>]
    ChatId: ChatId
  }
  static member Create(``type``: string, chatId: ChatId) = 
    {
      Type = ``type``
      ChatId = chatId
    }

/// Represents the scope of bot commands, covering a specific member of a group or supergroup chat.
and [<CLIMutable>] BotCommandScopeChatMember =
  {
    /// Scope type, must be chat_member
    [<DataMember(Name = "type")>]
    Type: string
    /// Unique identifier for the target chat or username of the target supergroup (in the format @supergroupusername)
    [<DataMember(Name = "chat_id")>]
    ChatId: ChatId
    /// Unique identifier of the target user
    [<DataMember(Name = "user_id")>]
    UserId: int64
  }
  static member Create(``type``: string, chatId: ChatId, userId: int64) = 
    {
      Type = ``type``
      ChatId = chatId
      UserId = userId
    }

/// This object represents the bot's name.
and [<CLIMutable>] BotName =
  {
    /// The bot's name
    [<DataMember(Name = "name")>]
    Name: string
  }
  static member Create(name: string) = 
    {
      Name = name
    }

/// This object represents the bot's description.
and [<CLIMutable>] BotDescription =
  {
    /// The bot's description
    [<DataMember(Name = "description")>]
    Description: string
  }
  static member Create(description: string) = 
    {
      Description = description
    }

/// This object represents the bot's short description.
and [<CLIMutable>] BotShortDescription =
  {
    /// The bot's short description
    [<DataMember(Name = "short_description")>]
    ShortDescription: string
  }
  static member Create(shortDescription: string) = 
    {
      ShortDescription = shortDescription
    }

/// This object describes the bot's menu button in a private chat. It should be one of
/// If a menu button other than MenuButtonDefault is set for a private chat, then it is applied in the chat. Otherwise the default menu button is applied. By default, the menu button opens the list of bot commands.
and MenuButton =
  | Commands of MenuButtonCommands
  | WebApp of MenuButtonWebApp
  | Default of MenuButtonDefault

/// Represents a menu button, which opens the bot's list of commands.
and [<CLIMutable>] MenuButtonCommands =
  {
    /// Type of the button, must be commands
    [<DataMember(Name = "type")>]
    Type: string
  }
  static member Create(``type``: string) = 
    {
      Type = ``type``
    }

/// Represents a menu button, which launches a Web App.
and [<CLIMutable>] MenuButtonWebApp =
  {
    /// Type of the button, must be web_app
    [<DataMember(Name = "type")>]
    Type: string
    /// Text on the button
    [<DataMember(Name = "text")>]
    Text: string
    /// Description of the Web App that will be launched when the user presses the button. The Web App will be able to send an arbitrary message on behalf of the user using the method answerWebAppQuery. Alternatively, a t.me link to a Web App of the bot can be specified in the object instead of the Web App's URL, in which case the Web App will be opened as if the user pressed the link.
    [<DataMember(Name = "web_app")>]
    WebApp: WebAppInfo
  }
  static member Create(``type``: string, text: string, webApp: WebAppInfo) = 
    {
      Type = ``type``
      Text = text
      WebApp = webApp
    }

/// Describes that no specific value for the menu button was set.
and [<CLIMutable>] MenuButtonDefault =
  {
    /// Type of the button, must be default
    [<DataMember(Name = "type")>]
    Type: string
  }
  static member Create(``type``: string) = 
    {
      Type = ``type``
    }

/// This object describes the source of a chat boost. It can be one of
and ChatBoostSource =
  | Premium of ChatBoostSourcePremium
  | GiftCode of ChatBoostSourceGiftCode
  | Giveaway of ChatBoostSourceGiveaway

/// The boost was obtained by subscribing to Telegram Premium or by gifting a Telegram Premium subscription to another user.
and [<CLIMutable>] ChatBoostSourcePremium =
  {
    /// Source of the boost, always “premium”
    [<DataMember(Name = "source")>]
    Source: string
    /// User that boosted the chat
    [<DataMember(Name = "user")>]
    User: User
  }
  static member Create(source: string, user: User) = 
    {
      Source = source
      User = user
    }

/// The boost was obtained by the creation of Telegram Premium gift codes to boost a chat. Each such code boosts the chat 4 times for the duration of the corresponding Telegram Premium subscription.
and [<CLIMutable>] ChatBoostSourceGiftCode =
  {
    /// Source of the boost, always “gift_code”
    [<DataMember(Name = "source")>]
    Source: string
    /// User for which the gift code was created
    [<DataMember(Name = "user")>]
    User: User
  }
  static member Create(source: string, user: User) = 
    {
      Source = source
      User = user
    }

/// The boost was obtained by the creation of a Telegram Premium or a Telegram Star giveaway. This boosts the chat 4 times for the duration of the corresponding Telegram Premium subscription for Telegram Premium giveaways and prize_star_count / 500 times for one year for Telegram Star giveaways.
and [<CLIMutable>] ChatBoostSourceGiveaway =
  {
    /// Source of the boost, always “giveaway”
    [<DataMember(Name = "source")>]
    Source: string
    /// Identifier of a message in the chat with the giveaway; the message could have been deleted already. May be 0 if the message isn't sent yet.
    [<DataMember(Name = "giveaway_message_id")>]
    GiveawayMessageId: int64
    /// User that won the prize in the giveaway if any; for Telegram Premium giveaways only
    [<DataMember(Name = "user")>]
    User: User option
    /// The number of Telegram Stars to be split between giveaway winners; for Telegram Star giveaways only
    [<DataMember(Name = "prize_star_count")>]
    PrizeStarCount: int64 option
    /// True, if the giveaway was completed, but there was no user to win the prize
    [<DataMember(Name = "is_unclaimed")>]
    IsUnclaimed: bool option
  }
  static member Create(source: string, giveawayMessageId: int64, ?user: User, ?prizeStarCount: int64, ?isUnclaimed: bool) = 
    {
      Source = source
      GiveawayMessageId = giveawayMessageId
      User = user
      PrizeStarCount = prizeStarCount
      IsUnclaimed = isUnclaimed
    }

/// This object contains information about a chat boost.
and [<CLIMutable>] ChatBoost =
  {
    /// Unique identifier of the boost
    [<DataMember(Name = "boost_id")>]
    BoostId: string
    /// Point in time (Unix timestamp) when the chat was boosted
    [<DataMember(Name = "add_date")>]
    AddDate: int64
    /// Point in time (Unix timestamp) when the boost will automatically expire, unless the booster's Telegram Premium subscription is prolonged
    [<DataMember(Name = "expiration_date")>]
    ExpirationDate: int64
    /// Source of the added boost
    [<DataMember(Name = "source")>]
    Source: ChatBoostSource
  }
  static member Create(boostId: string, addDate: int64, expirationDate: int64, source: ChatBoostSource) = 
    {
      BoostId = boostId
      AddDate = addDate
      ExpirationDate = expirationDate
      Source = source
    }

/// This object represents a boost added to a chat or changed.
and [<CLIMutable>] ChatBoostUpdated =
  {
    /// Chat which was boosted
    [<DataMember(Name = "chat")>]
    Chat: Chat
    /// Information about the chat boost
    [<DataMember(Name = "boost")>]
    Boost: ChatBoost
  }
  static member Create(chat: Chat, boost: ChatBoost) = 
    {
      Chat = chat
      Boost = boost
    }

/// This object represents a boost removed from a chat.
and [<CLIMutable>] ChatBoostRemoved =
  {
    /// Chat which was boosted
    [<DataMember(Name = "chat")>]
    Chat: Chat
    /// Unique identifier of the boost
    [<DataMember(Name = "boost_id")>]
    BoostId: string
    /// Point in time (Unix timestamp) when the boost was removed
    [<DataMember(Name = "remove_date")>]
    RemoveDate: int64
    /// Source of the removed boost
    [<DataMember(Name = "source")>]
    Source: ChatBoostSource
  }
  static member Create(chat: Chat, boostId: string, removeDate: int64, source: ChatBoostSource) = 
    {
      Chat = chat
      BoostId = boostId
      RemoveDate = removeDate
      Source = source
    }

/// This object represents a list of boosts added to a chat by a user.
and [<CLIMutable>] UserChatBoosts =
  {
    /// The list of boosts added to the chat by the user
    [<DataMember(Name = "boosts")>]
    Boosts: ChatBoost[]
  }
  static member Create(boosts: ChatBoost[]) = 
    {
      Boosts = boosts
    }

/// Describes the connection of the bot with a business account.
and [<CLIMutable>] BusinessConnection =
  {
    /// Unique identifier of the business connection
    [<DataMember(Name = "id")>]
    Id: string
    /// Business account user that created the business connection
    [<DataMember(Name = "user")>]
    User: User
    /// Identifier of a private chat with the user who created the business connection. This number may have more than 32 significant bits and some programming languages may have difficulty/silent defects in interpreting it. But it has at most 52 significant bits, so a 64-bit integer or double-precision float type are safe for storing this identifier.
    [<DataMember(Name = "user_chat_id")>]
    UserChatId: int64
    /// Date the connection was established in Unix time
    [<DataMember(Name = "date")>]
    Date: DateTime
    /// True, if the bot can act on behalf of the business account in chats that were active in the last 24 hours
    [<DataMember(Name = "can_reply")>]
    CanReply: bool
    /// True, if the connection is active
    [<DataMember(Name = "is_enabled")>]
    IsEnabled: bool
  }
  static member Create(id: string, user: User, userChatId: int64, date: DateTime, canReply: bool, isEnabled: bool) = 
    {
      Id = id
      User = user
      UserChatId = userChatId
      Date = date
      CanReply = canReply
      IsEnabled = isEnabled
    }

/// This object is received when messages are deleted from a connected business account.
and [<CLIMutable>] BusinessMessagesDeleted =
  {
    /// Unique identifier of the business connection
    [<DataMember(Name = "business_connection_id")>]
    BusinessConnectionId: string
    /// Information about a chat in the business account. The bot may not have access to the chat or the corresponding user.
    [<DataMember(Name = "chat")>]
    Chat: Chat
    /// The list of identifiers of deleted messages in the chat of the business account
    [<DataMember(Name = "message_ids")>]
    MessageIds: int64[]
  }
  static member Create(businessConnectionId: string, chat: Chat, messageIds: int64[]) = 
    {
      BusinessConnectionId = businessConnectionId
      Chat = chat
      MessageIds = messageIds
    }

/// Describes why a request was unsuccessful.
and [<CLIMutable>] ResponseParameters =
  {
    /// The group has been migrated to a supergroup with the specified identifier. This number may have more than 32 significant bits and some programming languages may have difficulty/silent defects in interpreting it. But it has at most 52 significant bits, so a signed 64-bit integer or double-precision float type are safe for storing this identifier.
    [<DataMember(Name = "migrate_to_chat_id")>]
    MigrateToChatId: int64 option
    /// In case of exceeding flood control, the number of seconds left to wait before the request can be repeated
    [<DataMember(Name = "retry_after")>]
    RetryAfter: int64 option
  }
  static member Create(?migrateToChatId: int64, ?retryAfter: int64) = 
    {
      MigrateToChatId = migrateToChatId
      RetryAfter = retryAfter
    }

/// This object represents the content of a media message to be sent. It should be one of
and InputMedia =
  | Animation of InputMediaAnimation
  | Document of InputMediaDocument
  | Audio of InputMediaAudio
  | Photo of InputMediaPhoto
  | Video of InputMediaVideo

/// Represents a photo to be sent.
and [<CLIMutable>] InputMediaPhoto =
  {
    /// Type of the result, must be photo
    [<DataMember(Name = "type")>]
    Type: string
    /// File to send. Pass a file_id to send a file that exists on the Telegram servers (recommended), pass an HTTP URL for Telegram to get a file from the Internet, or pass “attach://<file_attach_name>” to upload a new one using multipart/form-data under <file_attach_name> name. More information on Sending Files »
    [<DataMember(Name = "media")>]
    Media: InputFile
    /// Caption of the photo to be sent, 0-1024 characters after entities parsing
    [<DataMember(Name = "caption")>]
    Caption: string option
    /// Mode for parsing entities in the photo caption. See formatting options for more details.
    [<DataMember(Name = "parse_mode")>]
    ParseMode: ParseMode option
    /// List of special entities that appear in the caption, which can be specified instead of parse_mode
    [<DataMember(Name = "caption_entities")>]
    CaptionEntities: MessageEntity[] option
    /// Pass True, if the caption must be shown above the message media
    [<DataMember(Name = "show_caption_above_media")>]
    ShowCaptionAboveMedia: bool option
    /// Pass True if the photo needs to be covered with a spoiler animation
    [<DataMember(Name = "has_spoiler")>]
    HasSpoiler: bool option
  }
  static member Create(``type``: string, media: InputFile, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?showCaptionAboveMedia: bool, ?hasSpoiler: bool) = 
    {
      Type = ``type``
      Media = media
      Caption = caption
      ParseMode = parseMode
      CaptionEntities = captionEntities
      ShowCaptionAboveMedia = showCaptionAboveMedia
      HasSpoiler = hasSpoiler
    }

/// Represents a video to be sent.
and [<CLIMutable>] InputMediaVideo =
  {
    /// Type of the result, must be video
    [<DataMember(Name = "type")>]
    Type: string
    /// File to send. Pass a file_id to send a file that exists on the Telegram servers (recommended), pass an HTTP URL for Telegram to get a file from the Internet, or pass “attach://<file_attach_name>” to upload a new one using multipart/form-data under <file_attach_name> name. More information on Sending Files »
    [<DataMember(Name = "media")>]
    Media: InputFile
    /// Thumbnail of the file sent; can be ignored if thumbnail generation for the file is supported server-side. The thumbnail should be in JPEG format and less than 200 kB in size. A thumbnail's width and height should not exceed 320. Ignored if the file is not uploaded using multipart/form-data. Thumbnails can't be reused and can be only uploaded as a new file, so you can pass “attach://<file_attach_name>” if the thumbnail was uploaded using multipart/form-data under <file_attach_name>. More information on Sending Files »
    [<DataMember(Name = "thumbnail")>]
    Thumbnail: InputFile option
    /// Caption of the video to be sent, 0-1024 characters after entities parsing
    [<DataMember(Name = "caption")>]
    Caption: string option
    /// Mode for parsing entities in the video caption. See formatting options for more details.
    [<DataMember(Name = "parse_mode")>]
    ParseMode: ParseMode option
    /// List of special entities that appear in the caption, which can be specified instead of parse_mode
    [<DataMember(Name = "caption_entities")>]
    CaptionEntities: MessageEntity[] option
    /// Pass True, if the caption must be shown above the message media
    [<DataMember(Name = "show_caption_above_media")>]
    ShowCaptionAboveMedia: bool option
    /// Video width
    [<DataMember(Name = "width")>]
    Width: int64 option
    /// Video height
    [<DataMember(Name = "height")>]
    Height: int64 option
    /// Video duration in seconds
    [<DataMember(Name = "duration")>]
    Duration: int64 option
    /// Pass True if the uploaded video is suitable for streaming
    [<DataMember(Name = "supports_streaming")>]
    SupportsStreaming: bool option
    /// Pass True if the video needs to be covered with a spoiler animation
    [<DataMember(Name = "has_spoiler")>]
    HasSpoiler: bool option
  }
  static member Create(``type``: string, media: InputFile, ?thumbnail: InputFile, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?showCaptionAboveMedia: bool, ?width: int64, ?height: int64, ?duration: int64, ?supportsStreaming: bool, ?hasSpoiler: bool) = 
    {
      Type = ``type``
      Media = media
      Thumbnail = thumbnail
      Caption = caption
      ParseMode = parseMode
      CaptionEntities = captionEntities
      ShowCaptionAboveMedia = showCaptionAboveMedia
      Width = width
      Height = height
      Duration = duration
      SupportsStreaming = supportsStreaming
      HasSpoiler = hasSpoiler
    }

/// Represents an animation file (GIF or H.264/MPEG-4 AVC video without sound) to be sent.
and [<CLIMutable>] InputMediaAnimation =
  {
    /// Type of the result, must be animation
    [<DataMember(Name = "type")>]
    Type: string
    /// File to send. Pass a file_id to send a file that exists on the Telegram servers (recommended), pass an HTTP URL for Telegram to get a file from the Internet, or pass “attach://<file_attach_name>” to upload a new one using multipart/form-data under <file_attach_name> name. More information on Sending Files »
    [<DataMember(Name = "media")>]
    Media: InputFile
    /// Thumbnail of the file sent; can be ignored if thumbnail generation for the file is supported server-side. The thumbnail should be in JPEG format and less than 200 kB in size. A thumbnail's width and height should not exceed 320. Ignored if the file is not uploaded using multipart/form-data. Thumbnails can't be reused and can be only uploaded as a new file, so you can pass “attach://<file_attach_name>” if the thumbnail was uploaded using multipart/form-data under <file_attach_name>. More information on Sending Files »
    [<DataMember(Name = "thumbnail")>]
    Thumbnail: InputFile option
    /// Caption of the animation to be sent, 0-1024 characters after entities parsing
    [<DataMember(Name = "caption")>]
    Caption: string option
    /// Mode for parsing entities in the animation caption. See formatting options for more details.
    [<DataMember(Name = "parse_mode")>]
    ParseMode: ParseMode option
    /// List of special entities that appear in the caption, which can be specified instead of parse_mode
    [<DataMember(Name = "caption_entities")>]
    CaptionEntities: MessageEntity[] option
    /// Pass True, if the caption must be shown above the message media
    [<DataMember(Name = "show_caption_above_media")>]
    ShowCaptionAboveMedia: bool option
    /// Animation width
    [<DataMember(Name = "width")>]
    Width: int64 option
    /// Animation height
    [<DataMember(Name = "height")>]
    Height: int64 option
    /// Animation duration in seconds
    [<DataMember(Name = "duration")>]
    Duration: int64 option
    /// Pass True if the animation needs to be covered with a spoiler animation
    [<DataMember(Name = "has_spoiler")>]
    HasSpoiler: bool option
  }
  static member Create(``type``: string, media: InputFile, ?thumbnail: InputFile, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?showCaptionAboveMedia: bool, ?width: int64, ?height: int64, ?duration: int64, ?hasSpoiler: bool) = 
    {
      Type = ``type``
      Media = media
      Thumbnail = thumbnail
      Caption = caption
      ParseMode = parseMode
      CaptionEntities = captionEntities
      ShowCaptionAboveMedia = showCaptionAboveMedia
      Width = width
      Height = height
      Duration = duration
      HasSpoiler = hasSpoiler
    }

/// Represents an audio file to be treated as music to be sent.
and [<CLIMutable>] InputMediaAudio =
  {
    /// Type of the result, must be audio
    [<DataMember(Name = "type")>]
    Type: string
    /// File to send. Pass a file_id to send a file that exists on the Telegram servers (recommended), pass an HTTP URL for Telegram to get a file from the Internet, or pass “attach://<file_attach_name>” to upload a new one using multipart/form-data under <file_attach_name> name. More information on Sending Files »
    [<DataMember(Name = "media")>]
    Media: InputFile
    /// Thumbnail of the file sent; can be ignored if thumbnail generation for the file is supported server-side. The thumbnail should be in JPEG format and less than 200 kB in size. A thumbnail's width and height should not exceed 320. Ignored if the file is not uploaded using multipart/form-data. Thumbnails can't be reused and can be only uploaded as a new file, so you can pass “attach://<file_attach_name>” if the thumbnail was uploaded using multipart/form-data under <file_attach_name>. More information on Sending Files »
    [<DataMember(Name = "thumbnail")>]
    Thumbnail: InputFile option
    /// Caption of the audio to be sent, 0-1024 characters after entities parsing
    [<DataMember(Name = "caption")>]
    Caption: string option
    /// Mode for parsing entities in the audio caption. See formatting options for more details.
    [<DataMember(Name = "parse_mode")>]
    ParseMode: ParseMode option
    /// List of special entities that appear in the caption, which can be specified instead of parse_mode
    [<DataMember(Name = "caption_entities")>]
    CaptionEntities: MessageEntity[] option
    /// Duration of the audio in seconds
    [<DataMember(Name = "duration")>]
    Duration: int64 option
    /// Performer of the audio
    [<DataMember(Name = "performer")>]
    Performer: string option
    /// Title of the audio
    [<DataMember(Name = "title")>]
    Title: string option
  }
  static member Create(``type``: string, media: InputFile, ?thumbnail: InputFile, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?duration: int64, ?performer: string, ?title: string) = 
    {
      Type = ``type``
      Media = media
      Thumbnail = thumbnail
      Caption = caption
      ParseMode = parseMode
      CaptionEntities = captionEntities
      Duration = duration
      Performer = performer
      Title = title
    }

/// Represents a general file to be sent.
and [<CLIMutable>] InputMediaDocument =
  {
    /// Type of the result, must be document
    [<DataMember(Name = "type")>]
    Type: string
    /// File to send. Pass a file_id to send a file that exists on the Telegram servers (recommended), pass an HTTP URL for Telegram to get a file from the Internet, or pass “attach://<file_attach_name>” to upload a new one using multipart/form-data under <file_attach_name> name. More information on Sending Files »
    [<DataMember(Name = "media")>]
    Media: InputFile
    /// Thumbnail of the file sent; can be ignored if thumbnail generation for the file is supported server-side. The thumbnail should be in JPEG format and less than 200 kB in size. A thumbnail's width and height should not exceed 320. Ignored if the file is not uploaded using multipart/form-data. Thumbnails can't be reused and can be only uploaded as a new file, so you can pass “attach://<file_attach_name>” if the thumbnail was uploaded using multipart/form-data under <file_attach_name>. More information on Sending Files »
    [<DataMember(Name = "thumbnail")>]
    Thumbnail: InputFile option
    /// Caption of the document to be sent, 0-1024 characters after entities parsing
    [<DataMember(Name = "caption")>]
    Caption: string option
    /// Mode for parsing entities in the document caption. See formatting options for more details.
    [<DataMember(Name = "parse_mode")>]
    ParseMode: ParseMode option
    /// List of special entities that appear in the caption, which can be specified instead of parse_mode
    [<DataMember(Name = "caption_entities")>]
    CaptionEntities: MessageEntity[] option
    /// Disables automatic server-side content type detection for files uploaded using multipart/form-data. Always True, if the document is sent as part of an album.
    [<DataMember(Name = "disable_content_type_detection")>]
    DisableContentTypeDetection: bool option
  }
  static member Create(``type``: string, media: InputFile, ?thumbnail: InputFile, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?disableContentTypeDetection: bool) = 
    {
      Type = ``type``
      Media = media
      Thumbnail = thumbnail
      Caption = caption
      ParseMode = parseMode
      CaptionEntities = captionEntities
      DisableContentTypeDetection = disableContentTypeDetection
    }

/// This object describes the paid media to be sent. Currently, it can be one of
and InputPaidMedia =
  | Photo of InputPaidMediaPhoto
  | Video of InputPaidMediaVideo

/// The paid media to send is a photo.
and [<CLIMutable>] InputPaidMediaPhoto =
  {
    /// Type of the media, must be photo
    [<DataMember(Name = "type")>]
    Type: string
    /// File to send. Pass a file_id to send a file that exists on the Telegram servers (recommended), pass an HTTP URL for Telegram to get a file from the Internet, or pass “attach://<file_attach_name>” to upload a new one using multipart/form-data under <file_attach_name> name. More information on Sending Files »
    [<DataMember(Name = "media")>]
    Media: InputFile
  }
  static member Create(``type``: string, media: InputFile) = 
    {
      Type = ``type``
      Media = media
    }

/// The paid media to send is a video.
and [<CLIMutable>] InputPaidMediaVideo =
  {
    /// Type of the media, must be video
    [<DataMember(Name = "type")>]
    Type: string
    /// File to send. Pass a file_id to send a file that exists on the Telegram servers (recommended), pass an HTTP URL for Telegram to get a file from the Internet, or pass “attach://<file_attach_name>” to upload a new one using multipart/form-data under <file_attach_name> name. More information on Sending Files »
    [<DataMember(Name = "media")>]
    Media: InputFile
    /// Thumbnail of the file sent; can be ignored if thumbnail generation for the file is supported server-side. The thumbnail should be in JPEG format and less than 200 kB in size. A thumbnail's width and height should not exceed 320. Ignored if the file is not uploaded using multipart/form-data. Thumbnails can't be reused and can be only uploaded as a new file, so you can pass “attach://<file_attach_name>” if the thumbnail was uploaded using multipart/form-data under <file_attach_name>. More information on Sending Files »
    [<DataMember(Name = "thumbnail")>]
    Thumbnail: InputFile option
    /// Video width
    [<DataMember(Name = "width")>]
    Width: int64 option
    /// Video height
    [<DataMember(Name = "height")>]
    Height: int64 option
    /// Video duration in seconds
    [<DataMember(Name = "duration")>]
    Duration: int64 option
    /// Pass True if the uploaded video is suitable for streaming
    [<DataMember(Name = "supports_streaming")>]
    SupportsStreaming: bool option
  }
  static member Create(``type``: string, media: InputFile, ?thumbnail: InputFile, ?width: int64, ?height: int64, ?duration: int64, ?supportsStreaming: bool) = 
    {
      Type = ``type``
      Media = media
      Thumbnail = thumbnail
      Width = width
      Height = height
      Duration = duration
      SupportsStreaming = supportsStreaming
    }

/// This object represents a sticker.
and [<CLIMutable>] Sticker =
  {
    /// Identifier for this file, which can be used to download or reuse the file
    [<DataMember(Name = "file_id")>]
    FileId: string
    /// Unique identifier for this file, which is supposed to be the same over time and for different bots. Can't be used to download or reuse the file.
    [<DataMember(Name = "file_unique_id")>]
    FileUniqueId: string
    /// Type of the sticker, currently one of “regular”, “mask”, “custom_emoji”. The type of the sticker is independent from its format, which is determined by the fields is_animated and is_video.
    [<DataMember(Name = "type")>]
    Type: string
    /// Sticker width
    [<DataMember(Name = "width")>]
    Width: int64
    /// Sticker height
    [<DataMember(Name = "height")>]
    Height: int64
    /// True, if the sticker is animated
    [<DataMember(Name = "is_animated")>]
    IsAnimated: bool
    /// True, if the sticker is a video sticker
    [<DataMember(Name = "is_video")>]
    IsVideo: bool
    /// Sticker thumbnail in the .WEBP or .JPG format
    [<DataMember(Name = "thumbnail")>]
    Thumbnail: PhotoSize option
    /// Emoji associated with the sticker
    [<DataMember(Name = "emoji")>]
    Emoji: string option
    /// Name of the sticker set to which the sticker belongs
    [<DataMember(Name = "set_name")>]
    SetName: string option
    /// For premium regular stickers, premium animation for the sticker
    [<DataMember(Name = "premium_animation")>]
    PremiumAnimation: File option
    /// For mask stickers, the position where the mask should be placed
    [<DataMember(Name = "mask_position")>]
    MaskPosition: MaskPosition option
    /// For custom emoji stickers, unique identifier of the custom emoji
    [<DataMember(Name = "custom_emoji_id")>]
    CustomEmojiId: string option
    /// True, if the sticker must be repainted to a text color in messages, the color of the Telegram Premium badge in emoji status, white color on chat photos, or another appropriate color in other places
    [<DataMember(Name = "needs_repainting")>]
    NeedsRepainting: bool option
    /// File size in bytes
    [<DataMember(Name = "file_size")>]
    FileSize: int64 option
  }
  static member Create(fileId: string, fileUniqueId: string, ``type``: string, width: int64, height: int64, isAnimated: bool, isVideo: bool, ?thumbnail: PhotoSize, ?emoji: string, ?setName: string, ?premiumAnimation: File, ?maskPosition: MaskPosition, ?customEmojiId: string, ?needsRepainting: bool, ?fileSize: int64) = 
    {
      FileId = fileId
      FileUniqueId = fileUniqueId
      Type = ``type``
      Width = width
      Height = height
      IsAnimated = isAnimated
      IsVideo = isVideo
      Thumbnail = thumbnail
      Emoji = emoji
      SetName = setName
      PremiumAnimation = premiumAnimation
      MaskPosition = maskPosition
      CustomEmojiId = customEmojiId
      NeedsRepainting = needsRepainting
      FileSize = fileSize
    }

/// This object represents a sticker set.
and [<CLIMutable>] StickerSet =
  {
    /// Sticker set name
    [<DataMember(Name = "name")>]
    Name: string
    /// Sticker set title
    [<DataMember(Name = "title")>]
    Title: string
    /// Type of stickers in the set, currently one of “regular”, “mask”, “custom_emoji”
    [<DataMember(Name = "sticker_type")>]
    StickerType: string
    /// List of all set stickers
    [<DataMember(Name = "stickers")>]
    Stickers: Sticker[]
    /// Sticker set thumbnail in the .WEBP, .TGS, or .WEBM format
    [<DataMember(Name = "thumbnail")>]
    Thumbnail: PhotoSize option
  }
  static member Create(name: string, title: string, stickerType: string, stickers: Sticker[], ?thumbnail: PhotoSize) = 
    {
      Name = name
      Title = title
      StickerType = stickerType
      Stickers = stickers
      Thumbnail = thumbnail
    }

/// This object describes the position on faces where a mask should be placed by default.
and [<CLIMutable>] MaskPosition =
  {
    /// The part of the face relative to which the mask should be placed. One of “forehead”, “eyes”, “mouth”, or “chin”.
    [<DataMember(Name = "point")>]
    Point: MaskPoint
    /// Shift by X-axis measured in widths of the mask scaled to the face size, from left to right. For example, choosing -1.0 will place mask just to the left of the default mask position.
    [<DataMember(Name = "x_shift")>]
    XShift: float
    /// Shift by Y-axis measured in heights of the mask scaled to the face size, from top to bottom. For example, 1.0 will place the mask just below the default mask position.
    [<DataMember(Name = "y_shift")>]
    YShift: float
    /// Mask scaling coefficient. For example, 2.0 means double size.
    [<DataMember(Name = "scale")>]
    Scale: float
  }
  static member Create(point: MaskPoint, xShift: float, yShift: float, scale: float) = 
    {
      Point = point
      XShift = xShift
      YShift = yShift
      Scale = scale
    }

/// This object describes a sticker to be added to a sticker set.
and [<CLIMutable>] InputSticker =
  {
    /// The added sticker. Pass a file_id as a String to send a file that already exists on the Telegram servers, pass an HTTP URL as a String for Telegram to get a file from the Internet, upload a new one using multipart/form-data, or pass “attach://<file_attach_name>” to upload a new one using multipart/form-data under <file_attach_name> name. Animated and video stickers can't be uploaded via HTTP URL. More information on Sending Files »
    [<DataMember(Name = "sticker")>]
    Sticker: InputFile
    /// Format of the added sticker, must be one of “static” for a .WEBP or .PNG image, “animated” for a .TGS animation, “video” for a WEBM video
    [<DataMember(Name = "format")>]
    Format: string
    /// List of 1-20 emoji associated with the sticker
    [<DataMember(Name = "emoji_list")>]
    EmojiList: string[]
    /// Position where the mask should be placed on faces. For “mask” stickers only.
    [<DataMember(Name = "mask_position")>]
    MaskPosition: MaskPosition option
    /// List of 0-20 search keywords for the sticker with total length of up to 64 characters. For “regular” and “custom_emoji” stickers only.
    [<DataMember(Name = "keywords")>]
    Keywords: string[] option
  }
  static member Create(sticker: InputFile, format: string, emojiList: string[], ?maskPosition: MaskPosition, ?keywords: string[]) = 
    {
      Sticker = sticker
      Format = format
      EmojiList = emojiList
      MaskPosition = maskPosition
      Keywords = keywords
    }

/// This object represents an incoming inline query. When the user sends an empty query, your bot could return some default or trending results.
and [<CLIMutable>] InlineQuery =
  {
    /// Unique identifier for this query
    [<DataMember(Name = "id")>]
    Id: string
    /// Sender
    [<DataMember(Name = "from")>]
    From: User
    /// Text of the query (up to 256 characters)
    [<DataMember(Name = "query")>]
    Query: string
    /// Offset of the results to be returned, can be controlled by the bot
    [<DataMember(Name = "offset")>]
    Offset: string
    /// Type of the chat from which the inline query was sent. Can be either “sender” for a private chat with the inline query sender, “private”, “group”, “supergroup”, or “channel”. The chat type should be always known for requests sent from official clients and most third-party clients, unless the request was sent from a secret chat
    [<DataMember(Name = "chat_type")>]
    ChatType: ChatType option
    /// Sender location, only for bots that request user location
    [<DataMember(Name = "location")>]
    Location: Location option
  }
  static member Create(id: string, from: User, query: string, offset: string, ?chatType: ChatType, ?location: Location) = 
    {
      Id = id
      From = from
      Query = query
      Offset = offset
      ChatType = chatType
      Location = location
    }

/// This object represents a button to be shown above inline query results. You must use exactly one of the optional fields.
and [<CLIMutable>] InlineQueryResultsButton =
  {
    /// Label text on the button
    [<DataMember(Name = "text")>]
    Text: string
    /// Description of the Web App that will be launched when the user presses the button. The Web App will be able to switch back to the inline mode using the method switchInlineQuery inside the Web App.
    [<DataMember(Name = "web_app")>]
    WebApp: WebAppInfo option
    /// Deep-linking parameter for the /start message sent to the bot when a user presses the button. 1-64 characters, only A-Z, a-z, 0-9, _ and - are allowed.
    /// 
    /// Example: An inline bot that sends YouTube videos can ask the user to connect the bot to their YouTube account to adapt search results accordingly. To do this, it displays a 'Connect your YouTube account' button above the results, or even before showing any. The user presses the button, switches to a private chat with the bot and, in doing so, passes a start parameter that instructs the bot to return an OAuth link. Once done, the bot can offer a switch_inline button so that the user can easily return to the chat where they wanted to use the bot's inline capabilities.
    [<DataMember(Name = "start_parameter")>]
    StartParameter: string option
  }
  static member Create(text: string, ?webApp: WebAppInfo, ?startParameter: string) = 
    {
      Text = text
      WebApp = webApp
      StartParameter = startParameter
    }

/// This object represents one result of an inline query. Telegram clients currently support results of the following 20 types:
/// Note: All URLs passed in inline query results will be available to end users and therefore must be assumed to be public.
and InlineQueryResult =
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

/// Represents a link to an article or web page.
and [<CLIMutable>] InlineQueryResultArticle =
  {
    /// Type of the result, must be article
    [<DataMember(Name = "type")>]
    Type: string
    /// Unique identifier for this result, 1-64 Bytes
    [<DataMember(Name = "id")>]
    Id: string
    /// Title of the result
    [<DataMember(Name = "title")>]
    Title: string
    /// Content of the message to be sent
    [<DataMember(Name = "input_message_content")>]
    InputMessageContent: InputMessageContent
    /// Inline keyboard attached to the message
    [<DataMember(Name = "reply_markup")>]
    ReplyMarkup: InlineKeyboardMarkup option
    /// URL of the result
    [<DataMember(Name = "url")>]
    Url: string option
    /// Pass True if you don't want the URL to be shown in the message
    [<DataMember(Name = "hide_url")>]
    HideUrl: bool option
    /// Short description of the result
    [<DataMember(Name = "description")>]
    Description: string option
    /// Url of the thumbnail for the result
    [<DataMember(Name = "thumbnail_url")>]
    ThumbnailUrl: string option
    /// Thumbnail width
    [<DataMember(Name = "thumbnail_width")>]
    ThumbnailWidth: int64 option
    /// Thumbnail height
    [<DataMember(Name = "thumbnail_height")>]
    ThumbnailHeight: int64 option
  }
  static member Create(``type``: string, id: string, title: string, inputMessageContent: InputMessageContent, ?replyMarkup: InlineKeyboardMarkup, ?url: string, ?hideUrl: bool, ?description: string, ?thumbnailUrl: string, ?thumbnailWidth: int64, ?thumbnailHeight: int64) = 
    {
      Type = ``type``
      Id = id
      Title = title
      InputMessageContent = inputMessageContent
      ReplyMarkup = replyMarkup
      Url = url
      HideUrl = hideUrl
      Description = description
      ThumbnailUrl = thumbnailUrl
      ThumbnailWidth = thumbnailWidth
      ThumbnailHeight = thumbnailHeight
    }

/// Represents a link to a photo. By default, this photo will be sent by the user with optional caption. Alternatively, you can use input_message_content to send a message with the specified content instead of the photo.
and [<CLIMutable>] InlineQueryResultPhoto =
  {
    /// Type of the result, must be photo
    [<DataMember(Name = "type")>]
    Type: string
    /// Unique identifier for this result, 1-64 bytes
    [<DataMember(Name = "id")>]
    Id: string
    /// A valid URL of the photo. Photo must be in JPEG format. Photo size must not exceed 5MB
    [<DataMember(Name = "photo_url")>]
    PhotoUrl: string
    /// URL of the thumbnail for the photo
    [<DataMember(Name = "thumbnail_url")>]
    ThumbnailUrl: string
    /// Width of the photo
    [<DataMember(Name = "photo_width")>]
    PhotoWidth: int64 option
    /// Height of the photo
    [<DataMember(Name = "photo_height")>]
    PhotoHeight: int64 option
    /// Title for the result
    [<DataMember(Name = "title")>]
    Title: string option
    /// Short description of the result
    [<DataMember(Name = "description")>]
    Description: string option
    /// Caption of the photo to be sent, 0-1024 characters after entities parsing
    [<DataMember(Name = "caption")>]
    Caption: string option
    /// Mode for parsing entities in the photo caption. See formatting options for more details.
    [<DataMember(Name = "parse_mode")>]
    ParseMode: ParseMode option
    /// List of special entities that appear in the caption, which can be specified instead of parse_mode
    [<DataMember(Name = "caption_entities")>]
    CaptionEntities: MessageEntity[] option
    /// Pass True, if the caption must be shown above the message media
    [<DataMember(Name = "show_caption_above_media")>]
    ShowCaptionAboveMedia: bool option
    /// Inline keyboard attached to the message
    [<DataMember(Name = "reply_markup")>]
    ReplyMarkup: InlineKeyboardMarkup option
    /// Content of the message to be sent instead of the photo
    [<DataMember(Name = "input_message_content")>]
    InputMessageContent: InputMessageContent option
  }
  static member Create(``type``: string, id: string, photoUrl: string, thumbnailUrl: string, ?photoWidth: int64, ?photoHeight: int64, ?title: string, ?description: string, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?showCaptionAboveMedia: bool, ?replyMarkup: InlineKeyboardMarkup, ?inputMessageContent: InputMessageContent) = 
    {
      Type = ``type``
      Id = id
      PhotoUrl = photoUrl
      ThumbnailUrl = thumbnailUrl
      PhotoWidth = photoWidth
      PhotoHeight = photoHeight
      Title = title
      Description = description
      Caption = caption
      ParseMode = parseMode
      CaptionEntities = captionEntities
      ShowCaptionAboveMedia = showCaptionAboveMedia
      ReplyMarkup = replyMarkup
      InputMessageContent = inputMessageContent
    }

/// Represents a link to an animated GIF file. By default, this animated GIF file will be sent by the user with optional caption. Alternatively, you can use input_message_content to send a message with the specified content instead of the animation.
and [<CLIMutable>] InlineQueryResultGif =
  {
    /// Type of the result, must be gif
    [<DataMember(Name = "type")>]
    Type: string
    /// Unique identifier for this result, 1-64 bytes
    [<DataMember(Name = "id")>]
    Id: string
    /// A valid URL for the GIF file. File size must not exceed 1MB
    [<DataMember(Name = "gif_url")>]
    GifUrl: string
    /// Width of the GIF
    [<DataMember(Name = "gif_width")>]
    GifWidth: int64 option
    /// Height of the GIF
    [<DataMember(Name = "gif_height")>]
    GifHeight: int64 option
    /// Duration of the GIF in seconds
    [<DataMember(Name = "gif_duration")>]
    GifDuration: int64 option
    /// URL of the static (JPEG or GIF) or animated (MPEG4) thumbnail for the result
    [<DataMember(Name = "thumbnail_url")>]
    ThumbnailUrl: string
    /// MIME type of the thumbnail, must be one of “image/jpeg”, “image/gif”, or “video/mp4”. Defaults to “image/jpeg”
    [<DataMember(Name = "thumbnail_mime_type")>]
    ThumbnailMimeType: string option
    /// Title for the result
    [<DataMember(Name = "title")>]
    Title: string option
    /// Caption of the GIF file to be sent, 0-1024 characters after entities parsing
    [<DataMember(Name = "caption")>]
    Caption: string option
    /// Mode for parsing entities in the caption. See formatting options for more details.
    [<DataMember(Name = "parse_mode")>]
    ParseMode: ParseMode option
    /// List of special entities that appear in the caption, which can be specified instead of parse_mode
    [<DataMember(Name = "caption_entities")>]
    CaptionEntities: MessageEntity[] option
    /// Pass True, if the caption must be shown above the message media
    [<DataMember(Name = "show_caption_above_media")>]
    ShowCaptionAboveMedia: bool option
    /// Inline keyboard attached to the message
    [<DataMember(Name = "reply_markup")>]
    ReplyMarkup: InlineKeyboardMarkup option
    /// Content of the message to be sent instead of the GIF animation
    [<DataMember(Name = "input_message_content")>]
    InputMessageContent: InputMessageContent option
  }
  static member Create(``type``: string, id: string, gifUrl: string, thumbnailUrl: string, ?gifWidth: int64, ?gifHeight: int64, ?gifDuration: int64, ?thumbnailMimeType: string, ?title: string, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?showCaptionAboveMedia: bool, ?replyMarkup: InlineKeyboardMarkup, ?inputMessageContent: InputMessageContent) = 
    {
      Type = ``type``
      Id = id
      GifUrl = gifUrl
      ThumbnailUrl = thumbnailUrl
      GifWidth = gifWidth
      GifHeight = gifHeight
      GifDuration = gifDuration
      ThumbnailMimeType = thumbnailMimeType
      Title = title
      Caption = caption
      ParseMode = parseMode
      CaptionEntities = captionEntities
      ShowCaptionAboveMedia = showCaptionAboveMedia
      ReplyMarkup = replyMarkup
      InputMessageContent = inputMessageContent
    }

/// Represents a link to a video animation (H.264/MPEG-4 AVC video without sound). By default, this animated MPEG-4 file will be sent by the user with optional caption. Alternatively, you can use input_message_content to send a message with the specified content instead of the animation.
and [<CLIMutable>] InlineQueryResultMpeg4Gif =
  {
    /// Type of the result, must be mpeg4_gif
    [<DataMember(Name = "type")>]
    Type: string
    /// Unique identifier for this result, 1-64 bytes
    [<DataMember(Name = "id")>]
    Id: string
    /// A valid URL for the MPEG4 file. File size must not exceed 1MB
    [<DataMember(Name = "mpeg4_url")>]
    Mpeg4Url: string
    /// Video width
    [<DataMember(Name = "mpeg4_width")>]
    Mpeg4Width: int64 option
    /// Video height
    [<DataMember(Name = "mpeg4_height")>]
    Mpeg4Height: int64 option
    /// Video duration in seconds
    [<DataMember(Name = "mpeg4_duration")>]
    Mpeg4Duration: int64 option
    /// URL of the static (JPEG or GIF) or animated (MPEG4) thumbnail for the result
    [<DataMember(Name = "thumbnail_url")>]
    ThumbnailUrl: string
    /// MIME type of the thumbnail, must be one of “image/jpeg”, “image/gif”, or “video/mp4”. Defaults to “image/jpeg”
    [<DataMember(Name = "thumbnail_mime_type")>]
    ThumbnailMimeType: string option
    /// Title for the result
    [<DataMember(Name = "title")>]
    Title: string option
    /// Caption of the MPEG-4 file to be sent, 0-1024 characters after entities parsing
    [<DataMember(Name = "caption")>]
    Caption: string option
    /// Mode for parsing entities in the caption. See formatting options for more details.
    [<DataMember(Name = "parse_mode")>]
    ParseMode: ParseMode option
    /// List of special entities that appear in the caption, which can be specified instead of parse_mode
    [<DataMember(Name = "caption_entities")>]
    CaptionEntities: MessageEntity[] option
    /// Pass True, if the caption must be shown above the message media
    [<DataMember(Name = "show_caption_above_media")>]
    ShowCaptionAboveMedia: bool option
    /// Inline keyboard attached to the message
    [<DataMember(Name = "reply_markup")>]
    ReplyMarkup: InlineKeyboardMarkup option
    /// Content of the message to be sent instead of the video animation
    [<DataMember(Name = "input_message_content")>]
    InputMessageContent: InputMessageContent option
  }
  static member Create(``type``: string, id: string, mpeg4Url: string, thumbnailUrl: string, ?mpeg4Width: int64, ?mpeg4Height: int64, ?mpeg4Duration: int64, ?thumbnailMimeType: string, ?title: string, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?showCaptionAboveMedia: bool, ?replyMarkup: InlineKeyboardMarkup, ?inputMessageContent: InputMessageContent) = 
    {
      Type = ``type``
      Id = id
      Mpeg4Url = mpeg4Url
      ThumbnailUrl = thumbnailUrl
      Mpeg4Width = mpeg4Width
      Mpeg4Height = mpeg4Height
      Mpeg4Duration = mpeg4Duration
      ThumbnailMimeType = thumbnailMimeType
      Title = title
      Caption = caption
      ParseMode = parseMode
      CaptionEntities = captionEntities
      ShowCaptionAboveMedia = showCaptionAboveMedia
      ReplyMarkup = replyMarkup
      InputMessageContent = inputMessageContent
    }

/// Represents a link to a page containing an embedded video player or a video file. By default, this video file will be sent by the user with an optional caption. Alternatively, you can use input_message_content to send a message with the specified content instead of the video.
and [<CLIMutable>] InlineQueryResultVideo =
  {
    /// Type of the result, must be video
    [<DataMember(Name = "type")>]
    Type: string
    /// Unique identifier for this result, 1-64 bytes
    [<DataMember(Name = "id")>]
    Id: string
    /// A valid URL for the embedded video player or video file
    [<DataMember(Name = "video_url")>]
    VideoUrl: string
    /// MIME type of the content of the video URL, “text/html” or “video/mp4”
    [<DataMember(Name = "mime_type")>]
    MimeType: string
    /// URL of the thumbnail (JPEG only) for the video
    [<DataMember(Name = "thumbnail_url")>]
    ThumbnailUrl: string
    /// Title for the result
    [<DataMember(Name = "title")>]
    Title: string
    /// Caption of the video to be sent, 0-1024 characters after entities parsing
    [<DataMember(Name = "caption")>]
    Caption: string option
    /// Mode for parsing entities in the video caption. See formatting options for more details.
    [<DataMember(Name = "parse_mode")>]
    ParseMode: ParseMode option
    /// List of special entities that appear in the caption, which can be specified instead of parse_mode
    [<DataMember(Name = "caption_entities")>]
    CaptionEntities: MessageEntity[] option
    /// Pass True, if the caption must be shown above the message media
    [<DataMember(Name = "show_caption_above_media")>]
    ShowCaptionAboveMedia: bool option
    /// Video width
    [<DataMember(Name = "video_width")>]
    VideoWidth: int64 option
    /// Video height
    [<DataMember(Name = "video_height")>]
    VideoHeight: int64 option
    /// Video duration in seconds
    [<DataMember(Name = "video_duration")>]
    VideoDuration: int64 option
    /// Short description of the result
    [<DataMember(Name = "description")>]
    Description: string option
    /// Inline keyboard attached to the message
    [<DataMember(Name = "reply_markup")>]
    ReplyMarkup: InlineKeyboardMarkup option
    /// Content of the message to be sent instead of the video. This field is required if InlineQueryResultVideo is used to send an HTML-page as a result (e.g., a YouTube video).
    [<DataMember(Name = "input_message_content")>]
    InputMessageContent: InputMessageContent option
  }
  static member Create(``type``: string, id: string, videoUrl: string, mimeType: string, thumbnailUrl: string, title: string, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?showCaptionAboveMedia: bool, ?videoWidth: int64, ?videoHeight: int64, ?videoDuration: int64, ?description: string, ?replyMarkup: InlineKeyboardMarkup, ?inputMessageContent: InputMessageContent) = 
    {
      Type = ``type``
      Id = id
      VideoUrl = videoUrl
      MimeType = mimeType
      ThumbnailUrl = thumbnailUrl
      Title = title
      Caption = caption
      ParseMode = parseMode
      CaptionEntities = captionEntities
      ShowCaptionAboveMedia = showCaptionAboveMedia
      VideoWidth = videoWidth
      VideoHeight = videoHeight
      VideoDuration = videoDuration
      Description = description
      ReplyMarkup = replyMarkup
      InputMessageContent = inputMessageContent
    }

/// Represents a link to an MP3 audio file. By default, this audio file will be sent by the user. Alternatively, you can use input_message_content to send a message with the specified content instead of the audio.
and [<CLIMutable>] InlineQueryResultAudio =
  {
    /// Type of the result, must be audio
    [<DataMember(Name = "type")>]
    Type: string
    /// Unique identifier for this result, 1-64 bytes
    [<DataMember(Name = "id")>]
    Id: string
    /// A valid URL for the audio file
    [<DataMember(Name = "audio_url")>]
    AudioUrl: string
    /// Title
    [<DataMember(Name = "title")>]
    Title: string
    /// Caption, 0-1024 characters after entities parsing
    [<DataMember(Name = "caption")>]
    Caption: string option
    /// Mode for parsing entities in the audio caption. See formatting options for more details.
    [<DataMember(Name = "parse_mode")>]
    ParseMode: ParseMode option
    /// List of special entities that appear in the caption, which can be specified instead of parse_mode
    [<DataMember(Name = "caption_entities")>]
    CaptionEntities: MessageEntity[] option
    /// Performer
    [<DataMember(Name = "performer")>]
    Performer: string option
    /// Audio duration in seconds
    [<DataMember(Name = "audio_duration")>]
    AudioDuration: int64 option
    /// Inline keyboard attached to the message
    [<DataMember(Name = "reply_markup")>]
    ReplyMarkup: InlineKeyboardMarkup option
    /// Content of the message to be sent instead of the audio
    [<DataMember(Name = "input_message_content")>]
    InputMessageContent: InputMessageContent option
  }
  static member Create(``type``: string, id: string, audioUrl: string, title: string, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?performer: string, ?audioDuration: int64, ?replyMarkup: InlineKeyboardMarkup, ?inputMessageContent: InputMessageContent) = 
    {
      Type = ``type``
      Id = id
      AudioUrl = audioUrl
      Title = title
      Caption = caption
      ParseMode = parseMode
      CaptionEntities = captionEntities
      Performer = performer
      AudioDuration = audioDuration
      ReplyMarkup = replyMarkup
      InputMessageContent = inputMessageContent
    }

/// Represents a link to a voice recording in an .OGG container encoded with OPUS. By default, this voice recording will be sent by the user. Alternatively, you can use input_message_content to send a message with the specified content instead of the the voice message.
and [<CLIMutable>] InlineQueryResultVoice =
  {
    /// Type of the result, must be voice
    [<DataMember(Name = "type")>]
    Type: string
    /// Unique identifier for this result, 1-64 bytes
    [<DataMember(Name = "id")>]
    Id: string
    /// A valid URL for the voice recording
    [<DataMember(Name = "voice_url")>]
    VoiceUrl: string
    /// Recording title
    [<DataMember(Name = "title")>]
    Title: string
    /// Caption, 0-1024 characters after entities parsing
    [<DataMember(Name = "caption")>]
    Caption: string option
    /// Mode for parsing entities in the voice message caption. See formatting options for more details.
    [<DataMember(Name = "parse_mode")>]
    ParseMode: ParseMode option
    /// List of special entities that appear in the caption, which can be specified instead of parse_mode
    [<DataMember(Name = "caption_entities")>]
    CaptionEntities: MessageEntity[] option
    /// Recording duration in seconds
    [<DataMember(Name = "voice_duration")>]
    VoiceDuration: int64 option
    /// Inline keyboard attached to the message
    [<DataMember(Name = "reply_markup")>]
    ReplyMarkup: InlineKeyboardMarkup option
    /// Content of the message to be sent instead of the voice recording
    [<DataMember(Name = "input_message_content")>]
    InputMessageContent: InputMessageContent option
  }
  static member Create(``type``: string, id: string, voiceUrl: string, title: string, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?voiceDuration: int64, ?replyMarkup: InlineKeyboardMarkup, ?inputMessageContent: InputMessageContent) = 
    {
      Type = ``type``
      Id = id
      VoiceUrl = voiceUrl
      Title = title
      Caption = caption
      ParseMode = parseMode
      CaptionEntities = captionEntities
      VoiceDuration = voiceDuration
      ReplyMarkup = replyMarkup
      InputMessageContent = inputMessageContent
    }

/// Represents a link to a file. By default, this file will be sent by the user with an optional caption. Alternatively, you can use input_message_content to send a message with the specified content instead of the file. Currently, only .PDF and .ZIP files can be sent using this method.
and [<CLIMutable>] InlineQueryResultDocument =
  {
    /// Type of the result, must be document
    [<DataMember(Name = "type")>]
    Type: string
    /// Unique identifier for this result, 1-64 bytes
    [<DataMember(Name = "id")>]
    Id: string
    /// Title for the result
    [<DataMember(Name = "title")>]
    Title: string
    /// Caption of the document to be sent, 0-1024 characters after entities parsing
    [<DataMember(Name = "caption")>]
    Caption: string option
    /// Mode for parsing entities in the document caption. See formatting options for more details.
    [<DataMember(Name = "parse_mode")>]
    ParseMode: ParseMode option
    /// List of special entities that appear in the caption, which can be specified instead of parse_mode
    [<DataMember(Name = "caption_entities")>]
    CaptionEntities: MessageEntity[] option
    /// A valid URL for the file
    [<DataMember(Name = "document_url")>]
    DocumentUrl: string
    /// MIME type of the content of the file, either “application/pdf” or “application/zip”
    [<DataMember(Name = "mime_type")>]
    MimeType: string
    /// Short description of the result
    [<DataMember(Name = "description")>]
    Description: string option
    /// Inline keyboard attached to the message
    [<DataMember(Name = "reply_markup")>]
    ReplyMarkup: InlineKeyboardMarkup option
    /// Content of the message to be sent instead of the file
    [<DataMember(Name = "input_message_content")>]
    InputMessageContent: InputMessageContent option
    /// URL of the thumbnail (JPEG only) for the file
    [<DataMember(Name = "thumbnail_url")>]
    ThumbnailUrl: string option
    /// Thumbnail width
    [<DataMember(Name = "thumbnail_width")>]
    ThumbnailWidth: int64 option
    /// Thumbnail height
    [<DataMember(Name = "thumbnail_height")>]
    ThumbnailHeight: int64 option
  }
  static member Create(``type``: string, id: string, title: string, documentUrl: string, mimeType: string, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?description: string, ?replyMarkup: InlineKeyboardMarkup, ?inputMessageContent: InputMessageContent, ?thumbnailUrl: string, ?thumbnailWidth: int64, ?thumbnailHeight: int64) = 
    {
      Type = ``type``
      Id = id
      Title = title
      DocumentUrl = documentUrl
      MimeType = mimeType
      Caption = caption
      ParseMode = parseMode
      CaptionEntities = captionEntities
      Description = description
      ReplyMarkup = replyMarkup
      InputMessageContent = inputMessageContent
      ThumbnailUrl = thumbnailUrl
      ThumbnailWidth = thumbnailWidth
      ThumbnailHeight = thumbnailHeight
    }

/// Represents a location on a map. By default, the location will be sent by the user. Alternatively, you can use input_message_content to send a message with the specified content instead of the location.
and [<CLIMutable>] InlineQueryResultLocation =
  {
    /// Type of the result, must be location
    [<DataMember(Name = "type")>]
    Type: string
    /// Unique identifier for this result, 1-64 Bytes
    [<DataMember(Name = "id")>]
    Id: string
    /// Location latitude in degrees
    [<DataMember(Name = "latitude")>]
    Latitude: float
    /// Location longitude in degrees
    [<DataMember(Name = "longitude")>]
    Longitude: float
    /// Location title
    [<DataMember(Name = "title")>]
    Title: string
    /// The radius of uncertainty for the location, measured in meters; 0-1500
    [<DataMember(Name = "horizontal_accuracy")>]
    HorizontalAccuracy: float option
    /// Period in seconds during which the location can be updated, should be between 60 and 86400, or 0x7FFFFFFF for live locations that can be edited indefinitely.
    [<DataMember(Name = "live_period")>]
    LivePeriod: int64 option
    /// For live locations, a direction in which the user is moving, in degrees. Must be between 1 and 360 if specified.
    [<DataMember(Name = "heading")>]
    Heading: int64 option
    /// For live locations, a maximum distance for proximity alerts about approaching another chat member, in meters. Must be between 1 and 100000 if specified.
    [<DataMember(Name = "proximity_alert_radius")>]
    ProximityAlertRadius: int64 option
    /// Inline keyboard attached to the message
    [<DataMember(Name = "reply_markup")>]
    ReplyMarkup: InlineKeyboardMarkup option
    /// Content of the message to be sent instead of the location
    [<DataMember(Name = "input_message_content")>]
    InputMessageContent: InputMessageContent option
    /// Url of the thumbnail for the result
    [<DataMember(Name = "thumbnail_url")>]
    ThumbnailUrl: string option
    /// Thumbnail width
    [<DataMember(Name = "thumbnail_width")>]
    ThumbnailWidth: int64 option
    /// Thumbnail height
    [<DataMember(Name = "thumbnail_height")>]
    ThumbnailHeight: int64 option
  }
  static member Create(``type``: string, id: string, latitude: float, longitude: float, title: string, ?horizontalAccuracy: float, ?livePeriod: int64, ?heading: int64, ?proximityAlertRadius: int64, ?replyMarkup: InlineKeyboardMarkup, ?inputMessageContent: InputMessageContent, ?thumbnailUrl: string, ?thumbnailWidth: int64, ?thumbnailHeight: int64) = 
    {
      Type = ``type``
      Id = id
      Latitude = latitude
      Longitude = longitude
      Title = title
      HorizontalAccuracy = horizontalAccuracy
      LivePeriod = livePeriod
      Heading = heading
      ProximityAlertRadius = proximityAlertRadius
      ReplyMarkup = replyMarkup
      InputMessageContent = inputMessageContent
      ThumbnailUrl = thumbnailUrl
      ThumbnailWidth = thumbnailWidth
      ThumbnailHeight = thumbnailHeight
    }

/// Represents a venue. By default, the venue will be sent by the user. Alternatively, you can use input_message_content to send a message with the specified content instead of the venue.
and [<CLIMutable>] InlineQueryResultVenue =
  {
    /// Type of the result, must be venue
    [<DataMember(Name = "type")>]
    Type: string
    /// Unique identifier for this result, 1-64 Bytes
    [<DataMember(Name = "id")>]
    Id: string
    /// Latitude of the venue location in degrees
    [<DataMember(Name = "latitude")>]
    Latitude: float
    /// Longitude of the venue location in degrees
    [<DataMember(Name = "longitude")>]
    Longitude: float
    /// Title of the venue
    [<DataMember(Name = "title")>]
    Title: string
    /// Address of the venue
    [<DataMember(Name = "address")>]
    Address: string
    /// Foursquare identifier of the venue if known
    [<DataMember(Name = "foursquare_id")>]
    FoursquareId: string option
    /// Foursquare type of the venue, if known. (For example, “arts_entertainment/default”, “arts_entertainment/aquarium” or “food/icecream”.)
    [<DataMember(Name = "foursquare_type")>]
    FoursquareType: string option
    /// Google Places identifier of the venue
    [<DataMember(Name = "google_place_id")>]
    GooglePlaceId: string option
    /// Google Places type of the venue. (See supported types.)
    [<DataMember(Name = "google_place_type")>]
    GooglePlaceType: string option
    /// Inline keyboard attached to the message
    [<DataMember(Name = "reply_markup")>]
    ReplyMarkup: InlineKeyboardMarkup option
    /// Content of the message to be sent instead of the venue
    [<DataMember(Name = "input_message_content")>]
    InputMessageContent: InputMessageContent option
    /// Url of the thumbnail for the result
    [<DataMember(Name = "thumbnail_url")>]
    ThumbnailUrl: string option
    /// Thumbnail width
    [<DataMember(Name = "thumbnail_width")>]
    ThumbnailWidth: int64 option
    /// Thumbnail height
    [<DataMember(Name = "thumbnail_height")>]
    ThumbnailHeight: int64 option
  }
  static member Create(``type``: string, id: string, latitude: float, longitude: float, title: string, address: string, ?foursquareId: string, ?foursquareType: string, ?googlePlaceId: string, ?googlePlaceType: string, ?replyMarkup: InlineKeyboardMarkup, ?inputMessageContent: InputMessageContent, ?thumbnailUrl: string, ?thumbnailWidth: int64, ?thumbnailHeight: int64) = 
    {
      Type = ``type``
      Id = id
      Latitude = latitude
      Longitude = longitude
      Title = title
      Address = address
      FoursquareId = foursquareId
      FoursquareType = foursquareType
      GooglePlaceId = googlePlaceId
      GooglePlaceType = googlePlaceType
      ReplyMarkup = replyMarkup
      InputMessageContent = inputMessageContent
      ThumbnailUrl = thumbnailUrl
      ThumbnailWidth = thumbnailWidth
      ThumbnailHeight = thumbnailHeight
    }

/// Represents a contact with a phone number. By default, this contact will be sent by the user. Alternatively, you can use input_message_content to send a message with the specified content instead of the contact.
and [<CLIMutable>] InlineQueryResultContact =
  {
    /// Type of the result, must be contact
    [<DataMember(Name = "type")>]
    Type: string
    /// Unique identifier for this result, 1-64 Bytes
    [<DataMember(Name = "id")>]
    Id: string
    /// Contact's phone number
    [<DataMember(Name = "phone_number")>]
    PhoneNumber: string
    /// Contact's first name
    [<DataMember(Name = "first_name")>]
    FirstName: string
    /// Contact's last name
    [<DataMember(Name = "last_name")>]
    LastName: string option
    /// Additional data about the contact in the form of a vCard, 0-2048 bytes
    [<DataMember(Name = "vcard")>]
    Vcard: string option
    /// Inline keyboard attached to the message
    [<DataMember(Name = "reply_markup")>]
    ReplyMarkup: InlineKeyboardMarkup option
    /// Content of the message to be sent instead of the contact
    [<DataMember(Name = "input_message_content")>]
    InputMessageContent: InputMessageContent option
    /// Url of the thumbnail for the result
    [<DataMember(Name = "thumbnail_url")>]
    ThumbnailUrl: string option
    /// Thumbnail width
    [<DataMember(Name = "thumbnail_width")>]
    ThumbnailWidth: int64 option
    /// Thumbnail height
    [<DataMember(Name = "thumbnail_height")>]
    ThumbnailHeight: int64 option
  }
  static member Create(``type``: string, id: string, phoneNumber: string, firstName: string, ?lastName: string, ?vcard: string, ?replyMarkup: InlineKeyboardMarkup, ?inputMessageContent: InputMessageContent, ?thumbnailUrl: string, ?thumbnailWidth: int64, ?thumbnailHeight: int64) = 
    {
      Type = ``type``
      Id = id
      PhoneNumber = phoneNumber
      FirstName = firstName
      LastName = lastName
      Vcard = vcard
      ReplyMarkup = replyMarkup
      InputMessageContent = inputMessageContent
      ThumbnailUrl = thumbnailUrl
      ThumbnailWidth = thumbnailWidth
      ThumbnailHeight = thumbnailHeight
    }

/// Represents a Game.
and [<CLIMutable>] InlineQueryResultGame =
  {
    /// Type of the result, must be game
    [<DataMember(Name = "type")>]
    Type: string
    /// Unique identifier for this result, 1-64 bytes
    [<DataMember(Name = "id")>]
    Id: string
    /// Short name of the game
    [<DataMember(Name = "game_short_name")>]
    GameShortName: string
    /// Inline keyboard attached to the message
    [<DataMember(Name = "reply_markup")>]
    ReplyMarkup: InlineKeyboardMarkup option
  }
  static member Create(``type``: string, id: string, gameShortName: string, ?replyMarkup: InlineKeyboardMarkup) = 
    {
      Type = ``type``
      Id = id
      GameShortName = gameShortName
      ReplyMarkup = replyMarkup
    }

/// Represents a link to a photo stored on the Telegram servers. By default, this photo will be sent by the user with an optional caption. Alternatively, you can use input_message_content to send a message with the specified content instead of the photo.
and [<CLIMutable>] InlineQueryResultCachedPhoto =
  {
    /// Type of the result, must be photo
    [<DataMember(Name = "type")>]
    Type: string
    /// Unique identifier for this result, 1-64 bytes
    [<DataMember(Name = "id")>]
    Id: string
    /// A valid file identifier of the photo
    [<DataMember(Name = "photo_file_id")>]
    PhotoFileId: string
    /// Title for the result
    [<DataMember(Name = "title")>]
    Title: string option
    /// Short description of the result
    [<DataMember(Name = "description")>]
    Description: string option
    /// Caption of the photo to be sent, 0-1024 characters after entities parsing
    [<DataMember(Name = "caption")>]
    Caption: string option
    /// Mode for parsing entities in the photo caption. See formatting options for more details.
    [<DataMember(Name = "parse_mode")>]
    ParseMode: ParseMode option
    /// List of special entities that appear in the caption, which can be specified instead of parse_mode
    [<DataMember(Name = "caption_entities")>]
    CaptionEntities: MessageEntity[] option
    /// Pass True, if the caption must be shown above the message media
    [<DataMember(Name = "show_caption_above_media")>]
    ShowCaptionAboveMedia: bool option
    /// Inline keyboard attached to the message
    [<DataMember(Name = "reply_markup")>]
    ReplyMarkup: InlineKeyboardMarkup option
    /// Content of the message to be sent instead of the photo
    [<DataMember(Name = "input_message_content")>]
    InputMessageContent: InputMessageContent option
  }
  static member Create(``type``: string, id: string, photoFileId: string, ?title: string, ?description: string, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?showCaptionAboveMedia: bool, ?replyMarkup: InlineKeyboardMarkup, ?inputMessageContent: InputMessageContent) = 
    {
      Type = ``type``
      Id = id
      PhotoFileId = photoFileId
      Title = title
      Description = description
      Caption = caption
      ParseMode = parseMode
      CaptionEntities = captionEntities
      ShowCaptionAboveMedia = showCaptionAboveMedia
      ReplyMarkup = replyMarkup
      InputMessageContent = inputMessageContent
    }

/// Represents a link to an animated GIF file stored on the Telegram servers. By default, this animated GIF file will be sent by the user with an optional caption. Alternatively, you can use input_message_content to send a message with specified content instead of the animation.
and [<CLIMutable>] InlineQueryResultCachedGif =
  {
    /// Type of the result, must be gif
    [<DataMember(Name = "type")>]
    Type: string
    /// Unique identifier for this result, 1-64 bytes
    [<DataMember(Name = "id")>]
    Id: string
    /// A valid file identifier for the GIF file
    [<DataMember(Name = "gif_file_id")>]
    GifFileId: string
    /// Title for the result
    [<DataMember(Name = "title")>]
    Title: string option
    /// Caption of the GIF file to be sent, 0-1024 characters after entities parsing
    [<DataMember(Name = "caption")>]
    Caption: string option
    /// Mode for parsing entities in the caption. See formatting options for more details.
    [<DataMember(Name = "parse_mode")>]
    ParseMode: ParseMode option
    /// List of special entities that appear in the caption, which can be specified instead of parse_mode
    [<DataMember(Name = "caption_entities")>]
    CaptionEntities: MessageEntity[] option
    /// Pass True, if the caption must be shown above the message media
    [<DataMember(Name = "show_caption_above_media")>]
    ShowCaptionAboveMedia: bool option
    /// Inline keyboard attached to the message
    [<DataMember(Name = "reply_markup")>]
    ReplyMarkup: InlineKeyboardMarkup option
    /// Content of the message to be sent instead of the GIF animation
    [<DataMember(Name = "input_message_content")>]
    InputMessageContent: InputMessageContent option
  }
  static member Create(``type``: string, id: string, gifFileId: string, ?title: string, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?showCaptionAboveMedia: bool, ?replyMarkup: InlineKeyboardMarkup, ?inputMessageContent: InputMessageContent) = 
    {
      Type = ``type``
      Id = id
      GifFileId = gifFileId
      Title = title
      Caption = caption
      ParseMode = parseMode
      CaptionEntities = captionEntities
      ShowCaptionAboveMedia = showCaptionAboveMedia
      ReplyMarkup = replyMarkup
      InputMessageContent = inputMessageContent
    }

/// Represents a link to a video animation (H.264/MPEG-4 AVC video without sound) stored on the Telegram servers. By default, this animated MPEG-4 file will be sent by the user with an optional caption. Alternatively, you can use input_message_content to send a message with the specified content instead of the animation.
and [<CLIMutable>] InlineQueryResultCachedMpeg4Gif =
  {
    /// Type of the result, must be mpeg4_gif
    [<DataMember(Name = "type")>]
    Type: string
    /// Unique identifier for this result, 1-64 bytes
    [<DataMember(Name = "id")>]
    Id: string
    /// A valid file identifier for the MPEG4 file
    [<DataMember(Name = "mpeg4_file_id")>]
    Mpeg4FileId: string
    /// Title for the result
    [<DataMember(Name = "title")>]
    Title: string option
    /// Caption of the MPEG-4 file to be sent, 0-1024 characters after entities parsing
    [<DataMember(Name = "caption")>]
    Caption: string option
    /// Mode for parsing entities in the caption. See formatting options for more details.
    [<DataMember(Name = "parse_mode")>]
    ParseMode: ParseMode option
    /// List of special entities that appear in the caption, which can be specified instead of parse_mode
    [<DataMember(Name = "caption_entities")>]
    CaptionEntities: MessageEntity[] option
    /// Pass True, if the caption must be shown above the message media
    [<DataMember(Name = "show_caption_above_media")>]
    ShowCaptionAboveMedia: bool option
    /// Inline keyboard attached to the message
    [<DataMember(Name = "reply_markup")>]
    ReplyMarkup: InlineKeyboardMarkup option
    /// Content of the message to be sent instead of the video animation
    [<DataMember(Name = "input_message_content")>]
    InputMessageContent: InputMessageContent option
  }
  static member Create(``type``: string, id: string, mpeg4FileId: string, ?title: string, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?showCaptionAboveMedia: bool, ?replyMarkup: InlineKeyboardMarkup, ?inputMessageContent: InputMessageContent) = 
    {
      Type = ``type``
      Id = id
      Mpeg4FileId = mpeg4FileId
      Title = title
      Caption = caption
      ParseMode = parseMode
      CaptionEntities = captionEntities
      ShowCaptionAboveMedia = showCaptionAboveMedia
      ReplyMarkup = replyMarkup
      InputMessageContent = inputMessageContent
    }

/// Represents a link to a sticker stored on the Telegram servers. By default, this sticker will be sent by the user. Alternatively, you can use input_message_content to send a message with the specified content instead of the sticker.
and [<CLIMutable>] InlineQueryResultCachedSticker =
  {
    /// Type of the result, must be sticker
    [<DataMember(Name = "type")>]
    Type: string
    /// Unique identifier for this result, 1-64 bytes
    [<DataMember(Name = "id")>]
    Id: string
    /// A valid file identifier of the sticker
    [<DataMember(Name = "sticker_file_id")>]
    StickerFileId: string
    /// Inline keyboard attached to the message
    [<DataMember(Name = "reply_markup")>]
    ReplyMarkup: InlineKeyboardMarkup option
    /// Content of the message to be sent instead of the sticker
    [<DataMember(Name = "input_message_content")>]
    InputMessageContent: InputMessageContent option
  }
  static member Create(``type``: string, id: string, stickerFileId: string, ?replyMarkup: InlineKeyboardMarkup, ?inputMessageContent: InputMessageContent) = 
    {
      Type = ``type``
      Id = id
      StickerFileId = stickerFileId
      ReplyMarkup = replyMarkup
      InputMessageContent = inputMessageContent
    }

/// Represents a link to a file stored on the Telegram servers. By default, this file will be sent by the user with an optional caption. Alternatively, you can use input_message_content to send a message with the specified content instead of the file.
and [<CLIMutable>] InlineQueryResultCachedDocument =
  {
    /// Type of the result, must be document
    [<DataMember(Name = "type")>]
    Type: string
    /// Unique identifier for this result, 1-64 bytes
    [<DataMember(Name = "id")>]
    Id: string
    /// Title for the result
    [<DataMember(Name = "title")>]
    Title: string
    /// A valid file identifier for the file
    [<DataMember(Name = "document_file_id")>]
    DocumentFileId: string
    /// Short description of the result
    [<DataMember(Name = "description")>]
    Description: string option
    /// Caption of the document to be sent, 0-1024 characters after entities parsing
    [<DataMember(Name = "caption")>]
    Caption: string option
    /// Mode for parsing entities in the document caption. See formatting options for more details.
    [<DataMember(Name = "parse_mode")>]
    ParseMode: ParseMode option
    /// List of special entities that appear in the caption, which can be specified instead of parse_mode
    [<DataMember(Name = "caption_entities")>]
    CaptionEntities: MessageEntity[] option
    /// Inline keyboard attached to the message
    [<DataMember(Name = "reply_markup")>]
    ReplyMarkup: InlineKeyboardMarkup option
    /// Content of the message to be sent instead of the file
    [<DataMember(Name = "input_message_content")>]
    InputMessageContent: InputMessageContent option
  }
  static member Create(``type``: string, id: string, title: string, documentFileId: string, ?description: string, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?replyMarkup: InlineKeyboardMarkup, ?inputMessageContent: InputMessageContent) = 
    {
      Type = ``type``
      Id = id
      Title = title
      DocumentFileId = documentFileId
      Description = description
      Caption = caption
      ParseMode = parseMode
      CaptionEntities = captionEntities
      ReplyMarkup = replyMarkup
      InputMessageContent = inputMessageContent
    }

/// Represents a link to a video file stored on the Telegram servers. By default, this video file will be sent by the user with an optional caption. Alternatively, you can use input_message_content to send a message with the specified content instead of the video.
and [<CLIMutable>] InlineQueryResultCachedVideo =
  {
    /// Type of the result, must be video
    [<DataMember(Name = "type")>]
    Type: string
    /// Unique identifier for this result, 1-64 bytes
    [<DataMember(Name = "id")>]
    Id: string
    /// A valid file identifier for the video file
    [<DataMember(Name = "video_file_id")>]
    VideoFileId: string
    /// Title for the result
    [<DataMember(Name = "title")>]
    Title: string
    /// Short description of the result
    [<DataMember(Name = "description")>]
    Description: string option
    /// Caption of the video to be sent, 0-1024 characters after entities parsing
    [<DataMember(Name = "caption")>]
    Caption: string option
    /// Mode for parsing entities in the video caption. See formatting options for more details.
    [<DataMember(Name = "parse_mode")>]
    ParseMode: ParseMode option
    /// List of special entities that appear in the caption, which can be specified instead of parse_mode
    [<DataMember(Name = "caption_entities")>]
    CaptionEntities: MessageEntity[] option
    /// Pass True, if the caption must be shown above the message media
    [<DataMember(Name = "show_caption_above_media")>]
    ShowCaptionAboveMedia: bool option
    /// Inline keyboard attached to the message
    [<DataMember(Name = "reply_markup")>]
    ReplyMarkup: InlineKeyboardMarkup option
    /// Content of the message to be sent instead of the video
    [<DataMember(Name = "input_message_content")>]
    InputMessageContent: InputMessageContent option
  }
  static member Create(``type``: string, id: string, videoFileId: string, title: string, ?description: string, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?showCaptionAboveMedia: bool, ?replyMarkup: InlineKeyboardMarkup, ?inputMessageContent: InputMessageContent) = 
    {
      Type = ``type``
      Id = id
      VideoFileId = videoFileId
      Title = title
      Description = description
      Caption = caption
      ParseMode = parseMode
      CaptionEntities = captionEntities
      ShowCaptionAboveMedia = showCaptionAboveMedia
      ReplyMarkup = replyMarkup
      InputMessageContent = inputMessageContent
    }

/// Represents a link to a voice message stored on the Telegram servers. By default, this voice message will be sent by the user. Alternatively, you can use input_message_content to send a message with the specified content instead of the voice message.
and [<CLIMutable>] InlineQueryResultCachedVoice =
  {
    /// Type of the result, must be voice
    [<DataMember(Name = "type")>]
    Type: string
    /// Unique identifier for this result, 1-64 bytes
    [<DataMember(Name = "id")>]
    Id: string
    /// A valid file identifier for the voice message
    [<DataMember(Name = "voice_file_id")>]
    VoiceFileId: string
    /// Voice message title
    [<DataMember(Name = "title")>]
    Title: string
    /// Caption, 0-1024 characters after entities parsing
    [<DataMember(Name = "caption")>]
    Caption: string option
    /// Mode for parsing entities in the voice message caption. See formatting options for more details.
    [<DataMember(Name = "parse_mode")>]
    ParseMode: ParseMode option
    /// List of special entities that appear in the caption, which can be specified instead of parse_mode
    [<DataMember(Name = "caption_entities")>]
    CaptionEntities: MessageEntity[] option
    /// Inline keyboard attached to the message
    [<DataMember(Name = "reply_markup")>]
    ReplyMarkup: InlineKeyboardMarkup option
    /// Content of the message to be sent instead of the voice message
    [<DataMember(Name = "input_message_content")>]
    InputMessageContent: InputMessageContent option
  }
  static member Create(``type``: string, id: string, voiceFileId: string, title: string, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?replyMarkup: InlineKeyboardMarkup, ?inputMessageContent: InputMessageContent) = 
    {
      Type = ``type``
      Id = id
      VoiceFileId = voiceFileId
      Title = title
      Caption = caption
      ParseMode = parseMode
      CaptionEntities = captionEntities
      ReplyMarkup = replyMarkup
      InputMessageContent = inputMessageContent
    }

/// Represents a link to an MP3 audio file stored on the Telegram servers. By default, this audio file will be sent by the user. Alternatively, you can use input_message_content to send a message with the specified content instead of the audio.
and [<CLIMutable>] InlineQueryResultCachedAudio =
  {
    /// Type of the result, must be audio
    [<DataMember(Name = "type")>]
    Type: string
    /// Unique identifier for this result, 1-64 bytes
    [<DataMember(Name = "id")>]
    Id: string
    /// A valid file identifier for the audio file
    [<DataMember(Name = "audio_file_id")>]
    AudioFileId: string
    /// Caption, 0-1024 characters after entities parsing
    [<DataMember(Name = "caption")>]
    Caption: string option
    /// Mode for parsing entities in the audio caption. See formatting options for more details.
    [<DataMember(Name = "parse_mode")>]
    ParseMode: ParseMode option
    /// List of special entities that appear in the caption, which can be specified instead of parse_mode
    [<DataMember(Name = "caption_entities")>]
    CaptionEntities: MessageEntity[] option
    /// Inline keyboard attached to the message
    [<DataMember(Name = "reply_markup")>]
    ReplyMarkup: InlineKeyboardMarkup option
    /// Content of the message to be sent instead of the audio
    [<DataMember(Name = "input_message_content")>]
    InputMessageContent: InputMessageContent option
  }
  static member Create(``type``: string, id: string, audioFileId: string, ?caption: string, ?parseMode: ParseMode, ?captionEntities: MessageEntity[], ?replyMarkup: InlineKeyboardMarkup, ?inputMessageContent: InputMessageContent) = 
    {
      Type = ``type``
      Id = id
      AudioFileId = audioFileId
      Caption = caption
      ParseMode = parseMode
      CaptionEntities = captionEntities
      ReplyMarkup = replyMarkup
      InputMessageContent = inputMessageContent
    }

/// This object represents the content of a message to be sent as a result of an inline query. Telegram clients currently support the following 5 types:
and InputMessageContent =
  | TextMessageContent of InputTextMessageContent
  | LocationMessageContent of InputLocationMessageContent
  | VenueMessageContent of InputVenueMessageContent
  | ContactMessageContent of InputContactMessageContent
  | InvoiceMessageContent of InputInvoiceMessageContent

/// Represents the content of a text message to be sent as the result of an inline query.
and [<CLIMutable>] InputTextMessageContent =
  {
    /// Text of the message to be sent, 1-4096 characters
    [<DataMember(Name = "message_text")>]
    MessageText: string
    /// Mode for parsing entities in the message text. See formatting options for more details.
    [<DataMember(Name = "parse_mode")>]
    ParseMode: ParseMode option
    /// List of special entities that appear in message text, which can be specified instead of parse_mode
    [<DataMember(Name = "entities")>]
    Entities: MessageEntity[] option
    /// Link preview generation options for the message
    [<DataMember(Name = "link_preview_options")>]
    LinkPreviewOptions: LinkPreviewOptions option
  }
  static member Create(messageText: string, ?parseMode: ParseMode, ?entities: MessageEntity[], ?linkPreviewOptions: LinkPreviewOptions) = 
    {
      MessageText = messageText
      ParseMode = parseMode
      Entities = entities
      LinkPreviewOptions = linkPreviewOptions
    }

/// Represents the content of a location message to be sent as the result of an inline query.
and [<CLIMutable>] InputLocationMessageContent =
  {
    /// Latitude of the location in degrees
    [<DataMember(Name = "latitude")>]
    Latitude: float
    /// Longitude of the location in degrees
    [<DataMember(Name = "longitude")>]
    Longitude: float
    /// The radius of uncertainty for the location, measured in meters; 0-1500
    [<DataMember(Name = "horizontal_accuracy")>]
    HorizontalAccuracy: float option
    /// Period in seconds during which the location can be updated, should be between 60 and 86400, or 0x7FFFFFFF for live locations that can be edited indefinitely.
    [<DataMember(Name = "live_period")>]
    LivePeriod: int64 option
    /// For live locations, a direction in which the user is moving, in degrees. Must be between 1 and 360 if specified.
    [<DataMember(Name = "heading")>]
    Heading: int64 option
    /// For live locations, a maximum distance for proximity alerts about approaching another chat member, in meters. Must be between 1 and 100000 if specified.
    [<DataMember(Name = "proximity_alert_radius")>]
    ProximityAlertRadius: int64 option
  }
  static member Create(latitude: float, longitude: float, ?horizontalAccuracy: float, ?livePeriod: int64, ?heading: int64, ?proximityAlertRadius: int64) = 
    {
      Latitude = latitude
      Longitude = longitude
      HorizontalAccuracy = horizontalAccuracy
      LivePeriod = livePeriod
      Heading = heading
      ProximityAlertRadius = proximityAlertRadius
    }

/// Represents the content of a venue message to be sent as the result of an inline query.
and [<CLIMutable>] InputVenueMessageContent =
  {
    /// Latitude of the venue in degrees
    [<DataMember(Name = "latitude")>]
    Latitude: float
    /// Longitude of the venue in degrees
    [<DataMember(Name = "longitude")>]
    Longitude: float
    /// Name of the venue
    [<DataMember(Name = "title")>]
    Title: string
    /// Address of the venue
    [<DataMember(Name = "address")>]
    Address: string
    /// Foursquare identifier of the venue, if known
    [<DataMember(Name = "foursquare_id")>]
    FoursquareId: string option
    /// Foursquare type of the venue, if known. (For example, “arts_entertainment/default”, “arts_entertainment/aquarium” or “food/icecream”.)
    [<DataMember(Name = "foursquare_type")>]
    FoursquareType: string option
    /// Google Places identifier of the venue
    [<DataMember(Name = "google_place_id")>]
    GooglePlaceId: string option
    /// Google Places type of the venue. (See supported types.)
    [<DataMember(Name = "google_place_type")>]
    GooglePlaceType: string option
  }
  static member Create(latitude: float, longitude: float, title: string, address: string, ?foursquareId: string, ?foursquareType: string, ?googlePlaceId: string, ?googlePlaceType: string) = 
    {
      Latitude = latitude
      Longitude = longitude
      Title = title
      Address = address
      FoursquareId = foursquareId
      FoursquareType = foursquareType
      GooglePlaceId = googlePlaceId
      GooglePlaceType = googlePlaceType
    }

/// Represents the content of a contact message to be sent as the result of an inline query.
and [<CLIMutable>] InputContactMessageContent =
  {
    /// Contact's phone number
    [<DataMember(Name = "phone_number")>]
    PhoneNumber: string
    /// Contact's first name
    [<DataMember(Name = "first_name")>]
    FirstName: string
    /// Contact's last name
    [<DataMember(Name = "last_name")>]
    LastName: string option
    /// Additional data about the contact in the form of a vCard, 0-2048 bytes
    [<DataMember(Name = "vcard")>]
    Vcard: string option
  }
  static member Create(phoneNumber: string, firstName: string, ?lastName: string, ?vcard: string) = 
    {
      PhoneNumber = phoneNumber
      FirstName = firstName
      LastName = lastName
      Vcard = vcard
    }

/// Represents the content of an invoice message to be sent as the result of an inline query.
and [<CLIMutable>] InputInvoiceMessageContent =
  {
    /// Product name, 1-32 characters
    [<DataMember(Name = "title")>]
    Title: string
    /// Product description, 1-255 characters
    [<DataMember(Name = "description")>]
    Description: string
    /// Bot-defined invoice payload, 1-128 bytes. This will not be displayed to the user, use it for your internal processes.
    [<DataMember(Name = "payload")>]
    Payload: string
    /// Payment provider token, obtained via @BotFather. Pass an empty string for payments in Telegram Stars.
    [<DataMember(Name = "provider_token")>]
    ProviderToken: string option
    /// Three-letter ISO 4217 currency code, see more on currencies. Pass “XTR” for payments in Telegram Stars.
    [<DataMember(Name = "currency")>]
    Currency: string
    /// Price breakdown, a JSON-serialized list of components (e.g. product price, tax, discount, delivery cost, delivery tax, bonus, etc.). Must contain exactly one item for payments in Telegram Stars.
    [<DataMember(Name = "prices")>]
    Prices: LabeledPrice[]
    /// The maximum accepted amount for tips in the smallest units of the currency (integer, not float/double). For example, for a maximum tip of US$ 1.45 pass max_tip_amount = 145. See the exp parameter in currencies.json, it shows the number of digits past the decimal point for each currency (2 for the majority of currencies). Defaults to 0. Not supported for payments in Telegram Stars.
    [<DataMember(Name = "max_tip_amount")>]
    MaxTipAmount: int64 option
    /// A JSON-serialized array of suggested amounts of tip in the smallest units of the currency (integer, not float/double). At most 4 suggested tip amounts can be specified. The suggested tip amounts must be positive, passed in a strictly increased order and must not exceed max_tip_amount.
    [<DataMember(Name = "suggested_tip_amounts")>]
    SuggestedTipAmounts: int64[] option
    /// A JSON-serialized object for data about the invoice, which will be shared with the payment provider. A detailed description of the required fields should be provided by the payment provider.
    [<DataMember(Name = "provider_data")>]
    ProviderData: string option
    /// URL of the product photo for the invoice. Can be a photo of the goods or a marketing image for a service.
    [<DataMember(Name = "photo_url")>]
    PhotoUrl: string option
    /// Photo size in bytes
    [<DataMember(Name = "photo_size")>]
    PhotoSize: int64 option
    /// Photo width
    [<DataMember(Name = "photo_width")>]
    PhotoWidth: int64 option
    /// Photo height
    [<DataMember(Name = "photo_height")>]
    PhotoHeight: int64 option
    /// Pass True if you require the user's full name to complete the order. Ignored for payments in Telegram Stars.
    [<DataMember(Name = "need_name")>]
    NeedName: bool option
    /// Pass True if you require the user's phone number to complete the order. Ignored for payments in Telegram Stars.
    [<DataMember(Name = "need_phone_number")>]
    NeedPhoneNumber: bool option
    /// Pass True if you require the user's email address to complete the order. Ignored for payments in Telegram Stars.
    [<DataMember(Name = "need_email")>]
    NeedEmail: bool option
    /// Pass True if you require the user's shipping address to complete the order. Ignored for payments in Telegram Stars.
    [<DataMember(Name = "need_shipping_address")>]
    NeedShippingAddress: bool option
    /// Pass True if the user's phone number should be sent to the provider. Ignored for payments in Telegram Stars.
    [<DataMember(Name = "send_phone_number_to_provider")>]
    SendPhoneNumberToProvider: bool option
    /// Pass True if the user's email address should be sent to the provider. Ignored for payments in Telegram Stars.
    [<DataMember(Name = "send_email_to_provider")>]
    SendEmailToProvider: bool option
    /// Pass True if the final price depends on the shipping method. Ignored for payments in Telegram Stars.
    [<DataMember(Name = "is_flexible")>]
    IsFlexible: bool option
  }
  static member Create(title: string, description: string, payload: string, currency: string, prices: LabeledPrice[], ?sendPhoneNumberToProvider: bool, ?needShippingAddress: bool, ?needEmail: bool, ?needPhoneNumber: bool, ?needName: bool, ?photoHeight: int64, ?photoUrl: string, ?photoSize: int64, ?sendEmailToProvider: bool, ?providerData: string, ?suggestedTipAmounts: int64[], ?maxTipAmount: int64, ?providerToken: string, ?photoWidth: int64, ?isFlexible: bool) = 
    {
      Title = title
      Description = description
      Payload = payload
      Currency = currency
      Prices = prices
      SendPhoneNumberToProvider = sendPhoneNumberToProvider
      NeedShippingAddress = needShippingAddress
      NeedEmail = needEmail
      NeedPhoneNumber = needPhoneNumber
      NeedName = needName
      PhotoHeight = photoHeight
      PhotoUrl = photoUrl
      PhotoSize = photoSize
      SendEmailToProvider = sendEmailToProvider
      ProviderData = providerData
      SuggestedTipAmounts = suggestedTipAmounts
      MaxTipAmount = maxTipAmount
      ProviderToken = providerToken
      PhotoWidth = photoWidth
      IsFlexible = isFlexible
    }

/// Represents a result of an inline query that was chosen by the user and sent to their chat partner.
/// Note: It is necessary to enable inline feedback via @BotFather in order to receive these objects in updates.
and [<CLIMutable>] ChosenInlineResult =
  {
    /// The unique identifier for the result that was chosen
    [<DataMember(Name = "result_id")>]
    ResultId: string
    /// The user that chose the result
    [<DataMember(Name = "from")>]
    From: User
    /// Sender location, only for bots that require user location
    [<DataMember(Name = "location")>]
    Location: Location option
    /// Identifier of the sent inline message. Available only if there is an inline keyboard attached to the message. Will be also received in callback queries and can be used to edit the message.
    [<DataMember(Name = "inline_message_id")>]
    InlineMessageId: string option
    /// The query that was used to obtain the result
    [<DataMember(Name = "query")>]
    Query: string
  }
  static member Create(resultId: string, from: User, query: string, ?location: Location, ?inlineMessageId: string) = 
    {
      ResultId = resultId
      From = from
      Query = query
      Location = location
      InlineMessageId = inlineMessageId
    }

/// Describes an inline message sent by a Web App on behalf of a user.
/// Your bot can accept payments from Telegram users. Please see the introduction to payments for more details on the process and how to set up payments for your bot.
and [<CLIMutable>] SentWebAppMessage =
  {
    /// Identifier of the sent inline message. Available only if there is an inline keyboard attached to the message.
    [<DataMember(Name = "inline_message_id")>]
    InlineMessageId: string option
  }
  static member Create(?inlineMessageId: string) = 
    {
      InlineMessageId = inlineMessageId
    }

/// This object represents a portion of the price for goods or services.
and [<CLIMutable>] LabeledPrice =
  {
    /// Portion label
    [<DataMember(Name = "label")>]
    Label: string
    /// Price of the product in the smallest units of the currency (integer, not float/double). For example, for a price of US$ 1.45 pass amount = 145. See the exp parameter in currencies.json, it shows the number of digits past the decimal point for each currency (2 for the majority of currencies).
    [<DataMember(Name = "amount")>]
    Amount: int64
  }
  static member Create(label: string, amount: int64) = 
    {
      Label = label
      Amount = amount
    }

/// This object contains basic information about an invoice.
and [<CLIMutable>] Invoice =
  {
    /// Product name
    [<DataMember(Name = "title")>]
    Title: string
    /// Product description
    [<DataMember(Name = "description")>]
    Description: string
    /// Unique bot deep-linking parameter that can be used to generate this invoice
    [<DataMember(Name = "start_parameter")>]
    StartParameter: string
    /// Three-letter ISO 4217 currency code, or “XTR” for payments in Telegram Stars
    [<DataMember(Name = "currency")>]
    Currency: string
    /// Total price in the smallest units of the currency (integer, not float/double). For example, for a price of US$ 1.45 pass amount = 145. See the exp parameter in currencies.json, it shows the number of digits past the decimal point for each currency (2 for the majority of currencies).
    [<DataMember(Name = "total_amount")>]
    TotalAmount: int64
  }
  static member Create(title: string, description: string, startParameter: string, currency: string, totalAmount: int64) = 
    {
      Title = title
      Description = description
      StartParameter = startParameter
      Currency = currency
      TotalAmount = totalAmount
    }

/// This object represents a shipping address.
and [<CLIMutable>] ShippingAddress =
  {
    /// Two-letter ISO 3166-1 alpha-2 country code
    [<DataMember(Name = "country_code")>]
    CountryCode: string
    /// State, if applicable
    [<DataMember(Name = "state")>]
    State: string
    /// City
    [<DataMember(Name = "city")>]
    City: string
    /// First line for the address
    [<DataMember(Name = "street_line1")>]
    StreetLine1: string
    /// Second line for the address
    [<DataMember(Name = "street_line2")>]
    StreetLine2: string
    /// Address post code
    [<DataMember(Name = "post_code")>]
    PostCode: string
  }
  static member Create(countryCode: string, state: string, city: string, streetLine1: string, streetLine2: string, postCode: string) = 
    {
      CountryCode = countryCode
      State = state
      City = city
      StreetLine1 = streetLine1
      StreetLine2 = streetLine2
      PostCode = postCode
    }

/// This object represents information about an order.
and [<CLIMutable>] OrderInfo =
  {
    /// User name
    [<DataMember(Name = "name")>]
    Name: string option
    /// User's phone number
    [<DataMember(Name = "phone_number")>]
    PhoneNumber: string option
    /// User email
    [<DataMember(Name = "email")>]
    Email: string option
    /// User shipping address
    [<DataMember(Name = "shipping_address")>]
    ShippingAddress: ShippingAddress option
  }
  static member Create(?name: string, ?phoneNumber: string, ?email: string, ?shippingAddress: ShippingAddress) = 
    {
      Name = name
      PhoneNumber = phoneNumber
      Email = email
      ShippingAddress = shippingAddress
    }

/// This object represents one shipping option.
and [<CLIMutable>] ShippingOption =
  {
    /// Shipping option identifier
    [<DataMember(Name = "id")>]
    Id: string
    /// Option title
    [<DataMember(Name = "title")>]
    Title: string
    /// List of price portions
    [<DataMember(Name = "prices")>]
    Prices: LabeledPrice[]
  }
  static member Create(id: string, title: string, prices: LabeledPrice[]) = 
    {
      Id = id
      Title = title
      Prices = prices
    }

/// This object contains basic information about a successful payment.
and [<CLIMutable>] SuccessfulPayment =
  {
    /// Three-letter ISO 4217 currency code, or “XTR” for payments in Telegram Stars
    [<DataMember(Name = "currency")>]
    Currency: string
    /// Total price in the smallest units of the currency (integer, not float/double). For example, for a price of US$ 1.45 pass amount = 145. See the exp parameter in currencies.json, it shows the number of digits past the decimal point for each currency (2 for the majority of currencies).
    [<DataMember(Name = "total_amount")>]
    TotalAmount: int64
    /// Bot-specified invoice payload
    [<DataMember(Name = "invoice_payload")>]
    InvoicePayload: string
    /// Identifier of the shipping option chosen by the user
    [<DataMember(Name = "shipping_option_id")>]
    ShippingOptionId: string option
    /// Order information provided by the user
    [<DataMember(Name = "order_info")>]
    OrderInfo: OrderInfo option
    /// Telegram payment identifier
    [<DataMember(Name = "telegram_payment_charge_id")>]
    TelegramPaymentChargeId: string
    /// Provider payment identifier
    [<DataMember(Name = "provider_payment_charge_id")>]
    ProviderPaymentChargeId: string
  }
  static member Create(currency: string, totalAmount: int64, invoicePayload: string, telegramPaymentChargeId: string, providerPaymentChargeId: string, ?shippingOptionId: string, ?orderInfo: OrderInfo) = 
    {
      Currency = currency
      TotalAmount = totalAmount
      InvoicePayload = invoicePayload
      TelegramPaymentChargeId = telegramPaymentChargeId
      ProviderPaymentChargeId = providerPaymentChargeId
      ShippingOptionId = shippingOptionId
      OrderInfo = orderInfo
    }

/// This object contains basic information about a refunded payment.
and [<CLIMutable>] RefundedPayment =
  {
    /// Three-letter ISO 4217 currency code, or “XTR” for payments in Telegram Stars. Currently, always “XTR”
    [<DataMember(Name = "currency")>]
    Currency: string
    /// Total refunded price in the smallest units of the currency (integer, not float/double). For example, for a price of US$ 1.45, total_amount = 145. See the exp parameter in currencies.json, it shows the number of digits past the decimal point for each currency (2 for the majority of currencies).
    [<DataMember(Name = "total_amount")>]
    TotalAmount: int64
    /// Bot-specified invoice payload
    [<DataMember(Name = "invoice_payload")>]
    InvoicePayload: string
    /// Telegram payment identifier
    [<DataMember(Name = "telegram_payment_charge_id")>]
    TelegramPaymentChargeId: string
    /// Provider payment identifier
    [<DataMember(Name = "provider_payment_charge_id")>]
    ProviderPaymentChargeId: string option
  }
  static member Create(currency: string, totalAmount: int64, invoicePayload: string, telegramPaymentChargeId: string, ?providerPaymentChargeId: string) = 
    {
      Currency = currency
      TotalAmount = totalAmount
      InvoicePayload = invoicePayload
      TelegramPaymentChargeId = telegramPaymentChargeId
      ProviderPaymentChargeId = providerPaymentChargeId
    }

/// This object contains information about an incoming shipping query.
and [<CLIMutable>] ShippingQuery =
  {
    /// Unique query identifier
    [<DataMember(Name = "id")>]
    Id: string
    /// User who sent the query
    [<DataMember(Name = "from")>]
    From: User
    /// Bot-specified invoice payload
    [<DataMember(Name = "invoice_payload")>]
    InvoicePayload: string
    /// User specified shipping address
    [<DataMember(Name = "shipping_address")>]
    ShippingAddress: ShippingAddress
  }
  static member Create(id: string, from: User, invoicePayload: string, shippingAddress: ShippingAddress) = 
    {
      Id = id
      From = from
      InvoicePayload = invoicePayload
      ShippingAddress = shippingAddress
    }

/// This object contains information about an incoming pre-checkout query.
and [<CLIMutable>] PreCheckoutQuery =
  {
    /// Unique query identifier
    [<DataMember(Name = "id")>]
    Id: string
    /// User who sent the query
    [<DataMember(Name = "from")>]
    From: User
    /// Three-letter ISO 4217 currency code, or “XTR” for payments in Telegram Stars
    [<DataMember(Name = "currency")>]
    Currency: string
    /// Total price in the smallest units of the currency (integer, not float/double). For example, for a price of US$ 1.45 pass amount = 145. See the exp parameter in currencies.json, it shows the number of digits past the decimal point for each currency (2 for the majority of currencies).
    [<DataMember(Name = "total_amount")>]
    TotalAmount: int64
    /// Bot-specified invoice payload
    [<DataMember(Name = "invoice_payload")>]
    InvoicePayload: string
    /// Identifier of the shipping option chosen by the user
    [<DataMember(Name = "shipping_option_id")>]
    ShippingOptionId: string option
    /// Order information provided by the user
    [<DataMember(Name = "order_info")>]
    OrderInfo: OrderInfo option
  }
  static member Create(id: string, from: User, currency: string, totalAmount: int64, invoicePayload: string, ?shippingOptionId: string, ?orderInfo: OrderInfo) = 
    {
      Id = id
      From = from
      Currency = currency
      TotalAmount = totalAmount
      InvoicePayload = invoicePayload
      ShippingOptionId = shippingOptionId
      OrderInfo = orderInfo
    }

/// This object contains information about a paid media purchase.
and [<CLIMutable>] PaidMediaPurchased =
  {
    /// User who purchased the media
    [<DataMember(Name = "from")>]
    From: User
    /// Bot-specified paid media payload
    [<DataMember(Name = "paid_media_payload")>]
    PaidMediaPayload: string
  }
  static member Create(from: User, paidMediaPayload: string) = 
    {
      From = from
      PaidMediaPayload = paidMediaPayload
    }

/// This object describes the state of a revenue withdrawal operation. Currently, it can be one of
and RevenueWithdrawalState =
  | Pending of RevenueWithdrawalStatePending
  | Succeeded of RevenueWithdrawalStateSucceeded
  | Failed of RevenueWithdrawalStateFailed

/// The withdrawal is in progress.
and [<CLIMutable>] RevenueWithdrawalStatePending =
  {
    /// Type of the state, always “pending”
    [<DataMember(Name = "type")>]
    Type: string
  }
  static member Create(``type``: string) = 
    {
      Type = ``type``
    }

/// The withdrawal succeeded.
and [<CLIMutable>] RevenueWithdrawalStateSucceeded =
  {
    /// Type of the state, always “succeeded”
    [<DataMember(Name = "type")>]
    Type: string
    /// Date the withdrawal was completed in Unix time
    [<DataMember(Name = "date")>]
    Date: DateTime
    /// An HTTPS URL that can be used to see transaction details
    [<DataMember(Name = "url")>]
    Url: string
  }
  static member Create(``type``: string, date: DateTime, url: string) = 
    {
      Type = ``type``
      Date = date
      Url = url
    }

/// The withdrawal failed and the transaction was refunded.
and [<CLIMutable>] RevenueWithdrawalStateFailed =
  {
    /// Type of the state, always “failed”
    [<DataMember(Name = "type")>]
    Type: string
  }
  static member Create(``type``: string) = 
    {
      Type = ``type``
    }

/// This object describes the source of a transaction, or its recipient for outgoing transactions. Currently, it can be one of
and TransactionPartner =
  | User of TransactionPartnerUser
  | Fragment of TransactionPartnerFragment
  | TelegramAds of TransactionPartnerTelegramAds
  | Other of TransactionPartnerOther

/// Describes a transaction with a user.
and [<CLIMutable>] TransactionPartnerUser =
  {
    /// Type of the transaction partner, always “user”
    [<DataMember(Name = "type")>]
    Type: string
    /// Information about the user
    [<DataMember(Name = "user")>]
    User: User
    /// Bot-specified invoice payload
    [<DataMember(Name = "invoice_payload")>]
    InvoicePayload: string option
    /// Information about the paid media bought by the user
    [<DataMember(Name = "paid_media")>]
    PaidMedia: PaidMedia[] option
    /// Bot-specified paid media payload
    [<DataMember(Name = "paid_media_payload")>]
    PaidMediaPayload: string option
  }
  static member Create(``type``: string, user: User, ?invoicePayload: string, ?paidMedia: PaidMedia[], ?paidMediaPayload: string) = 
    {
      Type = ``type``
      User = user
      InvoicePayload = invoicePayload
      PaidMedia = paidMedia
      PaidMediaPayload = paidMediaPayload
    }

/// Describes a withdrawal transaction with Fragment.
and [<CLIMutable>] TransactionPartnerFragment =
  {
    /// Type of the transaction partner, always “fragment”
    [<DataMember(Name = "type")>]
    Type: string
    /// State of the transaction if the transaction is outgoing
    [<DataMember(Name = "withdrawal_state")>]
    WithdrawalState: RevenueWithdrawalState option
  }
  static member Create(``type``: string, ?withdrawalState: RevenueWithdrawalState) = 
    {
      Type = ``type``
      WithdrawalState = withdrawalState
    }

/// Describes a withdrawal transaction to the Telegram Ads platform.
and [<CLIMutable>] TransactionPartnerTelegramAds =
  {
    /// Type of the transaction partner, always “telegram_ads”
    [<DataMember(Name = "type")>]
    Type: string
  }
  static member Create(``type``: string) = 
    {
      Type = ``type``
    }

/// Describes a transaction with an unknown source or recipient.
and [<CLIMutable>] TransactionPartnerOther =
  {
    /// Type of the transaction partner, always “other”
    [<DataMember(Name = "type")>]
    Type: string
  }
  static member Create(``type``: string) = 
    {
      Type = ``type``
    }

/// Describes a Telegram Star transaction.
and [<CLIMutable>] StarTransaction =
  {
    /// Unique identifier of the transaction. Coincides with the identifer of the original transaction for refund transactions. Coincides with SuccessfulPayment.telegram_payment_charge_id for successful incoming payments from users.
    [<DataMember(Name = "id")>]
    Id: string
    /// Number of Telegram Stars transferred by the transaction
    [<DataMember(Name = "amount")>]
    Amount: int64
    /// Date the transaction was created in Unix time
    [<DataMember(Name = "date")>]
    Date: DateTime
    /// Source of an incoming transaction (e.g., a user purchasing goods or services, Fragment refunding a failed withdrawal). Only for incoming transactions
    [<DataMember(Name = "source")>]
    Source: TransactionPartner option
    /// Receiver of an outgoing transaction (e.g., a user for a purchase refund, Fragment for a withdrawal). Only for outgoing transactions
    [<DataMember(Name = "receiver")>]
    Receiver: TransactionPartner option
  }
  static member Create(id: string, amount: int64, date: DateTime, ?source: TransactionPartner, ?receiver: TransactionPartner) = 
    {
      Id = id
      Amount = amount
      Date = date
      Source = source
      Receiver = receiver
    }

/// Contains a list of Telegram Star transactions.
/// Telegram Passport is a unified authorization method for services that require personal identification. Users can upload their documents once, then instantly share their data with services that require real-world ID (finance, ICOs, etc.). Please see the manual for details.
and [<CLIMutable>] StarTransactions =
  {
    /// The list of transactions
    [<DataMember(Name = "transactions")>]
    Transactions: StarTransaction[]
  }
  static member Create(transactions: StarTransaction[]) = 
    {
      Transactions = transactions
    }

/// Describes Telegram Passport data shared with the bot by the user.
and [<CLIMutable>] PassportData =
  {
    /// Array with information about documents and other Telegram Passport elements that was shared with the bot
    [<DataMember(Name = "data")>]
    Data: EncryptedPassportElement[]
    /// Encrypted credentials required to decrypt the data
    [<DataMember(Name = "credentials")>]
    Credentials: EncryptedCredentials
  }
  static member Create(data: EncryptedPassportElement[], credentials: EncryptedCredentials) = 
    {
      Data = data
      Credentials = credentials
    }

/// This object represents a file uploaded to Telegram Passport. Currently all Telegram Passport files are in JPEG format when decrypted and don't exceed 10MB.
and [<CLIMutable>] PassportFile =
  {
    /// Identifier for this file, which can be used to download or reuse the file
    [<DataMember(Name = "file_id")>]
    FileId: string
    /// Unique identifier for this file, which is supposed to be the same over time and for different bots. Can't be used to download or reuse the file.
    [<DataMember(Name = "file_unique_id")>]
    FileUniqueId: string
    /// File size in bytes
    [<DataMember(Name = "file_size")>]
    FileSize: int64
    /// Unix time when the file was uploaded
    [<DataMember(Name = "file_date")>]
    FileDate: int64
  }
  static member Create(fileId: string, fileUniqueId: string, fileSize: int64, fileDate: int64) = 
    {
      FileId = fileId
      FileUniqueId = fileUniqueId
      FileSize = fileSize
      FileDate = fileDate
    }

/// Describes documents or other Telegram Passport elements shared with the bot by the user.
and [<CLIMutable>] EncryptedPassportElement =
  {
    /// Element type. One of “personal_details”, “passport”, “driver_license”, “identity_card”, “internal_passport”, “address”, “utility_bill”, “bank_statement”, “rental_agreement”, “passport_registration”, “temporary_registration”, “phone_number”, “email”.
    [<DataMember(Name = "type")>]
    Type: string
    /// Base64-encoded encrypted Telegram Passport element data provided by the user; available only for “personal_details”, “passport”, “driver_license”, “identity_card”, “internal_passport” and “address” types. Can be decrypted and verified using the accompanying EncryptedCredentials.
    [<DataMember(Name = "data")>]
    Data: string option
    /// User's verified phone number; available only for “phone_number” type
    [<DataMember(Name = "phone_number")>]
    PhoneNumber: string option
    /// User's verified email address; available only for “email” type
    [<DataMember(Name = "email")>]
    Email: string option
    /// Array of encrypted files with documents provided by the user; available only for “utility_bill”, “bank_statement”, “rental_agreement”, “passport_registration” and “temporary_registration” types. Files can be decrypted and verified using the accompanying EncryptedCredentials.
    [<DataMember(Name = "files")>]
    Files: PassportFile[] option
    /// Encrypted file with the front side of the document, provided by the user; available only for “passport”, “driver_license”, “identity_card” and “internal_passport”. The file can be decrypted and verified using the accompanying EncryptedCredentials.
    [<DataMember(Name = "front_side")>]
    FrontSide: PassportFile option
    /// Encrypted file with the reverse side of the document, provided by the user; available only for “driver_license” and “identity_card”. The file can be decrypted and verified using the accompanying EncryptedCredentials.
    [<DataMember(Name = "reverse_side")>]
    ReverseSide: PassportFile option
    /// Encrypted file with the selfie of the user holding a document, provided by the user; available if requested for “passport”, “driver_license”, “identity_card” and “internal_passport”. The file can be decrypted and verified using the accompanying EncryptedCredentials.
    [<DataMember(Name = "selfie")>]
    Selfie: PassportFile option
    /// Array of encrypted files with translated versions of documents provided by the user; available if requested for “passport”, “driver_license”, “identity_card”, “internal_passport”, “utility_bill”, “bank_statement”, “rental_agreement”, “passport_registration” and “temporary_registration” types. Files can be decrypted and verified using the accompanying EncryptedCredentials.
    [<DataMember(Name = "translation")>]
    Translation: PassportFile[] option
    /// Base64-encoded element hash for using in PassportElementErrorUnspecified
    [<DataMember(Name = "hash")>]
    Hash: string
  }
  static member Create(``type``: string, hash: string, ?data: string, ?phoneNumber: string, ?email: string, ?files: PassportFile[], ?frontSide: PassportFile, ?reverseSide: PassportFile, ?selfie: PassportFile, ?translation: PassportFile[]) = 
    {
      Type = ``type``
      Hash = hash
      Data = data
      PhoneNumber = phoneNumber
      Email = email
      Files = files
      FrontSide = frontSide
      ReverseSide = reverseSide
      Selfie = selfie
      Translation = translation
    }

/// Describes data required for decrypting and authenticating EncryptedPassportElement. See the Telegram Passport Documentation for a complete description of the data decryption and authentication processes.
and [<CLIMutable>] EncryptedCredentials =
  {
    /// Base64-encoded encrypted JSON-serialized data with unique user's payload, data hashes and secrets required for EncryptedPassportElement decryption and authentication
    [<DataMember(Name = "data")>]
    Data: string
    /// Base64-encoded data hash for data authentication
    [<DataMember(Name = "hash")>]
    Hash: string
    /// Base64-encoded secret, encrypted with the bot's public RSA key, required for data decryption
    [<DataMember(Name = "secret")>]
    Secret: string
  }
  static member Create(data: string, hash: string, secret: string) = 
    {
      Data = data
      Hash = hash
      Secret = secret
    }

/// This object represents an error in the Telegram Passport element which was submitted that should be resolved by the user. It should be one of:
and PassportElementError =
  | DataField of PassportElementErrorDataField
  | FrontSide of PassportElementErrorFrontSide
  | ReverseSide of PassportElementErrorReverseSide
  | Selfie of PassportElementErrorSelfie
  | File of PassportElementErrorFile
  | Files of PassportElementErrorFiles
  | TranslationFile of PassportElementErrorTranslationFile
  | TranslationFiles of PassportElementErrorTranslationFiles
  | Unspecified of PassportElementErrorUnspecified

/// Represents an issue in one of the data fields that was provided by the user. The error is considered resolved when the field's value changes.
and [<CLIMutable>] PassportElementErrorDataField =
  {
    /// Error source, must be data
    [<DataMember(Name = "source")>]
    Source: string
    /// The section of the user's Telegram Passport which has the error, one of “personal_details”, “passport”, “driver_license”, “identity_card”, “internal_passport”, “address”
    [<DataMember(Name = "type")>]
    Type: string
    /// Name of the data field which has the error
    [<DataMember(Name = "field_name")>]
    FieldName: string
    /// Base64-encoded data hash
    [<DataMember(Name = "data_hash")>]
    DataHash: string
    /// Error message
    [<DataMember(Name = "message")>]
    Message: string
  }
  static member Create(source: string, ``type``: string, fieldName: string, dataHash: string, message: string) = 
    {
      Source = source
      Type = ``type``
      FieldName = fieldName
      DataHash = dataHash
      Message = message
    }

/// Represents an issue with the front side of a document. The error is considered resolved when the file with the front side of the document changes.
and [<CLIMutable>] PassportElementErrorFrontSide =
  {
    /// Error source, must be front_side
    [<DataMember(Name = "source")>]
    Source: string
    /// The section of the user's Telegram Passport which has the issue, one of “passport”, “driver_license”, “identity_card”, “internal_passport”
    [<DataMember(Name = "type")>]
    Type: string
    /// Base64-encoded hash of the file with the front side of the document
    [<DataMember(Name = "file_hash")>]
    FileHash: string
    /// Error message
    [<DataMember(Name = "message")>]
    Message: string
  }
  static member Create(source: string, ``type``: string, fileHash: string, message: string) = 
    {
      Source = source
      Type = ``type``
      FileHash = fileHash
      Message = message
    }

/// Represents an issue with the reverse side of a document. The error is considered resolved when the file with reverse side of the document changes.
and [<CLIMutable>] PassportElementErrorReverseSide =
  {
    /// Error source, must be reverse_side
    [<DataMember(Name = "source")>]
    Source: string
    /// The section of the user's Telegram Passport which has the issue, one of “driver_license”, “identity_card”
    [<DataMember(Name = "type")>]
    Type: string
    /// Base64-encoded hash of the file with the reverse side of the document
    [<DataMember(Name = "file_hash")>]
    FileHash: string
    /// Error message
    [<DataMember(Name = "message")>]
    Message: string
  }
  static member Create(source: string, ``type``: string, fileHash: string, message: string) = 
    {
      Source = source
      Type = ``type``
      FileHash = fileHash
      Message = message
    }

/// Represents an issue with the selfie with a document. The error is considered resolved when the file with the selfie changes.
and [<CLIMutable>] PassportElementErrorSelfie =
  {
    /// Error source, must be selfie
    [<DataMember(Name = "source")>]
    Source: string
    /// The section of the user's Telegram Passport which has the issue, one of “passport”, “driver_license”, “identity_card”, “internal_passport”
    [<DataMember(Name = "type")>]
    Type: string
    /// Base64-encoded hash of the file with the selfie
    [<DataMember(Name = "file_hash")>]
    FileHash: string
    /// Error message
    [<DataMember(Name = "message")>]
    Message: string
  }
  static member Create(source: string, ``type``: string, fileHash: string, message: string) = 
    {
      Source = source
      Type = ``type``
      FileHash = fileHash
      Message = message
    }

/// Represents an issue with a document scan. The error is considered resolved when the file with the document scan changes.
and [<CLIMutable>] PassportElementErrorFile =
  {
    /// Error source, must be file
    [<DataMember(Name = "source")>]
    Source: string
    /// The section of the user's Telegram Passport which has the issue, one of “utility_bill”, “bank_statement”, “rental_agreement”, “passport_registration”, “temporary_registration”
    [<DataMember(Name = "type")>]
    Type: string
    /// Base64-encoded file hash
    [<DataMember(Name = "file_hash")>]
    FileHash: string
    /// Error message
    [<DataMember(Name = "message")>]
    Message: string
  }
  static member Create(source: string, ``type``: string, fileHash: string, message: string) = 
    {
      Source = source
      Type = ``type``
      FileHash = fileHash
      Message = message
    }

/// Represents an issue with a list of scans. The error is considered resolved when the list of files containing the scans changes.
and [<CLIMutable>] PassportElementErrorFiles =
  {
    /// Error source, must be files
    [<DataMember(Name = "source")>]
    Source: string
    /// The section of the user's Telegram Passport which has the issue, one of “utility_bill”, “bank_statement”, “rental_agreement”, “passport_registration”, “temporary_registration”
    [<DataMember(Name = "type")>]
    Type: string
    /// List of base64-encoded file hashes
    [<DataMember(Name = "file_hashes")>]
    FileHashes: string[]
    /// Error message
    [<DataMember(Name = "message")>]
    Message: string
  }
  static member Create(source: string, ``type``: string, fileHashes: string[], message: string) = 
    {
      Source = source
      Type = ``type``
      FileHashes = fileHashes
      Message = message
    }

/// Represents an issue with one of the files that constitute the translation of a document. The error is considered resolved when the file changes.
and [<CLIMutable>] PassportElementErrorTranslationFile =
  {
    /// Error source, must be translation_file
    [<DataMember(Name = "source")>]
    Source: string
    /// Type of element of the user's Telegram Passport which has the issue, one of “passport”, “driver_license”, “identity_card”, “internal_passport”, “utility_bill”, “bank_statement”, “rental_agreement”, “passport_registration”, “temporary_registration”
    [<DataMember(Name = "type")>]
    Type: string
    /// Base64-encoded file hash
    [<DataMember(Name = "file_hash")>]
    FileHash: string
    /// Error message
    [<DataMember(Name = "message")>]
    Message: string
  }
  static member Create(source: string, ``type``: string, fileHash: string, message: string) = 
    {
      Source = source
      Type = ``type``
      FileHash = fileHash
      Message = message
    }

/// Represents an issue with the translated version of a document. The error is considered resolved when a file with the document translation change.
and [<CLIMutable>] PassportElementErrorTranslationFiles =
  {
    /// Error source, must be translation_files
    [<DataMember(Name = "source")>]
    Source: string
    /// Type of element of the user's Telegram Passport which has the issue, one of “passport”, “driver_license”, “identity_card”, “internal_passport”, “utility_bill”, “bank_statement”, “rental_agreement”, “passport_registration”, “temporary_registration”
    [<DataMember(Name = "type")>]
    Type: string
    /// List of base64-encoded file hashes
    [<DataMember(Name = "file_hashes")>]
    FileHashes: string[]
    /// Error message
    [<DataMember(Name = "message")>]
    Message: string
  }
  static member Create(source: string, ``type``: string, fileHashes: string[], message: string) = 
    {
      Source = source
      Type = ``type``
      FileHashes = fileHashes
      Message = message
    }

/// Represents an issue in an unspecified place. The error is considered resolved when new data is added.
/// Your bot can offer users HTML5 games to play solo or to compete against each other in groups and one-on-one chats. Create games via @BotFather using the /newgame command. Please note that this kind of power requires responsibility: you will need to accept the terms for each game that your bots will be offering.
and [<CLIMutable>] PassportElementErrorUnspecified =
  {
    /// Error source, must be unspecified
    [<DataMember(Name = "source")>]
    Source: string
    /// Type of element of the user's Telegram Passport which has the issue
    [<DataMember(Name = "type")>]
    Type: string
    /// Base64-encoded element hash
    [<DataMember(Name = "element_hash")>]
    ElementHash: string
    /// Error message
    [<DataMember(Name = "message")>]
    Message: string
  }
  static member Create(source: string, ``type``: string, elementHash: string, message: string) = 
    {
      Source = source
      Type = ``type``
      ElementHash = elementHash
      Message = message
    }

/// This object represents a game. Use BotFather to create and edit games, their short names will act as unique identifiers.
and [<CLIMutable>] Game =
  {
    /// Title of the game
    [<DataMember(Name = "title")>]
    Title: string
    /// Description of the game
    [<DataMember(Name = "description")>]
    Description: string
    /// Photo that will be displayed in the game message in chats.
    [<DataMember(Name = "photo")>]
    Photo: PhotoSize[]
    /// Brief description of the game or high scores included in the game message. Can be automatically edited to include current high scores for the game when the bot calls setGameScore, or manually edited using editMessageText. 0-4096 characters.
    [<DataMember(Name = "text")>]
    Text: string option
    /// Special entities that appear in text, such as usernames, URLs, bot commands, etc.
    [<DataMember(Name = "text_entities")>]
    TextEntities: MessageEntity[] option
    /// Animation that will be displayed in the game message in chats. Upload via BotFather
    [<DataMember(Name = "animation")>]
    Animation: Animation option
  }
  static member Create(title: string, description: string, photo: PhotoSize[], ?text: string, ?textEntities: MessageEntity[], ?animation: Animation) = 
    {
      Title = title
      Description = description
      Photo = photo
      Text = text
      TextEntities = textEntities
      Animation = animation
    }

/// A placeholder, currently holds no information. Use BotFather to set up your game.
and CallbackGame =
  new() = {}

/// This object represents one row of the high scores table for a game.
/// And that's about all we've got for now.
/// If you've got any questions, please check out our Bot FAQ »
and [<CLIMutable>] GameHighScore =
  {
    /// Position in high score table for the game
    [<DataMember(Name = "position")>]
    Position: int64
    /// User
    [<DataMember(Name = "user")>]
    User: User
    /// Score
    [<DataMember(Name = "score")>]
    Score: int64
  }
  static member Create(position: int64, user: User, score: int64) = 
    {
      Position = position
      User = user
      Score = score
    }
