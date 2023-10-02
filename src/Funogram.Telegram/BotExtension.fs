module Funogram.Telegram.BotExtension

open Funogram.Telegram.Bot
open System
open System.IO
open System.Net.Http
open Funogram.Telegram
open Funogram.Telegram.Sscanf
open Funogram.Telegram.Types
open Funogram.Types
open Funogram.Api

let processUpdateAsync config inputStream updateArrivedAsync updatesArrivedAsync=
    async {

        let! meAsync =
            Api.getMe |> api config
                // me
        let test = meAsync
        |> function
            | Ok e -> e
            | Error error -> failwith error.Description

        // let me = asyncMe |> Async.RunSynchronously

      //let bot data = api config data
        let processUpdates updates =
          if updates |> Seq.isEmpty |> not then
            updates |> Seq.iter (fun f -> updateArrived { Update = f; Config = config; Me = me })
            updatesArrived |> Option.iter (fun x -> x updates)
        let input =Funogram.Tools.parseJsonStream<Update> inputStream
        match input with
                | Ok updates -> processUpdates [| updates |]
                | Error e -> config.OnError e
        match input with
                | Ok updates -> Some updates
                | Error e -> None


  }

let processUpdate config inputStream updateArrived updatesArrived =
