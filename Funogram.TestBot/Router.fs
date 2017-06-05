namespace Funogram.TestBot
open Funogram
open Funogram.Types
open Funogram.TestBot.Types

module Router =

    let (|AvailableCommands|_|) (msg: Message) text = 
        if text = "/commands" then
            Some("")
        else None

    let inline processError<'a> (result: Result<'a,Types.ApiResponseError>) = 
        match result with
        | Error e -> printf "Error: %s" e.Description
        | _ -> ()
    
    let showAvailableCommands token msg = 
        processError <| Telegram.SendMessage(token, ChatId.Long(msg.Chat.Id), """⭐️Available test commands:
/send_message1 - Markdown test
/send_message2 - HTML test
/send_message3 - Disable web page preview and notifications
/send_message4 - Test reply message
/send_message5 - Test ReplyKeyboardMarkup
/send_message6 - Test RemoveKeyboardMarkup

/forward_message - Test forward message
/show_my_photos_sizes - Test getUserProfilePhotos method
/get_chat_info - Returns id and type of current chat""")


    let sendMessage1 token msg = 
        processError <| Telegram.SendMessage(token, ChatId.Long(msg.Chat.Id), "Test *Markdown*", ParseMode.Markdown)

    let sendMessage2 token msg = 
        processError <| Telegram.SendMessage(token, ChatId.Long(msg.Chat.Id), "Test <b>HTML</b>", ParseMode.HTML)

    let sendMessage3 token msg =
        processError <| Telegram.SendMessage(token, ChatId.Long(msg.Chat.Id), "@Dolfik! See http://fsharplang.ru - Russian F# Community", disableWebPagePreview = true, disableNotification = true)

    let sendMessage4 token msg =
        processError <| Telegram.SendMessage(token, ChatId.Long(msg.Chat.Id), "That's message with reply!", replyToMessageId = msg.MessageId)

    let sendMessage5 token msg =
        let keyboard = (Seq.init 2 (fun x -> Seq.init 2 (fun y -> { Text = y.ToString() + x.ToString(); RequestContact = None; RequestLocation = None })))
        let markup = {  
            Keyboard = keyboard
            ResizeKeyboard = None
            OneTimeKeyboard = None
            Selective = None
        }
        processError <| Telegram.SendMessage(token, ChatId.Long(msg.Chat.Id), "That's keyboard!", replyMarkup = Markup.ReplyKeyboardMarkup(markup))

    let sendMessage6 token msg =
        processError <| Telegram.SendMessage
                (token, 
                ChatId.Long(msg.Chat.Id), "Keyboard was removed!", 
                replyMarkup = Markup.ReplyKeyboardRemove({ RemoveKeyboard = true; Selective = None; }))

    let forwardMessage token msg =
        processError <| Telegram.ForwardMessage(token, ChatId.Long(msg.Chat.Id), ChatId.Long(msg.Chat.Id), msg.MessageId)

    let showMyPhotos token msg =
        match Telegram.GetUserProfilePhotos(token, msg.Chat.Id |> int) with
        | Ok x -> processError <| Telegram.SendMessage(
                                    token, 
                                    ChatId.Long(msg.Chat.Id), 
                                    sprintf "Photos: %s" (x.Photos |> Seq.map (fun f -> f |> Seq.last) |> Seq.map (fun f -> sprintf "%ix%i" f.Width f.Height) |> String.concat ","))
        | Error e -> printf "Error: %s" e.Description 

    let getChatInfo token msg = 
        match Telegram.GetChat(token, ChatId.Long(msg.Chat.Id)) with
        | Ok x -> processError <| Telegram.SendMessage(
                                    token,
                                    ChatId.Long(msg.Chat.Id),
                                    sprintf "Id: %i, Type: %s" x.Id x.Type)
        | Error e -> printf "Error: %s" e.Description

    let processMessage (bot: Bot) (message: Message) = 
        match message.Text with
        | Some x -> 
            match x with
            | "/send_message1" -> sendMessage1 bot.Token message
            | "/send_message2" -> sendMessage2 bot.Token message
            | "/send_message3" -> sendMessage3 bot.Token message
            | "/send_message4" -> sendMessage4 bot.Token message
            | "/send_message5" -> sendMessage5 bot.Token message
            | "/send_message6" -> sendMessage6 bot.Token message
            | "/forward_message" -> forwardMessage bot.Token message
            | "/show_my_photos_sizes" -> showMyPhotos bot.Token message
            | "/get_chat_info" -> getChatInfo bot.Token message
            | _ -> showAvailableCommands bot.Token message
        | _ -> ()
            
    let updateArrived (bot: Bot) (update: Update) = 
        match update with
            | UpdateType.Message msg -> processMessage bot msg
            | _ -> ()