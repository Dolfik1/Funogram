[<Microsoft.FSharp.Core.RequireQualifiedAccess>]
module Funogram.Telegram.Api

open Funogram.Telegram
open Types

let deleteWebhookBase () =
  Req.GetWebhookInfo()

let getMe = Req.GetMe()

let sendMessage chatId text = Req.SendMessage.Make(ChatId.Int chatId, text)
  
let sendMessageByChatName chatName text = Req.SendMessage.Make(ChatId.String chatName, text)

let sendMessageMarkup chatId text replyMarkup = Req.SendMessage.Make(ChatId.Int chatId, text, replyMarkup = replyMarkup)

let sendMessageReply chatId text replyToMessageId = Req.SendMessage.Make(ChatId.Int chatId, text, replyToMessageId = replyToMessageId)

let forwardMessage chatId fromChatId messageId = Req.ForwardMessage.Make(ChatId.Int chatId, ChatId.Int fromChatId, messageId)

let sendPhoto chatId photo caption = Req.SendPhoto.Make(ChatId.Int chatId, photo, caption)

let sendAudio chatId audio caption = Req.SendAudio.Make(ChatId.Int chatId, audio, caption)

let sendDocument chatId document caption = Req.SendDocument.Make(ChatId.Int chatId, document, caption = caption)

let sendSticker chatId sticker = Req.SendSticker.Make(ChatId.Int chatId, sticker)

let sendVideo chatId video caption = Req.SendVideo.Make(ChatId.Int chatId, video, caption = caption)

let sendAnimation chatId animation caption = Req.SendAnimation.Make(ChatId.Int chatId, animation, caption = caption)

let sendVoice chatId voice caption = Req.SendVoice.Make(ChatId.Int chatId, voice, caption = caption)

let sendVideoNote chatId videoNote = Req.SendVideoNote.Make(ChatId.Int chatId, videoNote)

let sendMediaGroup chatId media = Req.SendMediaGroup.Make(ChatId.Int chatId, media)

let sendLocation chatId latitude longitude = Req.SendLocation.Make(ChatId.Int chatId, latitude, longitude)

let sendVenue chatId latitude longitude title address =  Req.SendVenue.Make(ChatId.Int chatId, latitude, longitude, title, address)

let sendContact chatId phoneNumber firstName lastName = Req.SendContact.Make(ChatId.Int chatId, phoneNumber, firstName, ?lastName = lastName)
  
let sendPoll chatId question options = Req.SendPoll.Make(ChatId.Int chatId, question, options)

let sendChatAction chatId action = Req.SendChatAction.Make(ChatId.Int chatId, action)
let sendChatActionByChatName chatName action = Req.SendChatAction.Make(ChatId.String chatName, action)

let private getUserProfilePhotosBase userId offset limit = Req.GetUserProfilePhotos.Make(userId, ?offset = offset, ?limit = limit)
let getUserProfilePhotos userId offset limit = getUserProfilePhotosBase userId (Some offset) (Some limit)
let getUserProfilePhotosAll userId = getUserProfilePhotosBase userId None None

let getFile fileId = { Req.GetFile.FileId = fileId }

let banChatMember chatId userId = Req.BanChatMember.Make(ChatId.Int chatId, userId)
let banChatMemberUntil chatId userId untilDate = Req.BanChatMember.Make(ChatId.Int chatId, userId, untilDate = untilDate)
let banChatMemberByChatName chatName userId = Req.BanChatMember.Make(ChatId.String chatName, userId) 
let banChatMemberByChatNameUntil chatName userId untilDate = Req.BanChatMember.Make(ChatId.String chatName, userId, untilDate = untilDate)

let unbanChatMember chatId userId = Req.UnbanChatMember.Make(ChatId.Int chatId, userId)
let unbanChatMemberByChatName chatName userId = Req.UnbanChatMember.Make(ChatId.String chatName, userId)

let restrictChatMember chatId userId permissions untilDate = Req.RestrictChatMember.Make(ChatId.Int chatId, userId, permissions)

let setChatPermission chatId permissions = Req.SetChatPermissions.Make(ChatId.Int chatId, permissions)

let exportChatInviteLink chatId = Req.ExportChatInviteLink.Make(ChatId.Int chatId)
let exportChatInviteLinkByChatName chatName = Req.ExportChatInviteLink.Make(ChatId.String chatName)

let setChatPhoto chatId photo = Req.SetChatPhoto.Make(ChatId.Int chatId, photo)
let setChatPhotoByChatName chatName photo = Req.SetChatPhoto.Make(ChatId.String chatName, photo)

let deleteChatPhoto chatId = Req.DeleteChatPhoto.Make(ChatId.Int chatId)
let deleteChatPhotoByChatName chatName = Req.DeleteChatPhoto.Make(ChatId.String chatName)

let setChatTitle chatId title = Req.SetChatTitle.Make(ChatId.Int chatId, title)
let setChatTitleByChatName chatName title = Req.SetChatTitle.Make(ChatId.String chatName, title)

let setChatDescription chatId description = Req.SetChatDescription.Make(ChatId.Int chatId, description)
let rec setChatDescriptionByChatName chatName description = Req.SetChatDescription.Make(ChatId.String chatName, description)

let pinChatMessage chatId messageId = Req.PinChatMessage.Make(ChatId.Int chatId, messageId)
let pinChatMessageByName chatName messageId = Req.PinChatMessage.Make(ChatId.String chatName, messageId)
let pinChatMessageNotify chatId messageId disableNotification = Req.PinChatMessage.Make(ChatId.Int chatId, messageId, disableNotification)
let pinChatMessageByNameNotify chatName messageId disableNotification = Req.PinChatMessage.Make(ChatId.String chatName, messageId, disableNotification)

