module Funogram.TestBot.Commands.Other

open Funogram.Telegram.Bot
open Funogram.Telegram.Types
open Funogram.TestBot.Core
open Funogram.Telegram

let testPhotosSize config (chatId: int64) =
  let x = Api.getUserProfilePhotosAll chatId |> botResult config |> processResultWithValue
  if x.IsNone then ()
  else
    let text =
      sprintf "Photos: %s" 
        (x.Value.Photos
          |> Seq.map (Seq.last >> (fun f -> sprintf "%ix%i" f.Width f.Height))
          |> String.concat ",")

    Req.SendMessage.Make(chatId, text) |> bot config

let testSendAction config chatId =
  Api.sendChatAction chatId ChatAction.Typing |> bot config

let testGetChatInfo (ctx: UpdateContext) config chatId =
  let msg = ctx.Update.Message.Value
  let result = botResult config (Api.getChat msg.Chat.Id)
  match result with
  | Ok x ->
    botResult config (Api.sendMessage msg.Chat.Id (sprintf "Id: %i, Type: %O" x.Id x.Type))
    |> processResultWithValue
    |> ignore
  | Error e -> printf "Error: %s" e.Description
