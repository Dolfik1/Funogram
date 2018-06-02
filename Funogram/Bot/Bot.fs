module Funogram.Bot

open Funogram.Sscanf
open Funogram.Types
open Funogram.Api

type BotConfig = 
    { Token : string
      Offset : int64 option
      Limit : int option
      Timeout : int option
      AllowedUpdates : string seq option }

let defaultConfig = 
    { Token = ""
      Offset = Some 0L
      Limit = Some 100
      Timeout = Some 60000
      AllowedUpdates = None }

type UpdateContext = 
    { Update : Update
      Config : BotConfig
      Me : User }

let private getTextForCommand (me : User) (text : string option) = 
    let username() = "@" + me.Username.Value
    if text.IsSome && text.Value.EndsWith(username()) then 
        Some(text.Value.Replace(username(), ""))
    else text

let cmd (command: string) (handler: UpdateContext -> unit) (context: UpdateContext) =
    context.Update.Message
    |> Option.bind (fun message -> getTextForCommand context.Me message.Text)
    |> Option.map (fun text -> if text = command then handler context; true else false)
    |> function 
    | Some value -> value
    | None -> false

let cmdScan (format: PrintfFormat<_, _, _, _, 't>) (handler: 't -> unit) (context: UpdateContext) = 
    let scan command = 
        try Some (sscanf format command)
        with _ -> None

    context.Update.Message
    |> Option.bind (fun message -> getTextForCommand context.Me message.Text)
    |> Option.bind scan
    |> Option.map handler
    |> function 
    | Some value -> true
    | None -> false 

let private runBot config me updateArrived updatesArrived = 
    let bot data = api config.Token data
    
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
                    do updates |> Seq.iter (fun f -> 
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
                printfn "Error: %s" ex.Message
                return! loopAsync (offset + 1L)
            //do! Async.Sleep 1000
            return! loopAsync offset
        }
    loopAsync (config.Offset |> Option.defaultValue 0L) 
    |> Async.RunSynchronously

let startBot config updateArrived updatesArrived = 
    getMe
    |> api config.Token
    |> Async.RunSynchronously
    |> function
    | Error error -> failwith error.Description
    | Ok me -> runBot config me updateArrived updatesArrived
    |> ignore

let processCommands (context: UpdateContext) =
    Seq.forall (fun command -> command context)
    