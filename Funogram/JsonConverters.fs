namespace Funogram

open Microsoft.FSharp.Reflection
open Newtonsoft.Json
open System
open System.Reflection

type OptionConverter() =
    inherit JsonConverter()
    
    override x.CanConvert(t) = 
        t.GetTypeInfo().IsGenericType && t.GetGenericTypeDefinition() = typedefof<option<_>>

    override x.WriteJson(writer, value, serializer) =
        let value =
            if isNull(value) then null
            else
                let _,fields = FSharpValue.GetUnionFields(value, value.GetType())
                fields.[0]
        serializer.Serialize(writer, value)

    override x.ReadJson(reader, t, existingValue, serializer) =      
        let innerType = t.GetTypeInfo().GetGenericArguments().[0]
        let innerType =
            if innerType.GetTypeInfo().IsValueType then (typedefof<Nullable<_>>).MakeGenericType([|innerType|])
            else innerType
        let value = serializer.Deserialize(reader, innerType)
        let cases = FSharpType.GetUnionCases(t)
        if isNull(value) then FSharpValue.MakeUnion(cases.[0], [||])
        else FSharpValue.MakeUnion(cases.[1], [|value|])

type DuConverter() = 
    inherit JsonConverter()
    
    override x.CanConvert(t) =
        FSharpType.IsUnion(t)
        
    override x.WriteJson(writer, value, serializer) = 
        let t = value.GetType()
        let caseInfo, fieldValues = FSharpValue.GetUnionFields(value, t)

        let value =
            match fieldValues.Length with
            | 0 -> null
            | 1 -> fieldValues.[0]
            | _ -> fieldValues :> obj
        serializer.Serialize(writer, value)
    
    override x.ReadJson(reader, t, existingValue, serializer) = 
        failwith "Not implemented!"