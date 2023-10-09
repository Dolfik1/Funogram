module Funogram.Telegram.BotExtension

open Funogram.Telegram.Bot
open Funogram.Telegram
open Funogram.Telegram.Types
open Funogram.Types
open Funogram.Api

let processUpdateAsync config inputStream (updateArrivedAsync:UpdateContext -> Async<unit>) =
    async {

        let! meAsync = Api.getMe |> apiAsync config
                // me
        let me =
            meAsync
            |> function
            | Ok e -> e
            | Error error -> failwith error.Description

        let processUpdatesAsync (updates: seq<Update>): Async<unit array>=
          async{
                match updates |> Seq.isEmpty with
                | false ->  return! (updates |> Seq.map (fun f -> updateArrivedAsync { Update = f; Config = config; Me = me }   ) |> Async.Sequential)
                // | true -> failwithf "no updates"
                // if 0 then


            }
            // updatesArrivedAsync |> Option.iter (fun x ->  x updates )


        let input =Funogram.Tools.parseJsonStream<Update> inputStream
        match input with
                | Ok updates ->
                    let! executionParallel = processUpdatesAsync [| updates |]
                    executionParallel |> Array.map (fun f -> do f)
                | Error e -> config.OnError e
        // match input with
        //         | Ok updates -> Some updates
        //         | Error e -> None

        return input
  }

// let processUpdate config inputStream updateArrived updatesArrived =
