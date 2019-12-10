module Funogram.Types

open System
open System.Net.Http
open System.Runtime.Serialization

type BotConfig = 
    { Token: string
      Offset: int64 option
      Limit: int option
      Timeout: int option
      AllowedUpdates: string seq option
      ApiEndpointUrl: Uri
      Client: HttpClient }

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

/// Bot Api Response Error
type ApiResponseError = 
  { Description: string
    ErrorCode: int }