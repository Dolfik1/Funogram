namespace Funogram.TestBot

open System
open System.IO
open Funogram
open Funogram.TestBot.Types

module Main =
    let private runBot token offset me = 
            let bot = { Token = token; Me = me }
            let updateArrived = Router.updateArrived bot

            let rec loopAsync offset = async {
                try
                    let! updatesResult = Telegram.GetUpdatesAsync(token, offset, 100, 60000)

                    match updatesResult with
                    | Ok updates -> 
                        if updates |> Seq.isEmpty then
                                        return! loopAsync offset

                        let offset = updates |> Seq.map (fun f -> f.UpdateId) |> Seq.max |> fun x -> x + 1L

                        do updates 
                            |> Seq.iter updateArrived

                        return! loopAsync offset // sends new offset
                    | Error e -> 
                                printf "Updates processing error: %s, code: %i" e.Description e.ErrorCode
                                return! loopAsync offset
                with
                    | ex -> printfn "Error: %s" ex.Message
                
                //do! Async.Sleep 1000
                return! loopAsync offset
            }

            loopAsync offset |> Async.RunSynchronously // ?

    let run token = async {
        let! meResult = Telegram.GetMeAsync token
        match meResult with 
        | Error e -> failwith(e.Description) 
        | Ok me -> runBot token 0L me
    }


    [<Literal>]
    let TokenFileName = "token"

    [<EntryPoint>]
    let main argv =
        printfn "Bot started..."

        if File.Exists(TokenFileName) then
            run (File.ReadAllText(TokenFileName)) |> Async.RunSynchronously
        else
            printf "Please, enter bot token: "
            let token = System.Console.ReadLine()
            File.WriteAllText(TokenFileName, token)
            run token |> Async.RunSynchronously
        
        0 // return an integer exit code