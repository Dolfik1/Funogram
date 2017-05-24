module Funogram.Tests.Json

open Funogram
open Funogram.Types
open Xunit
open Extensions


[<Fact>]
let ``JSON deserializing`` () =
    match Helpers.parseJson<MessageEntity>(Constants.jsonTestObjString) with
        | Ok r -> shouldEqual r Constants.jsonTestObj
        | Error e -> failwith(e)

[<Fact>]
let ``JSON serializing`` () =
    Helpers.serializeObject(Constants.jsonTestObj) |> shouldEqual Constants.jsonTestObjResultString