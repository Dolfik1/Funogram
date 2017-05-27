namespace Funogram.Tests

module Constants =
    open Funogram.Types
    
    let jsonTestObj = { Type = "italic"; Offset = 0L; Length = 100L; Url = Some("http://github.com"); User = None }
    let jsonTestObjString = """{"type":"italic","offset":0,"length":100,"url":"http://github.com"}"""
    let jsonTestObjResultString = """{"ok":true,result:{"type":"italic","offset":0,"length":100,"url":"http://github.com","user":null} }"""

    let jsonTestObjUser = { Id = 123456L; FirstName = "BotFather"; LastName = None; Username = Some("BotFather"); LanguageCode = None; }
    let jsonTestObjUserResultString = """{"ok":true,"result":{"id":123456,"first_name":"BotFather","username":"BotFather"}}"""