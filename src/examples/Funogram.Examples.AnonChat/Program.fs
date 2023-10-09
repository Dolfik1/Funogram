module Funogram.Examples.AnonChat.Program

open System
open Funogram.Api
open Funogram.Telegram
open Funogram.Telegram.Bot
open Funogram.Telegram.Types
open Funogram.Types

// hooks to catch errors
// you can add logger if you want
let processResultWithValue (result: Async<Result<'a, ApiResponseError>>) =
  async {
    let! result = result
    match result with
    | Ok _ -> ()
    | Error e -> printfn "Server error: %s" e.Description

    return result
  }

let processResult result = processResultWithValue result
let botResult config data = apiAsync config data
let bot config data = botResult config data |> processResult
let botIgnored config data = bot config data |> Async.Ignore |> Async.Start

type Command =
  | Search
  | Stop of next: bool
  | Message

type MailboxState =
  {
    UsersDialogs: Map<int64, int64>
    SearchUsersSet: Set<int64>
    SearchUsers: int64 array
  }

let sendMessage chatId text ctx = Api.sendMessage chatId text |> bot ctx.Config |> Async.Ignore |> Async.Start

let anonFoundText =
  """
Anonymous found. Enjoy chatting!

/next - search new anonymous
/stop - stop the chat
"""

let usersState =
  MailboxProcessor.Start(fun inbox ->
    let rnd = Random()
    let rec loop (state: MailboxState) =
      async {
        let! (message, ctx: UpdateContext) = inbox.Receive()
        let newState =
          match ctx.Update.Message with
          | Some { MessageId = messageId; From = Some from } ->
            match message with
            | Command.Search ->
              if state.SearchUsersSet |> Set.contains from.Id then
                sendMessage from.Id "You are already searching. Send /stop if you want to stop search." ctx
                state
              elif state.SearchUsers.Length > 0 then
                let idx = rnd.Next(0, state.SearchUsers.Length - 1)
                let newTalkerId = state.SearchUsers.[idx]


                sendMessage from.Id anonFoundText ctx
                sendMessage newTalkerId anonFoundText ctx

                { state with
                    SearchUsers = state.SearchUsers |> Array.removeAt idx
                    SearchUsersSet = state.SearchUsersSet |> Set.remove newTalkerId
                    UsersDialogs =
                      state.UsersDialogs
                      |> Map.add from.Id newTalkerId
                      |> Map.add newTalkerId from.Id
                }
              else
                sendMessage from.Id "Searching for chat... Send /stop to stop the search." ctx
                { state with
                    SearchUsers = state.SearchUsers |> Array.append [| from.Id |]
                    SearchUsersSet = state.SearchUsersSet |> Set.add from.Id
                }


            | Command.Stop next ->
              let chattingWithId = state.UsersDialogs |> Map.tryFind from.Id
              match chattingWithId with
              | Some chattingWithId ->
                sendMessage chattingWithId "Anonymous has stopped the chat! Send /search to start new chat" ctx

                if not next then
                  sendMessage from.Id "You have stopped the chat! Send /search to start new chat" ctx
                else
                  sendMessage from.Id "You have stopped the chat!" ctx
                  inbox.Post(Search, ctx)

                state

              | None when state.SearchUsersSet |> Set.contains from.Id ->
                sendMessage from.Id "You have stopped the search! Send /search to start it again" ctx
                let idx = Array.IndexOf(state.SearchUsers, from.Id)
                { state with
                    SearchUsersSet = state.SearchUsersSet |> Set.remove from.Id
                    SearchUsers = state.SearchUsers |> Array.removeAt idx
                }


              | None ->
                sendMessage from.Id "You have no active chat! Send /search to start new chat" ctx
                state

            | Command.Message ->
              let chattingWithId = state.UsersDialogs |> Map.tryFind from.Id
              match chattingWithId with
              | Some chattingWithId ->
                Req.CopyMessage.Make(chattingWithId, from.Id, messageId) |> botIgnored ctx.Config
                state

              | None ->
                sendMessage from.Id "You have no active chat! Send /search to start new chat" ctx
                state

          | _ ->
            state

        return! loop newState
      }

    loop { UsersDialogs = Map.empty; SearchUsersSet = Set.empty; SearchUsers = Array.empty }
  )

let updateArrived (ctx: UpdateContext) =
  let result =
    processCommands ctx [|
      cmd "/search" (fun _ -> usersState.Post(Command.Search, ctx))
      cmd "/stop" (fun _ -> usersState.Post(Command.Stop false, ctx))
      cmd "/next" (fun _ -> usersState.Post(Command.Stop true, ctx))
    |]

  if result then usersState.Post(Command.Message, ctx)

[<EntryPoint>]
let main _ =
  async {
    let config = Config.defaultConfig |> Config.withReadTokenFromFile
    let! _ = Api.deleteWebhookBase () |> apiAsync config
    return! startBot config updateArrived None
  } |> Async.RunSynchronously
  0
