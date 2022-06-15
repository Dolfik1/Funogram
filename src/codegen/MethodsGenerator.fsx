#r "nuget: FSharp.Data, 4.1.1"
#load "CodegenHelpers.fsx"

open CodegenHelpers
open FSharp.Data
open System
open System.IO
open System.Text
open System.Text.RegularExpressions
open System.Diagnostics

type ApiTypeField =
  {
    Name: string
    Description: string
    FieldType: string
    Optional: bool
  }

type ApiType =
  {
    Name: string
    Description: string
    Fields: ApiTypeField[]
    ReturnType: string
  }

[<Literal>]
let ApiUri = "https://core.telegram.org/bots/api"

[<Literal>]
let OutputDir = "out"

[<Literal>]
let GeneratedMethodsTypesFileName = "RequestsTypes.fs"

[<Literal>]
let GeneratedMethodsSourceFileName = "Api.fs"

let typesJsonPath = Path.Combine(OutputDir, "methods.json")

let returnTypeRegexes =
  [|
    // invite links
    Regex("Returns the new invite link as ([A|a]rray of \w+|\w+)\s")
    Regex("Returns the edited invite link as a ([A|a]rray of \w+|\w+)\s")
    Regex("Returns the revoked invite link as ([A|a]rray of \w+|\w+)\s")

    Regex("On success, a ([A|a]rray of \w+|\w+)\s")
    Regex("On success, an ([A|a]rray of \w+|\w+)\s")
    Regex("An ([A|a]rray of \w+|\w+) objects is returned")
    Regex("[R|r]eturns basic information about the bot in form of a ([A|a]rray of \w+|\w+)")

    Regex("([A|a]rray of \w+|\w+) is returned, otherwise ([A|a]rray of \w+|\w+)")

    Regex("[R|r]eturns the uploaded (\w+)\s")
    Regex("[R|r]eturns the ([A|a]rray of \w+|\w+)\s")
    Regex("[R|r]eturns an ([A|a]rray of \w+|\w+)\s")
    Regex("[R|r]eturns a ([A|a]rray of \w+|\w+)\s")
    Regex("[R|r]eturns ([A|a]rray of \w+|\w+)\s")

    
    Regex("\s([A|a]rray of \w+|\w+)\sis returned")
  |]


let convertType (tp: ApiType) (field: ApiTypeField) =
  let typeString =
    match tp, field with
    | { Name = "sendChatAction" }, { Name = "action" } -> "ChatAction"
    | _, { Name = "parse_mode" } -> "ParseMode"
    | _ -> field.FieldType
  
  Helpers.convertTLTypeToFSharpType typeString field.Description

let parseReturnType (str: string) =
  let m =
    returnTypeRegexes
    |> Seq.choose (fun r -> 
      let m = r.Match(str)
      if m.Success && m.Groups.Count > 1 then
        Some m
      else
        None
    )
    |> Seq.tryHead

  match m with
  | Some m -> 
    let values = m.Groups |> Seq.skip 1 |> Seq.map (fun g -> g.Value)
    String.Join(" or ", values)

  | None ->
    printfn "WARN: Could not parse return type for desc: %A" str
    "PARSE ERROR"

let isMethodSection (node: HtmlNode) =
  if node.Name() = "h4" then
    let text = node.DirectInnerText()
    let onlyLetters = text |> Seq.forall (fun t -> Char.IsLetter t)
    onlyLetters && text.Length > 0 && Char.IsLower text.[0]
  else
    false

let inline tryFindField (elements: HtmlNode list) (m: Map<string, int>) name =
  m |> Map.tryFind name |> Option.map (fun i -> elements.[i].InnerText())

let inline defaultFieldValue v =
  match v with
  | Some v -> v
  | None ->
    printfn "WARN: Could not read field value!"
    ""

