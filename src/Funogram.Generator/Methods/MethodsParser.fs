[<Microsoft.FSharp.Core.RequireQualifiedAccess>]
module Funogram.Generator.Methods.MethodsParser

open System
open System.Diagnostics
open System.IO
open System.Text.Json
open System.Text.RegularExpressions
open FSharp.Data
open Funogram.Generator
open Funogram.Generator.Methods.Types

type ParseConfig =
  {
    Document: HtmlDocument
    RemapMethods: ApiType[]
    ParseResultPath: string option
  }

let mkParser (document: HtmlDocument) =
  {
    Document = document
    RemapMethods = Array.empty
    ParseResultPath = None
  }

let withResultPath resultPath config =
  { config with ParseResultPath = Some resultPath }

let loadRemapData remapPath config =
  if File.Exists remapPath then
    let serializerOptions = Helpers.getJsonSerializerOptions ()
    try
      use file = File.OpenRead remapPath
      let result = JsonSerializer.Deserialize(file, serializerOptions)
      { config with RemapMethods = result }
    with
    | e ->
      printfn "ERR: Could not deserialize file! %A" e
      config
  else
    printfn "WARN: Remap file not found at path %s" remapPath
    config

let private returnTypeRegexes =
  let rxs =
    [|
      // Returns the list of gifts that can be sent by the bot to users. Requires no parameters. Returns a Gifts object
      
      // invite links
      Regex("Returns the new invite link as a ([A|a]rray of \w+|\w+)\s")
      Regex("Returns the new invite link as ([A|a]rray of \w+|\w+)\s")
      Regex("Returns the edited invite link as a ([A|a]rray of \w+|\w+)\s")
      Regex("Returns the revoked invite link as ([A|a]rray of \w+|\w+)\s")
      Regex("Returns the created invoice link as ([A|a]rray of \w+|\w+)\s.")

      Regex("On success, a ([A|a]rray of \w+|\w+)\s")
      Regex("On success, an ([A|a]rray of \w+|\w+)\s")
      Regex("An ([A|a]rray of \w+|\w+) objects is returned")
      Regex("[R|r]eturns basic information about the bot in form of a ([A|a]rray of \w+|\w+)")
      Regex("[R|r]eturns information about the created topic as a ([A|a]rray of \w+|\w+)")

      Regex("([A|a]rray of \w+|\w+) is returned, otherwise ([A|a]rray of \w+|\w+)")

      Regex("[R|r]eturns a ([A|a]rray of \w+|\w+)\s")
      Regex("[R|r]eturns the uploaded (\w+)\s")
      Regex("[R|r]eturns the ([A|a]rray of \w+|\w+)\s")
      Regex("[R|r]eturns an ([A|a]rray of \w+|\w+)\s")
      Regex("[R|r]eturns ([A|a]rray of \w+|\w+)\s")

      
      Regex("\s([A|a]rray of \w+|\w+)\sis returned")
    |]
  
  // Sometimes they have the following format in documentation:
  // Returns the list of gifts blablabla. Returns a Gifts object.
  // so, let's pass first "Returns" if any
  let prefixedRxs = rxs |> Array.map(fun r -> Regex($"Returns the .+\. {r}"))  
  Array.append prefixedRxs rxs

let private parseReturnType (str: string) =
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

let private isMethodSection (node: HtmlNode) =
  if node.Name() = "h4" then
    let text = Helpers.directInnerText node
    let onlyLetters = text |> Seq.forall Char.IsLetter
    onlyLetters && text.Length > 0 && Char.IsLower text[0]
  else
    false

let inline private tryFindField (elements: HtmlNode list) (m: Map<string, int>) name =
  m |> Map.tryFind name |> Option.map (fun i -> Helpers.innerText elements[i])

let inline private defaultFieldValue v =
  match v with
  | Some v -> v
  | None ->
    printfn "WARN: Could not read field value!"
    ""

