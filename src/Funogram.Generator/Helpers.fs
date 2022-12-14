module Funogram.Generator.Helpers

open System
open System.IO.Enumeration
open System.Text.Json
open System.Text.Json.Serialization

open FSharp.Data

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
      let c = if prev = '_' then Char.ToUpper c else if prev = '0' then Char.ToLower c else c
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

let getJsonSerializerOptions () =
  let serializerOptions = JsonSerializerOptions()
  serializerOptions.Converters.Add(JsonFSharpConverter(allowNullFields = true))
  serializerOptions.WriteIndented <- true
  serializerOptions

let compareWildcard (expression: string) (value: string) =
  FileSystemName.MatchesSimpleExpression(expression, value, false)

let private normalize (text: string) =
  text.ReplaceLineEndings("\n")

let directInnerText (node: HtmlNode) =
  normalize <| node.DirectInnerText()

let innerText (node: HtmlNode) =
  normalize <| node.InnerText()
