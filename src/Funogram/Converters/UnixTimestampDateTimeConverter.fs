namespace Funogram.Converters

open System
open System.Runtime.CompilerServices
open System.Text.Json.Serialization

[<assembly:InternalsVisibleTo("Funogram.Tests")>]
do ()

type internal UnixTimestampDateTimeConverter() =
  inherit JsonConverter<DateTime>()

  override x.Read(reader, _, _) =
    DateTimeOffset.FromUnixTimeSeconds(reader.GetInt64()).UtcDateTime

  override x.Write(writer, value, _) =
    writer.WriteNumberValue(DateTimeOffset(value.ToUniversalTime()).ToUnixTimeSeconds())
