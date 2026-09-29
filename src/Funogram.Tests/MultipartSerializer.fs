module Funogram.Tests.MultipartSerializer

open System.Net.Http
open Funogram.Telegram
open Funogram.Telegram.Bot
open Funogram.Telegram.Types
open Xunit
open Funogram.Tests.Extensions


[<Fact>]
let ``Multipart serializer builds for recursive rich message request`` () =
  let request =
    Req.SendRichMessage.Make(
      chatId = 1L,
      richMessage = InputRichMessage.Create(markdown = "__hello__"))

  let serialize = Funogram.Tools.Api.generateMultipartSerializer (request.GetType())
  use content = new MultipartFormDataContent()
  let hasData = serialize Config.defaultConfig request content
  shouldEqual true hasData

type SelfRecursive =
  | Leaf of string
  | Wrap of SelfRecursive

[<Fact>]
let ``Multipart generator builds for self-recursive union and serializes nested value`` () =
  let generate = Funogram.Tools.Api.mkRequestGenerator<SelfRecursive> ()
  use content = new MultipartFormDataContent()
  let hasData = generate (Wrap (Wrap (Leaf "value"))) Config.defaultConfig "prop" content
  shouldEqual true hasData

  let parts = content |> Seq.toArray
  shouldEqual 1 parts.Length
  shouldEqual "value" (parts[0].ReadAsStringAsync().Result)

[<Fact>]
let ``File finder traverses recursive RichText value without overflow`` () =
  let value =
    RichText.ArrayOf [|
      RichText.Plain "hello"
      RichText.ArrayOf [| RichText.Plain "world" |]
    |]

  let finder = Funogram.Tools.Api.mkFilesFinder<RichText> ()
  let files = finder value
  shouldEqual 0 files.Length
let private multipartValue<'T> (value: 'T) =
  let generate = Funogram.Tools.Api.mkRequestGenerator<'T> ()
  use content = new MultipartFormDataContent()
  generate value Config.defaultConfig "date" content |> ignore
  (content |> Seq.exactlyOne).ReadAsStringAsync().Result

[<Fact>]
let ``DateTimeOffset serializes to the same Unix time in JSON and multipart regardless of offset`` () =
  let date = System.DateTimeOffset(2117, 05, 28, 15, 47, 51, System.TimeSpan.FromHours 3.)
  let expected = string Constants.testDateUnix

  shouldEqual expected (Helpers.toJsonString date)
  shouldEqual expected (multipartValue date)


[<Fact>]
let ``Multipart serializer writes UnrecognizedCase as raw JSON`` () =
  let json = """{"type":"future_scope","chat_id":1}"""
  let request = Req.SetMyCommands.Make([||], scope = BotCommandScope.UnrecognizedCase { Json = json })
  let serialize = Funogram.Tools.Api.generateMultipartSerializer (request.GetType())
  use content = new MultipartFormDataContent()
  serialize Config.defaultConfig request content |> ignore
  let scope = content |> Seq.find (fun x -> x.Headers.ContentDisposition.Name.Trim('"') = "scope")
  shouldEqual json (scope.ReadAsStringAsync().Result)
