module Funogram.TestBot.Program

open System
open System.Threading
open Funogram.TestBot
open Funogram.Api
open Funogram.Telegram
open Funogram.Telegram.Bot
  
type ConsoleLogger(color: ConsoleColor) =
  interface Funogram.Types.IBotLogger with
    member x.Log(text) =
      let fc = Console.ForegroundColor
      Console.ForegroundColor <- color
      Console.WriteLine(text)
      Console.ForegroundColor <- fc
    member x.Enabled = true

  
[<EntryPoint>]
let main _ =
  let cts = new CancellationTokenSource()
  Console.CancelKeyPress.Add(fun x ->
    x.Cancel <- true
    cts.Cancel()
  )
  
  AppDomain.CurrentDomain.ProcessExit.Add(fun _ ->
    cts.Cancel()
  )
  
  try
    Async.RunSynchronously(
      async {
        let config = Config.defaultConfig |> Config.withReadTokenFromFile
        let config =
          { config with
              RequestLogger = Some (ConsoleLogger(ConsoleColor.Green)) }
        let! _ = Api.deleteWebhookBase () |> api config
        return! startBot config Commands.Base.updateArrived None
      }, cancellationToken = cts.Token
    )
  with
  | :? OperationCanceledException ->
    printfn "Graceful shutdown completed!"
  0
