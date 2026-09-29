module Funogram.Tests.Deserializer

open System.IO
open System.Text

open Funogram.Tests.Helpers
open Xunit

open Funogram
open Funogram.Telegram.Types

[<Fact>]
let ``Deserializing a deeply nested update fails fast instead of hanging``(): unit =
    let brokenUpdate = """{
    "ok": true,
    "result": [
        {
            "update_id": 1,
            "message": {
                "message_id": 1,
                "from": {
                    "id": 1,
                    "is_bot": false,
                    "first_name": "1",
                    "last_name": "1",
                    "username": "1"
                },
                "chat": {
                    "id": -1,
                    "title": "1",
                    "username": "1",
                    "type": "supergroup"
                },
                "date": 1707128434,
                "message_thread_id": 1,
                "reply_to_message": {
                    "message_id": 1,
                    "from": {
                        "id": 1,
                        "is_bot": false,
                        "first_name": "2",
                        "username": "2",
                        "is_premium": true
                    },
                    "chat": {
                        "id": -1,
                        "title": "1",
                        "username": "1",
                        "type": "supergroup"
                    },
                    "date": 1707117257,
                    "message_thread_id": 1,
                    "quote": {
                        "text": "Ins\\",
                        "position": 9,
                        "is_manual": true
                    }
                }
            }
        }
    ]
}"""
    let input = Encoding.UTF8.GetBytes brokenUpdate
    use stream = new MemoryStream(input)
    match parseJsonStream<Update[]> stream with
    | Error e -> Assert.True(false, e.ToString())
    | Ok result ->

    let update = Assert.Single result
    match update.Message with
    | None -> Assert.True(false, "No message")
    | Some message -> Assert.Equal(1L, message.MessageId)


let private parseUpdates (json: string) =
    use stream = new MemoryStream(Encoding.UTF8.GetBytes json)
    match parseJsonStream<Update[]> stream with
    | Ok result -> result
    | Error e -> failwithf "Expected Ok, got %A" e

[<Fact>]
let ``Broken update falls back to empty update and keeps the rest of the batch``(): unit =
    let result = parseUpdates """{"ok": true, "result": [
        {"update_id": 1, "message": {"message_id": 1, "date": "broken", "chat": {"id": 1, "type": "private"}}},
        {"update_id": 2, "message": {"message_id": 2, "date": 1707128500, "chat": {"id": 1, "type": "private"}}}
    ]}"""

    Assert.Equal(2, result.Length)
    Assert.Equal(1L, result[0].UpdateId)
    Assert.True(result[0].Message.IsNone)
    Assert.Equal(2L, result[1].UpdateId)
    Assert.Equal(Some 2L, result[1].Message |> Option.map _.MessageId)

[<Fact>]
let ``Broken update without update_id falls back to zero id``(): unit =
    let update = parseUpdates """{"ok": true, "result": [{"message": "broken"}]}""" |> Assert.Single
    Assert.Equal(0L, update.UpdateId)
    Assert.True(update.Message.IsNone)