let unpinChatMessage chatId = Req.UnpinChatMessage.Make(ChatId.Int chatId)
let unpinChatMessageByChatName chatName = Req.UnpinChatMessage.Make(ChatId.String chatName)

let leaveChat chatId = Req.LeaveChat.Make(ChatId.Int chatId)
let leaveChatByChatName chatName = Req.LeaveChat.Make(ChatId.String chatName)

let getChat chatId = Req.GetChat.Make(ChatId.Int chatId)
let getChatByName chatUsername = Req.GetChat.Make(ChatId.String chatUsername)

let getChatAdministrators chatId = Req.GetChatAdministrators.Make(ChatId.Int chatId)
let getChatAdministratorsByChatName chatName = Req.GetChatAdministrators.Make(ChatId.String chatName)


let getChatMembersCount chatId = Req.GetChatMemberCount.Make(ChatId.Int chatId)
let getChatMembersCountByChatName chatName = Req.GetChatMemberCount.Make(ChatId.String chatName)

let getChatMember chatId userId = Req.GetChatMember.Make(ChatId.Int chatId, userId)
let getChatMemberByChatName chatName userId = Req.GetChatMember.Make(ChatId.String chatName, userId)

let rec setChatStickerSet chatId stickerSetName = Req.SetChatStickerSet.Make(ChatId.Int chatId, stickerSetName)
let deleteChatStickerSet chatId = Req.DeleteChatStickerSet.Make(ChatId.Int chatId)

let answerCallbackQuery callbackQueryId text showAlert url cacheTime = Req.AnswerCallbackQuery.Make(callbackQueryId, text, showAlert, url, cacheTime)

let editMessageMediaBase chatId messageId inlineMessageId media replyMarkup =
  Req.EditMessageMedia.Make(media = media, ?chatId = chatId, ?messageId = messageId, ?inlineMessageId = inlineMessageId, ?replyMarkup = replyMarkup)

let editMessageReplyMarkupBase (chatId: ChatId option) messageId inlineMessageId replyMarkup =
  Req.EditMessageReplyMarkup.Make(?chatId = chatId, ?messageId = messageId, ?inlineMessageId = inlineMessageId, ?replyMarkup = replyMarkup)

let stopPollBase chatId messageId replyMarkup =
  ({ ChatId = chatId; MessageId = messageId; ReplyMarkup = replyMarkup }: Req.StopPoll)

let private deleteMessageBase chatId messageId = 
  ({ ChatId = chatId; MessageId = messageId }: Req.DeleteMessage)
let deleteMessage chatId messageId = deleteMessageBase (ChatId.Int chatId) messageId
let deleteMessageByChatName chatName messageId = deleteMessageBase (ChatId.String chatName) messageId

let answerInlineQueryBase inlineQueryId results cacheTime isPersonal nextOffset switchPmText switchPmParameter =
  ({ InlineQueryId = inlineQueryId; Results = results; CacheTime = cacheTime;
  IsPersonal = isPersonal; NextOffset = nextOffset; SwitchPmText = switchPmText; SwitchPmParameter = switchPmParameter }: Req.AnswerInlineQuery)

let private answerShippingQueryBase shippingQueryId ok shippingOptions errorMessage = 
  ({ ShippingQueryId = shippingQueryId; Ok = ok; 
  ShippingOptions = shippingOptions; ErrorMessage = errorMessage }: Req.AnswerShippingQuery)
let answerShippingQuery shippingQueryId shippingOptions =
  answerShippingQueryBase shippingQueryId true shippingOptions None
let answerShippingQueryError shippingQueryId errorMessage =
  answerShippingQueryBase shippingQueryId false None (Some errorMessage)

let private answerPreCheckoutQueryBase preCheckoutQueryId ok errorMessage = 
  ({ PreCheckoutQueryId = preCheckoutQueryId; Ok = ok; ErrorMessage = errorMessage; }: Req.AnswerPreCheckoutQuery)
let answerPreCheckoutQuery preCheckoutQueryId = 
  answerPreCheckoutQueryBase preCheckoutQueryId true None
let answerPreCheckoutQueryError preCheckoutQueryId errorMessage = 
  answerPreCheckoutQueryBase preCheckoutQueryId false (Some errorMessage)

let setPassportDataErrors userId errors =
  ({ UserId = userId; Errors = errors }: Req.SetPassportDataErrors)

let setGameScoreBase userId score force disableEditMessage chatId messageId inlineMessageId =
  ({ UserId = userId; Score = score; Force = force; DisableEditMessage = disableEditMessage;
  ChatId = chatId; MessageId = messageId; InlineMessageId = inlineMessageId }: Req.SetGameScore)

let private getGameHighScoresBase userId chatId messageId inlineMessageId =
  ({ UserId = userId; ChatId = chatId; 
  MessageId = messageId; InlineMessageId = inlineMessageId }: Req.GetGameHighScores)

let getGameHighScores userId chatId messageId = 
  getGameHighScoresBase userId (Some chatId) (Some messageId) None

let getGameHighScoresInline userId inlineMessageId = 
  getGameHighScoresBase userId None None (Some inlineMessageId)

// Stickers

let getStickerSet name =
  ({ Name = name }: Req.GetStickerSet)

let uploadStickerFile userId pngSticker =
  ({ UserId = userId; PngSticker = pngSticker }: Req.UploadStickerFile)
let setStickerPositionInSet sticker position =
  ({ Sticker = sticker; Position = position }: Req.SetStickerPositionInSet)