module Funogram.TestBot.Commands.Files

open System.IO
open FunHttp
open Funogram.Telegram
open Funogram.Telegram.Types
open Funogram.TestBot.Core

[<Literal>]
let private PhotoUrl = "https://www.w3.org/People/mimasa/test/imgformat/img/w3c_home.jpg"

let testUploadAndSendPhotoGroup config (chatId: int64) =
  let pack name stream =
    {
      InputMediaPhoto.Media = InputFile.File(name, stream) 
      Type = "photo"
      Caption = Some name
      ParseMode = None
      CaptionEntities = None
      HasSpoiler = None
    } |> InputMedia.Photo
  
  let image1 = Http.RequestStream(PhotoUrl).ResponseStream
  let image2 = Http.RequestStream(PhotoUrl).ResponseStream
  let media = [| pack "Image 1" image1; pack "Image 2" image2 |]
  Req.SendMediaGroup.Make(chatId, media) |> bot config

let testUploadAndSendSinglePhoto config (chatId: int64) =
  let image = Http.RequestStream(PhotoUrl)
  Req.SendPhoto.Make(chatId, InputFile.File("example.jpg", image.ResponseStream), caption = "Example") |> bot config

let testUploadAndSendPhotoGroupAsBytes config (chatId: int64) =
  let pack name bytes =
    {
      InputMediaPhoto.Media = InputFile.FileBytes(name, bytes) 
      Type = "photo"
      Caption = Some name
      ParseMode = None
      CaptionEntities = None
      HasSpoiler = None
    } |> InputMedia.Photo
  
  use image1 = Http.RequestStream(PhotoUrl).ResponseStream
  use image2 = Http.RequestStream(PhotoUrl).ResponseStream
  
  use ms1 = new MemoryStream()
  image1.CopyTo(ms1)
  ms1.Seek(0, SeekOrigin.Begin) |> ignore
  
  use ms2 = new MemoryStream()
  image2.CopyTo(ms2)
  ms2.Seek(0, SeekOrigin.Begin) |> ignore
  
  let media = [| pack "Image.jpg" (ms1.ToArray()); pack "Image.jpg" (ms2.ToArray()) |]
  Req.SendMediaGroup.Make(chatId, media) |> bot config
