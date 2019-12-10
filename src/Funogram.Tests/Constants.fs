namespace Funogram.Tests

open Funogram
open System
open RequestsTypes

module Constants =
    open Funogram.Types
    
    let private ok = sprintf """{"ok":true,"result":%s}"""
    
    let testDate = System.DateTime(2117, 05, 28, 12, 47, 51, DateTimeKind.Utc)
    let testDateUnix = 4651649271L
    
    let jsonTestObj = { Type = "italic"; Offset = 0L; Length = 100L; Url = Some("http://github.com"); User = None }
    let jsonTestObjString = """{"type":"italic","offset":0,"length":100,"url":"http://github.com"}"""
    let jsonTestObjResultString = """{"ok":true,"result":{"type":"italic","offset":0,"length":100,"url":"http://github.com","user":null} }"""

    let jsonTestObjUser = { Id = 123456L; FirstName = "BotFather"; LastName = None; Username = (Some "BotFather"); LanguageCode = None; IsBot = false; }
    let jsonTestObjUserResultString = """{"ok":true,"result":{"id":123456,"first_name":"BotFather","username":"BotFather","is_bot":false}}"""

    let jsonTestEditResult1 = EditMessageResult.Success(true)
    let jsonTestEditResult1String = "true"
    let jsonTestEditResult1ApiString = ok "true"

    let jsonTestChat = { defaultChat with Id = 1L; Type = "group"; Title = (Some "Test group"); AllMembersAreAdministrators = (Some true) }
    let jsonTestMessage = 
        { defaultMessage with MessageId = 123L;  Date = testDate; Text = Some("abc"); Chat = jsonTestChat }

    let jsonTestEditResult2 = EditMessageResult.Message(jsonTestMessage)
    let jsonTestEditResult2String = sprintf """{"message_id":123,"date":%i,"chat":{"id":1,"type":"group","title":"Test group","all_members_are_administrators":true},"text":"abc"}""" testDateUnix
    let jsonTestEditResult2ApiString = ok jsonTestEditResult2String
    
    let jsonTestEditResult3ApiString = """{"ok":true,"result":{"message_id":123,"from":{"id":321,"is_bot":true,"first_name":"FSharpBot","username":"FSharpBot"},"chat":{"id":123,"first_name":"Test","last_name":"Test","username":"test","type":"private"},"date":4651649271,"edit_date":4651649271,"text":"Updated"}}"""


    let testMaskPosition = { MaskPosition.Point = MaskPoint.Eyes; XShift = 0.0; YShift = 0.0; Scale = 0.0 }
    let jsonTestMaskPosition = """{"point":"eyes","x_shift":0,"y_shift":0,"scale":0}"""
    let jsonTestMaskPositionResult = ok jsonTestMaskPosition
    
    let jsonMessageForwardDate = { defaultMessage with Date = testDate; ForwardDate = Some testDate }
    
    let jsonMessageForward = 
        { defaultMessage with MessageId = 1L;  Date = testDate; ForwardDate = Some testDate; Text = None; Chat = defaultChat }
    
    let jsonMessageForwardDateString = sprintf """{"message_id":1,"date":%i,"chat":{"id":0,"type":""},"forward_date":%i}""" testDateUnix testDateUnix    
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
      {
        ForwardMessageReq.ChatId = ChatId.String "Dolfik"
        FromChatId = ChatId.Int 10L
        MessageId = 10L
        ForwardMessageReq.DisableNotification = None
      } :> IBotRequest
    let jsonForwardMessageReq = """{"chat_id":"Dolfik","from_chat_id":10,"message_id":10}"""
   