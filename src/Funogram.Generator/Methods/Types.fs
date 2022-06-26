module Funogram.Generator.Methods.Types

open System.Runtime.Serialization

type ApiTypeField =
  {
    OriginalName: string
    ConvertedName: string
    Description: string
    OriginalFieldType: string
    ConvertedFieldType: string
    Optional: bool Option
  }
  [<IgnoreDataMember>]
  member x.VisibleFieldType =
    if not x.IsOptional then x.ConvertedFieldType
    else sprintf "%s option" x.ConvertedFieldType
    
  [<IgnoreDataMember>]
  member x.IsOptional = x.Optional |> Option.defaultValue false

type ApiType =
  {
    OriginalName: string
    ConvertedName: string
    Description: string
    Fields: ApiTypeField[]
    OriginalReturnType: string
    ConvertedReturnType: string
  }
