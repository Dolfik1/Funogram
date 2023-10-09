module Funogram.Examples.Banhammer.Program

open System
open System.IO
open Funogram.Api
open Funogram.Telegram
open Funogram.Telegram.Bot
open Funogram.Telegram.Types

[<Literal>]
let BanListFile = "banlist"

let readBanlist () =
  try
    if File.Exists(BanListFile) |> not then System.IO.File.Create(BanListFile).Dispose()

    File.ReadAllLines(BanListFile)
    |> Seq.choose (fun x -> match Int64.TryParse(x) with | true, v -> Some v | _ -> None)
    |> Set.ofSeq
  with
  | _ -> Set.empty

let processBanCommand (msg: Message) (from: User) banlist ctx =
  async {
    match! Api.getChatMember msg.Chat.Id from.Id |> apiAsync ctx.Config with
    | Result.Ok (ChatMember.Administrator { CanRestrictMembers = true })
    | Result.Ok (ChatMember.Owner _) ->

      match msg.ReplyToMessage with
      | Some { From = Some replyMessageFrom } ->
        let newBanlist = banlist |> Set.add replyMessageFrom.Id
        try
          File.WriteAllLines(BanListFile, newBanlist |> Seq.map string)
        with
        | e -> printfn "Could not save data to banlist! %A" e

        match! Api.banChatMember msg.Chat.Id replyMessageFrom.Id |> apiAsync ctx.Config with
        | Result.Ok _ ->
          Api.sendMessageReply msg.Chat.Id "The user was banned!" msg.MessageId |> apiAsync ctx.Config |> Async.Ignore |> Async.Start
        | Result.Error e ->
          let text = sprintf "Could not ban user! %s" e.Description
          Api.sendMessageReply msg.Chat.Id text msg.MessageId |> apiAsync ctx.Config |> Async.Ignore |> Async.Start

        return newBanlist
      | _ ->
        Api.sendMessageReply
          msg.Chat.Id
          "You need to reply user's message to ban user!"
          msg.MessageId
        |> apiAsync ctx.Config |> Async.Ignore |> Async.Start

        return banlist

    | _ ->
      return banlist
  }

let processMembersJoin (msg: Message) (newChatMembers: User[]) banlist ctx  =
  async {
    let bannedUsers = newChatMembers |> Array.filter (fun x -> banlist |> Set.contains x.Id)
    if bannedUsers.Length = 0 then
      return banlist
    else
      let! result =
        bannedUsers
        |> Seq.map (fun banned ->
          async {
            match! Api.banChatMember msg.Chat.Id banned.Id |> apiAsync ctx.Config with
            | Result.Ok _ -> return Result.Ok banned
            | Result.Error e -> return Result.Error (banned, e)
          })
        |> Async.Sequential

      let banned = result |> Array.choose (fun x -> match x with | Result.Ok x -> Some x | Result.Error _ -> None)
      let failed = result |> Array.choose (fun x -> match x with | Result.Ok _ -> None | Result.Error x -> Some x)

      let text =
        if banned.Length > 0 then sprintf "I have banned %s" (banned |> Seq.map (fun x -> x.FirstName) |> String.concat ", ")
        else ""

      let text =
        if failed.Length > 0 then
          sprintf "%s\r\nCould not ban %s"
            text
            (failed |> Seq.map (fun (x, e) -> sprintf "%s (%s)" x.FirstName e.Description) |> String.concat ", ")
        else
          text

      Api.sendMessage msg.Chat.Id text |> apiAsync ctx.Config |> Async.Ignore |> Async.Start

      return banlist
  }

let mailbox = MailboxProcessor.Start(fun inbox ->
  let rec loop banlist =
    async {
      let! ctx = inbox.Receive()
      match ctx.Update.Message with
      | Some ({ NewChatMembers = Some newChatMembers } as msg) when newChatMembers.Length > 0 ->
        let! banlist = processMembersJoin msg newChatMembers banlist ctx
        return! loop banlist

      | Some ({ From = Some from } as msg) when checkCommand ctx "/ban" ->
        let! banlist = processBanCommand msg from banlist ctx
        return! loop banlist
      | _ ->
        return! loop banlist
    }

  loop (readBanlist ())
)

[<EntryPoint>]
let main _ =
  async {
    let config = Config.defaultConfig |> Config.withReadTokenFromFile
    let! _ = Api.deleteWebhookBase () |> apiAsync config
    return! startBot config mailbox.Post None
  } |> Async.RunSynchronously
  0
