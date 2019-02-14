module Funogram.Tests.Json

open Funogram
open Funogram.Types
open Xunit
open Extensions

[<Fact>]
let ``JSON deserializing MessageEntity`` () =
    let a = Tools.parseJson<MessageEntity>(Constants.jsonTestObjResultString)
    
    match a with
        | Ok r -> shouldEqual r Constants.jsonTestObj
        | Error e -> failwith e.Description

[<Fact>]
let ``JSON serializing MessageEntity``() =
    Constants.jsonTestObj
    |> Tools.toJsonString
    |> shouldEqual Constants.jsonTestObjString

[<Fact>]
let ``JSON deserializing User``() =
    Constants.jsonTestObjUserResultString
    |> Tools.parseJson<User>
    |> function
    | Ok result -> shouldEqual result Constants.jsonTestObjUser
    | Error error -> failwith error.Description

[<Fact>]
let ``JSON deserializing EditMessageResult 1``() =
    Constants.jsonTestEditResult1ApiString
    |> Tools.parseJson<EditMessageResult>
    |> function
    | Ok result -> shouldEqual result Constants.jsonTestEditResult1
    | Error error -> failwith error.Description

[<Fact>]
let ``JSON deserializing EditMessageResult 2`` () =
    Constants.jsonTestEditResult2ApiString
    |> Tools.parseJson<EditMessageResult>
    |> function
    | Ok result -> shouldEqual result Constants.jsonTestEditResult2
    | Error error -> failwith error.Description

[<Fact>]
let ``JSON deserializing EditMessageResult 3`` () =
    Constants.jsonTestEditResult3ApiString
    |> Tools.parseJson<EditMessageResult>
    |> ignore

[<Fact>]
let ``JSON serializing EditMessageResult 1`` () =
    Constants.jsonTestEditResult1
    |> Tools.toJsonString
    |> shouldEqual Constants.jsonTestEditResult1String

[<Fact>]
let ``JSON serializing EditMessageResult 2`` () =
    Constants.jsonTestEditResult2
    |> Tools.toJsonString
    |> shouldEqual Constants.jsonTestEditResult2String

[<Fact>]
let ``JSON deserializing MaskPosition`` () =
    Constants.jsonTestMaskPositionResult
    |> Tools.parseJson<MaskPosition>
    |> function
    | Ok result -> shouldEqual result Constants.testMaskPosition
    | Error error -> failwith error.Description
