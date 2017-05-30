namespace Funogram

module internal JsonHelpers =
    open Newtonsoft.Json
    open Newtonsoft.Json.Serialization
    open System
    open System.Reflection
    open FSharp.Reflection

    /// Used for convert Unit to DateTime
    type UnixDateTimeConverter() =
        inherit JsonConverter()

        override this.CanConvert objectType = 
            objectType = typeof<DateTime>

        override this.ReadJson (reader, objectType, existingValue, serializer) =
            let isOption = objectType.GetTypeInfo().IsGenericType && objectType.GetGenericTypeDefinition() = typedefof<option<_>>
            if (isNull(reader.Value) && isOption) then
                box None
            elif isNull(reader.Value) then
                box null
            else
                let v = (reader.Value |> string |> float |> DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds)
                if isOption then
                    box (Some v)
                else
                    box v

        override this.WriteJson (writer, value, serializer) =
            raise(NotImplementedException())