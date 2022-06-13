module Funogram.Telegram.Api

open Funogram.Telegram.RequestsTypes
open Types
open RequestsTypes

let deleteWebhookBase () =
  GetWebhookInfoReq()

let getMe = GetMeReq()

let sendMessage chatId text = SendMessageReq.Make(ChatId.Int chatId, text)
  
let sendMessageByChatName chatName text = SendMessageReq.Make(ChatId.String chatName, text)

let sendMessageMarkup chatId text replyMarkup = SendMessageReq.Make(ChatId.Int chatId, text, replyMarkup = replyMarkup)

let sendMessageReply chatId text replyToMessageId = SendMessageReq.Make(ChatId.Int chatId, text, replyToMessageId = replyToMessageId)

let forwardMessage chatId fromChatId messageId = ForwardMessageReq.Make(ChatId.Int chatId, ChatId.Int fromChatId, messageId)

let sendPhoto chatId photo caption = SendPhotoReq.Make(ChatId.Int chatId, photo, caption)

let sendAudio chatId audio caption = SendAudioReq.Make(ChatId.Int chatId, audio, caption)

let sendDocument chatId document caption = SendDocumentReq.Make(ChatId.Int chatId, document, caption = caption)

let sendSticker chatId sticker = SendStickerReq.Make(ChatId.Int chatId, sticker)

let sendVideo chatId video caption = SendVideoReq.Make(ChatId.Int chatId, video, caption = caption)

let sendAnimation chatId animation caption = SendAnimationReq.Make(ChatId.Int chatId, animation, caption = caption)

let sendVoice chatId voice caption = SendVoiceReq.Make(ChatId.Int chatId, voice, caption = caption)

let sendVideoNote chatId videoNote = SendVideoNoteReq.Make(ChatId.Int chatId, videoNote)

let sendMediaGroup chatId media = SendMediaGroupReq.Make(ChatId.Int chatId, media)

let sendLocation chatId latitude longitude = SendLocationReq.Make(ChatId.Int chatId, latitude, longitude)

let sendVenue chatId latitude longitude title address =  SendVenueReq.Make(ChatId.Int chatId, latitude, longitude, title, address)

let sendContact chatId phoneNumber firstName lastName = SendContactReq.Make(ChatId.Int chatId, phoneNumber, firstName, ?lastName = lastName)
  
let sendPoll chatId question options = SendPollReq.Make(ChatId.Int chatId, question, options)

let sendChatAction chatId action = SendChatActionReq.Make(ChatId.Int chatId, action)
let sendChatActionByChatName chatName action = SendChatActionReq.Make(ChatId.String chatName, action)

let private getUserProfilePhotosBase userId offset limit = { UserId = userId; Offset = offset; Limit = limit; }
let getUserProfilePhotos userId offset limit = getUserProfilePhotosBase userId (Some offset) (Some limit)
let getUserProfilePhotosAll userId = getUserProfilePhotosBase userId None None

let getFile fileId = { GetFileReq.FileId = fileId }

let banChatMember chatId userId = BanChatMemberReq.Make(ChatId.Int chatId, userId)
let banChatMemberUntil chatId userId untilDate = BanChatMemberReq.Make(ChatId.Int chatId, userId, untilDate = untilDate)
let banChatMemberByChatName chatName userId = BanChatMemberReq.Make(ChatId.String chatName, userId) 
let banChatMemberByChatNameUntil chatName userId untilDate = BanChatMemberReq.Make(ChatId.String chatName, userId, untilDate = untilDate)

let unbanChatMember chatId userId = UnbanChatMemberReq.Make(ChatId.Int chatId, userId)
let unbanChatMemberByChatName chatName userId = UnbanChatMemberReq.Make(ChatId.String chatName, userId)

let restrictChatMember chatId userId permissions untilDate = RestrictChatMemberReq.Make(ChatId.Int chatId, userId, permissions)

let setChatPermission chatId permissions = SetChatPermissionsReq.Make(ChatId.Int chatId, permissions)

let exportChatInviteLink chatId = ExportChatInviteLinkReq.Make(ChatId.Int chatId)
let exportChatInviteLinkByChatName chatName = ExportChatInviteLinkReq.Make(ChatId.String chatName)

let setChatPhoto chatId photo = SetChatPhotoReq.Make(ChatId.Int chatId, photo)
let setChatPhotoByChatName chatName photo = SetChatPhotoReq.Make(ChatId.String chatName, photo)

