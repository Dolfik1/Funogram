module Funogram.TestBot.Program

open System.IO
open Funogram.TestBot
open Funogram.Api
open Funogram.Telegram.RequestsTypes
open Funogram.Types
open Funogram.Telegram.Api
open Funogram.Telegram.Bot
open System.Net

let [<Literal>] TokenFileName = "token"
let mutable botToken = "none"

let [<Literal>] PhotoUrl = "https://www.w3.org/People/mimasa/test/imgformat/img/w3c_home.jpg"

let webSocketEndpoint = None // Some "https://1c0860ec2320.ngrok.io"

let start token =
  (*
  * Set poxy
  *```fsharp
  * let handler = new HttpClientHadler ()
  * handler.Proxy <- createMyProxy ()
  * handler.UseProxy <- true
  * let config = { defaultConfig with Token = token
  *                                   Cleint = new HttpClient(handler, true) }
  *```
  *)
  let config = { defaultConfig with Token = token }

  match webSocketEndpoint with
  | Some webSocketEndpoint ->
    async {
      let apiPath = sprintf "/%s" config.Token
      let webSocketEndpoint = sprintf "%s%s" webSocketEndpoint apiPath
      let! hook = SetWebhookReq.Make(webSocketEndpoint) |> api config
      match hook with
      | Ok _ ->
        use listener = new HttpListener()
        listener.Prefixes.Add("http://*:4444/")
        listener.Start()

        let webhook = { Listener = listener; ValidateRequest = (fun req -> req.Url.LocalPath = apiPath) }
        return! startBot { config with WebHook = Some webhook } Commands.Base.updateArrived None
      | Error e -> 
        printf "Can't set webhook: %A" e
        return ()
    }
  | _ ->
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
