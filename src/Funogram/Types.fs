module Funogram.Types

open System
open System.Net.Http
open System.Runtime.Serialization
open System.Net

type BotWebHook = { Listener: HttpListener; ValidateRequest: HttpListenerRequest -> bool }

type IBotLogger =
  abstract member Log: message: string -> unit
  abstract member Enabled: bool

/// Marks a generated Telegram type as a tag-discriminated union payload: the JSON
/// property `Field` always carries the literal `Value` for this subtype (e.g.
/// ChatMemberOwner -> ("status", "creator")). The union converter uses it to select
/// the case by discriminator value; several subtypes (ChatMemberLeft vs
/// ChatMemberMember) are shape-identical on the wire, so field-shape matching alone
/// cannot discriminate them.
[<AttributeUsage(AttributeTargets.Class, AllowMultiple = false)>]
type TelegramTagAttribute(field: string, value: string) =
  inherit Attribute()
  member _.Field = field
  member _.Value = value

type BotConfig = 
  { IsTest: bool
    Token: string
    Offset: int64 option
    Limit: int64 option
    Timeout: int64 option
    AllowedUpdates: string seq option
    OnError: Exception -> unit
    ApiEndpointUrl: Uri
    Client: HttpClient
    WebHook: BotWebHook option
    RequestLogger: IBotLogger option }

type IBotRequest =
  [<IgnoreDataMember>]
  abstract MethodName: string

type IRequestBase<'a> =
  inherit IBotRequest

/// Bot Api Response
[<CLIMutable>]
type ApiResponse<'a> = 
  { /// True if request success
    Ok: bool
    /// Result of request
    Result: 'a option
    Description: string option
    ErrorCode: int option }

type ApiResponseException(error: ApiResponseError) =
  inherit Exception()

  member _.Error = error
  override _.ToString() =
    sprintf "ApiResponseException: %s. Code: %A" error.Description error.ErrorCode

/// Bot Api Response Error
and ApiResponseError = 
  { Description: string
    ErrorCode: int }
  member x.AsException() =
    ApiResponseException(x)