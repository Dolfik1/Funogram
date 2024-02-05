module Funogram.Tests.Deserializer

open System.IO
open System.Text

open Xunit

open Funogram
open Funogram.Telegram.Types

[<Fact>]
let ``Deserializer should work``(): unit =
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
    let result = Tools.parseJsonStreamApiResponse<Update[]> stream
    Assert.True(Result.isOk result)
