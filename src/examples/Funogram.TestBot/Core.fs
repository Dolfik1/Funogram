module Funogram.TestBot.Core

open Funogram.Types
open Funogram.Api

let processResultWithValue (result: Result<'a, ApiError>) =
  match result with
  | Ok v -> Some v
  | Error error ->
    printfn "Server error: %A" error
    None

let processResult (result: Result<'a, ApiError>) =
  processResultWithValue result |> ignore

let botResult config data = api config data |> Async.RunSynchronously
let bot config data = botResult config data |> processResult
