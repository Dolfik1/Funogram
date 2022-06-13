module Funogram.Tests.Json

open Funogram.Types
open Xunit
open Extensions
open Helpers

[<Fact>]
let ``JSON deserializing MessageEntity`` () =
  let a = parseJson(Constants.jsonTestObjResultString)
    
  match a with
  | Ok r -> shouldEqual r Constants.jsonTestObj
  | Error e -> failwith e.Description

[<Fact>]
let ``JSON serializing MessageEntity``() =
  Constants.jsonTestObj
  |> toJsonString
  |> shouldEqual Constants.jsonTestObjString

[<Fact>]
let ``JSON deserializing User``() =
  Constants.jsonTestObjUserResultString
  |> parseJson
  |> function
  | Ok result -> shouldEqual result Constants.jsonTestObjUser
  | Error error -> failwith error.Description

[<Fact>]
let ``JSON deserializing EditMessageResult 1``() =
  Constants.jsonTestEditResult1ApiString
  |> parseJson
  |> function
  | Ok result -> shouldEqual result Constants.jsonTestEditResult1
  | Error error -> failwith error.Description

[<Fact>]
let ``JSON deserializing EditMessageResult 2`` () =
  Constants.jsonTestEditResult2ApiString
  |> parseJson
  |> function
  | Ok result -> shouldEqual result Constants.jsonTestEditResult2
  | Error error -> failwith error.Description

[<Fact>]
let ``JSON deserializing EditMessageResult 3`` () =
  Constants.jsonTestEditResult3ApiString
  |> parseJson
  |> ignore

[<Fact>]
let ``JSON serializing EditMessageResult 1`` () =
  Constants.jsonTestEditResult1
  |> toJsonString
  |> shouldEqual Constants.jsonTestEditResult1String

[<Fact>]
let ``JSON serializing EditMessageResult 2`` () =
  Constants.jsonTestEditResult2
  |> toJsonString
  |> shouldEqual Constants.jsonTestEditResult2String

[<Fact>]
let ``JSON deserializing MaskPosition`` () =
  Constants.jsonTestMaskPositionResult
  |> parseJson
  |> function
  | Ok result -> shouldEqual result Constants.testMaskPosition
  | Error error -> failwith error.Description
    

[<Fact>]
let ``JSON serializing MaskPosition`` () =
  Constants.testMaskPosition
  |> toJsonString
  |> shouldEqual Constants.jsonTestMaskPosition

[<Fact>]
let ``JSON serializing ForwardMessage`` () =
  Constants.jsonMessageForwardDate
  |> toJsonString
  |> shouldEqual Constants.jsonMessageForwardDateString

[<Fact>]
let ``JSON deserializing ForwardMessage`` () =
  Constants.jsonMessageForwardDateApiString
  |> parseJson
  |> function
  | Ok result -> shouldEqual result Constants.jsonMessageForward
  | Error error -> failwith error.Description
    
[<Fact>]
let ``JSON serializing params dictionary`` () =
  Constants.paramsDictionary
  |> toJsonString
  |> shouldEqual Constants.jsonParamsDictionary

[<Fact>]
let ``JSON serializing forward message request`` () =
  Constants.forwardMessageReq
  |> toJsonBotRequestString
  |> shouldEqual Constants.jsonForwardMessageReq
  
[<Fact>]
let ``JSON deserializing ChatMember``() =
  Constants.jsonTestObjChatMemberResultString
  |> parseJson
  |> function
  | Ok result -> shouldEqual result Constants.jsonTestObjChatMember
  | Error error -> failwith error.Description