module Funogram.TestBot.Commands.Markup

open Funogram.Telegram
open Funogram.Telegram.Types
open Funogram.TestBot.Core

let private sendMessageMarkup text replyMarkup config (chatId: int64) = Req.SendMessage.Make(chatId, text, replyMarkup = replyMarkup) |> bot config

let testReplyKeyboard config chatId =
  let keyboard = Array.init 2 (fun x -> Array.init 2 (fun y -> KeyboardButton.Create(y.ToString() + x.ToString())))
  let markup = Markup.ReplyKeyboardMarkup (ReplyKeyboardMarkup.Create(keyboard))
  sendMessageMarkup "That's keyboard!" markup config chatId
  
let testRemoveKeyboard config chatId =
  let markup = Markup.ReplyKeyboardRemove { RemoveKeyboard = true; Selective = None; }
  sendMessageMarkup "Keyboard was removed" markup config chatId

let testInlineKeyboard config chatId =
  let keyboard =
    [|
       [| InlineKeyboardButton.Create("Test", callbackData = "callback1") |]
       [| InlineKeyboardButton.Create("Replace", callbackData = "callback2") |]
    |]
  let markup = Markup.InlineKeyboardMarkup { InlineKeyboard = keyboard }
  sendMessageMarkup "That's inline keyboard!" markup config chatId