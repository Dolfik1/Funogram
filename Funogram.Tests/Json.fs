module Funogram.Tests.Json

open Funogram
open Funogram.Types
open Xunit
open Extensions


let jsonUser = { Id = 1L; FirstName = "FirstName"; LastName = "LastName"; Username = "Username" }
let jsonUserString = "{id:1,first_name:\"FirstName\",last_name:\"LastName\",username:\"Username\"}"

[<Fact>]
let ``JSON deserializing`` () =
    Helpers.parseJson<Types.User>(jsonUserString) 
        |> shouldEqual jsonUser

[<Fact>]
let ``JSON serializing`` () =
    let o = Helpers.serializeObject(jsonUser) 
    shouldEqual o jsonUserString