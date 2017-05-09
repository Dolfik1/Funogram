module Funogram.Tests.Json

open Funogram
open Funogram.Types
open Xunit
open Extensions


let jsonObj = { Type = "italic"; Offset = 0L; Length = 100L; Url = Some("http://github.com"); User = None }
let jsonObjString = "{\"type\":\"italic\",\"offset\":0,\"length\":100,\"url\":\"http://github.com\",\"user\":null}"

[<Fact>]
let ``JSON deserializing`` () =
    Helpers.parseJson<MessageEntity>(jsonObjString) |> shouldEqual jsonObj

[<Fact>]
let ``JSON serializing`` () =
    Helpers.serializeObject(jsonObj) |> shouldEqual jsonObjString