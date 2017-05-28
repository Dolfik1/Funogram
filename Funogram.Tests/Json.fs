module Funogram.Tests.Json

open Funogram
open Funogram.Types
open Xunit
open Extensions


[<Fact>]
let ``JSON deserializing MessageEntity`` () =
    let a = Helpers.parseJson<MessageEntity>(Constants.jsonTestObjResultString)
    
    match Helpers.parseJson<MessageEntity>(Constants.jsonTestObjResultString) with
        | Ok r -> shouldEqual r Constants.jsonTestObj
        | Error e -> failwith(e.Description)

[<Fact>]
let ``JSON serializing MessageEntity`` () =
    Helpers.serializeObject(Constants.jsonTestObj) |> shouldEqual Constants.jsonTestObjString

[<Fact>]
let ``JSON deserializing User`` () =
        match Helpers.parseJson<User>(Constants.jsonTestObjUserResultString) with
        | Ok r -> shouldEqual r Constants.jsonTestObjUser
        | Error e -> failwith(e.Description)

[<Fact>]
let ``JSON deserializing EditMessageResult 1`` () =
        match Helpers.parseJson<EditMessageResult>(Constants.jsonTestEditResult1String) with
        | Ok r -> shouldEqual r Constants.jsonTestEditResult1
        | Error e -> failwith(e.Description)