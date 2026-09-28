module Funogram.Tests.Json

open Xunit
open Extensions
open Helpers

[<Fact>]
let ``JSON deserializing MessageEntity`` () =
  parseJson(Constants.jsonTestObjResultString)
  |> shouldEqual (Result.Ok Constants.jsonTestObj)

[<Fact>]
let ``JSON deserializing MessageEntity as Object`` () =
  parseJson(Constants.jsonTestObjResultString)
  |> Result.map (fun x -> x <> null)
  |> shouldEqual (Result.Ok true)

[<Fact>]
let ``JSON serializing MessageEntity``() =
  Constants.jsonTestObj
  |> toJsonString
  |> shouldEqual Constants.jsonTestObjString

[<Fact>]
let ``JSON deserializing User``() =
  Constants.jsonTestObjUserResultString
  |> parseJson
  |> shouldEqual (Result.Ok Constants.jsonTestObjUser)

[<Fact>]
let ``JSON deserializing EditMessageResult 1``() =
  Constants.jsonTestEditResult1ApiString
  |> parseJson
  |> shouldEqual (Result.Ok Constants.jsonTestEditResult1)

[<Fact>]
let ``JSON deserializing EditMessageResult 2`` () =
  Constants.jsonTestEditResult2ApiString
  |> parseJson
  |> shouldEqual (Result.Ok Constants.jsonTestEditResult2)

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
  |> shouldEqual (Result.Ok Constants.testMaskPosition)

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
  |> shouldEqual (Result.Ok Constants.jsonMessageForward)

[<Fact>]
let ``JSON serializing params dictionary`` () =
  Constants.paramsDictionary
  |> toJsonString
  |> shouldEqual Constants.jsonParamsDictionary

[<Fact>]
let ``JSON serializing forward message request`` () =
  Constants.forwardMessageReq
  |> toJsonString
  |> shouldEqual Constants.jsonForwardMessageReq

[<Fact>]
let ``JSON deserializing ChatMember``() =
  Constants.jsonTestObjChatMemberResultString
  |> parseJson
  |> shouldEqual (Result.Ok Constants.jsonTestObjChatMember)

[<Fact>]
let ``JSON serializing send message request`` () =
  Constants.sendMessageReq
  |> toJsonString
  |> shouldEqual Constants.jsonSendMessageReq
  
[<Fact>]
let ``JSON deserializing message request with RichText`` () =
  Constants.jsonMessageWithRichTextResultString
  |> parseJson
  |> Result.map (fun _ -> ())
  |> shouldEqual (Result.Ok ())

[<Fact>]
let ``JSON deserializing RichText`` () =
  let json = """{"ok":true,"result":{"type":"bot_command","text":"example","bot_command":"/test"}}"""
  json
  |> parseJson
  |> Result.map (fun _ -> ())
  |> shouldEqual (Result.Ok ())