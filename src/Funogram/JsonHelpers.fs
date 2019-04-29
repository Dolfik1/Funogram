module internal Funogram.JsonHelpers

open Newtonsoft.Json
open System
open System.Reflection

type InSnakeCaseAttribute() = 
    inherit System.Attribute()

/// Used for convert Unix to DateTime
type UnixDateTimeConverter() = 
    inherit JsonConverter()
    
    static let UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
    
    let getUnixTime (time: DateTime) =
        int64 (time.ToUniversalTime() - UnixEpoch).TotalSeconds
        
    let isOption (t: Type) = 
        t.GetTypeInfo().IsGenericType 
        && t.GetGenericTypeDefinition() = typedefof<option<_>>
        
    let isNullable (t: Type) =
        Nullable.GetUnderlyingType(t) <> null
        
    override __.CanConvert objectType =
        if objectType = typeof<DateTime> ||
           objectType = typeof<DateTime option> then
            true
        else
            false
    
    override __.ReadJson(reader, objectType, _, _) =
        let convertToDateTime (seconds: int64) =
            if seconds > 0L then
                let resultType =
                    if isNullable objectType then
                        Nullable.GetUnderlyingType(objectType)
                    else
                        objectType
                let unixTime = UnixEpoch.AddSeconds(float seconds)
                if isOption resultType then
                    unixTime
                    |> Some
                    |> box
                else
                    unixTime
                    |> box
            else
                null
                
        match reader.TokenType with
        | JsonToken.Null ->
            null
        | JsonToken.Integer ->
            convertToDateTime(unbox reader.Value)
        | JsonToken.String ->
            let success, value = Int64.TryParse(unbox reader.Value)
            if success then
                convertToDateTime(value)
            else
                null
        | _ ->
            null
    
    override __.WriteJson(writer, value, serializer) =
        let value =
            match value with
            | :? Option<DateTime> as date ->
                match date with
                | Some x ->
                    getUnixTime x
                    |> box
                | None ->
                    null
            | :? DateTime as date ->
                getUnixTime date
                |> box
            | _ ->
                null
        serializer.Serialize(writer, value)