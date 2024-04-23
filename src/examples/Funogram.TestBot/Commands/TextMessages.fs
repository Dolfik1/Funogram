module Funogram.TestBot.Commands.TextMessages

open Funogram.Telegram
open Funogram.Telegram.Bot
open Funogram.TestBot.Core
open Funogram.Telegram.Types

let private sendMessageFormatted text parseMode config chatId =
  Req.SendMessage.Make(ChatId.Int chatId, text, parseMode = parseMode) |> bot config

[<Literal>]
let MarkdownExample = """*bold text*
_italic text_
[inline URL](http://www.example.com/)
[inline mention of a user](tg://user?id=123456789)
`inline fixed-width code`
```
pre-formatted fixed-width code block
```
```python
pre-formatted fixed-width code block written in the Python programming language
```"""

[<Literal>]
let MarkdownV2Example = """*bold \*text*
_italic \*text_
__underline__
~strikethrough~
||spoiler||
*bold _italic bold ~italic bold strikethrough ||italic bold strikethrough spoiler||~ __underline italic bold___ bold*
[inline URL](http://www.example.com/)
[inline mention of a user](tg://user?id=123456789)
![👍](tg://emoji?id=5368324170671202286)
`inline fixed-width code`
```
pre-formatted fixed-width code block
```
```python
pre-formatted fixed-width code block written in the Python programming language
```
>Block quotation started
>Block quotation continued
>The last line of the block quotation**
>The second block quotation started right after the previous\r
>The third block quotation started right after the previous"""

[<Literal>]
let HtmlExample = """
<b>bold</b>, <strong>bold</strong>
<i>italic</i>, <em>italic</em>
<u>underline</u>, <ins>underline</ins>
<s>strikethrough</s>, <strike>strikethrough</strike>, <del>strikethrough</del>
<span class="tg-spoiler">spoiler</span>, <tg-spoiler>spoiler</tg-spoiler>
<b>bold <i>italic bold <s>italic bold strikethrough <span class="tg-spoiler">italic bold strikethrough spoiler</span></s> <u>underline italic bold</u></i> bold</b>
<a href="http://www.example.com/">inline URL</a>
<a href="tg://user?id=123456789">inline mention of a user</a>
<tg-emoji emoji-id="5368324170671202286">👍</tg-emoji>
<code>inline fixed-width code</code>
<pre>pre-formatted fixed-width code block</pre>
<pre><code class="language-python">pre-formatted fixed-width code block written in the Python programming language</code></pre>
<blockquote>Block quotation started\nBlock quotation continued\nThe last line of the block quotation</blockquote>
"""

let testMarkdown = sendMessageFormatted MarkdownExample ParseMode.Markdown
let testMarkdownV2 = sendMessageFormatted MarkdownV2Example ParseMode.MarkdownV2
let testHtml = sendMessageFormatted HtmlExample ParseMode.HTML
let testNoWebpageAndNotification config chatId =
  Req.SendMessage.Make(
    ChatId.Int chatId,
    "@Dolfik! See http://fsharplang.ru - Russian F# Community",
    disableNotification = true,
    linkPreviewOptions = LinkPreviewOptions.Create(isDisabled = true)
  ) |> bot config

let testReply (ctx: UpdateContext) config (chatId: int64) =
  Req.SendMessage.Make(
    chatId,
    "That's message with reply!",
    replyParameters = ReplyParameters.Create(messageId = ctx.Update.Message.Value.MessageId)
  ) |> bot config
 
let testForwardMessage (ctx: UpdateContext) config (chatId: int64) =
  Req.ForwardMessage.Make(
    chatId,
    chatId,
    messageId = ctx.Update.Message.Value.MessageId
  ) |> bot config

let testScan a b =
  sendMessageFormatted (sprintf "%s %s" a b) ParseMode.Markdown