namespace Funogram.TestBot

open System
open System.IO
open Funogram.Api
open Funogram.TestBot.Types
open Funogram.Types
open Funogram.Bot
open FunHttp


module Main =

    [<Literal>]
    let TokenFileName = "token"
    let mutable botToken = "none"
    let bot data = api botToken data |> Async.RunSynchronously

    let processResultWithValue (result: Result<'a, ApiResponseError>) =
        match result with
        | Ok v -> Some v
        | Error e -> 
            printfn "Error: %s" e.Description 
            None
    let processResult (result: Result<'a, ApiResponseError>) =
        processResultWithValue result |> ignore

    let getChatInfo msg = ()
        //match bot (getChat msg.Chat.Id) with
        //| Ok x -> processResultWithValue <| bot (sendMessage msg.Chat.Id (sprintf "Id: %i, Type: %s" x.Id x.Type)) |> ignore
        //| Error e -> printf "Error: %s" e.Description

    let sendPhoto msg =
        let image = Http.RequestStream("https://upload.wikimedia.org/wikipedia/commons/f/f5/Example_image.jpg")
        bot (sendPhoto msg.Chat.Id (FileToSend.File("example.jpg", image.ResponseStream)) "Example") |> processResult

    let updateArrived ctx = 
        let fromId() = ctx.Update.Message.Value.From.Value.Id

            
        let sayWithArgs text parseMode disableWebPagePreview disableNotification replyToMessageId replyMarkup = 
            bot (sendMessageBase (ChatId.Int (fromId())) text parseMode disableWebPagePreview disableNotification replyToMessageId replyMarkup) |> processResult

        let say text = sayWithArgs text None None None None None

        let result =
            processCommands ctx [
                cmd "/send_message1" (fun _ -> sayWithArgs "Test *Markdown*" (Some ParseMode.Markdown) None None None None)
                cmd "/send_message2" (fun _ -> sayWithArgs "Test <b>HTML</b>" (Some ParseMode.HTML) None None None None)
                cmd "/send_message3" (fun _ -> sayWithArgs "@Dolfik! See http://fsharplang.ru - Russian F# Community" (Some ParseMode.Markdown) None None None None)
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
                    sayWithArgs "That's keyboard!" None None None None (Some markup)))
                cmd "/send_message6" (fun _ ->
                    let markup = Markup.ReplyKeyboardRemove { RemoveKeyboard = true; Selective = None; }
                    sayWithArgs "Keyboard was removed!" None None None None (Some markup))
                
                cmd "/forward_message" (fun _ -> bot (forwardMessage (fromId()) (fromId()) ctx.Update.Message.Value.MessageId) |> processResult)
                cmd "/show_my_photos_sizes" (fun _ -> 
                (   
                    let x = bot (getUserProfilePhotos ctx.Update.Message.Value.Chat.Id) |> processResultWithValue
                    if x.IsNone then ()
                    else
                        say (sprintf "Photos: %s" (x.Value.Photos |> Seq.map (fun f -> f |> Seq.last) |> Seq.map (fun f -> sprintf "%ix%i" f.Width f.Height) |> String.concat ","))
                ))
                cmd "/get_chat_info" (fun _ -> getChatInfo ctx.Update.Message.Value)
                cmd "/send_photo" (fun _ -> sendPhoto ctx.Update.Message.Value)
            ]

        if result then ()
        else say """⭐️Available test commands:
/send_message1 - Markdown test
/send_message2 - HTML test
/send_message3 - Disable web page preview and notifications
/send_message4 - Test reply message
/send_message5 - Test ReplyKeyboardMarkup
/send_message6 - Test RemoveKeyboardMarkup

/forward_message - Test forward message
/show_my_photos_sizes - Test getUserProfilePhotos method
/get_chat_info - Returns id and type of current chat
/send_photo - Send example photo"""

    let start token =
        botToken <- token
        let config = { defaultConfig with Token = token }
        startBot config updateArrived
    
    [<EntryPoint>]
    let main argv =
        printfn "Bot started..."
        if File.Exists(TokenFileName) then
            start (File.ReadAllText(TokenFileName))
        else
            printf "Please, enter bot token: "
            let token = System.Console.ReadLine()
            File.WriteAllText(TokenFileName, token)
            start token
        0 // return an integer exit code
