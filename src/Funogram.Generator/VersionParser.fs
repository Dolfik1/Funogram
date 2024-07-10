[<Microsoft.FSharp.Core.RequireQualifiedAccess>]
module Funogram.Generator.VersionParser

open System
open System.IO
open System.Xml
open System.Xml.Linq
open FSharp.Data

type ParseConfig =
  {
    Document: HtmlDocument
    BuildPropsPath: string option
    ReadmePath: string option
  }
  
let mkParser (html: HtmlDocument) =
  {
    Document = html
    BuildPropsPath = None
    ReadmePath = None
  }

let withBuildPropsPath path config =
  { config with BuildPropsPath = Some path }

let withReadmePath path config =
  { config with ReadmePath = Some path }

let private parseVersion (text: string) =
  try
    Some (text.Split('.') |> Array.map Int32.Parse)
  with
  | _ -> None

let private updateReadme (apiVersion: int[]) (path: string) =
  let version = apiVersion |> Seq.map string |> String.concat "."
  let lines = File.ReadAllLines(path)
  match lines |> Seq.tryFindIndex (_.StartsWith("[![NuGet](https://img.shields.io/badge/Bot")) with
  | Some badgeIndex ->
    lines[badgeIndex] <- $"[![NuGet](https://img.shields.io/badge/Bot%%20API-{version}-blue?logo=telegram)](https://www.nuget.org/packages/Funogram.Telegram/)"
    File.WriteAllLines(path, lines)
  | None ->
    printfn "Unable to find line with version badge."

let parseAndWrite (config: ParseConfig) =
  let content = config.Document.CssSelect("#dev_page_content > p > strong") |> Seq.head
  let version = content.InnerText() |> String.filter (fun x -> x = '.' || Char.IsDigit(x))
  
  match parseVersion version with
  | Some newApiVersion ->
    match config.BuildPropsPath with
    | Some buildPropsPath ->
      use fs = File.Open(buildPropsPath, FileMode.Open)
      let doc = XDocument.Load(fs)
      fs.Close()
      let versionNode = doc.Root.Element("PropertyGroup").Element("Version")
      match parseVersion versionNode.Value with
      | Some currentVersion ->
        let newVersion =
          if currentVersion[0] <> newApiVersion[0] || currentVersion[1] <> newApiVersion[1] then
            let zeros = Array.zeroCreate (4 - newApiVersion.Length)
            Array.append newApiVersion zeros |> Seq.map string |> String.concat "."
          else
            currentVersion[currentVersion.Length - 1] <- currentVersion[currentVersion.Length - 1] + 1
            currentVersion |> Seq.map string |> String.concat "."
        versionNode.Value <- newVersion
        printfn $"Updating {buildPropsPath} with new version {newVersion}"
        doc.Save(buildPropsPath)
        config.ReadmePath |> Option.iter (updateReadme newApiVersion)
      | None -> ()
    | None ->
      printfn "WARN: Version parsed, but not used, because Build.Props path is not set!"
    ()
  | None ->
    printfn $"WARN: Unable to parse API version: {version} (from element with content: {content})"
  
  
  

