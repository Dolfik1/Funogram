module Funogram.TestBot.Commands.Other

open Funogram.Telegram.Bot
open Funogram.Telegram.RequestsTypes
open Funogram.Telegram.Types
open Funogram.TestBot.Core
open Funogram.Telegram.Api

let testPhotosSize config chatId =
  let x = getUserProfilePhotosAll chatId |> botResult config |> processResultWithValue
  if x.IsNone then ()
  else
    let text =
      sprintf "Photos: %s" 
        (x.Value.Photos
          |> Seq.map (Seq.last >> (fun f -> sprintf "%ix%i" f.Width f.Height))
          |> String.concat ",")

    SendMessageReq.Make(ChatId.Int chatId, text) |> bot config

let testSendAction config chatId =
  sendChatAction chatId ChatAction.Typing |> bot config

let testGetChatInfo (ctx: UpdateContext) config chatId =
  let msg = ctx.Update.Message.Value
  let result = botResult config (getChat msg.Chat.Id)
  match result with
  | Ok x ->
    botResult config (sendMessage msg.Chat.Id (sprintf "Id: %i, Type: %O" x.Id x.Type))
    |> processResultWithValue
    |> ignore
  | Error e -> printf "Error: %s" e.Description
