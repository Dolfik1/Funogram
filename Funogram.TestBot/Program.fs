namespace Funogram.TestBot

open System
open System.IO
open Funogram
open Funogram.TestBot.Types
open Funogram.Types
open Funogram.Bot


module Main =

    [<Literal>]
    let TokenFileName = "token"

    let processResultWithValue (result: Result<'a, ApiResponseError>) =
        match result with
        | Ok v -> Some v
        | Error e -> 
            printfn "Error: %s" e.Description 
            None
    let processResult (result: Result<'a, ApiResponseError>) =
        processResultWithValue result |> ignore

    let getChatInfo token msg = 
        match Telegram.GetChat(token, ChatId.Long(msg.Chat.Id)) with
        | Ok x -> processResultWithValue <| Telegram.SendMessage(
                                    token,
                                    ChatId.Long(msg.Chat.Id),
                                    sprintf "Id: %i, Type: %s" x.Id x.Type) |> ignore
        | Error e -> printf "Error: %s" e.Description

    let updateArrived ctx = 
        let fromId() = ChatId.Long(ctx.Update.Message.Value.From.Value.Id)

            
        let sayWithArgs text parseMode disableWebPagePreview disableNotification replyToMessageId replyMarkup = 
            Telegram.SendMessageBaseAsync (
                ctx.Config.Token, 
                fromId(), 
                text, 
                parseMode, 
                disableWebPagePreview, 
                disableNotification, 
                replyToMessageId, 
                replyMarkup) |> Async.RunSynchronously |> processResult

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
                    sayWithArgs "That's keyboard!" (Some ParseMode.Markdown) None None None (Some markup)))
                cmd "/send_message6" (fun _ ->
                    let markup = Markup.ReplyKeyboardRemove { RemoveKeyboard = true; Selective = None; }
                    sayWithArgs "Keyboard was removed!" (Some ParseMode.Markdown) None None None (Some markup))
                
                cmd "/forward_message" (fun _ -> Telegram.ForwardMessage(ctx.Config.Token, fromId(), fromId(), ctx.Update.Message.Value.MessageId) |> processResult)
                cmd "/show_my_photos_sizes" (fun _ -> 
                (   
                    let x = Telegram.GetUserProfilePhotos(ctx.Config.Token, ctx.Update.Message.Value.Chat.Id |> int) |> processResultWithValue
                    if x.IsNone then ()
                    else
                        say (sprintf "Photos: %s" (x.Value.Photos |> Seq.map (fun f -> f |> Seq.last) |> Seq.map (fun f -> sprintf "%ix%i" f.Width f.Height) |> String.concat ","))
                ))
                cmd "/get_chat_info" (fun _ -> getChatInfo ctx.Config.Token ctx.Update.Message.Value)
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
/get_chat_info - Returns id and type of current chat"""

    let start token =
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
