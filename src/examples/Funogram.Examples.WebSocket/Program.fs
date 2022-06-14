module Funogram.Examples.WebSocket.Program

open System.IO
open System.Net.Http
open Funogram.Api
open Funogram.Telegram.RequestsTypes
open Funogram.Types
open Funogram.Telegram.Api
open Funogram.Telegram.Bot
open System.Net

let [<Literal>] TokenFileName = "token"
let mutable botToken = "none"

let webSocketEndpoint = Some "https://1c0860ec2320.ngrok.io" // I am using ngrok for tests

let updateArrived (ctx: UpdateContext) =
  match ctx.Update.Message with
  | Some ({ From = Some from } as msg) ->
    async {
      match! sendMessage from.Id "I got an update via websocket!" |> api ctx.Config with
      | Result.Ok _ -> printfn "Message sent successfully!"
      | Result.Error e -> printf "Cannot send message!"
    } |> Async.Start
  | _ -> ()

let start token =
  // setup 
  let handler = new HttpClientHandler ()
  
  // you can use proxy if you want
  // handler.Proxy <- createMyProxy ()
  // handler.UseProxy <- true
  let config = { defaultConfig with Token = token; Client = new HttpClient(handler, true) }

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
        return! startBot { config with WebHook = Some webhook } updateArrived None
      | Error e -> 
        printf "Can't setup webhook: %A" e
        return ()
    }
  | _ ->
    async {
      let! _ = deleteWebhookBase () |> api config
      return! startBot config updateArrived None
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
