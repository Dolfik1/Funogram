namespace Funogram.Converters

open System
open System.Runtime.CompilerServices
open System.Text.Json
open System.Text.Json.Serialization
open TypeShape.Core

[<assembly:InternalsVisibleTo("Funogram.Tests")>]
do ()

type internal OptionConverter<'T>() =
  inherit JsonConverter<Option<'T>>()

  let mutable inner: JsonConverter<'T> = null

  let getInner (options: JsonSerializerOptions) =
    if isNull inner then
      inner <- options.GetConverter(typeof<'T>) :?> JsonConverter<'T>
    inner

  override x.Read(reader, _, options) =
    match reader.TokenType with
    | JsonTokenType.Null ->
      None
    | _ ->
      (getInner options).Read(&reader, typeof<'T>, options) |> Some

  override x.Write(writer, value, options) =
    match value with
    | Some v ->
      (getInner options).Write(writer, v, options)
    | None ->
      writer.WriteNullValue()

// The FSharpOptionTypeConverter in STJ seems to be broken
// There is no stable repro to check this, so I used my own Option converter
type internal OptionConverterFactory() =
  inherit JsonConverterFactory()

  override x.CreateConverter(typeToConvert, _) =
    let innerType = typeToConvert.GetGenericArguments()[0]
    let optionConverterType = typedefof<OptionConverter<_>>.MakeGenericType(innerType)
    Activator.CreateInstance(optionConverterType) :?> JsonConverter
    
  override x.CanConvert(typeToConvert) =
    match TypeShape.Create(typeToConvert) with
    | Shape.FSharpOption _ -> true
    | _ -> false