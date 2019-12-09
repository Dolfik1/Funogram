namespace Funogram.Tests
open System.Text
open Funogram

module internal Helpers =
    let inline toJsonString<'a> (o: 'a) = Tools.toJson o |> Encoding.UTF8.GetString
    let parseJson (str: string) = Encoding.UTF8.GetBytes str |> Tools.parseJson