let parseFields (table: HtmlNode) =
  let columns = table.CssSelect("thead > tr > th")
  let firstHeaderColumnName = columns |> List.tryHead |> Option.map (fun x -> x.DirectInnerText())

  match firstHeaderColumnName with
  | Some "Field" -> Array.empty
  | _ ->
    let columns = 
      columns
      |> Seq.mapi (fun i n -> n.DirectInnerText(), i)
      |> Map.ofSeq


    table.CssSelect("tbody > tr")
    |> Seq.map (fun node ->
      let tryFindField = tryFindField (node.Elements()) columns
      let parameter = 
        tryFindField "Parameter" 
        |> Option.orElseWith (fun _ -> tryFindField "Field")
        |> defaultFieldValue
        

      let tp = tryFindField "Type" |> defaultFieldValue
      let description = tryFindField "Description" |> defaultFieldValue

      let required = 
        match tryFindField "Required" with
        | Some required -> required
        | None -> 
          if description.Contains("Optional") then "Optional" else "Yes"

      {
        Name = parameter
        FieldType = tp
        Description = description
        Optional =
          match required with
          | "Yes" -> false
          | "Optional" -> true
          | _ ->
            printfn "WARN: Unknown `required` field value: %A (count %A)" required (node.Name())
            false
      }
    ) |> Array.ofSeq

printfn "Fetching HTML page..."

let html = HtmlDocument.Load(ApiUri)

printfn "Parsing table..."
let sw = Stopwatch()
sw.Start()
// getting table with notification types
let content = html.CssSelect("#dev_page_content").Head

let descendants = content.Descendants() 

let mutable currentMethodNodeIndex = -1
let mutable currentApiType = None

let apiMethods =
  [
    for node in descendants do
      let methodStart = isMethodSection node
      if methodStart then
        currentMethodNodeIndex <- 0
        let methodName = node.DirectInnerText()
        printfn "Processing method %s" methodName

        match currentApiType with
        | Some tp -> yield tp
        | None -> ()

        currentApiType <-
          { Name = methodName; Description = ""; Fields = Array.empty; ReturnType = "" } |> Some

      elif currentMethodNodeIndex = 0 && node.Name() = "p" then
        currentMethodNodeIndex <- 1
        let desc = node.InnerText() 
        match currentApiType with
        | Some tp -> 
          currentApiType <- 
            { tp with 
                Description = desc
                ReturnType = parseReturnType desc
            } |> Some
        | None ->
          printfn "WARN: Unknown case!"

      elif currentMethodNodeIndex = 1 && node.Name() = "table" then
        currentMethodNodeIndex <- 2
        match currentApiType with
        | Some tp -> 
          currentApiType <- { tp with Fields = parseFields node } |> Some

        | None ->
          printfn "WARN: Unknown case!"

    match currentApiType with
    | Some tp -> yield tp
    | None -> ()
  ]

sw.Stop()
printfn "Parsing table done in %ims! Writing methods to %s" sw.ElapsedMilliseconds typesJsonPath

let serializerOptions = Json.JsonSerializerOptions()
serializerOptions.WriteIndented <- true

let serializedTypes = Json.JsonSerializer.Serialize(apiMethods, serializerOptions)
File.WriteAllText(typesJsonPath, serializedTypes)


let methodsTypesCodePath = Path.Combine(OutputDir, GeneratedMethodsTypesFileName)

printfn "Generating methods types source code..."
sw.Reset()
sw.Start()

let code = 
  Code.code
  |> Code.print "[<Microsoft.FSharp.Core.RequireQualifiedAccess>]"
  |> Code.printNewLine "module Funogram.Telegram.Req"
  |> Code.printNewLine 
   """
open Funogram.Types
open Types
open System
    """

let getTypeName apiType = Helpers.toPascalCase apiType.Name

let generateTypeRecord apiType code =
  let typeName = getTypeName apiType

  let typeStr =
    if apiType.Fields.Length = 0 then
      (sprintf "type %s() =" typeName)
    else
      (sprintf "type %s =" typeName)
  
  let code =
    code
    |> Code.setIndent 0
    |> Code.printNewLine typeStr
    |> Code.setIndent 1
  
  if apiType.Fields.Length > 0 then
    let code =
      code
      |> Code.printNewLine "{"
      |> Code.setIndent 2

    apiType.Fields
    |> Seq.fold (fun code tp ->
      code
      |> Code.printNewLine (sprintf "%s: %s" (Helpers.toPascalCase tp.Name) (convertType apiType tp tp.Optional))
    ) code
    |> Code.setIndent 1
    |> Code.printNewLine "}"
  else
    code

let sortFields fields = fields |> Array.sortBy (fun x -> x.Optional)

