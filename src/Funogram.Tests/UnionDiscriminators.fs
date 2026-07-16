module Funogram.Tests.UnionDiscriminators

open Funogram.Telegram.Types
open Xunit
open Helpers

let private caseName (m: ChatMember) =
  match m with
  | ChatMember.Owner _ -> "Owner"
  | ChatMember.Administrator _ -> "Administrator"
  | ChatMember.Member _ -> "Member"
  | ChatMember.Restricted _ -> "Restricted"
  | ChatMember.Left _ -> "Left"
  | ChatMember.Banned _ -> "Banned"

let private user = """{"id":42,"is_bot":false,"first_name":"x"}"""

let private parseResult<'a> (payload: string) : Result<'a, Funogram.Types.ApiResponseError> =
  parseJson<'a> ("""{"ok":true,"result":""" + payload + "}")

[<Theory>]
[<InlineData("creator", """{"status":"creator","user":%USER%,"is_anonymous":false}""", "Owner")>]
[<InlineData("administrator", """{"status":"administrator","user":%USER%,"is_anonymous":false,"can_be_edited":false,"can_manage_chat":true,"can_delete_messages":true,"can_manage_video_chats":false,"can_restrict_members":true,"can_promote_members":false,"can_change_info":false,"can_invite_users":false,"can_post_stories":false,"can_edit_stories":false,"can_delete_stories":false}""", "Administrator")>]
[<InlineData("member", """{"status":"member","user":%USER%}""", "Member")>]
[<InlineData("restricted", """{"status":"restricted","user":%USER%,"is_member":true,"can_send_messages":false,"can_send_audios":false,"can_send_documents":false,"can_send_photos":false,"can_send_videos":false,"can_send_video_notes":false,"can_send_voice_notes":false,"can_send_polls":false,"can_send_other_messages":false,"can_add_web_page_previews":false,"can_change_info":false,"can_invite_users":false,"can_pin_messages":false,"can_manage_topics":false,"until_date":0}""", "Restricted")>]
[<InlineData("left", """{"status":"left","user":%USER%}""", "Left")>]
[<InlineData("kicked", """{"status":"kicked","user":%USER%,"until_date":0}""", "Banned")>]
let ``ChatMember deserializes to the case matching the status value`` (status: string, jsonTemplate: string, expectedCase: string) =
  let json = jsonTemplate.Replace("%USER%", user)
  match parseResult<ChatMember> json with
  | Ok m ->
    Assert.Equal(expectedCase, caseName m)
    let actualStatus =
      match m with
      | ChatMember.Owner x -> x.Status
      | ChatMember.Administrator x -> x.Status
      | ChatMember.Member x -> x.Status
      | ChatMember.Restricted x -> x.Status
      | ChatMember.Left x -> x.Status
      | ChatMember.Banned x -> x.Status
    Assert.Equal(status, actualStatus)
  | Error e -> failwith e.Description

[<Fact>]
let ``ChatMember member round-trip does not invent fields`` () =
  let json = """{"status":"member","user":""" + user + "}"
  match parseResult<ChatMember> json with
  | Ok m ->
    let reserialized = toJsonString m
    Assert.DoesNotContain("is_anonymous", reserialized)
  | Error e -> failwith e.Description

[<Fact>]
let ``ChatMember with an unknown future status falls back to shape matching without throwing`` () =
  let json = """{"status":"holographic_member","user":""" + user + "}"
  match parseResult<ChatMember> json with
  | Ok m ->
    let status =
      match m with
      | ChatMember.Owner x -> x.Status
      | ChatMember.Administrator x -> x.Status
      | ChatMember.Member x -> x.Status
      | ChatMember.Restricted x -> x.Status
      | ChatMember.Left x -> x.Status
      | ChatMember.Banned x -> x.Status
    Assert.Equal("holographic_member", status)
  | Error e -> failwith e.Description

[<Theory>]
[<InlineData("""{"type":"user","date":1,"sender_user":{"id":1,"is_bot":false,"first_name":"x"}}""", "User")>]
[<InlineData("""{"type":"hidden_user","date":1,"sender_user_name":"x"}""", "HiddenUser")>]
[<InlineData("""{"type":"chat","date":1,"sender_chat":{"id":-1,"type":"channel"}}""", "Chat")>]
[<InlineData("""{"type":"channel","date":1,"chat":{"id":-1,"type":"channel"},"message_id":5}""", "Channel")>]
let ``MessageOrigin deserializes to the case matching the type value`` (json: string, expectedCase: string) =
  match parseResult<MessageOrigin> json with
  | Ok o ->
    let actual =
      match o with
      | MessageOrigin.User _ -> "User"
      | MessageOrigin.HiddenUser _ -> "HiddenUser"
      | MessageOrigin.Chat _ -> "Chat"
      | MessageOrigin.Channel _ -> "Channel"
    Assert.Equal(expectedCase, actual)
  | Error e -> failwith e.Description