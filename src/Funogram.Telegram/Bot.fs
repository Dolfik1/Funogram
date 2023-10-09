module Funogram.Telegram.Bot

open System
open System.IO
open System.Net.Http
open Funogram.Telegram
open Funogram.Telegram.Sscanf
open Funogram.Telegram.Types
open Funogram.Types
open Funogram.Api

[<Literal>]
let TokenFileName = "token"

[<RequireQualifiedAccess>]
module Config =
  let defaultConfig =
    { IsTest = false
      Token = ""
      Offset = Some 0L
      Limit = Some 100
      Timeout = Some 60000
      AllowedUpdates = None
      Client = new HttpClient()
      ApiEndpointUrl = Uri("https://api.telegram.org/bot")
      WebHook = None
      OnError = (fun e -> printfn "%A" e) }

  let withReadTokenFromFile config =
    if File.Exists(TokenFileName) then
      { config with Token = File.ReadAllText(TokenFileName) }
    else
      printf "Please, enter bot token: "
      let token = System.Console.ReadLine()
      File.WriteAllText(TokenFileName, token)
      { config with Token = token }

  let withReadTokenFromEnv envName config =
    { config with Token = Environment.GetEnvironmentVariable(envName) }

type UpdateContext =
  { Update: Update
    Config: BotConfig
    Me: User }

// Text of the command; 1-32 characters. Can contain only lowercase English letters, digits and underscores.
let inline private isAllowedChar (c: char) = Char.IsLetter c || Char.IsDigit c || c = '_'

// returns -1 if the command is not valid otherwise index of last character
let private validateCommand (text: string) =
  let rec iter (text: string) i len =
    if i >= len || isAllowedChar text.[i] |> not then
      (i - 1)
    else
      iter text (i + 1) len

  if text.Length <= 1 || text.[0] <> '/' then -1
  else iter text 1 text.Length

let getTextForCommand (me: User) (textOriginal: string option) =
  match me.Username, textOriginal with
  | Some username, Some text when text.Length > 0 && text.[0] = '/' ->
    match validateCommand text with
    | -1 -> textOriginal
    | idx when text.Length = idx + 1 -> Some text
    | idx when text.[idx + 1] = '@' && text.IndexOf(username, idx + 1) = idx + 2 ->
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
  let bot data = apiAsync config data

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
            Req.GetUpdates.Make(offset, ?limit = config.Limit, ?timeout = config.Timeout)
            |> bot

          match updatesResult with
          | Ok updates when updates |> Seq.isEmpty |> not ->
            let offset = updates |> Seq.map (fun f -> f.UpdateId) |> Seq.max |> fun x -> x + 1L
            processUpdates updates
            return! loopAsync offset // send new offset
          | Error e ->
            config.OnError (e.AsException() :> Exception)

            // add delay in case of HTTP error
            // for example: the server may be "busy"
            if e.Description = "HTTP_ERROR" then
              do! Async.Sleep 1000

            return! loopAsync offset
          | _ ->
            return! loopAsync offset
        with
        | :? HttpRequestException as e ->
          // in case of HTTP error we should not increment offset
          config.OnError e
          do! Async.Sleep 1000
          return! loopAsync offset

        | ex ->
          config.OnError ex
          // in case of "general" error we should increment offset to skip problematic update
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
          | Ok updates -> processUpdates [| updates |]
          | Error e -> config.OnError e
        else
          context.Response.StatusCode <- 403
        context.Response.Close()

    }

let startBot config updateArrived updatesArrived =
  async {
    let! me = Api.getMe |> apiAsync config
    return! me
    |> function
    | Error error -> failwith error.Description
    | Ok me -> runBot config me updateArrived updatesArrived
  }

let processCommands (context: UpdateContext) =
  Seq.forall (fun command -> command context)