module Funogram.Telegram.Bot

open System
open System.IO
open System.Net
open System.Net.Http
open Funogram.Telegram
open Funogram.Telegram.Sscanf
open Funogram.Telegram.Types
open Funogram.Types
open Funogram.Api
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Logging.Abstractions

[<Literal>]
let TokenFileName = "token"

[<RequireQualifiedAccess>]
module Config =
  let defaultConfig =
    { IsTest = false
      Token = BotToken.Create(String.Empty)
      Offset = Some 0L
      Limit = Some 100
      Timeout = Some 60
      AllowedUpdates = None
      Client = new HttpClient()
      ApiEndpointUrl = Uri("https://api.telegram.org/bot")
      WebHook = None
      Logger = NullLogger.Instance
      JsonOptions = Funogram.Tools.options }

  let withReadTokenFromFile config =
    if File.Exists(TokenFileName) then
      { config with Token = File.ReadAllText(TokenFileName) |> BotToken.Create }
    else
      printf "Please, enter bot token: "
      let token = Console.ReadLine()
      File.WriteAllText(TokenFileName, token)
      { config with Token = token |> BotToken.Create }

  let withReadTokenFromEnv envName config =
    { config with Token = Environment.GetEnvironmentVariable(envName) |> BotToken.Create }

type UpdateContext =
  { Update: Update
    Config: BotConfig
    Me: User }

// Text of the command; 1-32 characters. Can contain only lowercase English letters, digits and underscores.
let inline private isAllowedChar (c: char) = Char.IsLetter c || Char.IsDigit c || c = '_'

// returns -1 if the command is not valid otherwise index of last character 
let private validateCommand (text: string) =
  let rec iter (text: string) i len =
    if i >= len || isAllowedChar text[i] |> not then
      (i - 1)
    else
      iter text (i + 1) len
  
  if text.Length <= 1 || text[0] <> '/' then -1
  else iter text 1 text.Length
  
let getTextForCommand (me: User) (textOriginal: string option) =
  match me.Username, textOriginal with
  | Some username, Some text when text.Length > 0 && text[0] = '/' ->
    match validateCommand text with
    | -1 -> textOriginal
    | idx when text.Length = idx + 1 -> Some text
    | idx when text[idx + 1] = '@' && text.IndexOf(username, idx + 1, StringComparison.OrdinalIgnoreCase) = idx + 2 ->
      text.Remove(idx + 1, username.Length + 1) |> Some
    | _ -> textOriginal
  | _ -> textOriginal
  
let checkCommand (context: UpdateContext) (command: string) =
  match context.Update.Message with
  | Some { Text = text } when (getTextForCommand context.Me text) = Some command -> true
  | _ -> false
    
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

  let processUpdates (updates: Update[]) =
    if updates.Length > 0 then
      for update in updates do
        try updateArrived { Update = update; Config = config; Me = me }
        with ex -> config.Logger.LogError(ex, "Handler failed for update {UpdateId}", update.UpdateId)
      try updatesArrived |> Option.iter (fun x -> x updates)
      with ex -> config.Logger.LogError(ex, "Batch handler failed")

  match config.WebHook with
  | None ->
    let fetchUpdatesAsync offset =
      async {
        let! updatesResult =
          Req.GetUpdates.Make(offset, ?limit = config.Limit, ?timeout = config.Timeout, ?allowedUpdates = (config.AllowedUpdates |> Option.map Seq.toArray))
          |> bot

        match updatesResult with
        | Ok updates when updates.Length > 0 ->
          let nextOffset = (updates |> Array.maxBy _.UpdateId).UpdateId + 1L
          processUpdates updates
          return nextOffset
        | Error error ->
          let delaySeconds =
            match error with
            | ApiError.Rejected { Parameters = Some { RetryAfter = Some seconds } } -> float seconds
            | ApiError.Rejected { ErrorCode = 409 } -> 5.
            | _ -> 1.
          let delay = TimeSpan.FromSeconds(delaySeconds)
          match error with
          | ApiError.Rejected { Parameters = Some { RetryAfter = Some _ } } ->
            config.Logger.LogWarning("Flood control on getUpdates, retrying in {Delay}", delay)
          | _ ->
            config.Logger.LogError(error.AsException(), "Unable to read updates, retrying in {Delay}", delay)

          do! Async.Sleep(delay)
          return offset
        | _ ->
          return offset
      }

    let loopAsync offset =
      async {
        let mutable offset = offset
        let! ct = Async.CancellationToken
        while not ct.IsCancellationRequested do
          try
            let! next = fetchUpdatesAsync offset
            offset <- next
          with ex when not ct.IsCancellationRequested ->
            config.Logger.LogError(ex, "Unexpected error in polling loop")
            do! Async.Sleep 1000
      }

    loopAsync (config.Offset |> Option.defaultValue 0L)
  | Some webHook ->
    async {
      let! ct = Async.CancellationToken
      let listener = webHook.Listener
      if not listener.IsListening then listener.Start()
      use _ = ct.Register(fun () -> listener.Stop())

      let handleAsync (context: HttpListenerContext) =
        async {
          try
            if not (webHook.ValidateRequest context.Request) then
              context.Response.StatusCode <- 403
              context.Response.Close()
            else
              use body = new MemoryStream()
              do! context.Request.InputStream.CopyToAsync(body) |> Async.AwaitTask
              context.Response.StatusCode <- 200
              context.Response.Close()

              body.Seek(0L, SeekOrigin.Begin) |> ignore
              match (try Ok (Funogram.Tools.parseJsonUtf8Stream<Update> config body) with ex -> Error ex) with
              | Ok update -> processUpdates [| update |]
              | Error ex ->
                config.Logger.LogError(ex, "Unable to parse webhook update")
                if config.Logger.IsEnabled LogLevel.Trace then
                  body.Seek(0L, SeekOrigin.Begin) |> ignore
                  use sr = new StreamReader(body)
                  config.Logger.LogTrace("Raw webhook body: {Body}", sr.ReadToEnd())
          with ex ->
            config.Logger.LogError(ex, "Webhook request failed")
            try context.Response.Abort() with _ -> ()
        }

      while listener.IsListening do
        try
          let! context = listener.GetContextAsync() |> Async.AwaitTask
          do! handleAsync context
        with
        | :? HttpListenerException | :? ObjectDisposedException when not listener.IsListening -> ()
    }
    
let startBot config updateArrived updatesArrived =
  async {
    let! me = Api.getMe |> api config
    return! me 
    |> function
    | Error error -> error.AsException() |> raise
    | Ok me -> runBot config me updateArrived updatesArrived
  }

let processCommands (context: UpdateContext) =
  Seq.forall (fun command -> command context)