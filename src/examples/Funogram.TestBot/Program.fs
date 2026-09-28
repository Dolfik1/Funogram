module Funogram.TestBot.Program

open System
open System.Threading
open Funogram.TestBot
open Funogram.Api
open Funogram.Telegram
open Funogram.Telegram.Bot
open Serilog
open Serilog.Extensions.Logging
open Serilog.Sinks.SystemConsole.Themes

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
    let serilog =
      LoggerConfiguration()
        .MinimumLevel.Verbose()
        .WriteTo.Console(theme = AnsiConsoleTheme.Code)
        .CreateLogger()
    use loggerFactory = new SerilogLoggerFactory(serilog, dispose = true)

    Async.RunSynchronously(
      async {
        let config = Config.defaultConfig |> Config.withReadTokenFromFile
        let config =
          { config with
              Logger = loggerFactory.CreateLogger("Funogram") }
        let! _ = Api.deleteWebhookBase () |> api config
        return! startBot config Commands.Base.updateArrived None
      }, cancellationToken = cts.Token
    )
  with
  | :? OperationCanceledException ->
    printfn "Graceful shutdown completed!"
  0
