namespace Funogram.Tests

open System
open Funogram.Telegram
open Funogram.Telegram.Types
open Funogram.Types

module Constants =
  let private ok = sprintf """{"ok":true,"result":%s}"""
    
  let testDate = System.DateTime(2117, 05, 28, 12, 47, 51, DateTimeKind.Utc)
  let testDateUnix = 4651649271L
  let testForwardOrigin = MessageOrigin.HiddenUser(
                            MessageOriginHiddenUser.Create(
                              ``type`` = "hidden_user",
                              date = testDate,
                              senderUserName = "test user"
                            )
                          )

  let jsonTestObj = { Type = "italic"; Offset = 0L; Length = 100L; Url = Some("http://github.com"); User = None; Language = None; CustomEmojiId = None }
  let jsonTestObjString = """{"type":"italic","offset":0,"length":100,"url":"http://github.com"}"""
  let jsonTestObjResultString = """{"ok":true,"result":{"type":"italic","offset":0,"length":100,"url":"http://github.com","user":null,"language":null} }"""

  let jsonTestObjUser = { Id = 123456L; FirstName = "BotFather"; LastName = None; Username = (Some "BotFather"); LanguageCode = None; IsBot = false; CanJoinGroups = None; CanReadAllGroupMessages = None; SupportsInlineQueries = None; IsPremium = None; AddedToAttachmentMenu = None }
  let jsonTestObjUserResultString = """{"ok":true,"result":{"id":123456,"first_name":"BotFather","username":"BotFather","language_code":null,"is_bot":false,"can_join_groups":null,"can_read_all_group_messages":null,"supports_inline_queries":null}}"""

  let jsonTestEditResult1 = EditMessageResult.Success(true)
  let jsonTestEditResult1String = "true"
  let jsonTestEditResult1ApiString = ok "true"

  let jsonTestChat = Chat.Create(1L, ChatType.Group, title = "Test group")
  let jsonTestMessage =  Message.Create(123L, testDate, jsonTestChat, text = "abc")

  let jsonTestEditResult2 = EditMessageResult.Message(jsonTestMessage)
  let jsonTestEditResult2String = sprintf """{"message_id":123,"date":%i,"chat":{"id":1,"type":"group","title":"Test group"},"text":"abc"}""" testDateUnix
  let jsonTestEditResult2ApiString = ok jsonTestEditResult2String
   
  let jsonTestEditResult3ApiString = """{"ok":true,"result":{"message_id":123,"from":{"id":321,"is_bot":true,"first_name":"FSharpBot","username":"FSharpBot"},"chat":{"id":123,"first_name":"Test","last_name":"Test","username":"test","type":"private"},"date":4651649271,"edit_date":4651649271,"text":"Updated"}}"""


  let testMaskPosition = { MaskPosition.Point = MaskPoint.Eyes; XShift = 0.0; YShift = 0.0; Scale = 0.0 }
  let jsonTestMaskPosition = """{"point":"eyes","x_shift":0,"y_shift":0,"scale":0}"""
  let jsonTestMaskPositionResult = ok jsonTestMaskPosition


  let jsonMessageForwardDate = Message.Create(1L, testDate, jsonTestChat, text = "abc", forwardOrigin = testForwardOrigin)
    
  let jsonMessageForward = Message.Create(1L, testDate, jsonTestChat, forwardOrigin = testForwardOrigin, text = "abc")
    
  let jsonMessageForwardDateString = sprintf """{"message_id":1,"date":%i,"chat":{"id":1,"type":"group","title":"Test group"},"forward_date":%i,"text":"abc"}""" testDateUnix testDateUnix    
  let jsonMessageForwardDateApiString = ok jsonMessageForwardDateString
    
  let paramsDictionary =
    [
      "offset", box 0
      "limit", box 100
      "timeout", box 600
      "allowed_updates", box [||]
    ] |> dict
  let jsonParamsDictionary = """{"offset":0,"limit":100,"timeout":600,"allowed_updates":[]}"""
    
  let forwardMessageReq = 
    ({
      ChatId = ChatId.String "Dolfik"
      MessageThreadId = None
      FromChatId = ChatId.Int 10L
      MessageId = 10L
      DisableNotification = None
      ProtectContent = None
    }: Req.ForwardMessage) :> IBotRequest
  let jsonForwardMessageReq = """{"chat_id":"Dolfik","from_chat_id":10,"message_id":10}"""
  
  let jsonTestObjChatMember = ChatMemberMember.Create("member", User.Create(600000000L, false, "firstName", "lastName", "userName", "ru"))
  let jsonTestObjChatMemberResultString = """{"ok":true,"result":{"status":"member","user":{"id":600000000,"is_bot":false,"first_name":"firstName","last_name":"lastName","username":"userName","language_code":"ru"}}}"""
   