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


    type SnakeCaseContractResolver() =
        inherit DefaultContractResolver()
        let toTitleCase (str: string) = System.Char.ToUpper(str.ToCharArray().[0]).ToString() + str.Substring(1)
        let isNullOrEmpty text = System.String.IsNullOrEmpty text

        override this.ResolvePropertyName propertyName =
            match isNullOrEmpty propertyName with
                | true -> propertyName
                | _ -> propertyName.Split [|'_'|] 
                       |> Seq.map toTitleCase
                       |> System.String.Concat

