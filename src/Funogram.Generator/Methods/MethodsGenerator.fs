[<Microsoft.FSharp.Core.RequireQualifiedAccess>]
module Funogram.Generator.MethodsGenerator

open System.Diagnostics
open System.IO
open Funogram.Generator.Methods.Types

type GeneratorConfig =
  {
    Methods: ApiType[]
    OutputPath: string
  }

let mkGenerator path methods =
  {
    Methods = methods
    OutputPath = path
  }

let private createHeaderCode () =
  Code.init ()
  |> Code.print "[<Microsoft.FSharp.Core.RequireQualifiedAccess>]"
  |> Code.printNewLine "module Funogram.Telegram.Req"
  |> Code.printNewLine """
open Funogram.Types
open Types
open System
    """

let generateTypeRecord apiType code =
  let typeName = apiType.ConvertedName

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
      |> Code.printNewLine (sprintf "%s: %s" tp.ConvertedName tp.VisibleFieldType)
    ) code
    |> Code.setIndent 1
    |> Code.printNewLine "}"
  else
    code

let sortFields fields = fields |> Array.sortBy (fun x -> x.Optional)

let generateMakeMethodSignature apiType (fields: ApiTypeField[]) convertFn code =
  let code =
    code
    |> Code.setIndent 1
    |> Code.printNewLine "static member Make("
    
  fields
  |> Seq.fold (fun code tp ->
    let o = if tp.IsOptional then "?" else ""
    let c = if fields.[0] <> tp then ", " else ""

    let argName = Helpers.toCamelCase tp.OriginalName |> Helpers.fixReservedKeywords
    let argType = convertFn tp

    code
    |> Code.print (sprintf "%s%s%s: %s" c o argName argType)
  ) code
  |> Code.print ") = "

let generateMakeMethodInvocation apiType (fields: ApiTypeField[]) convertFn code =
  let typeName = apiType.ConvertedName
  
  let code =
    code
    |> Code.setIndent 2
    |> Code.printNewLine (sprintf "%s.Make(" typeName)
    
  fields
  |> Seq.fold (fun code tp ->
    let c = if fields.[0] <> tp then ", " else ""

    let argNameOriginal = Helpers.toCamelCase tp.OriginalName |> Helpers.fixReservedKeywords
    let argName = convertFn tp argNameOriginal
    
    if tp.IsOptional then
      code
      |> Code.print (sprintf "%s?%s = %s" c argNameOriginal argName)
    else
      code
      |> Code.print (sprintf "%s%s" c argName)

  ) code
  |> Code.print ")"

let generateMakeMethodOverloads apiType (fields: ApiTypeField[]) code =
  if fields.Length = 0 || (fields.[0].ConvertedFieldType <> "ChatId") then
    code
  else
    let convertFieldSignatureType replaceType (tp: ApiTypeField) =
      if tp.ConvertedFieldType = "ChatId" then replaceType else tp.ConvertedFieldType

    let convertFieldInvocationType convert (tp: ApiTypeField) argName =
      if tp.ConvertedFieldType = "ChatId" then
        if tp.IsOptional then
          sprintf "(%s |> Option.map %s)" argName convert
        else
          sprintf "%s %s" convert argName
      else argName
    
    seq { "int64", "ChatId.Int"; "string", "ChatId.String" }
    |> Seq.fold (fun code (replaceType, convert) ->
      code
      |> generateMakeMethodSignature apiType fields (convertFieldSignatureType replaceType)
      |> generateMakeMethodInvocation apiType fields (convertFieldInvocationType convert)
    ) code

let generateMakeMethod apiType code =
  let typeName = apiType.ConvertedName
  if apiType.Fields.Length = 0 then
    code
    |> generateMakeMethodSignature apiType apiType.Fields (fun tp -> tp.ConvertedFieldType)
    |> Code.print (sprintf "%s()" typeName)
  else
    let fields = apiType.Fields |> sortFields
    
    let code =
      code
      |> generateMakeMethodSignature apiType fields (fun tp -> tp.ConvertedFieldType)
      |> Code.setIndent 2
      |> Code.printNewLine "{"
      |> Code.setIndent 3

    apiType.Fields
    |> Seq.fold (fun code tp ->
      code
      |> Code.printNewLine (sprintf "%s = %s" tp.ConvertedName (Helpers.toCamelCase tp.ConvertedName |> Helpers.fixReservedKeywords))
    ) code
    |> Code.setIndent 2
    |> Code.printNewLine "}"
    |> Code.setIndent 1
    
    |> generateMakeMethodOverloads apiType fields

let generateInterface apiType code =
  code
  |> Code.setIndent 1
  |> Code.printNewLine (sprintf "interface IRequestBase<%s> with" apiType.ConvertedReturnType)
  |> Code.setIndent 2
  |> Code.printNewLine (sprintf "member _.MethodName = \"%s\"" apiType.OriginalName)
  
let generate config =
  let dir = Path.GetDirectoryName config.OutputPath
  if Directory.Exists dir |> not then
    printfn "Creating output directory..."
    Directory.CreateDirectory dir |> ignore
  
  let sw = Stopwatch()
  sw.Start()
    
  let code =
    config.Methods
    |> Seq.fold (fun code apiType ->

      code
      |> generateTypeRecord apiType
      |> generateMakeMethod apiType
      |> generateInterface apiType
      |> Code.printNewLine ""
      
    ) (createHeaderCode ())

  
  
  
  sw.Stop()
  printfn "Code generated successfully in %i ms!" sw.ElapsedMilliseconds

  printfn "Writing source code to %s" config.OutputPath
  File.WriteAllText(config.OutputPath, code.StringBuilder.ToString())

  printfn "Done!"