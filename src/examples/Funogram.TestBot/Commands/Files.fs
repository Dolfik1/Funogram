module Funogram.TestBot.Commands.Files

open FunHttp
open Funogram.Telegram.RequestsTypes
open Funogram.Telegram.Types
open Funogram.TestBot.Core

[<Literal>]
let private PhotoUrl = "https://www.w3.org/People/mimasa/test/imgformat/img/w3c_home.jpg"

let testUploadAndSendPhotoGroup config chatId =
  let pack name stream =
    {
      InputMediaPhoto.Media = InputFile.File(name, stream) 
      Type = "photo"
      Caption = Some name
      ParseMode = None
      CaptionEntities = None
    } |> InputMedia.Photo
  
  let image1 = Http.RequestStream(PhotoUrl).ResponseStream
  let image2 = Http.RequestStream(PhotoUrl).ResponseStream
  let media = [| pack "Image 1" image1; pack "Image 2" image2 |]
  SendMediaGroupReq.Make(ChatId.Int chatId, media) |> bot config

let testUploadAndSendSinglePhoto config chatId =
  let image = Http.RequestStream(PhotoUrl)
  SendPhotoReq.Make(ChatId.Int chatId, InputFile.File("example.jpg", image.ResponseStream), caption = "Example") |> bot config
