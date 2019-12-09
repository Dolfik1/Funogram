module Funogram.TestBot

open System.IO
open Funogram.Api
open Funogram.Types
open Funogram.Bot
open FunHttp


[<Literal>]
let TokenFileName = "token"
let mutable botToken = "none"

let processMessageBuild config =

    let defaultText = """⭐️Available test commands:
    /send_message1 - Markdown test
    /send_message2 - HTML test
    /send_message3 - Disable web page preview and notifications
    /send_message4 - Test reply message
    /send_message5 - Test ReplyKeyboardMarkup
    /send_message6 - Test RemoveKeyboardMarkup
    /send_message7 - Test inline keyboard
    
    /send_action - Test action

    /forward_message - Test forward message
    /show_my_photos_sizes - Test getUserProfilePhotos method
    /get_chat_info - Returns id and type of current chat
    /send_photo - Send example photo"""


    let processResultWithValue (result: Result<'a, ApiResponseError>) =
        match result with
        | Ok v -> Some v
        | Error e ->
            printfn "Error: %s" e.Description
            None

    let processResult (result: Result<'a, ApiResponseError>) =
        processResultWithValue result |> ignore

    let botResult data = api config data |> Async.RunSynchronously
    let bot data = botResult data |> processResult


    let getChatInfo msg =
        let result = botResult (getChat msg.Chat.Id)
        match result with
        | Ok x ->
            botResult (sendMessage msg.Chat.Id (sprintf "Id: %i, Type: %s" x.Id x.Type))
            |> processResultWithValue
            |> ignore
        | Error e -> printf "Error: %s" e.Description

    let sendPhoto msg =
        let image = Http.RequestStream("https://upload.wikimedia.org/wikipedia/commons/f/f5/Example_image.jpg")
        bot (sendPhoto msg.Chat.Id (FileToSend.File("example.jpg", image.ResponseStream)) "Example")

    let updateArrived ctx =
        let fromId() = ctx.Update.Message.Value.From.Value.Id
        let sayWithArgs text parseMode disableWebPagePreview disableNotification replyToMessageId replyMarkup =
            bot (sendMessageBase (ChatId.Int (fromId())) text parseMode disableWebPagePreview disableNotification replyToMessageId replyMarkup)

        let sendMessageFormatted text parseMode = (sendMessageBase (ChatId.Int(fromId())) text (Some parseMode) None None None None) |> bot

        let result =
            processCommands ctx [
                cmd "/send_action" (fun _ -> sendChatAction ctx.Update.Message.Value.Chat.Id ChatAction.UploadPhoto |> bot)
                cmd "/send_message1" (fun _ -> sendMessageFormatted "Test *Markdown*" ParseMode.Markdown)
                cmd "/send_message2" (fun _ -> sendMessageFormatted "Test <b>HTML</b>" ParseMode.HTML)
                cmd "/send_message3" (fun _ -> sayWithArgs "@Dolfik! See http://fsharplang.ru - Russian F# Community" None (Some true) (Some true) None None)
                cmd "/send_message4" (fun _ -> sayWithArgs "That's message with reply!" None None None (Some ctx.Update.Message.Value.MessageId) None)
                cmd "/send_message5" (fun _ ->
                (
                    let keyboard = (Seq.init 2 (fun x -> Seq.init 2 (fun y -> { Text = y.ToString() + x.ToString(); RequestContact = None; RequestLocation = None })))
                    let markup = Markup.ReplyKeyboardMarkup {
                        Keyboard = keyboard
                        ResizeKeyboard = None
                        OneTimeKeyboard = None
                        Selective = None
                    }
                    bot (sendMessageMarkup (fromId()) "That's keyboard!" markup)
                ))
                cmd "/send_message6" (fun _ ->
                (
                    let markup = Markup.ReplyKeyboardRemove { RemoveKeyboard = true; Selective = None; }
                    bot (sendMessageMarkup (fromId()) "Keyboard was removed!" markup)
                ))
                cmd "/send_message7" (fun _ ->
                (
                    let keyboard = [[ {
                      Text = "Test"
                      CallbackData = Some("1234")
                      Url = None
                      CallbackGame = None
                      SwitchInlineQuery = None
                      SwitchInlineQueryCurrentChat = None
                    } ] |> List.toSeq ]
                    let markup = Markup.InlineKeyboardMarkup { InlineKeyboard = keyboard }
                    (sendMessageMarkup (fromId()) "Thats inline keyboard!" markup) |> bot
                ))
                cmd "/forward_message" (fun _ -> bot (forwardMessage (fromId()) (fromId()) ctx.Update.Message.Value.MessageId))
                cmd "/show_my_photos_sizes" (fun _ ->
                (
                    let x = botResult (getUserProfilePhotosAll (fromId())) |> processResultWithValue
                    if x.IsNone then ()
                    else
                        let text = sprintf "Photos: %s" (x.Value.Photos
                                    |> Seq.map (Seq.last >> (fun f -> sprintf "%ix%i" f.Width f.Height))
                                    |> String.concat ",")

                        bot (sendMessage (fromId()) text)
                ))
                cmd "/get_chat_info" (fun _ -> getChatInfo ctx.Update.Message.Value)
                cmd "/send_photo" (fun _ -> sendPhoto ctx.Update.Message.Value)
            ]

        if result then ()
        else bot (sendMessage (fromId()) defaultText)
    updateArrived

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
    startBot config updateArrived None

[<EntryPoint>]
let main argv =
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
