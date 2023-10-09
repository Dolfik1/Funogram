module Funogram.TestBot.Program

open Funogram.TestBot
open Funogram.Api
open Funogram.Telegram
open Funogram.Telegram.Bot

[<EntryPoint>]
let main _ =
  async {
    let config = Config.defaultConfig |> Config.withReadTokenFromFile
    let! _ = Api.deleteWebhookBase () |> apiAsync config
    return! startBot config Commands.Base.updateArrived None
  } |> Async.RunSynchronously
  0
