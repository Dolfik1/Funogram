[<Microsoft.FSharp.Core.RequireQualifiedAccess>]
module Funogram.Generator.Types.TypesParser

open System
open System.Diagnostics
open System.IO
open System.Text.Json
open FSharp.Data
open Funogram.Generator
open Funogram.Generator.Types.Types

type ParseConfig =
  {
    Document: HtmlDocument
    RemapTypes: ApiType[]
    ParseResultPath: string option
  }

let mkParser (document: HtmlDocument) =
  {
    Document = document
    RemapTypes = Array.empty
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
      { config with RemapTypes = result }
    with
    | e ->
      printfn "ERR: Could not deserialize file! %A" e
      config
  else
    printfn "WARN: Remap file not found at path %s" remapPath
    config

let private splitCaseNameAndType (typeName: string) (nameAndType: string) =
  let typeName =
    match typeName with
    | "InputMessageContent" -> "Input"
    | _ -> typeName

  try nameAndType.Substring(typeName.Length) with | _ -> "ERROR!"

let private isValidTypeNode (typeNodeInfo: ApiTypeNodeInfo) =
  let name = Helpers.innerText typeNodeInfo.TypeName
  Char.IsUpper name.[0] && (name.Replace(" ", "").Length = name.Length)

let private setConvertedFieldType (field: ApiTypeField) =
  { field with ConvertedFieldType = Helpers.convertTLTypeToFSharpType field.OriginalFieldType field.Description false }

let private parseApiTypeFields apiTypeName (node: HtmlNode) =
  node.CssSelect("tr")
  |> Seq.skip 1 // skip the header
  |> Seq.map (fun n -> n.Elements()) 
  |> Seq.filter (fun e -> e.Length = 3)
  |> Seq.map (fun elements ->
    let desc = Helpers.innerText elements.[2]
    let optionalIndex = desc.IndexOf("Optional. ") 
    let trimmedDesc = if optionalIndex >= 0 then desc.Substring(10) else desc
    {
      OriginalName = Helpers.innerText elements.[0]
      ConvertedName = elements.[0] |> Helpers.innerText |> Helpers.toPascalCase
      Description = trimmedDesc
      OriginalFieldType = Helpers.innerText elements.[1]
      ConvertedFieldType = ""
      Optional = Some (optionalIndex >= 0)
    } |> setConvertedFieldType
  )
  |> Array.ofSeq

let private parseApiTypeCases (typeName: string) (node: HtmlNode) =
  node.CssSelect("li")
  |> Seq.map (fun n ->
    let nameAndType = Helpers.innerText n
    {
      Name = splitCaseNameAndType typeName nameAndType
      CaseType = nameAndType
    }
  )
  |> Array.ofSeq

let private remap (remapTypes: ApiType[]) (types: ApiType[]) =
  types
  |> Array.map (fun tp ->
      remapTypes
      |> Array.fold (fun tp remapTp ->
        let matched =
          (String.IsNullOrEmpty remapTp.Name || Helpers.compareWildcard remapTp.Name tp.Name)
          && (String.IsNullOrEmpty remapTp.Description || Helpers.compareWildcard remapTp.Description tp.Description)
        
        if matched then
          match tp.Kind, remapTp.Kind with
          | ApiTypeKind.Stub, ApiTypeKind.Stub -> tp
          | ApiTypeKind.Fields fields, ApiTypeKind.Fields remapFields ->
            let fields =
              fields
              |> Array.map (fun field ->
                remapFields
                |> Array.fold (fun field remapField ->
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
            
            // append manually added fields (for example: deprecated fields)
            let fields =
              if remapTp.Name = tp.Name then
                let manuallyAddedFields = 
                  remapFields
                  |> Array.filter(fun x ->
                    if String.IsNullOrEmpty x.OriginalName then
                      false
                    else
                      fields |> Array.forall (fun f -> f.OriginalName <> x.OriginalName)
                  )
                manuallyAddedFields |> Array.append fields
              else
                fields

            { tp with Kind = ApiTypeKind.Fields fields }
            
          | ApiTypeKind.Cases cases, ApiTypeKind.Cases remapCases -> tp
          | _ -> tp
        else
          tp
      ) tp
  )

let parse (config: ParseConfig) =
  let sw = Stopwatch()
  sw.Start()
  printfn "Parsing page..."
  
  // getting table with types
  let nodes = config.Document.CssSelect("#dev_page_content").Head.Elements()

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
  printfn "Loaded %i types in %i ms!" typesNodes.Length sw.ElapsedMilliseconds

  printfn "Processing types..."
  
  sw.Reset()
  sw.Start()

  let types =
    typesNodes
    |> Array.map (fun node ->
      let typeName = Helpers.innerText node.TypeName
      printfn "Processing type %s" typeName
      {
        Name = typeName
        Description = 
          node.TypeDesc 
          |> Seq.map Helpers.innerText
          |> String.concat "\r\n"

        Kind = 
          match node.TypeFields, node.TypeCases with
          | Some fields, _ -> 
            parseApiTypeFields typeName fields
            |> ApiTypeKind.Fields

          | _, Some cases -> 
            parseApiTypeCases typeName cases
            |> ApiTypeKind.Cases

          | _ -> ApiTypeKind.Stub

      }
    )

  sw.Stop()
  
  printfn "Types are read successfully in %i ms!" sw.ElapsedMilliseconds

  let types = types |> remap config.RemapTypes
  
  match config.ParseResultPath with
  | Some parseResultPath ->
    let dir = Path.GetDirectoryName parseResultPath
    if Directory.Exists dir |> not then
      printfn "Creating output directory..."
      Directory.CreateDirectory dir |> ignore

    printfn "Writing types to %s" parseResultPath
    let serializerOptions = Helpers.getJsonSerializerOptions ()
    let serializedTypes = JsonSerializer.Serialize(types, serializerOptions)
    File.WriteAllText(parseResultPath, serializedTypes)
    
  | None ->
    ()
    
  types