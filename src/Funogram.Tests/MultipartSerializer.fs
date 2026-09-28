module Funogram.Tests.MultipartSerializer

open System.Net.Http
open Funogram.Telegram
open Funogram.Telegram.Types
open Xunit
open Funogram.Tests.Extensions

// Regression tests for recursive Telegram types (e.g. RichText / InputRichBlock,
// introduced with Bot API 10.2). Building the multipart file-finder used to recurse
// through the whole type graph eagerly, which overflowed the stack on self-referential
// types and aborted the process (SIGABRT / exit 134).

[<Fact>]
let ``Multipart serializer builds for recursive rich message request`` () =
  // Mirrors the /send_message12 command path that crashed.
  let request =
    Req.SendRichMessage.Make(
      chatId = 1L,
      richMessage = InputRichMessage.Create(markdown = "__hello__"))

  let serialize = Funogram.Tools.Api.generateMultipartSerializer (request.GetType())
  use content = new MultipartFormDataContent()
  let hasData = serialize request content
  shouldEqual true hasData

[<Fact>]
let ``File finder traverses recursive RichText value without overflow`` () =
  // A self-referential value: ArrayOf contains nested ArrayOf.
  let value =
    RichText.ArrayOf [|
      RichText.Plain "hello"
      RichText.ArrayOf [| RichText.Plain "world" |]
    |]

  let finder = Funogram.Tools.Api.mkFilesFinder<RichText> ()
  let files = finder value
  shouldEqual 0 files.Length