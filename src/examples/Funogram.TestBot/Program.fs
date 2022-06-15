module Funogram.TestBot.Program

open System.IO
open Funogram.TestBot
open Funogram.Api
open Funogram.Types
open Funogram.Telegram.Api
open Funogram.Telegram.Bot

let [<Literal>] TokenFileName = "token"
let mutable botToken = "none"

let start token =
  let config = { defaultConfig with Token = token }
  async {
    let! _ = deleteWebhookBase () |> api config
    return! startBot config Commands.Base.updateArrived None
  }
  

[<EntryPoint>]
let main _ =
  printfn "Bot started..."
  let startBot = 
    if File.Exists(TokenFileName) then
      start (File.ReadAllText(TokenFileName))
    else
      printf "Please, enter bot token: "
      let token = System.Console.ReadLine()
      File.WriteAllText(TokenFileName, token)
      start token
  startBot |> Async.RunSynchronously
  0
