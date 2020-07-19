module Funogram.Telegram.Bot

open System
open System.Net.Http
open Funogram.Telegram.Sscanf
open Funogram.Telegram.Types
open Funogram.Telegram.Api
open Funogram.Types
open Funogram.Api

let defaultConfig =
  { Token = ""
    Offset = Some 0L
    Limit = Some 100
    Timeout = Some 60000
    AllowedUpdates = None
    Client = new HttpClient()
    ApiEndpointUrl = new Uri("https://api.telegram.org/bot")
    WebHook = None
    OnError = (fun e -> printfn "%A" e) }

type UpdateContext =
  { Update: Update
    Config: BotConfig
    Me: User }

let getTextForCommand (me: User) =
  let username  = "@" + me.Username.Value
  function
  | Some (text: string) when text.EndsWith username ->
    text.Replace(username, "") |> Some
  | text -> text

let cmd (command: string) (handler: UpdateContext -> unit) (context: UpdateContext) =
  context.Update.Message
  |> Option.bind (fun message -> getTextForCommand context.Me message.Text)
  |> Option.filter ((=) command)
  |> Option.map (fun _ -> handler context)
  |> Option.isSome
  |> not

let cmdScan (format: PrintfFormat<_, _, _, _, 't>) (handler: 't -> UpdateContext -> unit) (context: UpdateContext) =
  let scan command =
    try Some (sscanf format command)
    with _ -> None
  context.Update.Message
  |> Option.bind (fun message -> getTextForCommand context.Me message.Text)
  |> Option.bind scan
  |> Option.map (fun x -> handler x context)
  |> Option.isSome
  |> not

let private runBot config me updateArrived updatesArrived =
  let bot data = api config data

  let processUpdates updates =
    if updates |> Seq.isEmpty |> not then
      updates |> Seq.iter (fun f -> updateArrived { Update = f; Config = config; Me = me })
      updatesArrived |> Option.iter (fun x -> x updates)

  match config.WebHook with
  | None ->
    let rec loopAsync offset =
      async {
        try
          let! updatesResult =
            getUpdatesBase (Some offset) (config.Limit) config.Timeout []
            |> bot

          match updatesResult with
          | Ok updates when updates |> Seq.isEmpty |> not ->
            let offset = updates |> Seq.map (fun f -> f.UpdateId) |> Seq.max |> fun x -> x + 1L
            processUpdates updates
            return! loopAsync offset // send new offset
          | Error e ->
            config.OnError (e.AsException() :> Exception)
            return! loopAsync offset
          | _ -> return! loopAsync offset
        with ex ->
          config.OnError ex
          return! loopAsync (offset + 1L)
        return! loopAsync offset
      }

    loopAsync (config.Offset |> Option.defaultValue 0L)
  | Some webHook ->
    async {
      let listener = webHook.Listener
      let validateRequest = webHook.ValidateRequest

      if not listener.IsListening then listener.Start()

      while listener.IsListening do
        let! context = listener.GetContextAsync() |> Async.AwaitTask
        
        if validateRequest context.Request then
          match Funogram.Tools.parseJsonStream<Update> context.Request.InputStream with
          | Ok updates -> processUpdates (seq { updates })
          | Error e -> config.OnError e
        else
          context.Response.StatusCode <- 403
        context.Response.Close()
          
    }
    
let startBot config updateArrived updatesArrived =
  async {
    let! me = getMe |> api config
    return! me 
    |> function
    | Error error -> failwith error.Description
    | Ok me -> runBot config me updateArrived updatesArrived
  }

let processCommands (context: UpdateContext) =
  Seq.forall (fun command -> command context)