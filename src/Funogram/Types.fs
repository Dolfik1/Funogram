module Funogram.Types

open System
open System.ComponentModel
open System.Net.Http
open System.Runtime.Serialization
open System.Net
open System.Text.Json
open Microsoft.Extensions.Logging

[<AttributeUsage(AttributeTargets.Property, AllowMultiple = false)>]
type AlwaysAttribute(value: string) =
  inherit Attribute()
  member _.Value = value

[<Serialization.JsonConverter(typeof<RawJsonConverter>)>]
type RawJson =
  { Json: string }
and RawJsonConverter() =
  inherit Serialization.JsonConverter<RawJson>()

  override _.Read(reader, _, _) =
    use doc = JsonDocument.ParseValue(&reader)
    { Json = doc.RootElement.GetRawText() }

  override _.Write(writer, value, _) =
    writer.WriteRawValue(value.Json)

type BotWebHook = { Listener: HttpListener; ValidateRequest: HttpListenerRequest -> bool }

[<StructuredFormatDisplay("{Masked}")>]
type BotToken =
  private { value: string; masked: string }
  static member Create(value: string) =
    let masked =
      if String.IsNullOrEmpty value then "<no token>"
      else
        let visible =
          match value.IndexOf(':') with
          | -1 -> 3
          | i -> i + 4
        value.Substring(0, min visible value.Length) + "***"
    { value = value; masked = masked }
  member x.Masked = x.masked
  member x.Reveal() = x.value
  override x.ToString() = x.masked

type BotConfig = 
  { IsTest: bool
    Token: BotToken
    Offset: int64 option
    Limit: int64 option
    Timeout: int64 option
    AllowedUpdates: string seq option
    ApiEndpointUrl: Uri
    Client: HttpClient
    WebHook: BotWebHook option
    Logger: ILogger
    JsonOptions: JsonSerializerOptions }

type IBotRequest =
  [<IgnoreDataMember>]
  abstract MethodName: string

type IRequestBase<'a> =
  inherit IBotRequest

type [<CLIMutable>] ResponseParameters =
  {
    [<DataMember(Name = "migrate_to_chat_id")>]
    MigrateToChatId: int64 option
    [<DataMember(Name = "retry_after")>]
    RetryAfter: int64 option
  }

type ErrorResponseException(error: ErrorResponse) =
  inherit Exception($"{error.ErrorCode}: {error.Description}")

  member _.Error = error
and ErrorResponse =
  {
    [<DataMember(Name = "error_code")>]
    ErrorCode: int
    [<DataMember(Name = "description")>]
    Description: string
    [<DataMember(Name = "parameters")>]
    Parameters: ResponseParameters option
  }

[<RequireQualifiedAccess>]
type ApiError =
  | Rejected of ErrorResponse
  | Network of exn
  | InvalidResponse of statusCode: int * exn
  | UnexpectedResult of json: string * exn
  member x.AsException(): Exception =
    match x with
    | Rejected error -> ErrorResponseException(error)
    | Network exn -> exn
    | InvalidResponse (_, exn) -> exn
    | UnexpectedResult (_, exn) -> exn
  
[<CLIMutable; EditorBrowsable(EditorBrowsableState.Never)>]
type ApiResponse<'a> = 
  {
    [<DataMember(Name = "ok")>]
    Ok: bool
    [<DataMember(Name = "result")>]
    Result: 'a option
    [<DataMember(Name = "description")>]
    Description: string option
    [<DataMember(Name = "error_code")>]
    ErrorCode: int option
    [<DataMember(Name = "parameters")>]
    Parameters: ResponseParameters option
  }
