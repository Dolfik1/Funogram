module Funogram.TestBot

open System.IO
open Funogram.Api
open Funogram.Telegram.RequestsTypes
open Funogram.Types
open Funogram.Telegram.Api
open Funogram.Telegram.Types
open Funogram.Telegram.Bot
open FunHttp
open System.Net

let [<Literal>] TokenFileName = "token"
let mutable botToken = "none"

let [<Literal>] PhotoUrl = "https://www.w3.org/People/mimasa/test/imgformat/img/w3c_home.jpg"

let processMessageBuild config =

  let defaultText = """⭐️Available test commands:
  /send_message1 - Markdown test
  /send_message2 - HTML test
  /send_message3 - Disable web page preview and notifications
  /send_message4 - Test reply message
  /send_message5 - Test ReplyKeyboardMarkup
  /send_message6 - Test RemoveKeyboardMarkup
  /send_message7 - Test inline keyboard
  /send_message8 - Test multiple media
    
  /send_action - Test action

  /forward_message - Test forward message
  /show_my_photos_sizes - Test getUserProfilePhotos method
  /get_chat_info - Returns id and type of current chat
  /send_photo - Send example photo
  /cmdscan stringA stringB - Test cmdScan, concatenate stringA and stringB"""


  let processResultWithValue (result: Result<'a, ApiResponseError>) =
    match result with
    | Ok v -> Some v
    | Error e ->
      printfn "Server error: %s" e.Description
      None

  let processResult (result: Result<'a, ApiResponseError>) =
    processResultWithValue result |> ignore

  let botResult data = api config data |> Async.RunSynchronously
  let bot data = botResult data |> processResult


  let getChatInfo (msg: Message) =
    let result = botResult (getChat msg.Chat.Id)
    match result with
    | Ok x ->
      botResult (sendMessage msg.Chat.Id (sprintf "Id: %i, Type: %O" x.Id x.Type))
      |> processResultWithValue
      |> ignore
    | Error e -> printf "Error: %s" e.Description

  let sendPhoto (msg: Message) =
    let image = Http.RequestStream(PhotoUrl)
    bot (sendPhoto msg.Chat.Id (InputFile.File("example.jpg", image.ResponseStream)) "Example")

  let updateArrived ctx =
    let fromId () = ctx.Update.Message.Value.From.Value.Id
            
    let sayWithArgs text parseMode disableWebPagePreview disableNotification replyToMessageId replyMarkup =
        SendMessageReq.Make(ChatId.Int (fromId ()), text, ?parseMode = parseMode, ?disableWebPagePreview = disableWebPagePreview, ?disableNotification = disableNotification, ?replyToMessageId = replyToMessageId, ?replyMarkup = replyMarkup)
        |> bot

    let sendMessageFormatted text parseMode = SendMessageReq.Make(ChatId.Int (fromId ()), text, parseMode = parseMode) |> bot

    let result =
      processCommands ctx [|
        cmdScan "/cmdscan %s %s" (fun (a, b) _ ->  sendMessageFormatted (sprintf "%s %s" a b) ParseMode.Markdown)
        
        cmd "/send_action" (fun _ -> sendChatAction ctx.Update.Message.Value.Chat.Id ChatAction.UploadPhoto |> bot)
        
        cmd "/send_message1" (fun _ -> sendMessageFormatted "Test *Markdown*" ParseMode.Markdown)
        cmd "/send_message2" (fun _ -> sendMessageFormatted "Test <b>HTML</b>" ParseMode.HTML)
        cmd "/send_message3" (fun _ -> sayWithArgs "@Dolfik! See http://fsharplang.ru - Russian F# Community" None (Some true) (Some true) None None)
        cmd "/send_message4" (fun _ -> sayWithArgs "That's message with reply!" None None None (Some ctx.Update.Message.Value.MessageId) None)
        cmd "/send_message5" (fun _ ->
        (
          let keyboard = Array.init 2 (fun x -> Array.init 2 (fun y -> KeyboardButton.Create(y.ToString() + x.ToString())))
          let markup = Markup.ReplyKeyboardMarkup (ReplyKeyboardMarkup.Create(keyboard))
          bot (sendMessageMarkup (fromId()) "That's keyboard!" markup)
        ))
        cmd "/send_message6" (fun _ ->
        (
          let markup = Markup.ReplyKeyboardRemove { RemoveKeyboard = true; Selective = None; }
          bot (sendMessageMarkup (fromId()) "Keyboard was removed!" markup)
        ))
        cmd "/send_message7" (fun _ ->
        (
          let keyboard =
            [|
               [| InlineKeyboardButton.Create("Test", "1234") |]
            |]
          let markup = Markup.InlineKeyboardMarkup { InlineKeyboard = keyboard }
          (sendMessageMarkup (fromId()) "Thats inline keyboard!" markup) |> bot
         ))
         
        cmd "/send_message8" (fun _ ->
         (
           let pack name stream = InputMedia.Photo <| {
             InputMediaPhoto.Media = InputFile.File(name, stream)
             
             Type = "photo"; Caption = Some name; ParseMode = None; CaptionEntities = None
           }
           let image1 = Http.RequestStream(PhotoUrl).ResponseStream
           let image2 = Http.RequestStream(PhotoUrl).ResponseStream
           let media = [| pack "Image 1" image1; pack "Image 2" image2 |]
           sendMediaGroup (fromId()) media |> bot 
         ))
         
        cmd "/forward_message" (fun _ -> forwardMessage (fromId()) (fromId()) ctx.Update.Message.Value.MessageId |> bot)
         
        cmd "/show_my_photos_sizes" (fun _ ->
         (
           let x = botResult (getUserProfilePhotosAll (fromId())) |> processResultWithValue
           if x.IsNone then ()
           else
             let text =
               sprintf "Photos: %s" 
                 (x.Value.Photos
                   |> Seq.map (Seq.last >> (fun f -> sprintf "%ix%i" f.Width f.Height))
                   |> String.concat ",")
 
             bot (sendMessage (fromId()) text)
         ))
         
        cmd "/get_chat_info" (fun _ -> getChatInfo ctx.Update.Message.Value)
        cmd "/send_photo" (fun _ -> sendPhoto ctx.Update.Message.Value)
      |]

    if result then ()
    else bot (sendMessage (fromId()) defaultText)
  updateArrived
  
let webSocketEndpoint = None // Some "https://1c0860ec2320.ngrok.io"

let start token =
  (*
  * Set poxy
  *```fsharp
  * let handler = new HttpClientHadler ()
  * handler.Proxy <- createMyProxy ()
  * handler.UseProxy <- true
  * let config = { defaultConfig with Token = token
  *                                   Cleint = new HttpClient(handler, true) }
  *```
  *)
  let config = { defaultConfig with Token = token }
  let updateArrived = processMessageBuild config

  match webSocketEndpoint with
  | Some webSocketEndpoint ->
    async {
      let apiPath = sprintf "/%s" config.Token
      let webSocketEndpoint = sprintf "%s%s" webSocketEndpoint apiPath
      let! hook = SetWebhookReq.Make(webSocketEndpoint) |> api config
      match hook with
      | Ok _ ->
        use listener = new HttpListener()
        listener.Prefixes.Add("http://*:4444/")
        listener.Start()

        let webhook = { Listener = listener; ValidateRequest = (fun req -> req.Url.LocalPath = apiPath) }
        return! startBot { config with WebHook = Some webhook } updateArrived None
      | Error e -> 
        printf "Can't set webhook: %A" e
        return ()
    }
  | _ ->
    async {
      let! _ = deleteWebhookBase () |> api config
      return! startBot config updateArrived None
    }
  

[<EntryPoint>]
let main _ =
  printfn "Bot started..."
  let startBot = 
    if File.Exists(TokenFileName) then
      start (File.ReadAllText(TokenFileName))
    else
      printf "Please, enter bot token: "
      let token = System.Console.ReadLine()
      File.WriteAllText(TokenFileName, token)
      start token
  startBot |> Async.RunSynchronously
  0
