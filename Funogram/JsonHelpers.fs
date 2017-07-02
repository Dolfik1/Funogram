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

        let getUnix (date: DateTime) = Convert.ToInt64(date.Subtract( DateTime(1970, 1, 1)).TotalSeconds);
        let isOption (t: Type) = t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition() = typedefof<option<_>>

        override this.CanConvert objectType = 
            objectType = typeof<DateTime>

        override this.ReadJson (reader, objectType, existingValue, serializer) =
            if (isNull(reader.Value) && isOption objectType) then
                box None
            elif isNull(reader.Value) then
                box null
            else
                let v = (reader.Value |> string |> float |> DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds)
                if isOption objectType then
                    box (Some v)
                else
                    box v

        override this.WriteJson (writer, value, serializer) =
            let value =
                if isNull value then null
                elif isOption (value.GetType()) then
                    let v = value :?> Option<DateTime>
                    match v with
                    | Some x -> box (getUnix x)
                    | _ -> null
                else
                    box (getUnix (value :?> DateTime))
            
            serializer.Serialize(writer, value)