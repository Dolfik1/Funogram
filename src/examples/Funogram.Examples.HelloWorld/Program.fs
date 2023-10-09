module Funogram.Examples.HelloWorld.Program

open Funogram.Api
open Funogram.Telegram
open Funogram.Telegram.Bot

let updateArrived (ctx: UpdateContext) =
  match ctx.Update.Message with
  | Some { MessageId = messageId; Chat = chat } ->
    Api.sendMessageReply chat.Id "Hello, world!" messageId |> apiAsync ctx.Config
    |> Async.Ignore
    |> Async.Start
  | _ -> ()

[<EntryPoint>]
let main _ =
  async {
    let config = Config.defaultConfig |> Config.withReadTokenFromFile
    let! _ = Api.deleteWebhookBase () |> apiAsync config
    return! startBot config updateArrived None
  } |> Async.RunSynchronously
  0
