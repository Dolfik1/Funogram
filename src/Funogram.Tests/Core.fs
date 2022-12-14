module Funogram.Tests.Core

open Funogram.Tests.Extensions
open Funogram.Telegram
open Types

open Xunit

let botUser = User.Create(0L, true, "TestBot", username = "testbot")

[<Fact>]
let ``Simple command test`` () =
  Bot.getTextForCommand botUser (Some "/cmd") 
  |> shouldEqual (Some "/cmd")

[<Fact>]
let ``Command test with args`` () =
  Bot.getTextForCommand botUser (Some "/cmd hello world 1 2 3") 
  |> shouldEqual (Some "/cmd hello world 1 2 3")
  
[<Fact>]
let ``Command test with username`` () =
  Bot.getTextForCommand botUser (Some "/cmd@testbot") 
  |> shouldEqual (Some "/cmd")
  
[<Fact>]
let ``Command test with two usernames`` () =
  Bot.getTextForCommand botUser (Some "/cmd@testbot @testbot") 
  |> shouldEqual (Some "/cmd @testbot")
  
[<Fact>]
let ``Command test with username and args`` () =
  Bot.getTextForCommand botUser (Some "/cmd@testbot hello world 1 2 3") 
  |> shouldEqual (Some "/cmd hello world 1 2 3")
  
[<Fact>]
let ``Command test invalid command`` () =
  Bot.getTextForCommand botUser (Some "/") 
  |> shouldEqual (Some "/")
  
[<Fact>]
let ``Command test with no command`` () =
  Bot.getTextForCommand botUser (Some "Hello, world!") 
  |> shouldEqual (Some "Hello, world!")