let private parseFields (table: HtmlNode) =
  let columns = table.CssSelect("thead > tr > th")
  let firstHeaderColumnName = columns |> List.tryHead |> Option.map Helpers.directInnerText

  match firstHeaderColumnName with
  | Some "Field" -> Array.empty
  | _ ->
    let columns = 
      columns
      |> Seq.mapi (fun i n -> Helpers.directInnerText n, i)
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
        OriginalName = parameter
        ConvertedName = parameter |> Helpers.toPascalCase
        Description = description
        OriginalFieldType = tp
        ConvertedFieldType = Helpers.convertTLTypeToFSharpType tp description false
        Optional =
          match required with
          | "Yes" -> Some false
          | "Optional" -> Some true
          | _ ->
            printfn "WARN: Unknown `required` field value: %A (count %A)" required (node.Name())
            Some false
      }
    ) |> Array.ofSeq

let private remap (remapTypes: ApiType[]) (types: ApiType[]) =
  types
  |> Array.map (fun tp ->
      remapTypes
      |> Array.fold (fun tp remapTp ->
        let matched =
          (String.IsNullOrEmpty remapTp.OriginalName || Helpers.compareWildcard remapTp.OriginalName tp.OriginalName)
          && (String.IsNullOrEmpty remapTp.Description || Helpers.compareWildcard remapTp.Description tp.Description)
        
        if matched then
            let fields =
              tp.Fields
              |> Array.map (fun field ->
                remapTp.Fields
                |> Array.fold (fun (field: ApiTypeField) remapField ->
                  let matched =
                    (String.IsNullOrEmpty remapField.OriginalName || Helpers.compareWildcard remapField.OriginalName field.OriginalName)
                    && (String.IsNullOrEmpty remapField.Description || Helpers.compareWildcard remapField.Description field.Description)
                    && (String.IsNullOrEmpty remapField.OriginalFieldType || Helpers.compareWildcard remapField.OriginalFieldType field.OriginalFieldType)
                  
                  if matched then
                    { field with
                        ConvertedName = remapField.ConvertedName |> Option.ofObj |> Option.defaultValue field.ConvertedName
                        ConvertedFieldType = remapField.ConvertedFieldType |> Option.ofObj |> Option.defaultValue field.ConvertedFieldType
                        Optional = remapField.Optional |> Option.orElse field.Optional }
                  else
                    field
                ) field
              )
            { tp with Fields = fields }
        else
          tp
      ) tp
  )

let parse (config: ParseConfig) =
  let sw = Stopwatch()
  sw.Start()
  printfn "Parsing page for methods..."
    
  // getting table with notification types
  let content = config.Document.CssSelect("#dev_page_content").Head

  let descendants = content.Descendants() 

  let mutable currentMethodNodeIndex = -1
  let mutable currentApiType = None

  let apiMethods =
    [|
      for node in descendants do
        let methodStart = isMethodSection node
        if methodStart then
          currentMethodNodeIndex <- 0
          let methodName = Helpers.directInnerText node
          printfn "Processing method %s" methodName

          match currentApiType with
          | Some tp -> yield tp
          | None -> ()

          currentApiType <-
            {
              OriginalName = methodName
              ConvertedName = methodName |> Helpers.toPascalCase
              Description = ""
              Fields = Array.empty
              OriginalReturnType = ""
              ConvertedReturnType = ""
            } |> Some

        elif currentMethodNodeIndex = 0 && node.Name() = "p" then
          currentMethodNodeIndex <- 1
          let desc = Helpers.innerText node
          match currentApiType with
          | Some tp ->
            let returnType = parseReturnType desc
            currentApiType <- 
              { tp with 
                  Description = desc
                  OriginalReturnType = returnType
                  ConvertedReturnType = Helpers.convertTLTypeToFSharpType returnType "" false
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
    |]

  sw.Stop()
  printfn "Parsing methods table done in %ims! " sw.ElapsedMilliseconds

  printfn "Remapping methods..."
  sw.Restart()
  let apiMethods = apiMethods |> remap config.RemapMethods
  sw.Stop()
  printfn "Methods are remapped in %ims" sw.ElapsedMilliseconds
  
  match config.ParseResultPath with
  | Some parseResultPath ->
    let dir = Path.GetDirectoryName parseResultPath
    if Directory.Exists dir |> not then
      printfn "Creating output directory..."
      Directory.CreateDirectory dir |> ignore

    printfn "Writing types to %s" parseResultPath
    let serializerOptions = Helpers.getJsonSerializerOptions ()
    let serializedTypes = JsonSerializer.Serialize(apiMethods, serializerOptions)
    File.WriteAllText(parseResultPath, serializedTypes)
    
  | None ->
    ()
  
  apiMethods
    
  