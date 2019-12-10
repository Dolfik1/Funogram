module Funogram.Tests.Files

open System.Net.Http
open Xunit
open Extensions
open Funogram.Telegram.Api
open Funogram.Telegram.RequestsTypes
open Funogram.Telegram.Types
open Funogram.Tools

[<Fact>]
let ``Can find files in request`` () =
  async {
    use client = new HttpClient()
    let! image = client.GetStreamAsync("https://upload.wikimedia.org/wikipedia/commons/f/f5/Example_image.jpg")
                 |> Async.AwaitTask
    let f = ("example.jpg", image)
    let file = FileToSend.File(f)
    let request = sendPhoto 0L file "Example"
    let find = Api.generateFileFinder typeof<SendPhotoReq>
    let files = find request
    files |> shouldEqual [|f|]
  }