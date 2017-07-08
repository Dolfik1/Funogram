module Funogram.Bot

open Funogram.Sscanf
open Funogram.Types
open Funogram.Rest


type BotConfig = 
  {
    Token: string
    
    Offset: int64 option
    Limit: int option
    Timeout: int option
    AllowedUpdates: string seq option
  }

let defaultConfig = { Token = ""; Offset = Some 0L; Limit = Some 100; Timeout = Some 60000; AllowedUpdates = None }

type UpdateContext =
  {
    Update: Update
    Config: BotConfig
    Me: User
  }

let cmd (command: string) (h: _ -> unit) =
    let f (r: Message) =
      match r.Text with
      | Some text ->
        if text = command then
          h()
          true
        else
          false
      | None -> false
    f

let cmdScan (pf : PrintfFormat<_,_,_,_,'t>) (h : 't -> unit) =

    let scan command =
      try
        let r = sscanf pf command
        Some r
      with _ ->
        None

    let f (r: Message) =
      match r.Text with
      | Some text -> 
        match scan text with
        | Some p -> 
          h p |> ignore
          true
        | None -> 
          false
      | None -> false
    f

let private runBot config me updateArrived = 

  let rec loopAsync offset = async {
    try
      let! updatesResult = Telegram.GetUpdatesBaseAsync(config.Token, Some offset, config.Limit, config.Timeout)
      match updatesResult with
      | Ok updates -> 
        if updates |> Seq.isEmpty then
          return! loopAsync offset

        let offset = updates |> Seq.map (fun f -> f.UpdateId) |> Seq.max |> fun x -> x + 1L

        do updates |> Seq.iter (fun f -> updateArrived { Update = f; Config = config; Me = me })

        return! loopAsync offset // sends new offset
      | Error e -> 
        printf "Updates processing error: %s, code: %i" e.Description e.ErrorCode
        return! loopAsync offset
    with
    | ex -> printfn "Error: %s" ex.Message
            return! loopAsync (offset + 1L)

    //do! Async.Sleep 1000
    return! loopAsync offset
  }
  loopAsync (config.Offset |> Option.defaultValue 0L) |> Async.RunSynchronously

let startBot config updateArrived =
  let meResult = Telegram.GetMeAsync config.Token |> Async.RunSynchronously
  match meResult with 
  | Error e -> failwith(e.Description) 
  | Ok me -> runBot config me updateArrived
  ()

let processCommands (ctx: UpdateContext) (commands: (Message -> bool) seq) =
  let message = ctx.Update.Message
  match message with
  | Some msg -> commands 
                |> Seq.map (fun f -> f msg) 
                |> Seq.exists id
  | None -> false