namespace Funogram.Tests

open System.Text
open Funogram
open Funogram.Telegram.Bot
open Funogram.Types

module internal Helpers =
  let inline toJsonString<'a> (o: 'a) = Tools.toJsonString Config.defaultConfig o
  let parseJson<'a> (str: string): Result<'a, ApiError> = Encoding.UTF8.GetBytes str |> Tools.parseJsonResponseUtf8<'a> Config.defaultConfig 200
  
  let inline parseJsonStream<'a> stream = Tools.parseJsonResponseUtf8Stream<'a> Config.defaultConfig 200 stream