let deleteChatPhoto chatId = DeleteChatPhotoReq.Make(ChatId.Int chatId)
let deleteChatPhotoByChatName chatName = DeleteChatPhotoReq.Make(ChatId.String chatName)

let setChatTitle chatId title = SetChatTitleReq.Make(ChatId.Int chatId, title)
let setChatTitleByChatName chatName title = SetChatTitleReq.Make(ChatId.String chatName, title)

let setChatDescription chatId description = SetChatDescriptionReq.Make(ChatId.Int chatId, description)
let rec setChatDescriptionByChatName chatName description = SetChatDescriptionReq.Make(ChatId.String chatName, description)

let pinChatMessage chatId messageId = PinChatMessageReq.Make(ChatId.Int chatId, messageId)
let pinChatMessageByName chatName messageId = PinChatMessageReq.Make(ChatId.String chatName, messageId)
let pinChatMessageNotify chatId messageId disableNotification = PinChatMessageReq.Make(ChatId.Int chatId, messageId, disableNotification)
let pinChatMessageByNameNotify chatName messageId disableNotification = PinChatMessageReq.Make(ChatId.String chatName, messageId, disableNotification)

let unpinChatMessage chatId = UnpinChatMessageReq.Make(ChatId.Int chatId)
let unpinChatMessageByChatName chatName = UnpinChatMessageReq.Make(ChatId.String chatName)

let leaveChat chatId = LeaveChatReq.Make(ChatId.Int chatId)
let leaveChatByChatName chatName = LeaveChatReq.Make(ChatId.String chatName)

let getChat chatId = GetChatReq.Make(ChatId.Int chatId)
let getChatByName chatUsername = GetChatReq.Make(ChatId.String chatUsername)

let getChatAdministrators chatId = GetChatAdministratorsReq.Make(ChatId.Int chatId)
let getChatAdministratorsByChatName chatName = GetChatAdministratorsReq.Make(ChatId.String chatName)


let getChatMembersCount chatId = GetChatMemberCountReq.Make(ChatId.Int chatId)
let getChatMembersCountByChatName chatName = GetChatMemberCountReq.Make(ChatId.String chatName)

let getChatMember chatId userId = GetChatMemberReq.Make(ChatId.Int chatId, userId)
let getChatMemberByChatName chatName userId = GetChatMemberReq.Make(ChatId.String chatName, userId)

let rec setChatStickerSet chatId stickerSetName = SetChatStickerSetReq.Make(ChatId.Int chatId, stickerSetName)
let deleteChatStickerSet chatId = DeleteChatStickerSetReq.Make(ChatId.Int chatId)

let answerCallbackQuery callbackQueryId text showAlert url cacheTime = AnswerCallbackQueryReq.Make(callbackQueryId, text, showAlert, url, cacheTime)

let editMessageMediaBase chatId messageId inlineMessageId media replyMarkup =
  { ChatId = chatId; MessageId = messageId; InlineMessageId = inlineMessageId; Media = media; ReplyMarkup = replyMarkup }

let editMessageReplyMarkupBase chatId messageId inlineMessageId replyMarkup =
  { EditMessageReplyMarkupReq.ChatId = chatId; MessageId = messageId; 
  InlineMessageId = inlineMessageId; ReplyMarkup = replyMarkup }

let stopPollBase chatId messageId replyMarkup =
  { StopPollReq.ChatId = chatId; MessageId = messageId; ReplyMarkup = replyMarkup }

let private deleteMessageBase chatId messageId = 
  { DeleteMessageReq.ChatId = chatId; MessageId = messageId }
let deleteMessage chatId messageId = deleteMessageBase (ChatId.Int chatId) messageId
let deleteMessageByChatName chatName messageId = deleteMessageBase (ChatId.String chatName) messageId

let answerInlineQueryBase inlineQueryId results cacheTime isPersonal nextOffset switchPmText switchPmParameter =
  { AnswerInlineQueryReq.InlineQueryId = inlineQueryId; Results = results; CacheTime = cacheTime;
  IsPersonal = isPersonal; NextOffset = nextOffset; SwitchPmText = switchPmText; SwitchPmParameter = switchPmParameter }

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

let setPassportDataErrors userId errors =
  { SetPassportDataErrorsReq.UserId = userId; Errors = errors }

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
let setStickerPositionInSet sticker position =
  { SetStickerPositionInSetReq.Sticker = sticker; Position = position }