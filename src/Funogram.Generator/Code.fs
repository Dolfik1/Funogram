[<RequireQualifiedAccess>]
module Funogram.Generator.Code

open System
open System.Text

type Code =
  {
    CurrentIndent: int
    StringBuilder: StringBuilder
  }

let init () =
  {
    CurrentIndent = 0
    StringBuilder = StringBuilder()
  }

let private appendIndent code =
  let indent = Array.create (code.CurrentIndent * 2) ' ' |> System.String
  { code with StringBuilder = code.StringBuilder.Append(indent) }

let private appendLine code =
  { code with StringBuilder = code.StringBuilder.Append("\n") }

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

let printNewLineFormatted (line: string, args: Object[]) code =
  { (code |> appendLine |> appendIndent) with
      StringBuilder =
        code.StringBuilder.AppendFormat(line, args)
  }

let printNewLineComment (comment: string) code =
  comment.Split("\n")
  |> Seq.fold (fun code comment ->
    printNewLine (sprintf "/// %s" comment) code
  ) code