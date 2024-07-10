module Funogram.Generator.Program

open System
open System.IO
open Argu
open Funogram.Generator.Methods
open Funogram.Generator.Types

type CliArguments =
  | [<AltCommandLine("-t")>] Types
  | [<AltCommandLine("-m")>] Methods
  | Cached

  interface IArgParserTemplate with
    member x.Usage =
      match x with
      | Types -> "Generate types"
      | Methods -> "Generate methods"
      | Cached -> "Take HTML page from local cache if possible"

let processAsync (args: CliArguments list) =
  async {
    let cached = args |> List.contains CliArguments.Cached
    let! html =
      Loader.mkLoader ()
      |> Loader.withCache cached
      |> Loader.loadAsync
    
    let hasTypes = args |> List.contains CliArguments.Types
    let hasMethods = args |> List.contains CliArguments.Methods
    
    let all = not hasTypes && not hasMethods
    let genTypes = hasTypes || all
    let genMethods = hasMethods || all

    if genTypes then    
      TypesParser.mkParser html
      |> TypesParser.withResultPath (Path.Combine(Constants.OutputDir, "types.json"))
      |> TypesParser.loadRemapData "./RemapTypes.json"
      |> TypesParser.parse
      |> TypesParser.mergeCustomFields "./CustomFields.json"
      
      |> TypesGenerator.mkGenerator (Path.Combine(Constants.CodeOutputDir, Constants.TypesFileName))
      |> TypesGenerator.generate

    if genMethods then
      MethodsParser.mkParser html
      |> MethodsParser.withResultPath (Path.Combine(Constants.OutputDir, "methods.json"))
      |> MethodsParser.loadRemapData "./RemapMethods.json"
      |> MethodsParser.parse
      
      |> MethodsGenerator.mkGenerator (Path.Combine(Constants.CodeOutputDir, Constants.MethodsFileName))
      |> MethodsGenerator.generate
    
    VersionParser.mkParser html
    |> VersionParser.withBuildPropsPath (Path.Combine(Constants.CodeOutputDir, Constants.BuildPropsFileName))
    |> VersionParser.withReadmePath (Path.Combine(Constants.RootDir, Constants.ReadmeFileName))
    |> VersionParser.parseAndWrite
  }

[<EntryPoint>]
let main argv =
  let parser = ArgumentParser.Create<CliArguments>()
  try
    let result = parser.ParseCommandLine(inputs = argv, raiseOnUsage = true)
    let result = result.GetAllResults()

    try
      processAsync result |> Async.RunSynchronously
    with
    | e ->
      printfn "%A" e
  with e ->
    printfn "%s" (parser.PrintUsage())
  0
