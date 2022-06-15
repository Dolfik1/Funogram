open System
open System.Text

module Helpers =
  let reservedKeywords =
    [
      "type"
    ] |> Set.ofList

  let typeMap =
    [ 
      "Integer", "int64"
      "Int", "int"
      "Boolean", "bool"
      "String", "string"
      "True", "bool"
      "Integer or String", "ChatId"
      "InputFile or String", "InputFile"
      "Float", "float"
      "Float number", "float"
      "InlineKeyboardMarkup or ReplyKeyboardMarkup or ReplyKeyboardRemove or ForceReply", "Markup"
      "array of Messages", "Message[]"
      "InputMediaAudio, InputMediaDocument, InputMediaPhoto and InputMediaVideo", "InputMedia"
      "Message or True", "EditMessageResult"
    ] |> Map.ofList
    
  let toPascalCase (str: string) =
    let mutable prev = '_'
    seq {
      for c in str do
        let c = if prev = '_' then Char.ToUpper c else c
        if c <> '_' then c
        prev <- c
    } |> Array.ofSeq |> String

  let toCamelCase (str: string) =
    let mutable prev = '0'
    seq {
      for c in str do
        let c = if prev = '_' then Char.ToUpper c else c
        if c <> '_' then c
        prev <- c
    } |> Array.ofSeq |> String
  
  let fixReservedKeywords (name: string) =
    if reservedKeywords |> Set.contains name then sprintf "``%s``" name
    else name

  let private convertTLTypeToFSharpTypeInner (typeName: string) (description: string) =
    let isArray = typeName.StartsWith("Array of ")
    let isArrayOfArray = typeName.StartsWith("Array of Array of ")

    let typeName = typeName.Replace("Array of Array of", "").Replace("Array of ", "").Trim(' ')

    typeMap
    |> Map.tryFind typeName
    |> Option.orElseWith (fun _ ->
      if description.Contains "File to send" then
        Some "InputFile"
      else
        None
    )
    |> Option.defaultValue typeName
    |> (
        if isArrayOfArray then sprintf "%s[][]"
        elif isArray  then sprintf "%s[]"
        else id
      )
  
  let convertTLTypeToFSharpType (typeString: string) (description: string) (optional: bool) =
    let typeString = convertTLTypeToFSharpTypeInner typeString description
    if optional then
      sprintf "%s option" typeString
    else
      typeString

[<RequireQualifiedAccess>]
module Code =
  type Code =
    {
      CurrentIndent: int
      StringBuilder: StringBuilder
    }
  
  let code =
    {
      CurrentIndent = 0
      StringBuilder = StringBuilder()
    }
  
  let private appendIndent code =
    let indent = Array.create (code.CurrentIndent * 2) ' ' |> System.String
    { code with StringBuilder = code.StringBuilder.Append(indent) }
  
  let private appendLine code =
    { code with StringBuilder = code.StringBuilder.AppendLine() }
  
  let increaseIndent code =
    { code with CurrentIndent = code.CurrentIndent + 1 }
  
  let decreaseIndent code =
    { code with CurrentIndent = code.CurrentIndent - 1 }
  
  let setIndent indent code =
    { code with CurrentIndent = indent }

  let print (line: string) code =
    { code with
        StringBuilder =
          code.StringBuilder.Append(line)
    }

  let printLine (line: string) code =
    { (code |> appendIndent) with
        StringBuilder =
          code.StringBuilder.Append(line)
    }

  let printNewLine (line: string) code =
    { (code |> appendLine |> appendIndent) with
        StringBuilder =
          code.StringBuilder.Append(line)
    }

  let printNewLineComment (comment: string) code =
    comment.Split("\n")
    |> Seq.fold (fun code comment ->
      printNewLine (sprintf "/// %s" comment) code
    ) code