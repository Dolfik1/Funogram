![.NET Core](https://github.com/Dolfik1/Funogram/workflows/.NET/badge.svg)
[![NuGet](https://img.shields.io/nuget/v/Funogram.svg)](https://www.nuget.org/packages/Funogram/)
[![NuGet](https://img.shields.io/nuget/v/Funogram.Telegram.svg)](https://www.nuget.org/packages/Funogram.Telegram/)
[![NuGet](https://img.shields.io/badge/Bot%20API-7.3-blue?logo=telegram)](https://www.nuget.org/packages/Funogram.Telegram/)

<img src="https://github.com/Dolfik1/Funogram/raw/master/docs/files/img/logo.png" alt="Funogram Logo" width="200" align="right" />

## Introduction
This library provides F# interface to [Telegram Bot API](https://core.telegram.org/bots/api). The library has two parts. The first one is Funogram. It contains basic functions to interact with Telegram-like servers (REST interaction, serialization, etc). 
And the second part is Funogram.Telegram. It contains all Telegram methods and types for specified Telegram Bot API version.  

In addition to the pure API implementation, this library provides high-level functions to make the development of bots easy. These functions are located in `Funogram.Telegram.Bot` module.

## Breaking changes

Funogram.Telegram 6.0.0.x has breaking changes.
* All request types are moved from `RequestsTypes` module to `Req` module;
* Some types changed due to Telegram Bot API changes

## Installation
You need to install latest Funogram and Funogram.Telegram packages from nuget:
```shell
dotnet add package Funogram
dotnet add package Funogram.Telegram
```
Installation for .NET Framework users:
```shell
Install-Package Funogram
Install-Package Funogram.Telegram
```

## Examples
You may start learning Funogram from [example bots](https://github.com/Dolfik1/Funogram/tree/master/src/examples).

## Hello, world!
First of all you need to get an API key. See [Telegram Bot API documentation](https://core.telegram.org/bots#6-botfather).

When Bot API token is registered you may start to write your first bot. Let's write simple bot that sends "Hello, world!" in reply to any message. You can find source code of the bot [here](https://github.com/Dolfik1/Funogram/tree/master/src/examples/Funogram.Examples.HelloWorld).

Let's open the necessary namespaces:
```f#
open Funogram.Api
open Funogram.Telegram
open Funogram.Telegram.Bot
```


There are two mutually exclusive ways of receiving updates for your bot — the `getUpdates` method on one hand and `webhooks` on the other. Incoming updates are stored on the server until the bot receives them either way, but they will not be kept longer than 24 hours.

The Funogram library automatically receive updates and passes them to your updates handler. Let's write updates handle:
```f#
let updateArrived (ctx: UpdateContext) =
  match ctx.Update.Message with
  | Some { MessageId = messageId; Chat = chat } ->
    Api.sendMessageReply chat.Id "Hello, world!" messageId 
    |> api ctx.Config
    |> Async.Ignore
    |> Async.Start
  | _ -> ()
```

This simple handler is checking if update contains `Message`. If a `Message` found in the `Update` then bot will send answer:
```f#
Api.sendMessageReply chat.Id "Hello, world!" messageId 
|> api ctx.Config
|> Async.Ignore
|> Async.Start
```

This code also might be written like this:
```f#
Req.SendMessage.Make(chat.Id, text = "Hello, world!", replyToMessageId = messageId) 
|> api ctx.Config
|> Async.Ignore
|> Async.Start
```

Now we need to set bot token, pass update handler and start the bot. It might be done with few lines of code:
```f#
[<EntryPoint>]
let main _ =
  async {
    let config = Config.defaultConfig |> Config.withReadTokenFromFile
    let! _ = Api.deleteWebhookBase () |> api config
    return! startBot config updateArrived None
  } |> Async.RunSynchronously
  0
```

Let's take a look at each line in detail. Firstly we need to setup bot token
```f#
let config = Config.defaultConfig |> Config.withReadTokenFromFile
```

The default config looks like:
```f#
let defaultConfig =
  { Token = ""
    Offset = Some 0L
    Limit = Some 100
    Timeout = Some 60000
    AllowedUpdates = None
    Client = new HttpClient()
    ApiEndpointUrl = Uri("https://api.telegram.org/bot")
    WebHook = None
    OnError = (fun e -> printfn "%A" e) }
```

as you can notice there is no set token by default. There are two built-in ways to setup token:
1. `Config.withReadTokenFromFile` function will read token from file named `token` (file should be located in run directory). If file does not exist the app will wait for token input in stdin and then save it to `token` file.
2. `Config.withReadTokenFromEnv "mytokenenv"` function will read value from `mytokenenv` environment variable.

You also can specify token manually:
```f#
let config = { Config.defaultConfig with Token = "mysecrettoken" }
```

When token is set we are ready to make requests. Let's invoke `deleteWebhook` function:

```f#
let! _ = Api.deleteWebhookBase () |> api config
```

This function is not necessary but I always invoking it to force reset websocket configuration. If you have no plans to use websocket connection you can skip this step. Otherwise it may result to missing updates.

Finally we are ready to start the bot loop:
```f#
return! startBot config updateArrived None
```

This function will start main bot loop. For now on all updates will be collected and redirected to our update hook.

You may have noticed that `getUpdates` method returns Array of Update but in our handler we have only single update.
This is done for the convenience of the end developer. In most of cases you need to process updates sequentially. But if you want to process whole received updates you may pass hook function as last parameter of startBot. For example:
```f#
return! startBot config (fun _ -> ()) ((fun updates -> printfn "Got updates array: %A" updates) |> Some)
```

## Processing commands
Funogram can automatically parse specified commands with arguments:

```f#

let updateArrived (ctx: UpdateContext) =
  processCommands ctx [|
    cmd "/start" (fun _ -> printfn "User invoked /start command!")
    cmdScan "/say %s" (fun text _ -> printfn "User invoked say command with text %s" text)
    cmdScan "/sum %i %i" (fun a b _ -> printfn "Sum of %i and %i is %i" a b (a + b))
  |] |> ignore
```

`processCommands` function returns false if at least one of commands is matched. 

There are two main functions: `cmd` and `cmdScan`. There are only one difference between both commands. `cmdScan` is used for command with arguments when the `cmd` is for command without arguments. Both functions has two parameters. First parameter is command itself (it may contain arguments in case of `cmdScan`). The second parameter is callback function.

`cmdScan` works mostly like `sprintf` function but it's logic is reversed. The values will be extracted and passed as first parameter of callback function.

Commands processor also correctly process commands with bot username. For example the are two valid commands for bot:
* /start
* /start@MyExampleFSharpBot

when @MyExampleFSharpBot is bot's username.

You also may write your own command function with custom logic (if required) which may check text for keywords instead of command (see [Bot.fs](https://github.com/Dolfik1/Funogram/tree/master/src/Funogram.Telegram/Bot.fs) for reference) 

## Configure WebSockets
If you want to use webhooks, you should start application with admin privileges. The sample app is located [here](https://github.com/Dolfik1/Funogram/tree/master/src/examples/Funogram.Examples.WebSocket).

To get updates via webhooks you need send your endpoint address to Telegram server:
```f#
let! hook = setWebhookBase webSocketEndpoint None None None |> api config
```
You can use [ngrok](https://ngrok.com/) service to test webhooks on your local machine that no have public address.

Then you should set `WebHook` field in `BotConfig`. `WebHook` field have `BotWebHook` type that contains two fields: `Listener` and `ValidateRequest`:
```f#
let apiPath = sprintf "/%s" config.Token
let webSocketEndpoint = sprintf "https://1c0860ec2320.ngrok.io/%s" webSocketEndpoint apiPath
let! hook = setWebhookBase webSocketEndpoint None None None |> api config
match hook with
| Ok ->
  use listener = new HttpListener()
  listener.Prefixes.Add("http://*:4444/")
  listener.Start()

  let webhook = { Listener = listener; ValidateRequest = (fun req -> req.Url.LocalPath = apiPath) }
  return! startBot { config with WebHook = Some webhook } updateArrived None

| Error e -> 
  printf "Can't set webhook: %A" e
  return ()
```
Telegram [recommends](https://core.telegram.org/bots/api#setwebhook) using a secret path in URL with your bot's token. You can validate telegram request in `ValidateRequest` function.

Note: if you want to rollback to `getUpdates` method you need to clear webhook. This might be done via `deleteWebhook` function.

## Codegen
Funogram types are generated automatically from Telegram Bot API [reference](https://core.telegram.org/bots/api). The code generation tool is located in `src/Funogram.Generator` folder.

To start generator run Funogram.Generator project. You should specify project's directory as working directory to get correct result.

Telegram Bot API reference will parsed and output will copied to `src/Funogram.Generator/out` folder. The generated files are:
* types.json
* methods.json

The generated code will be copied to `src/Funogram.Telegram` folder:
* Types.fs
* RequestsTypes.fs

If you want to generate types and methods for old Telegram server version you may specify link to [web archive](https://web.archive.org/web/*/https://core.telegram.org/bots/api).

You also can patch code to load reference data from *.json files. This will allow you to make changes if needed.

The generator supports remapping. This make it possible to replace some values in generated json to your own (see `RemapTypes.json` and `RemapMethods.json`)

## Advanced
The library is built around types. Any API request or response is F# record type:
```f#
type GetUpdates =
  {
    Offset: int64 option
    Limit: int64 option
    Timeout: int64 option
    AllowedUpdates: string[] option
  }
  static member Make(?offset: int64, ?limit: int64, ?timeout: int64, ?allowedUpdates: string[]) = 
    {
      Offset = offset
      Limit = limit
      Timeout = timeout
      AllowedUpdates = allowedUpdates
    }
  interface IRequestBase<Update[]> with
    member _.MethodName = "getUpdates"
```

The request types should implement `IRequestBase<T>` interface when `T` is return type and `MethodName` is Telegram's Bot API method name.
You can create your own request type and send it to the server without patching Funogram.Telegram library. This might be useful if Funogram is not updated to latest Bot API version but you want to use some new features without waiting for new release.

Response types are also record types:
```f#

/// This object contains information about one answer option in a poll.
and [<CLIMutable>] PollOption =
  {
    /// Option text, 1-100 characters
    [<DataMember(Name = "text")>]
    Text: string
    /// Number of users that voted for this option
    [<DataMember(Name = "voter_count")>]
    VoterCount: int64
  }
  static member Create(text: string, voterCount: int64) = 
    {
      Text = text
      VoterCount = voterCount
    }
 ```

All fields in type should be marked with `DataMember` attribute to avoid problems with parameters naming.

Telegram Bot API have four ways of passing parameters in Bot API requests:

* URL query string
* application/x-www-form-urlencoded
* application/json (except for uploading files)
* multipart/form-data (use to upload files)

The library uses `URL query string` method if possible and `multipart/form-data` method for all requests with files. The preferred method will be choosen by request data. If you want to use application/json method for some reasons you can use `Funogram.Tools.Api.makeJsonBodyRequestAsync` function (this function will not work with files). You also can implement your own method of Bot API request (see `Funogram/Tools.fs` for reference) 


## Articles

DEPRECATED: [Funogram: Writing Telegram Bots In F#](https://medium.com/@worldbeater/funogram-writing-telegram-bots-in-f-f27a873fa548)

DEPRECATED: [Funogram.Keyboard: How to reserve seats on an airplane using F # and telegram](https://medium.com/@fsharpfan/funogram-keyboard-how-to-reserve-seats-on-an-airplane-using-f-and-telegram-6f7035e9c698)

## Plugins and Extensions

DEPRECATED: [Funogram.Keyboard](https://github.com/dohly/funogram.keyboard)
