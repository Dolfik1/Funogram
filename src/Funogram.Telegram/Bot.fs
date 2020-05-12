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
      ApiEndpointUrl = new Uri("https://api.telegram.org/bot") }

type UpdateContext =
    { Update: Update
      Config: BotConfig
      Me: User }

let private getTextForCommand (me: User) =
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
    let rec loopAsync offset =
        async {
            try
                let! updatesResult = getUpdatesBase (Some offset) (config.Limit)
                                         config.Timeout [] |> bot
                match updatesResult with
                | Ok updates ->
                    if updates |> Seq.isEmpty then return! loopAsync offset
                    let offset =
                        updates
                        |> Seq.map (fun f -> f.UpdateId)
                        |> Seq.max
                        |> fun x -> x + 1L
                    do updates 
                    |> Seq.iter (
                        fun f ->
                        updateArrived { Update = f
                                        Config = config
                                        Me = me })
                    match updatesArrived with
                    | Some updatesArrived -> do updates |> updatesArrived
                    | _ -> ()
                    return! loopAsync offset // sends new offset
                | Error e ->
                    printf "Updates processing error: %s, code: %i"
                        e.Description e.ErrorCode
                    return! loopAsync offset
            with ex ->
                printfn "Internal error: %s" ex.Message
                return! loopAsync (offset + 1L)
            return! loopAsync offset
        }
    loopAsync (config.Offset |> Option.defaultValue 0L)

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
