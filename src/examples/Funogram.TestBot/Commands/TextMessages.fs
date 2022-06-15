module Funogram.TestBot.Commands.TextMessages

open Funogram.Telegram
open Funogram.Telegram.Bot
open Funogram.TestBot.Core
open Funogram.Telegram.Types

let private sendMessageFormatted text parseMode config chatId =
  Req.SendMessage.Make(ChatId.Int chatId, text, parseMode = parseMode) |> bot config

let testMarkdown = sendMessageFormatted "Test *Markdown*" ParseMode.Markdown
let testHtml = sendMessageFormatted "Test <b>HTML</b>" ParseMode.HTML
let testNoWebpageAndNotification config chatId =
  Req.SendMessage.Make(
    ChatId.Int chatId,
    "@Dolfik! See http://fsharplang.ru - Russian F# Community",
    disableNotification = true,
    disableWebPagePreview = true
  ) |> bot config

let testReply (ctx: UpdateContext) config (chatId: int64) =
  Req.SendMessage.Make(
    chatId,
    "That's message with reply!",
    replyToMessageId = ctx.Update.Message.Value.MessageId
  ) |> bot config
 
let testForwardMessage (ctx: UpdateContext) config (chatId: int64) =
  Req.ForwardMessage.Make(
    chatId,
    chatId,
    messageId = ctx.Update.Message.Value.MessageId
  ) |> bot config

let testScan a b =
  sendMessageFormatted (sprintf "%s %s" a b) ParseMode.Markdown