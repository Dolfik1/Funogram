namespace Funogram.Tests

module Constants =
    open Funogram
    open Funogram.Types
    
    let jsonTestObj = { Type = "italic"; Offset = 0L; Length = 100L; Url = Some("http://github.com"); User = None }
    let jsonTestObjResultString = "{\"type\":\"italic\",\"offset\":0,\"length\":100,\"url\":\"http://github.com\",\"user\":null}"
    let jsonTestObjString = "{ \"ok\":true, result: {\"type\":\"italic\",\"offset\":0,\"length\":100,\"url\":\"http://github.com\",\"user\":null} }"