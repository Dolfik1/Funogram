module Funogram.Tests.WebHook

open System
open System.Net
open System.Net.Http
open System.Net.Sockets
open System.Text
open System.Threading
open System.Threading.Tasks
open Funogram.Telegram.Bot
open Xunit

type private FakeTelegram(response: string) =
  inherit HttpMessageHandler()
  override _.SendAsync(_, _) =
    Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK, Content = new StringContent(response)))

let private getMeResponse =
  """{"ok":true,"result":{"id":1,"is_bot":true,"first_name":"Test","username":"test_bot"}}"""

let private update (id: int64) (text: string) =
  $"""{{"update_id":{id},"message":{{"message_id":{id},"date":1707128500,"chat":{{"id":1,"type":"private"}},"text":"{text}"}}}}"""

let private timeout = TimeSpan.FromSeconds 5.

let private freePort () =
  let listener = new TcpListener(IPAddress.Loopback, 0)
  listener.Start()
  let port = (listener.LocalEndpoint :?> IPEndPoint).Port
  listener.Stop()
  port

type private WebHookBot(validate: HttpListenerRequest -> bool, onUpdate: UpdateContext -> unit) =
  let prefix = $"http://localhost:{freePort ()}/"
  let listener = new HttpListener()
  do
    listener.Prefixes.Add prefix
    listener.Start()

  let cts = new CancellationTokenSource()
  let http = new HttpClient()
  let config =
    { Config.defaultConfig with
        Client = new HttpClient(new FakeTelegram(getMeResponse))
        WebHook = Some { Listener = listener; ValidateRequest = validate } }
  let run = Async.StartAsTask(startBot config onUpdate None, cancellationToken = cts.Token)

  member _.PostAsync(body: string) =
    http.PostAsync(prefix, new StringContent(body, Encoding.UTF8, "application/json"))

  member _.Run: Task = run
  member _.Stop() = cts.Cancel()

  interface IDisposable with
    member _.Dispose() =
      cts.Cancel()
      http.Dispose()
      listener.Close()

[<Fact>]
let ``Webhook delivers a bare update to the handler`` () = task {
  let received = TaskCompletionSource<UpdateContext>()
  use bot = new WebHookBot((fun _ -> true), received.SetResult)

  let! response = bot.PostAsync(update 42L "hi")
  let! ctx = received.Task.WaitAsync timeout

  Assert.Equal(HttpStatusCode.OK, response.StatusCode)
  Assert.Equal(42L, ctx.Update.UpdateId)
  Assert.Equal(Some "hi", ctx.Update.Message |> Option.bind _.Text)
  Assert.Equal(Some "test_bot", ctx.Me.Username)
}

[<Fact>]
let ``Webhook rejects requests that fail validation`` () = task {
  let mutable called = false
  use bot = new WebHookBot((fun _ -> false), fun _ -> called <- true)

  let! response = bot.PostAsync(update 1L "hi")

  Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode)
  Assert.False called
}

[<Fact>]
let ``Webhook keeps working after a broken body and a failing handler`` () = task {
  let received = TaskCompletionSource<int64>()
  let handler (ctx: UpdateContext) =
    if ctx.Update.UpdateId = 1L then failwith "handler bug"
    else received.TrySetResult ctx.Update.UpdateId |> ignore
  use bot = new WebHookBot((fun _ -> true), handler)

  let! broken = bot.PostAsync "<html>not json</html>"
  let! failing = bot.PostAsync(update 1L "throws")
  let! _ = bot.PostAsync(update 2L "ok")
  let! id = received.Task.WaitAsync timeout

  Assert.Equal(HttpStatusCode.OK, broken.StatusCode)
  Assert.Equal(HttpStatusCode.OK, failing.StatusCode)
  Assert.Equal(2L, id)
}

[<Fact>]
let ``Webhook loop stops on cancellation`` () = task {
  let received = TaskCompletionSource()
  use bot = new WebHookBot((fun _ -> true), fun _ -> received.TrySetResult() |> ignore)

  let! _ = bot.PostAsync(update 1L "hi")
  do! received.Task.WaitAsync timeout
  bot.Stop()

  let! finished = Task.WhenAny(bot.Run, Task.Delay timeout)
  Assert.Same(bot.Run, finished)
}
