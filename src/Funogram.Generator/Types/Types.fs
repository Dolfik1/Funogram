module Funogram.Generator.Types.Types

open System.Runtime.Serialization
open FSharp.Data

type ApiTypeNodeInfo =
  {
    TypeName: HtmlNode
    TypeDesc: HtmlNode list
    TypeFields: HtmlNode option
    TypeCases: HtmlNode option
  }
  static member Create(typeName) = { TypeName = typeName; TypeDesc = []; TypeFields = None; TypeCases = None }

type ApiTypeField =
  {
    OriginalName: string
    ConvertedName: string
    Description: string
    OriginalFieldType: string
    ConvertedFieldType: string
    Optional: bool option
  }
  [<IgnoreDataMember>]
  member x.IsOptional = x.Optional |> Option.defaultValue false

  [<IgnoreDataMember>]
  member x.VisibleFieldType =
    if not x.IsOptional then x.ConvertedFieldType
    else sprintf "%s option" x.ConvertedFieldType

type ApiTypeCase =
  {
    Name: string
    CaseType: string
  }

type ApiTypeKind =
  | Stub
  | Cases of ApiTypeCase[]
  | Fields of ApiTypeField[]

type ApiType =
  {
    Name: string
    Description: string
    Kind: ApiTypeKind
  }