let generateMakeMethodSignature apiType (fields: ApiTypeField[]) convertTypeFn code =
  let code =
    code
    |> Code.setIndent 1
    |> Code.printNewLine "static member Make("
    
  fields
  |> Seq.fold (fun code tp ->
    let o = if tp.Optional then "?" else ""
    let c = if fields.[0] <> tp then ", " else ""

    let argName = Helpers.toCamelCase tp.Name |> Helpers.fixReservedKeywords
    let argType = convertTypeFn apiType tp false

    code
    |> Code.print (sprintf "%s%s%s: %s" c o argName argType)
  ) code
  |> Code.print ") = "

let generateMakeMethodInvocation apiType (fields: ApiTypeField[]) convertArgSet code =
  let typeName = getTypeName apiType
  
  let code =
    code
    |> Code.setIndent 2
    |> Code.printNewLine (sprintf "%s.Make(" typeName)
    
  fields
  |> Seq.fold (fun code tp ->
    let c = if fields.[0] <> tp then ", " else ""

    let argNameOriginal = Helpers.toCamelCase tp.Name |> Helpers.fixReservedKeywords
    let argName = argNameOriginal |> convertArgSet apiType tp tp.Optional
    if tp.Optional then
      code
      |> Code.print (sprintf "%s?%s = %s" c argNameOriginal argName)
    else
      code
      |> Code.print (sprintf "%s%s" c argName)

  ) code
  |> Code.print ")"

let generateMakeMethodOverloads apiType (fields: ApiTypeField[]) code =
  if fields.Length = 0 || (convertType apiType fields.[0] false <> "ChatId") then
    code
  else
    let convertTypeFn replaceTypeName apiType tp optional =
      let typeName = convertType apiType tp false
      if typeName = "ChatId" then replaceTypeName
      elif not optional then typeName
      else convertType apiType tp optional
      
    let convertArgSet convert apiType tp optional argName =
      let typeName = convertType apiType tp false
      if typeName = "ChatId" then
        if optional then
          sprintf "(%s |> Option.map %s)" argName convert
        else
          sprintf "%s %s" convert argName
      else argName
    
    
    seq { "int64", "ChatId.Int"; "string", "ChatId.String" }
    |> Seq.fold (fun code (replaceType, convert) ->
      code
      |> generateMakeMethodSignature apiType fields (convertTypeFn replaceType)
      |> generateMakeMethodInvocation apiType fields (convertArgSet convert)
    ) code

let generateMakeMethod apiType code =
  let typeName = getTypeName apiType
  if apiType.Fields.Length = 0 then
    code
    |> generateMakeMethodSignature apiType apiType.Fields convertType
    |> Code.print (sprintf "%s()" typeName)
  else
    let fields = apiType.Fields |> sortFields
    
    let code =
      code
      |> generateMakeMethodSignature apiType fields convertType
      |> Code.setIndent 2
      |> Code.printNewLine "{"
      |> Code.setIndent 3

    apiType.Fields
    |> Seq.fold (fun code tp ->
      code
      |> Code.printNewLine (sprintf "%s = %s" (Helpers.toPascalCase tp.Name) (Helpers.toCamelCase tp.Name |> Helpers.fixReservedKeywords))
    ) code
    |> Code.setIndent 2
    |> Code.printNewLine "}"
    |> Code.setIndent 1
    
    |> generateMakeMethodOverloads apiType fields

let generateInterface apiType code =
  code
  |> Code.setIndent 1
  |> Code.printNewLine (sprintf "interface IRequestBase<%s> with" (Helpers.convertTLTypeToFSharpType apiType.ReturnType "" false))
  |> Code.setIndent 2
  |> Code.printNewLine (sprintf "member _.MethodName = \"%s\"" apiType.Name)
  

let codeResult =
  apiMethods
  |> Seq.fold (fun code apiType ->

    code
    |> generateTypeRecord apiType
    |> generateMakeMethod apiType
    |> generateInterface apiType
    |> Code.printNewLine ""
    
  ) code

sw.Stop()
printfn "Methods types code generated successfully in %i ms!" sw.ElapsedMilliseconds

printfn "Writing source code to %s" methodsTypesCodePath

File.WriteAllText(methodsTypesCodePath, codeResult.StringBuilder.ToString())

printfn "Done!"

