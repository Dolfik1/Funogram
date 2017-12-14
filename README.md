# Funogram
F# Telegram Bot Api library!

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
So, that is it! Use Intellisence-driven development approach to explore all Funogram features! For further learning you may take a look at sample Telegram bots built using this library: <a href="https://github.com/Dolfik1/Funogram/tree/master/Funogram.TestBot">Test Bot</a>, <a href="https://github.com/worldbeater/Memes.Bot/tree/master/Memes">Memes Bot</a>

# Work in Progress

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
