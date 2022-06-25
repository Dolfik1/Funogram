[<Microsoft.FSharp.Core.RequireQualifiedAccess>]
module Funogram.Generator.Loader

open System.IO
open System.Net.Http
open FSharp.Data

type LoaderConfig =
  {
    Cache: bool
  }

let mkLoader () =
  {
    Cache = false
  }

let withCache cache config =
  { config with Cache = cache }
  
let rec loadAsync config =
  async {
    if config.Cache then
      let cachePath = Path.Combine(Constants.CacheDir, "index.html")
      if File.Exists cachePath then
        use fs = File.OpenRead(cachePath)
        return HtmlDocument.Load(fs)
      else
        let dir = Path.GetDirectoryName cachePath
        if Directory.Exists dir |> not then
          Directory.CreateDirectory dir |> ignore

        use client = new HttpClient()
        let! stream = client.GetStreamAsync(Constants.ApiUri) |> Async.AwaitTask
        
        
        let write = File.OpenWrite(cachePath)
        do! stream.CopyToAsync(write) |> Async.AwaitTask
        write.Close()
        write.Dispose()
        return! loadAsync config
    else
      return HtmlDocument.Load(Constants.ApiUri)
  }
  
