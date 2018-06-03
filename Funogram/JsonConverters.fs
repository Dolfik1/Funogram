module internal Funogram.JsonConverters

open Microsoft.FSharp.Reflection
open Newtonsoft.Json
open System
open System.Reflection
open Newtonsoft.Json.Linq
open System.Collections.Generic
open Newtonsoft.Json.Serialization

let getSnakeCaseName (name : string) = 
    let chars = 
        seq { 
            let chars = name.ToCharArray()
            for i in 0..chars.Length - 1 do
                if i > 0 && Char.IsUpper(chars.[i]) then yield '_'
                yield chars.[i]
        }
    String.Concat(chars).ToLower()

type OptionConverter() = 
    inherit JsonConverter()
    override x.CanConvert(t) = 
        t.GetTypeInfo().IsGenericType 
        && t.GetGenericTypeDefinition() = typedefof<option<_>>
    
    override x.WriteJson(writer, value, serializer) = 
        let value = 
            if isNull (value) then null
            else 
                let _, fields = 
                    FSharpValue.GetUnionFields(value, value.GetType())
                fields.[0]
        serializer.Serialize(writer, value)
    
    override x.ReadJson(reader, t, existingValue, serializer) = 
        let innerType = t.GetTypeInfo().GetGenericArguments().[0]
        
        let innerType = 
            if innerType.GetTypeInfo().IsValueType then 
                (typedefof<Nullable<_>>).MakeGenericType([| innerType |])
            else innerType
        
        let value = serializer.Deserialize(reader, innerType)
        let cases = FSharpType.GetUnionCases(t)
        if isNull (value) then FSharpValue.MakeUnion(cases.[0], [||])
        else FSharpValue.MakeUnion(cases.[1], [| value |])

type DuConverter() = 
    inherit JsonConverter()
    
    let getCasesTypes (cases : UnionCaseInfo []) = 
        cases |> Array.map (fun f -> 
                     let fields = f.GetFields()
                     if fields
                        |> isNull
                        || fields.Length = 0 then (f, null)
                     else (f, fields.[0].PropertyType))
    
    let getTypeProps (t : Type) (serializer : JsonSerializer) = 
        let props = t.GetTypeInfo().GetProperties()
        if serializer.ContractResolver :? DefaultContractResolver then 
            let cr = (serializer.ContractResolver :?> DefaultContractResolver)
            let joc = cr.ResolveContract(t) :?> JsonObjectContract
            joc.Properties 
            |> Seq.map 
                   (fun f -> 
                   (f.PropertyName, 
                    (props |> Seq.findBack (fun h -> h.Name = f.UnderlyingName))))
        else props |> Seq.map (fun f -> (f.Name, f))
    
    let isRequiredType (t : Type) = 
        t.GetTypeInfo().IsGenericType 
        && t.GetGenericTypeDefinition() = typedefof<option<_>>
    
    let rec isJsonObjectAndTypeEquals (t : Type) (jObject : JObject) 
            (serializer : JsonSerializer) = 
        let jsonProps = (jObject :> seq<KeyValuePair<string, JToken>>)
        let sourceProps = getTypeProps t serializer
        let isJsonEquals() = 
            jsonProps 
            |> Seq.forall 
                   (fun f -> 
                   not (sourceProps |> Seq.forall (fun (k, _) -> k <> f.Key)))
        
        let isTypeEquals() = 
            sourceProps
            |> Seq.where (fun (_, v) -> isRequiredType (v.GetType()))
            |> Seq.forall 
                   (fun (k, _) -> 
                   not (jsonProps |> Seq.forall (fun v -> v.Key <> k)))
        if not (isJsonEquals()) || not (isTypeEquals()) then false
        else 
            let objs = jsonProps |> Seq.where (fun f -> f.Value.HasValues)
            if objs |> Seq.isEmpty then true
            else 
                let getType name = 
                    let _, x = 
                        sourceProps |> Seq.findBack (fun (k, _) -> k = name)
                    x.PropertyType
                objs 
                |> Seq.forall 
                       (fun f -> 
                       isJsonObjectAndTypeEquals (getType f.Key) 
                           (f.Value :?> JObject) serializer)
    
    override x.CanConvert(t) = FSharpType.IsUnion(t)
    
    override x.WriteJson(writer, value, serializer) = 
        let t = value.GetType()
        let caseInfo, fieldValues = FSharpValue.GetUnionFields(value, t)
        
        let getCaseInfoName() = 
            let snakeCase = 
                t.GetTypeInfo().CustomAttributes 
                |> Seq.exists 
                       (fun f -> 
                       f.AttributeType = typeof<SnakeCaseNamingStrategy>)
            if snakeCase then getSnakeCaseName caseInfo.Name
            else caseInfo.Name
        
        let value = 
            match fieldValues.Length with
            | 0 -> getCaseInfoName() :> obj
            | 1 -> fieldValues.[0]
            | _ -> fieldValues :> obj
        
        serializer.Serialize(writer, value)
    
    override x.ReadJson(reader, t, existingValue, serializer) = 
        //let jObject = JObject.Load(reader);
        let cases = FSharpType.GetUnionCases t
        let casesTypes = getCasesTypes cases
        let isSimpleDu = casesTypes |> Seq.forall (fun (_, x) -> x |> isNull)
        if isSimpleDu && reader.ValueType = typedefof<string> then 
            let name = reader.Value :?> string
            match casesTypes |> Array.tryFind (fun (_, f) -> f.Name
                                                             |> getSnakeCaseName = name) with
            | Some(u, t) -> FSharpValue.MakeUnion(u, [||])
            | None -> null
        else if reader.ValueType
                |> isNull
                |> not
        then // if type known
            match casesTypes 
                  |> Array.tryFind (fun (_, f) -> f = reader.ValueType) with
            | Some(u, t) -> 
                FSharpValue.MakeUnion
                    (u, [| serializer.Deserialize(reader, t) |])
            | None -> null
        else 
            let jObject = JObject.Load(reader)
            let tp = 
                casesTypes 
                |> Seq.tryFind 
                       (fun (_, t) -> 
                       isJsonObjectAndTypeEquals t jObject serializer)
            match tp with
            | Some(u, t) -> 
                FSharpValue.MakeUnion(u, [| jObject.ToObject(t, serializer) |])
            | None -> null