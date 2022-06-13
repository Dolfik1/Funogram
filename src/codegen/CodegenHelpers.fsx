open System
open System.Text

module Helpers =
  let typeMap =
    [ 
      "Integer", "int64"
      "Boolean", "bool"
      "String", "string"
      "True", "bool"
      "Integer or String", "ChatId"
      "InputFile or String", "FileToSend"
      "Float", "float"
      "Float number", "float"
    ] |> Map.ofList
    
  let toPascalCase (str: string) =
    let mutable prev = '_'
    seq {
      for c in str do
        let c = if prev = '_' then Char.ToUpper c else c
        if c <> '_' then c
        prev <- c
    } |> Array.ofSeq |> String
  
  let private convertTLTypeToFSharpTypeInner (typeName: string) (description: string) =
    let isArray = typeName.StartsWith("Array of ")
    let isArrayOfArray = typeName.StartsWith("Array of Array of ")

    let typeName = typeName.Replace("Array of Array of", "").Replace("Array of ", "")

    typeMap
    |> Map.tryFind typeName
    |> Option.orElseWith (fun _ ->
      if description.Contains "File to send" then
        Some "FileToSend"
      else
        None
    )
    |> Option.defaultValue typeName
    |> (
        if isArray  then sprintf "%s[]" 
        elif isArrayOfArray then sprintf "%s[][]"
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
      printNewLine (sprintf "// %s" comment) code
    ) code