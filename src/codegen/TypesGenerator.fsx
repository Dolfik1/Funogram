#r "nuget: FSharp.Data, 4.1.1"
#r "nuget: System.Text.Json, 6.0.2"
#r "nuget: FSharp.SystemTextJson, 0.17.4"

#load "CodegenHelpers.fsx"

open FSharp.Data
open System
open System.IO
open System.Text.Json
open System.Text.Json.Serialization
open CodegenHelpers
open System.Diagnostics

type ApiTypeNodeInfo =
  {
    TypeName: HtmlNode
    TypeDesc: HtmlNode list
    TypeFields: HtmlNode option
    TypeCases: HtmlNode option
  }
  static member Create(typeName) = { TypeName = typeName; TypeDesc = []; TypeFields = None; TypeCases = None }

type ApiTypeField =
  {
    Name: string
    Description: string
    FieldType: string
    Optional: bool
  }

type ApiTypeCase =
  {
    Name: string
    CaseType: string
  }

type ApiTypeKind =
  | Stub
  | Cases of ApiTypeCase[]
  | Fields of ApiTypeField[]

type ApiType =
  {
    Name: string
    Description: string
    Kind: ApiTypeKind
  }

[<Literal>]
let ApiUri = "https://core.telegram.org/bots/api"

[<Literal>]
let OutputDir = "out"

[<Literal>]
let GeneratedTypesFileName = "Types.fs"

let ignoreTypes = 
  [ 
    "InputFile" 
    "Determining list of commands"
  ] |> Set.ofList

let splitCaseNameAndType (typeName: string) (nameAndType: string) =
  let typeName =
    match typeName with
    | "InputMessageContent" -> "Input"
    | _ -> typeName

  try nameAndType.Substring(typeName.Length) with | _ -> "ERROR!"

let parseApiTypeFields (node: HtmlNode) =
  node.CssSelect("tr")
  |> Seq.skip 1 // skip the header
  |> Seq.map (fun n -> n.Elements()) 
  |> Seq.filter (fun e -> e.Length = 3)
  |> Seq.map (fun elements ->
    let desc = elements.[2].InnerText()
    let optionalIndex = desc.IndexOf("Optional. ") 
    let trimmedDesc = if optionalIndex >= 0 then desc.Substring(10) else desc
    {
      Name = elements.[0].InnerText()
      Description = trimmedDesc
      FieldType = elements.[1].InnerText()
      Optional = optionalIndex >= 0
    }
  )
  |> Array.ofSeq

let parseApiTypeCases (typeName: string) (node: HtmlNode) =
  node.CssSelect("li")
  |> Seq.map (fun n ->
    let nameAndType = n.InnerText()
    {
      Name = splitCaseNameAndType typeName nameAndType
      CaseType = nameAndType
    }
  )
  |> Array.ofSeq

let isValidTypeNode (typeNodeInfo: ApiTypeNodeInfo) =
  let name = typeNodeInfo.TypeName.InnerText()
  Char.IsUpper name.[0] && (name.Replace(" ", "").Length = name.Length)

let convertType (tp: ApiType) (field: ApiTypeField) =
  let typeString =
    match tp, field with
    | { Name = "Chat" }, { Name = "type" }
    | { Name = "InlineQuery" }, { Name = "chat_type" } ->
      "ChatType"
     
    | _, { Name = "parse_mode" } ->
      "ParseMode"
    | _ -> field.FieldType
  
  Helpers.convertTLTypeToFSharpType typeString field.Description field.Optional
 
let generateFieldsBody tp (fields: ApiTypeField[]) code =
  let code =
    code
    |> Code.setIndent 1
    |> Code.printNewLine "{"
    |> Code.setIndent 2
    

  fields
  |> Seq.fold (fun code field ->
    code
    |> Code.printNewLineComment field.Description
    |> Code.printNewLine (sprintf "[<DataMember(Name = \"%s\")>]" field.Name)
    |> Code.printNewLine (sprintf "%s: %s" (Helpers.toPascalCase field.Name) (convertType tp field))
  ) code

  |> Code.setIndent 1
  |> Code.printNewLine "}"
 
let generateCasesBody (cases: ApiTypeCase[]) code =
  let code = code |> Code.setIndent 1

  cases
  |> Seq.fold (fun code case ->
    code
    |> Code.printNewLine (sprintf "| %s of %s" (Helpers.toPascalCase case.Name) case.CaseType)
  ) code

let attributesForType tp =
  match tp.Kind with
  | ApiTypeKind.Fields _ -> " [<CLIMutable>]"
  | _ -> ""

