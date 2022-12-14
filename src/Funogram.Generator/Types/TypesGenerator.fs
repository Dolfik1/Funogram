[<Microsoft.FSharp.Core.RequireQualifiedAccess>]
module Funogram.Generator.TypesGenerator

open System.Diagnostics
open System.IO
open Funogram.Generator.Types.Types

type GeneratorConfig =
  {
    Types: ApiType[]
    OutputPath: string
  }

let private createHeaderCode () =
  Code.init ()
  |> Code.print "module Funogram.Telegram.Types"
  |> Code.printNewLine 
     """
open System
open System.IO
open System.Runtime.Serialization

type ChatId = 
  | Int of int64
  | String of string
    
type InputFile = 
  | Url of Uri 
  | File of string * Stream
  | FileId of string

type ChatType =
  | Private
  | Group
  | [<DataMember(Name = "supergroup")>] SuperGroup
  | Channel
  | Sender
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
  | ChooseSticker

type ChatMemberStatus =
  | Creator
  | Administrator
  | Member
  | Restricted
  | Left
  | Kicked
  | Unknown

type MaskPoint =
  | Forehead
  | Eyes
  | Mouth
  | Chin

type Markup = 
  | InlineKeyboardMarkup of InlineKeyboardMarkup 
  | ReplyKeyboardMarkup of ReplyKeyboardMarkup
  | ReplyKeyboardRemove of ReplyKeyboardRemove
  | ForceReply of ForceReply

/// If edited message is sent by the bot, used Message, otherwise Success.
and EditMessageResult = 
  /// Message sent by the bot
  | Message of Message
  /// Message sent via the bot or another...
  | Success of bool
"""

let mkGenerator path types =
  {
    Types = types
    OutputPath = path
  }

let private ignoreTypes = 
  [ 
    "InputFile" 
    "Determining list of commands"
  ] |> Set.ofList

let private generateFieldsBody tp (fields: ApiTypeField[]) code =
  let code =
    code
    |> Code.setIndent 1
    |> Code.printNewLine "{"
    |> Code.setIndent 2
    

  fields
  |> Seq.fold (fun code field ->
    code
    |> Code.printNewLineComment field.Description
    |> Code.printNewLine (sprintf "[<DataMember(Name = \"%s\")>]" field.OriginalName)
    |> Code.printNewLine (sprintf "%s: %s" field.ConvertedName field.VisibleFieldType)
  ) code

  |> Code.setIndent 1
  |> Code.printNewLine "}"
 
let private generateCasesBody (cases: ApiTypeCase[]) code =
  let code = code |> Code.setIndent 1

  cases
  |> Seq.fold (fun code case ->
    code
    |> Code.printNewLine (sprintf "| %s of %s" (Helpers.toPascalCase case.Name) case.CaseType)
  ) code

let private generateFieldsCreateMember tp (fields: ApiTypeField[]) code =
  let code =
    code
    |> Code.setIndent 1
    |> Code.printNewLine "static member Create("
    |> Code.setIndent 2

  let fields = fields |> Array.sortBy (fun x -> x.Optional)
  let firstField = fields |> Array.tryHead
  
  fields
  |> Seq.fold (fun code field ->
    let o = if field.IsOptional then "?" else ""
    let c = if field = firstField.Value then "" else ", "
    let propName = Helpers.toCamelCase field.OriginalName |> Helpers.fixReservedKeywords
    let fieldType = field.ConvertedFieldType
    code
    |> Code.print (sprintf "%s%s%s: %s" c o propName fieldType)
  ) code
  |> Code.print ") = "
  |> Code.setIndent 2
  |> Code.printNewLine "{"
    |> Code.setIndent 3
  |> (fun code ->
    fields
    |> Seq.fold (fun code field ->
      let propName = Helpers.toCamelCase field.OriginalName |> Helpers.fixReservedKeywords
      code |> Code.printNewLine (sprintf "%s = %s" field.ConvertedName propName)
    ) code
  )
  |> Code.setIndent 2
  |> Code.printNewLine "}"

let private attributesForType tp =
  match tp.Kind with
  | ApiTypeKind.Fields _ -> " [<CLIMutable>]"
  | _ -> ""

let generate config =
  let dir = Path.GetDirectoryName config.OutputPath
  if Directory.Exists dir |> not then
    printfn "Creating output directory..."
    Directory.CreateDirectory dir |> ignore
  
  let sw = Stopwatch()
  sw.Start()
  
  let code =
    config.Types
    |> Seq.filter (fun tp -> ignoreTypes |> Set.contains tp.Name |> not)
    |> Seq.fold (fun code tp ->

      code
      |> Code.printNewLineComment tp.Description
      |> Code.printNewLine (sprintf "and%s %s =" (attributesForType tp) tp.Name)

      |> (fun code ->
        match tp.Kind with
        | Stub -> code |> Code.setIndent 1 |> Code.printNewLine "new() = {}"
        | Cases cases -> generateCasesBody cases code
        | Fields fields -> code |> generateFieldsBody tp fields |> generateFieldsCreateMember tp fields)

      |> Code.setIndent 0
      |> Code.printNewLine ""

    ) (createHeaderCode ())
  
  
  sw.Stop()
  printfn "Code generated successfully in %i ms!" sw.ElapsedMilliseconds

  printfn "Writing source code to %s" config.OutputPath
  File.WriteAllText(config.OutputPath, code.StringBuilder.ToString())

  printfn "Done!"