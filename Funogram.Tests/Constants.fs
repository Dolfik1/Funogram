namespace Funogram.Tests

module Constants =
    open Funogram.Types
    
    let jsonTestObj = { Type = "italic"; Offset = 0L; Length = 100L; Url = Some("http://github.com"); User = None }
    let jsonTestObjString = """{"type":"italic","offset":0,"length":100,"url":"http://github.com"}"""
    let jsonTestObjResultString = """{"ok":true,result:{"type":"italic","offset":0,"length":100,"url":"http://github.com","user":null} }"""

    let jsonTestObjUser = { Id = 123456L; FirstName = "BotFather"; LastName = None; Username = Some("BotFather"); LanguageCode = None; }
    let jsonTestObjUserResultString = """{"ok":true,"result":{"id":123456,"first_name":"BotFather","username":"BotFather"}}"""

    let jsonTestEditResult1 = EditMessageResult.Success(true)
    let jsonTestEditResult1String = "true"

    let jsonTestChat = { Id = 1L; Type = "group"; Title = Some("Test group"); Username = None; FirstName = None; LastName = None; AllMembersAreAdministrators = Some(true) }
    let jsonTestMessage = 
        { MessageId = 123
          Date = System.DateTime(2117, 05, 28, 12, 47, 51)
          Text = Some("abc")
          Chat = jsonTestChat
          From = None
          ForwardFrom = None
          ForwardFromChat = None
          ForwardFromMessageId = None
          ForwardDate = None
          ReplyToMessage = None
          EditDate = None
          Entities = None
          Audio = None
          Document = None
          Game = None
          Photo = None
          Sticker = None
          Video = None
          Voice = None
          Caption = None
          Contact = None
          Location = None
          Venue = None
          NewChatMember = None
          LeftChatMember = None
          NewChatTitle = None
          NewChatPhoto = None
          DeleteChatPhoto = None
          GroupChatCreated = None
          SupergroupChatCreated = None
          ChannelChatCreated = None
          MigrateToChatId = None
          MigrateFromChatId = None
          PinnedMessage = None }

    let jsonTestEditResult2 = EditMessageResult.Message(jsonTestMessage)
    let jsonTestEditResult2String = """{"message_id":123,"date":4651649271,"chat":{"id":1,"type":"group","title":"Test group","all_members_are_administrators":true},"text":"abc"}"""