let generate () =

  if Directory.Exists OutputDir |> not then
    printfn "Creating output directory..."
    Directory.CreateDirectory OutputDir |> ignore

  printfn "Fetching HTML page..."
  let sw = Stopwatch()
  sw.Start()

  let html = HtmlDocument.Load(ApiUri)

  sw.Stop()
  printfn "Page fetched in %ims" sw.ElapsedMilliseconds

  sw.Reset()
  sw.Start()
  printfn "Parsing page..."
  // getting table with types
  let nodes = html.CssSelect("#dev_page_content").Head.Elements()

  let typesNodes =
    nodes
    |> Seq.fold (fun (nodes, currentTypeNodeInfo) node -> 
      
      match node.Name(), currentTypeNodeInfo with
      | "h4", Some currentTypeNodeInfo ->
        nodes @ [ currentTypeNodeInfo ], ApiTypeNodeInfo.Create node |> Some

      | "h4", None ->
        nodes, ApiTypeNodeInfo.Create node |> Some

      | "p", Some currentTypeNodeInfo ->
        nodes, Some { currentTypeNodeInfo with TypeDesc = currentTypeNodeInfo.TypeDesc @ [ node ] }

      | "table", Some currentTypeNodeInfo ->
        nodes, Some { currentTypeNodeInfo with TypeFields = Some node }

      | "ul", Some currentTypeNodeInfo ->
        nodes, Some { currentTypeNodeInfo with TypeCases = Some node }

      | _ ->
        nodes, currentTypeNodeInfo

    ) ([], None)
    |> (fun (nodes, tp) -> nodes @ [ match tp with | Some tp -> tp | _ -> () ])
    |> Seq.filter isValidTypeNode
    |> Array.ofSeq

  sw.Stop()
  printfn "Got %i types to process in %i ms!" typesNodes.Length sw.ElapsedMilliseconds

  printfn "Processing types..."
  
  sw.Reset()
  sw.Start()

  let types =
    typesNodes
    |> Array.map (fun node ->
      let typeName = node.TypeName.InnerText()
      printfn "Processing type %s" typeName
      {
        Name = typeName
        Description = 
          node.TypeDesc 
          |> Seq.map (fun x -> x.InnerText())
          |> String.concat "\r\n"

        Kind = 
          match node.TypeFields, node.TypeCases with
          | Some fields, _ -> 
            parseApiTypeFields fields
            |> ApiTypeKind.Fields

          | _, Some cases -> 
            parseApiTypeCases typeName cases
            |> ApiTypeKind.Cases

          | _ -> ApiTypeKind.Stub

      }
    )


  let typesJsonPath = Path.Combine(OutputDir, "types.json")

  sw.Stop()
  printfn "Types are read successfully in %i ms! Writing types to %s" sw.ElapsedMilliseconds typesJsonPath
  let serializerOptions = JsonSerializerOptions()
  serializerOptions.Converters.Add(JsonFSharpConverter())
  serializerOptions.WriteIndented <- true

  let serializedTypes = JsonSerializer.Serialize(types, serializerOptions)
  File.WriteAllText(typesJsonPath, serializedTypes)


  let typesCodePath = Path.Combine(OutputDir, GeneratedTypesFileName)

  printfn "Generating source code..."
  sw.Reset()
  sw.Start()

  let code = 
    Code.code
    |> Code.print "module Funogram.Telegram.Types"
    |> Code.printNewLine 
     """
  open System
  open System.IO
  open System.Runtime.Serialization
      """

    |> Code.printNewLine 
      """
  type ChatId = 
    | Int of int64
    | String of string
    
  type FileToSend = 
    | Url of Uri 
    | File of string * Stream
    | FileId of string

  type ChatType =
    | Private
    | Group
    | [<DataMember(Name = "supergroup")>] SuperGroup
    | Channel
    | Unknown


  /// Message text parsing mode
  type ParseMode = 
    /// Markdown parse syntax
    | Markdown
    /// Html parse syntax
    | HTML

  /// Type of action to broadcast
  type ChatAction =
    | Typing
    | UploadPhoto
    | RecordVideo
    | UploadVideo
    | RecordAudio
    | UploadAudio
    | UploadDocument
    | FindLocation
    | RecordVideoNote
    | UploadVideoNote

  type ChatMemberStatus =
    | Creator
    | Administrator
    | Member
    | Restricted
    | Left
    | Kicked
    | Unknown
      """
  let codeResult =
    types
    |> Seq.filter (fun tp -> ignoreTypes |> Set.contains tp.Name |> not)
    |> Seq.fold (fun code tp ->

      code
      |> Code.printNewLineComment tp.Description
      |> Code.printNewLine (sprintf "and%s %s =" (attributesForType tp) tp.Name)

      |> (fun code ->
        match tp.Kind with
        | Stub -> code |> Code.setIndent 1 |> Code.printNewLine "class end"
        | Cases cases -> generateCasesBody cases code
        | Fields fields -> generateFieldsBody tp fields code)

      |> Code.setIndent 0
      |> Code.printNewLine ""

    ) code
  
  sw.Stop()
  printfn "Code generated successfully in %i ms!" sw.ElapsedMilliseconds

  printfn "Writing source code to %s" typesCodePath
  File.WriteAllText(typesCodePath, codeResult.StringBuilder.ToString())

  printfn "Done!"

do generate ()