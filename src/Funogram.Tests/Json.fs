module Funogram.Tests.Json

open System
open System.Collections
open System.Collections.Generic
open System.IO
open System.Text
open Funogram
open Funogram.Types
open Xunit
open Extensions

[<Fact>]
let ``JSON deserializing MessageEntity`` () =
    let a = Tools.parseJson<MessageEntity>(Constants.jsonTestObjResultString)
    
    match a with
        | Ok r -> shouldEqual r Constants.jsonTestObj
        | Error e -> failwith e.Description

[<Fact>]
let ``JSON serializing MessageEntity``() =
    Constants.jsonTestObj
    |> Tools.toJsonString
    |> shouldEqual Constants.jsonTestObjString

[<Fact>]
let ``JSON deserializing User``() =
    Constants.jsonTestObjUserResultString
    |> Tools.parseJson<User>
    |> function
    | Ok result -> shouldEqual result Constants.jsonTestObjUser
    | Error error -> failwith error.Description

[<Fact>]
let ``JSON deserializing EditMessageResult 1``() =
    Constants.jsonTestEditResult1ApiString
    |> Tools.parseJson<EditMessageResult>
    |> function
    | Ok result -> shouldEqual result Constants.jsonTestEditResult1
    | Error error -> failwith error.Description

[<Fact>]
let ``JSON deserializing EditMessageResult 2`` () =
    Constants.jsonTestEditResult2ApiString
    |> Tools.parseJson<EditMessageResult>
    |> function
    | Ok result -> shouldEqual result Constants.jsonTestEditResult2
    | Error error -> failwith error.Description

[<Fact>]
let ``JSON deserializing EditMessageResult 3`` () =
    Constants.jsonTestEditResult3ApiString
    |> Tools.parseJson<EditMessageResult>
    |> ignore

[<Fact>]
let ``JSON serializing EditMessageResult 1`` () =
    Constants.jsonTestEditResult1
    |> Tools.toJsonString
    |> shouldEqual Constants.jsonTestEditResult1String

[<Fact>]
let ``JSON serializing EditMessageResult 2`` () =
    Constants.jsonTestEditResult2
    |> Tools.toJsonString
    |> shouldEqual Constants.jsonTestEditResult2String

[<Fact>]
let ``JSON deserializing MaskPosition`` () =
    Constants.jsonTestMaskPositionResult
    |> Tools.parseJson<MaskPosition>
    |> function
    | Ok result -> shouldEqual result Constants.testMaskPosition
    | Error error -> failwith error.Description

module UnixDateTimeConverterTests =
    open Newtonsoft.Json
    
    module Generators =
        type DateTimeWriteTestValue = {
            Time: obj
            UnixTime: int64
        }
        
        type DateTimeWriteTest() =
            interface IEnumerable<obj array> with
                member __.GetEnumerator() =
                    let time = DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc)
                    let seconds = 946684800L
                    let s =
                        seq {
                            yield { Time = box time; UnixTime = seconds }
                            yield { Time = box (Some time); UnixTime = seconds }
                            yield { Time = box (time.ToLocalTime()); UnixTime = seconds }
                        }
                        |> Seq.map (fun i -> [|box i|])
                    s.GetEnumerator()
                    
            interface IEnumerable with
                member __.GetEnumerator() =
                    (__ :> IEnumerable<_>).GetEnumerator() :> IEnumerator
                    
    [<Theory>]
    [<ClassData(typeof<Generators.DateTimeWriteTest>)>]
    let ``UnixDateTimeConverter WriteJson DateTime as option, nullable and just DateTime`` (data: Generators.DateTimeWriteTestValue) =
        let converter = new Funogram.JsonHelpers.UnixDateTimeConverter()
        let builder = new StringBuilder()
        converter.WriteJson(new JsonTextWriter(new StringWriter(builder)), data.Time, new JsonSerializer())
        
        shouldEqual data.UnixTime (int64 <| builder.ToString())
        
    let readJson<'T> value =
        let converter = new Funogram.JsonHelpers.UnixDateTimeConverter()
        let reader = new JsonTextReader(new StringReader(value))
        reader.Read() |> ignore
        converter.ReadJson(reader, typeof<'T>, value, new JsonSerializer())
        :?> 'T
        
    [<Fact>]
    let ``UnixDateTimeConverter ReadJson reads DateTime as option, nullable and just DateTime`` () =
        let expected = DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc)
        let timestamp = string 946684800L
        let dateTime = readJson<DateTime> timestamp
        let dateTimeOption = readJson<DateTime option> timestamp
        
        shouldEqual expected dateTime
        shouldEqual expected (Option.get dateTimeOption)