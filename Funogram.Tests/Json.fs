module Funogram.Tests.Json

open Funogram
open Funogram.Types
open Xunit
open Extensions


[<Fact>]
let ``JSON deserializing`` () =
    Helpers.parseJson<MessageEntity>(Constants.jsonTestObjString) |> shouldEqual Constants.jsonTestObj

[<Fact>]
let ``JSON serializing`` () =
    Helpers.serializeObject(Constants.jsonTestObj) |> shouldEqual Constants.jsonTestObjString