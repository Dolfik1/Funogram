![.NET Core](https://github.com/Dolfik1/Funogram/workflows/.NET%20Core/badge.svg)
[![NuGet](https://img.shields.io/nuget/v/FunHttp.svg)](https://www.nuget.org/packages/Funogram/)

# Funogram
F# Telegram Bot Api library!

# Breaking changes

Funogram 2.0 has breaking changes. You should add two nuget packages: Funogram and Funogram.Telegram.

### Getting Started
Firstly you need to install <a href="https://www.nuget.org/packages/Funogram">Funogram</a>. Installation for .NET Core users:
```
dotnet add package Funogram
```
Installation for .NET Framework users:
```
Install-Package Funogram
```
### Writing a Bot
To get your first bot running just use *startBot* function from Funogram.Bot module. If you don't need any additional configuration, you can use *defaultConfig* constant that stores default settings. Note that you need to extend default config with your own bot token received from <a href="t.me/botfather">BotFather</a>. Here you go:
```fsharp
open Funogram.Bot

startBot { defaultConfig with Token = "your token" } onUpdate None
```
Every update received from Telegram Bot API will be passed to *onUpdate* function. It should handle user input according to UpdateContext passed. Use *processCommands* function to process commands addressed to your Bot, *cmd* and *cmdScan* can help you.
```fsharp
let onUpdate (context: UpdateContext) =
  processCommands context [
    cmd "/start" onStart
    cmd "/help" onHelp
  ]
```
Use Funogram.Api module to communicate with Telegram API with the help of request builders implemented using curried functions, for example *sendMessage*, *sendPhoto* and so on.
```fsharp
"Hello, world!"
|> sendMessage chatId
|> api accessToken
|> Async.RunSynchronously
|> ignore (* or process API response somehow *)
```
So, that is it! Use Intellisence-driven development approach to explore all Funogram features! For further learning you may take a look at sample Telegram bots built using this library: [Test Bot](src/Funogram.TestBot/), <a href="https://github.com/worldbeater/Memes.Bot/tree/master/Memes">Memes Bot</a>

# Getting updates via webhooks
If you want to use webhooks, you should start application with admin privileges.



To get updates via webhooks you need send your endpoint address to Telegram server:
```fsharp
let! hook = setWebhookBase webSocketEndpoint None None None |> api config
```
You can use [ngrok](https://ngrok.com/) service to test webhooks on your local machine that no have public address.

Then you should set `WebHook` field in `BotConfig`. `WebHook` field have `BotWebHook` type that contains two fields: `Listener` and `ValidateRequest`:
```fsharp
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

# Articles

[Funogram: Writing Telegram Bots In F#](https://medium.com/@worldbeater/funogram-writing-telegram-bots-in-f-f27a873fa548)

[Funogram.Keyboard: How to reserve seats on an airplane using F # and telegram](https://medium.com/@fsharpfan/funogram-keyboard-how-to-reserve-seats-on-an-airplane-using-f-and-telegram-6f7035e9c698)

# Plugins and Extensions

[Funogram.Keyboard](https://github.com/dohly/funogram.keyboard)

# Work in Progress

Old methods moved in `Funogram.Rest` module.
New more functional api available in `Funogram.Api` module.

Not recommended to use functions who ends with `Base`, because it's may be changed in future. If you want to use this functions, you need to minimize usage of it (write wrapper, for example).

#### Completed 👍:
- getUpdates
- getMe
- forwardMessage
- ❕sendMessage (not tested ForceReply)
- getUserProfilePhotos
- ❕getFile (not tested)
- ❕kickChatMember (not tested)
- ❕unbanChatMember (not tested)
- ❕leaveChat (not tested)
- getChat
- ❕getChatAdministrators (not tested)
- ❕getChatMembersCount (not tested)
- ❕getChatMember (not tested)
- ❕answerCallbackQuery (not tested)
- ❕editMessageText (not tested)
- ❕editMessageCaption (not tested)
- ❕editMessageReplyMarkup (not tested)
- ❕deleteMessage (not tested)
- ❕answerInlineQuery (not tested)
- ❕sendInvoice (not tested)
- ❕answerShippingQuery (not tested)
- ❕answerPreCheckoutQuery (not tested)
- sendPhoto
- ❕sendAudio (not tested)
- ❕sendDocument (not tested)
- ❕sendSticker (not tested)
- ❕sendVideo (not tested)
- ❕sendVoice (not tested)
- ❕sendVideoNote (not tested)
- ❕sendLocation (not tested)
- ❕sendVenue (not tested)
- ❕sendContact (not tested)
- ❕sendChatAction (not tested)
- ❕sendGame (not tested)
- ❕setGameScore (not tested)
- ❕getGameHighScores (not tested)
- ❕restrictChatMember (not tested)
- ❕promoteChatMember (not tested)
- ❕kickChatMember (not tested)
- ❕exportChatInviteLink (not tested)
- ❕setChatPhoto (not tested)
- ❕deleteChatPhoto (not tested)
- ❕setChatTitle (not tested)
- ❕setChatDescription (not tested)
- ❕pinChatMessage (not tested)
- ❕unpinChatMessage (not tested)
