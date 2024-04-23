module Funogram.TestBot.Commands.Base

open Funogram.Telegram.Bot
open Funogram.Telegram
open Funogram.Telegram.Types
open Funogram.TestBot.Core

let defaultText = """⭐️Available test commands:
  /send_message1 - Markdown test
  /send_message2 - HTML test
  /send_message3 - Disable web page preview and notifications
  /send_message4 - Test reply message
  /send_message5 - Test ReplyKeyboardMarkup
  /send_message6 - Test RemoveKeyboardMarkup
  /send_message7 - Test inline keyboard
  /send_message8 - Test multiple media
  /send_message9 - Test multiple media as bytes
  /send_message10 - MarkdownV2 test
  /send_message11 - MarkdownV2 test (json body)
    
  /send_action - Test action

  /forward_message - Test forward message
  /show_my_photos_sizes - Test getUserProfilePhotos method
  /get_chat_info - Returns id and type of current chat
  /send_photo - Send example photo
  /cmdscan stringA stringB - Test cmdScan, concatenate stringA and stringB"""
  
let updateArrived (ctx: UpdateContext) =

  let wrap fn =
    let fromId () = ctx.Update.Message.Value.From.Value.Id
    fn ctx.Config (fromId ())
  
  let result =
    processCommands ctx [|
      cmdScan "/cmdscan %s %s" (fun (a, b) _ ->  TextMessages.testScan a b |> wrap)
      
      cmd "/send_action" (fun _ -> Other.testSendAction |> wrap)
      
      cmd "/send_message1" (fun _ -> TextMessages.testMarkdown |> wrap)
      cmd "/send_message2" (fun _ -> TextMessages.testHtml |> wrap)
      cmd "/send_message3" (fun _ -> TextMessages.testNoWebpageAndNotification |> wrap)
      cmd "/send_message4" (fun _ -> TextMessages.testReply ctx |> wrap)
      cmd "/send_message5" (fun _ -> Markup.testReplyKeyboard |> wrap)
      cmd "/send_message6" (fun _ -> Markup.testRemoveKeyboard |> wrap)
      cmd "/send_message7" (fun _ -> Markup.testInlineKeyboard |> wrap)
       
      cmd "/send_message8" (fun _ -> Files.testUploadAndSendPhotoGroup |> wrap)
      cmd "/send_message9" (fun _ -> Files.testUploadAndSendPhotoGroupAsBytes |> wrap)
      cmd "/send_message10" (fun _ -> TextMessages.testMarkdownV2 false |> wrap)
      cmd "/send_message11" (fun _ -> TextMessages.testMarkdownV2 true |> wrap)

      cmd "/forward_message" (fun _ -> TextMessages.testForwardMessage ctx |> wrap)

      cmd "/show_my_photos_sizes" (fun _ -> Other.testPhotosSize |> wrap)
       
      cmd "/get_chat_info" (fun _ -> Other.testGetChatInfo ctx |> wrap)
      cmd "/send_photo" (fun _ -> Files.testUploadAndSendSinglePhoto |> wrap)
    |]

  if result then
    match ctx.Update.CallbackQuery with
    | Some ({ Data = Some "callback2" } as c) ->
      match c.Message with
      | Some (MaybeInaccessibleMessage.Message msg) ->
        let inlineKeyboardMarkup = InlineKeyboardMarkup.Create([| [| InlineKeyboardButton.Create("Changed!", callbackData = "Test") |] |])
        Req.EditMessageReplyMarkup.Make(msg.Chat.Id, msg.MessageId, replyMarkup = inlineKeyboardMarkup)
        |> bot ctx.Config
      | _ -> ()
    | _ -> ()
    
    match ctx.Update.Message with
    | Some { From = Some { Id = id } } ->
      Api.sendMessage id defaultText |> bot ctx.Config
    | _ -> ()