module Funogram.Api

open Funogram.Types
open Funogram.RequestsTypes
open Funogram.Tools

open System.Reflection
open JsonConverters
open System.Net.Http

type BotConfig = 
    { Token: string
      Offset: int64 option
      Limit: int option
      Timeout: int option
      AllowedUpdates: string seq option
      Client: HttpClient }

let private getArgs (body: IRequestBase<'a>) =
    let props = body.GetType().GetTypeInfo().GetProperties() |> Array.toList
    props |> List.map (fun f -> (getSnakeCaseName f.Name, f.GetValue(body)))

let api config (body: IRequestBase<'a>) = 
    Api.MakeRequestAsync<'a> (config.Client, config.Token, body.MethodName, (getArgs body))

let getUpdatesBase offset limit timeout allowedUpdates =
    { Offset = offset; Limit = limit; Timeout = timeout; AllowedUpdates = allowedUpdates }

let getMe = GetMeReq()

let sendMessage chatId text =
    { sendMessageReqBase with ChatId = ChatId.Int chatId; Text = text }
let sendMessageByChatName chatName text =
    { sendMessageReqBase with ChatId = ChatId.String chatName; Text = text }
let sendMessageBase chatId text parseMode disableWebPagePreview disableNotification replyToMessageId replyMarkup =
    { ChatId = chatId; Text = text; ParseMode = parseMode; 
    DisableWebPagePreview = disableWebPagePreview; DisableNotification = disableNotification; 
    ReplyToMessageId = replyToMessageId; ReplyMarkup = replyMarkup }
let sendMessageMarkup chatId text replyMarkup =
    { sendMessageReqBase with ChatId = ChatId.Int chatId; Text = text; ReplyMarkup = Some replyMarkup }
let sendMessageReply chatId text replyToMessageId =
    { sendMessageReqBase with ChatId = ChatId.Int chatId; Text = text; ReplyToMessageId = replyToMessageId }

let forwardMessageBase chatId fromChatId messageId disableNotification =
    { ChatId = chatId; FromChatId = fromChatId; MessageId = messageId; DisableNotification = disableNotification }
let forwardMessage chatId fromChatId messageId = 
    forwardMessageBase (ChatId.Int chatId) (ChatId.Int fromChatId) messageId None

let sendPhotoBase chatId photo caption disableNotification replyToMessageId replyMarkup =
    { ChatId = chatId; Photo = photo; Caption = caption;
    DisableNotification = disableNotification;
    ReplyToMessageId = replyToMessageId; ReplyMarkup = replyMarkup }
let sendPhoto chatId photo caption =
    sendPhotoBase (ChatId.Int chatId) photo (Some caption) None None None

let sendAudioBase chatId audio caption duration performer title disableNotification replyToMessageId replyMarkup =
    { ChatId = chatId; Audio = audio; Caption = caption; Duration = duration; 
    Performer = performer; Title = title; DisableNotification = disableNotification;
    ReplyToMessageId = replyToMessageId; ReplyMarkup = replyMarkup }
let sendAudio chatId audio caption = sendAudioBase (ChatId.Int chatId) audio (Some caption) None None None None None None

let sendDocumentBase chatId document caption disableNotification replyToMessageId replyMarkup =
    { ChatId = chatId; Document = document; Caption = caption; DisableNotification = disableNotification;
    ReplyToMessageId = replyToMessageId; ReplyMarkup = replyMarkup }
let sendDocument chatId document caption = sendDocumentBase (ChatId.Int chatId) document (Some caption) None None None

let sendStickerBase chatId sticker disableNotification replyToMessageId replyMarkup =
    { ChatId = chatId; Sticker = sticker; DisableNotification = disableNotification; 
    ReplyToMessageId = replyToMessageId; ReplyMarkup = replyMarkup; }
let sendSticker chatId sticker = sendStickerBase (ChatId.Int chatId) sticker None None None

let sendVideoBase chatId video duration width height caption disableNotification replyToMessageId replyMarkup =
    { ChatId = chatId; Video = video; Duration = duration; Width = width; 
    Height = height; Caption = caption; DisableNotification = disableNotification; 
    ReplyToMessageId = replyToMessageId; ReplyMarkup = replyMarkup }
let sendVideo chatId video caption = sendVideoBase (ChatId.Int chatId) video None None None (Some caption) None None None

let sendVoiceBase chatId voice caption duration disableNotification replyToMessageId replyMarkup =
    { ChatId = chatId; Voice = voice; Caption = caption; Duration = duration;
    DisableNotification = disableNotification; ReplyToMessageId = replyToMessageId; ReplyMarkup = replyMarkup }
let sendVoice chatId voice caption = sendVoiceBase (ChatId.Int chatId) voice (Some caption) None None None None

let sendVideoNoteBase chatId videoNote duration length disableNotification replyToMessageId replyMarkup =
    { ChatId = chatId; VideoNote = videoNote; Duration = duration; Length = length; 
    DisableNotification = disableNotification; ReplyToMessageId = replyToMessageId; ReplyMarkup = replyMarkup }
let sendVideoNote chatId videoNote = sendVideoNoteBase (ChatId.Int chatId) videoNote None None None None None

let sendLocationBase chatId latitude longitude disableNotification replyToMessageId replyMarkup =
    { ChatId = chatId; Latitude = latitude; Longitude = longitude; DisableNotification = disableNotification; 
    ReplyToMessageId = replyToMessageId; ReplyMarkup = replyMarkup }
let sendLocation chatId latitude longitude = sendLocationBase (ChatId.Int chatId) latitude longitude None None None

let sendVenueBase chatId latitude longitude title address foursquareId disableNotification replyToMessageId replyMarkup =
    { ChatId = chatId; Latitude = latitude; Longitude = longitude; Title = title;
    Address = address; FoursquareId = foursquareId; DisableNotification = disableNotification; 
    ReplyToMessageId = replyToMessageId; ReplyMarkup = replyMarkup }
let sendVenue chatId latitude longitude title address = 
    sendVenueBase (ChatId.Int chatId) latitude longitude title address None None None None

let sendContactBase chatId phoneNumber firstName lastName disableNotification replyToMessageId replyMarkup =
    { ChatId = chatId; PhoneNumber = phoneNumber; FirstName = firstName; LastName = lastName;
    DisableNotification = disableNotification; ReplyToMessageId = replyToMessageId; ReplyMarkup = replyMarkup }
let sendContact chatId phoneNumber firstName lastName =
    sendContactBase (ChatId.Int chatId) phoneNumber firstName lastName None None None

let private sendChatActionBase chatId action = { ChatId = chatId; Action = action }
let sendChatAction chatId action = sendChatActionBase (ChatId.Int chatId) action
let sendChatActionByChatName chatName action = sendChatActionBase (ChatId.String chatName) action

let private getUserProfilePhotosBase userId offset limit = { UserId = userId; Offset = offset; Limit = limit; }
let getUserProfilePhotos userId offset limit = getUserProfilePhotosBase userId (Some offset) (Some limit)
let getUserProfilePhotosAll userId = getUserProfilePhotosBase userId None None

let getFile fileId = { GetFileReq.FileId = fileId }

let private kickChatMemberBase chatId userId untilDate = { ChatId = chatId; UserId = userId; UntilDate = untilDate }
let kickChatMember chatId userId = kickChatMemberBase (ChatId.Int chatId) userId None
let kickChatMemberUntil chatId userId untilDate = kickChatMemberBase (ChatId.Int chatId) userId (Some untilDate)
let kickChatMemberByChatName chatName userId = kickChatMemberBase (ChatId.String chatName) userId None
let kickChatMemberByChatNameUntil chatName userId untilDate = kickChatMemberBase (ChatId.String chatName) userId (Some untilDate)

let private unbanChatMemberBase chatId userId = { ChatId = chatId; UserId = userId; }
let unbanChatMember chatId userId = unbanChatMemberBase (ChatId.Int chatId) userId
let unbanChatMemberByChatName chatName userId = unbanChatMemberBase (ChatId.String chatName) userId

let restrictChatMemberBase chatId userId untilDate canSendMessages canSendMediaMessages canSendOtherMessages canAddWebPagePreviews =
    { ChatId = chatId; UserId = userId; UntilDate = untilDate; CanSendMessages = canSendMessages; 
    CanSendMediaMessages = canSendMediaMessages; CanSendOtherMessages = canSendOtherMessages; CanAddWebPagePreviews = canAddWebPagePreviews }

let promoteChatMemberBase chatId userId canChangeInfo canPostMessages canEditMessages canDeleteMessages canInviteUsers canRestrictMembers canPinMessages canPromoteMembers =
    { ChatId = chatId; UserId = userId; CanChangeInfo = canChangeInfo; CanPostMessages = canPostMessages;
    CanEditMessages = canEditMessages;  CanDeleteMessages = canDeleteMessages; CanInviteUsers = canInviteUsers;
    CanRestrictMembers = canRestrictMembers; CanPinMessages = canPinMessages; CanPromoteMembers = canPromoteMembers }

let private exportChatInviteLinkBase chatId = { ExportChatInviteLinkReq.ChatId = chatId }
let exportChatInviteLink chatId = exportChatInviteLinkBase (ChatId.Int chatId)
let exportChatInviteLinkByChatName chatName = exportChatInviteLinkBase (ChatId.String chatName)

let private setChatPhotoBase chatId photo = { ChatId = chatId; Photo = photo }
let setChatPhoto chatId photo = setChatPhotoBase (ChatId.Int chatId) photo
let setChatPhotoByChatName chatName photo = setChatPhotoBase (ChatId.String chatName) photo

let private deleteChatPhotoBase chatId = { DeleteChatPhotoReq.ChatId = chatId }
let deleteChatPhoto chatId = deleteChatPhotoBase (ChatId.Int chatId)
let deleteChatPhotoByChatName chatName = deleteChatPhotoBase (ChatId.String chatName)

let private setChatTitleBase chatId title = { SetChatTitleReq.ChatId = chatId; Title = title }
let setChatTitle chatId title = setChatTitleBase (ChatId.Int chatId) title
let setChatTitleByChatName chatName title = setChatTitleBase (ChatId.String chatName) title

let private setChatDescriptionBase chatId description = { SetChatDescriptionReq.ChatId = chatId; Description = description }
let setChatDescription chatId description = setChatDescriptionBase (ChatId.Int chatId) description
let setChatDescriptionByChatName chatName description = setChatDescriptionBase (ChatId.String chatName) description

let private pinChatMessageBase chatId messageId disableNotification =
    { PinChatMessageReq.ChatId = chatId; MessageId = messageId; DisableNotification = disableNotification }
let pinChatMessage chatId messageId = pinChatMessageBase (ChatId.Int chatId) messageId None
let pinChatMessageByName chatName messageId = pinChatMessageBase (ChatId.String chatName) messageId None
let pinChatMessageNotify chatId messageId disableNotification = pinChatMessageBase (ChatId.Int chatId) messageId (Some disableNotification)
let pinChatMessageByNameNotify chatName messageId disableNotification = pinChatMessageBase (ChatId.String chatName) messageId (Some disableNotification)

let private unpinChatMessageBase chatId = { UnpinChatMessageReq.ChatId = chatId }
let unpinChatMessage chatId = unpinChatMessageBase (ChatId.Int chatId)
let unpinChatMessageByChatName chatName = unpinChatMessageBase (ChatId.String chatName)

let private leaveChatBase chatId = { LeaveChatReq.ChatId = chatId }
let leaveChat chatId = leaveChatBase (ChatId.Int chatId)
let leaveChatByChatName chatName = leaveChatBase (ChatId.String chatName)

let private getChatBase chatId =  { GetChatReq.ChatId = chatId }
let getChat chatId = getChatBase (ChatId.Int chatId)
let getChatByName chatUsername = getChatBase (ChatId.String chatUsername)

let private getChatAdministratorsBase chatId = { GetChatAdministratorsReq.ChatId = chatId }
let getChatAdministrators chatId = getChatAdministratorsBase (ChatId.Int chatId)
let getChatAdministratorsByChatName chatName = getChatAdministratorsBase (ChatId.String chatName)


let private getChatMembersCountBase chatId = { GetChatMembersCountReq.ChatId = chatId }
let getChatMembersCount chatId = getChatMembersCountBase (ChatId.Int chatId)
let getChatMembersCountByChatName chatName = getChatMembersCountBase (ChatId.String chatName)

let private getChatMemberBase chatId userId = { GetChatMemberReq.ChatId = chatId; UserId = userId }
let getChatMember chatId userId = getChatMemberBase (ChatId.Int chatId) userId
let getChatMemberByChatName chatName userId = getChatMemberBase (ChatId.String chatName) userId

let answerCallbackQueryBase callbackQueryId text showAlert url cacheTime =
    { AnswerCallbackQueryReq.CallbackQueryId = callbackQueryId; Text = text; 
    ShowAlert = showAlert; Url = url; CacheTime = cacheTime }

let editMessageTextBase chatId messageId inlineMessageId text parseMode disableWebPagePreview replyMarkup =
    { EditMessageTextReq.ChatId = chatId; MessageId = messageId; InlineMessageId = inlineMessageId; 
    Text = text; ParseMode = parseMode; DisableWebPagePreview = disableWebPagePreview; ReplyMarkup = replyMarkup }

let editMessageCaptionBase chatId messageId inlineMessageId caption replyMarkup =
    { EditMessageCaptionReq.ChatId = chatId; MessageId = messageId; InlineMessageId = inlineMessageId;
    Caption = caption; ReplyMarkup = replyMarkup }

let editMessageReplyMarkupBase chatId messageId inlineMessageId replyMarkup =
    { EditMessageReplyMarkupReq.ChatId = chatId; MessageId = messageId; 
    InlineMessageId = inlineMessageId; ReplyMarkup = replyMarkup }

let private deleteMessageBase chatId messageId = 
    { DeleteMessageReq.ChatId = chatId; MessageId = messageId }
let deleteMessage chatId messageId = deleteMessageBase (ChatId.Int chatId) messageId
let deleteMessageByChatName chatName messageId = deleteMessageBase (ChatId.String chatName) messageId

let answerInlineQueryBase inlineQueryId results cacheTime isPersonal nextOffset switchPmText switchPmParameter =
    { AnswerInlineQueryReq.InlineQueryId = inlineQueryId; Results = results; CacheTime = cacheTime;
    IsPersonal = isPersonal; NextOffset = nextOffset; SwitchPmText = switchPmText; SwitchPmParameter = switchPmParameter }

let sendInvoice chatId title payload providerToken startParameter currency prices invoiceArgs =
    { SendInvoiceReq.ChatId = chatId; Title = title; Payload = payload; ProviderToken = providerToken;
    StartParameter = startParameter; Currency = currency; Prices = prices; PhotoUrl = invoiceArgs.PhotoUrl;
    Description = invoiceArgs.Description; PhotoSize = invoiceArgs.PhotoSize; PhotoWidth = invoiceArgs.PhotoWidth;
    PhotoHeight = invoiceArgs.PhotoHeight; NeedName = invoiceArgs.NeedName; NeedPhoneNumber = invoiceArgs.NeedPhoneNumber;
    NeedEmail = invoiceArgs.NeedEmail; NeedShippingAddress = invoiceArgs.NeedShippingAddress; IsFlexible = invoiceArgs.IsFlexible;
    ReplyToMessageId = invoiceArgs.ReplyToMessageId; ReplyMarkup = invoiceArgs.ReplyMarkup }

let private answerShippingQueryBase shippingQueryId ok shippingOptions errorMessage = 
    { AnswerShippingQueryReq.ShippingQueryId = shippingQueryId; Ok = ok; 
    ShippingOptions = shippingOptions; ErrorMessage = errorMessage }
let answerShippingQuery shippingQueryId shippingOptions =
    answerShippingQueryBase shippingQueryId true shippingOptions None
let answerShippingQueryError shippingQueryId errorMessage =
    answerShippingQueryBase shippingQueryId false None (Some errorMessage)

let private answerPreCheckoutQueryBase preCheckoutQueryId ok errorMessage = 
    { AnswerPreCheckoutQueryReq.PreCheckoutQueryId = preCheckoutQueryId; Ok = ok; ErrorMessage = errorMessage; }
let answerPreCheckoutQuery preCheckoutQueryId = 
    answerPreCheckoutQueryBase preCheckoutQueryId true None
let answerPreCheckoutQueryError preCheckoutQueryId errorMessage = 
    answerPreCheckoutQueryBase preCheckoutQueryId false (Some errorMessage)

let sendGameBase chatId gameShortName disableNotification replyToMessageId replyMarkup =
    { SendGameReq.ChatId = chatId; GameShortName = gameShortName; 
    DisableNotification = disableNotification; ReplyToMessageId = replyToMessageId; ReplyMarkup = replyMarkup }

let setGameScoreBase userId score force disableEditMessage chatId messageId inlineMessageId =
    { SetGameScoreReq.UserId = userId; Score = score; Force = force; DisableEditMessage = disableEditMessage;
    ChatId = chatId; MessageId = messageId; InlineMessageId = inlineMessageId }

let private getGameHighScoresBase userId chatId messageId inlineMessageId =
    { GetGameHighScoresReq.UserId = userId; ChatId = chatId; 
    MessageId = messageId; InlineMessageId = inlineMessageId }

let getGameHighScores userId chatId messageId = 
    getGameHighScoresBase userId (Some chatId) (Some messageId) None

let getGameHighScoresInline userId inlineMessageId = 
    getGameHighScoresBase userId None None (Some inlineMessageId)

// Stickers

let getStickerSet name =
    { GetStickerSetReq.Name = name }

let uploadStickerFile userId pngSticker =
    { UploadStickerFileReq.UserId = userId; PngSticker = pngSticker }

let createNewStickerSetBase userId name title pngSticker emojis containsMasks maskPosition =
    { CreateNewStickerSetReq.UserId = userId; Name = name; Title = title;
    PngSticker = pngSticker; Emojis = emojis; ContainsMasks = containsMasks;
    MaskPosition = maskPosition }

let private addStickerToSetBase userId name pngSticker emojis maskPosition = 
    { AddStickerToSetReq.UserId = userId; Name = name; PngSticker = pngSticker; 
    Emojis = emojis; MaskPosition = maskPosition  }

let addStickerToSet userId name pngSticker emojis =
    addStickerToSetBase userId name pngSticker emojis None

let addStickerToSetWithMask userId name pngSticker emojis maskPosition =
    addStickerToSetBase userId name pngSticker emojis (Some maskPosition)

let setStickerPositionInSet sticker position =
    { SetStickerPositionInSetReq.Sticker = sticker; Position = position }

let deleteStickerFromSet sticker =
    { DeleteStickerFromSet.Sticker = sticker }