module Funogram.Examples.WebSocket.Program

open System.Net.Http
open Funogram.Api
open Funogram.Types
open Funogram.Telegram
open Funogram.Telegram.Bot
open System.Net

[<Literal>]
let WebSocketEndpoint = "https://1c0860ec2320.ngrok.io" // I am using ngrok for tests

let updateArrived (ctx: UpdateContext) =
  match ctx.Update.Message with
  | Some { From = Some from } ->
    async {
      match! Api.sendMessage from.Id "I got an update via websocket!" |> apiAsync ctx.Config with
      | Result.Ok _ -> printfn "Message sent successfully!"
      | Result.Error _ -> printf "Cannot send message!"
    } |> Async.Start
  | _ -> ()

[<EntryPoint>]
let main _ =
  // setup
  let handler = new HttpClientHandler()

  // you can use proxy if you want
  // handler.Proxy <- createMyProxy ()
  // handler.UseProxy <- true
  let config = { Config.defaultConfig with Client = new HttpClient(handler, true) } |> Config.withReadTokenFromFile

  async {
    let apiPath = sprintf "/%s" config.Token
    let webSocketEndpoint = sprintf "%s%s" WebSocketEndpoint apiPath
    let! hook = Req.SetWebhook.Make(webSocketEndpoint) |> apiAsync config
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
  } |> Async.RunSynchronously
  0
