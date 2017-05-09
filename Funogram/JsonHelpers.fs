namespace Funogram

module internal JsonHelpers =
    open Newtonsoft.Json
    open Newtonsoft.Json.Serialization
    open System

    /// Used for convert Unit to DateTime
    type UnixDateTimeConverter() =
        inherit JsonConverter()

        override this.CanConvert objectType = 
            objectType = typeof<DateTime>

        override this.ReadJson (reader, objectType, existingValue, serializer) =
            box (reader.Value |> string |> float |> DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds)

        override this.WriteJson (writer, value, serializer) =
            raise(NotImplementedException())