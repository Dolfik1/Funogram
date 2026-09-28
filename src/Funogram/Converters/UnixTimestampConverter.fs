namespace Funogram.Converters

open System
open System.Runtime.CompilerServices
open System.Text.Json.Serialization

[<assembly:InternalsVisibleTo("Funogram.Tests")>]
do ()

type internal UnixTimestampConverter() =
  inherit JsonConverter<DateTimeOffset>()

  override x.Read(reader, _, _) =
    DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64())

  override x.Write(writer, value, _) =
    writer.WriteNumberValue(value.ToUnixTimeSeconds())
