namespace Funogram.Tests

module Constants =
    open Funogram.Types
    
    let jsonTestObj = { Type = "italic"; Offset = 0L; Length = 100L; Url = Some("http://github.com"); User = None }
    let jsonTestObjString = """{"type":"italic","offset":0,"length":100,"url":"http://github.com"}"""
    let jsonTestObjResultString = """{"ok":true,result:{"type":"italic","offset":0,"length":100,"url":"http://github.com","user":null} }"""

    let jsonTestObjUser = { Id = 123456L; FirstName = "BotFather"; LastName = None; Username = (Some "BotFather"); LanguageCode = None; IsBot = false; }
    let jsonTestObjUserResultString = """{"ok":true,"result":{"id":123456,"first_name":"BotFather","username":"BotFather","is_bot":false}}"""

    let jsonTestEditResult1 = EditMessageResult.Success(true)
    let jsonTestEditResult1String = "true"
    let jsonTestEditResult1ApiString = """{"ok":true,"result":true}"""

    let jsonTestChat = { defaultChat with Id = 1L; Type = "group"; Title = (Some "Test group"); AllMembersAreAdministrators = (Some true) }
    let jsonTestMessage = 
        { defaultMessage with MessageId = 123L;  Date = System.DateTime(2117, 05, 28, 12, 47, 51); Text = Some("abc"); Chat = jsonTestChat }

    let jsonTestEditResult2 = EditMessageResult.Message(jsonTestMessage)
    let jsonTestEditResult2String = """{"message_id":123,"date":4651649271,"chat":{"id":1,"type":"group","title":"Test group","all_members_are_administrators":true},"text":"abc"}"""
    let jsonTestEditResult2ApiString = sprintf """{"ok":true,"result":%s}""" jsonTestEditResult2String
    
    let jsonTestEditResult3ApiString = """{"ok":true,"result":{"message_id":123,"from":{"id":321,"is_bot":true,"first_name":"FSharpBot","username":"FSharpBot"},"chat":{"id":123,"first_name":"Test","last_name":"Test","username":"test","type":"private"},"date":4651649271,"edit_date":4651649271,"text":"Updated"}}"""


    let testMaskPosition = { MaskPosition.Point = MaskPoint.Eyes; XShift = 0.0; YShift = 0.0; Scale = 0.0 }
    let jsonTestMaskPositionResult = """{"ok":true,"result":{"point":"eyes","x_shift":0.0,"y_shift":0.0,"scale":0.0}}"""