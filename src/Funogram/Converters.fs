namespace Funogram

open System
open System.Collections.Generic
open System.IO
open System.Runtime.CompilerServices
open System.Runtime.Serialization
open System.Text.Json
open System.Text.Json.Serialization
open TypeShape.Core

[<assembly:InternalsVisibleTo("Funogram.Tests")>]
do ()
module internal Converters =
  open Funogram.StringUtils

  let private unixEpoch = DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
  let toUnix (x: DateTime) = (x.ToUniversalTime() - unixEpoch).TotalSeconds |> int64
  
  let mkMemberSerializer (case: ShapeFSharpUnionCase<'DeclaringType>) =
    let isFile =
      case.Fields.Length = 2
        && case.Fields[0].Member.Type = typeof<string>
        && (case.Fields[1].Member.Type = typeof<Stream> || case.Fields[1].Member.Type = typeof<byte[]>)
    
    if case.Fields.Length = 0 then
      fun (writer: Utf8JsonWriter) _ _ ->
        let name = toSnakeCase case.CaseInfo.Name
        writer.WriteStringValue(name)
    else
      case.Fields[0].Accept { new IMemberVisitor<'DeclaringType, Utf8JsonWriter -> 'DeclaringType -> JsonSerializerOptions -> unit> with
        member _.Visit (shape : ShapeMember<'DeclaringType, 'Field>) =
          fun writer value options ->
            if isFile then
              let str = box (shape.Get value) |> unbox<string>
              writer.WriteStringValue($"attach://{str}")
            else
              let v = shape.Get value
              let converter = options.GetConverter(v.GetType())
              let c = converter :?> JsonConverter<'a>
              c.Write(writer, v, options)
      }
  
  [<Interface>]
  type IUnionDeserializer<'T> =
    abstract member Deserialize: reader: byref<Utf8JsonReader> * options: JsonSerializerOptions -> 'T
  
  type CaseFullDeserializer<'DeclaringType, 'Field>(shape: ShapeMember<'DeclaringType, 'Field>, init: unit -> 'DeclaringType) =
    member x.Deserialize(reader: byref<Utf8JsonReader>, options: JsonSerializerOptions) =
      let converter = options.GetConverter(typeof<'DeclaringType>) :?> JsonConverter<'Field>
      converter.Read(&reader, typeof<'DeclaringType>, options) |> shape.Set (init ())

  type CaseNameDeserializer<'DeclaringType>(init: unit -> 'DeclaringType) =
    member x.Deserialize(reader: byref<Utf8JsonReader>, _: JsonSerializerOptions) =
      reader.Read() |> ignore
      init ()

  let mkMemberDeserializer (case: ShapeFSharpUnionCase<'DeclaringType>) (init: unit -> 'DeclaringType) =
    if case.Fields.Length = 0 then
      { new IUnionDeserializer<'DeclaringType> with
          member x.Deserialize(reader, _) =
            //reader.Read() |> ignore
            init ()
      }
    else
      case.Fields[0].Accept { new IMemberVisitor<'DeclaringType, IUnionDeserializer<'DeclaringType>> with
          member x.Visit (shape: ShapeMember<'DeclaringType, 'Field>) =
            { new IUnionDeserializer<'DeclaringType> with
                member x.Deserialize(reader, options) =
                  let converter = options.GetConverter(typeof<'Field>)
                  let converter = converter :?> JsonConverter<'Field>
                  converter.Read(&reader, typeof<'Field>, options) |> shape.Set (init ())
            }
      }

  type DiscriminatedUnionConverter<'a>() =
    inherit JsonConverter<'a>()
    
    let shape = shapeof<'a>
    let union =
      match shape with
      | Shape.FSharpUnion (:? ShapeFSharpUnion<'a> as union) -> union
      | _ -> failwith "Unsupported type"
    
    let enumUnion = union.UnionCases |> Seq.forall (fun x -> x.Fields.Length = 0)
    
    let cases =
      union.UnionCases
      |> Seq.map (fun c ->
        if c.Fields.Length = 0 then
          let dataMember =
            c.CaseInfo.GetCustomAttributes(typeof<DataMemberAttribute>)
            |> Seq.cast<DataMemberAttribute>
            |> Seq.filter (fun x -> String.IsNullOrEmpty(x.Name) |> not)
            |> Seq.toArray
          
          let name =
            if dataMember.Length > 0
            then dataMember[0].Name
            else c.CaseInfo.Name |> toSnakeCase
          (Set.ofList [name], None)
        else
          let tp = c.Fields[0].Member.Type
          if tp.IsPrimitive then (Set.empty, Some tp)
          else (tp.GetProperties()
                |> Seq.map(fun x -> x.Name |> toSnakeCase)
                |> Set.ofSeq,
                None))
      |> Seq.toArray
    
    let serializers =
      union.UnionCases
      |> Seq.map mkMemberSerializer
      |> Seq.toArray
      
    let deserializers =
      union.UnionCases
      |> Seq.map (fun case -> mkMemberDeserializer case case.CreateUninitialized)
      |> Seq.toArray
    
    override x.Write(writer, value, options) =
      let serialize = serializers[union.GetTag value] // all union cases
      serialize writer value options
    
    member x.ReadCasesOnly(reader: byref<Utf8JsonReader>) =
      let mutable types: Type list = []
      let caseNames = List<string>()

      let reader = reader // copy reader
      let mutable loop = true
      let mutable first = true
      
      if reader.TokenType = JsonTokenType.StartObject then
        reader.Read() |> ignore
      
      while loop do
        let token = reader.TokenType
        if first && not enumUnion then
          loop <- false
          types <-
            match token with
            | JsonTokenType.True | JsonTokenType.False -> [typeof<bool>]
            | JsonTokenType.String -> [typeof<string>]
            | JsonTokenType.Number -> [typeof<int>;typeof<int64>;typeof<float32>;typeof<float>]
            | _ ->
              types
          
          if types.Length > 0 then
            loop <- false

          first <- false

        if enumUnion then
          caseNames.Add(reader.GetString())
          reader.Read() |> ignore
          loop <- false
        else
          match token with
          | JsonTokenType.PropertyName ->
            caseNames.Add(reader.GetString())
          | JsonTokenType.StartObject
          | JsonTokenType.StartArray ->
            reader.Skip()
          | _ -> ()

          loop <- reader.Read()

      caseNames, types
    
    override x.Read(reader, _, options) =
      let jsonCaseNames, jsonCaseTypes = x.ReadCasesOnly(&reader)

      let idx =
        cases
        |> Array.tryFindIndex (fun (caseNames, tp) ->
          (jsonCaseTypes.Length = 0 || (tp.IsSome && jsonCaseTypes |> Seq.contains tp.Value))
          && jsonCaseNames |> Seq.forall (fun n ->
             caseNames |> Set.contains n))
      
      match idx with
      | Some idx ->
        deserializers[idx].Deserialize(&reader, options)
      | None ->
        // try to find most similar type
        let item =
          cases |> Array.maxBy (fun (caseNames, tp) ->
            if jsonCaseTypes.Length = 0 || (tp.IsSome && jsonCaseTypes |> Seq.contains tp.Value) then
              jsonCaseNames |> Seq.sumBy (fun n -> if caseNames |> Set.contains n then 1 else 0)
            else
              -1
          )
      
        let idx = cases |> Array.findIndex (fun x -> x = item)
        deserializers[idx].Deserialize(&reader, options)
    
    override x.CanConvert(typeToConvert) =
      match TypeShape.Create(typeToConvert) with
      | Shape.FSharpOption _ -> false
      | Shape.FSharpUnion _ -> true
      | _ -> false

  type DiscriminatedUnionConverterFactory() =
    inherit JsonConverterFactory()

    override x.CreateConverter(typeToConvert, _) =
      let g = typedefof<DiscriminatedUnionConverter<_>>.MakeGenericType(typeToConvert)
      Activator.CreateInstance(g) :?> JsonConverter
      
    override x.CanConvert(typeToConvert) =
      match TypeShape.Create(typeToConvert) with
      | Shape.FSharpOption _ -> false
      | Shape.FSharpUnion _ -> true
      | _ -> false

  type UnixTimestampDateTimeConverter() =
    inherit JsonConverter<DateTime>()
    let unixEpoch = DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)

    override x.Read(reader, _, _) =
      let v = reader.GetInt64() |> float
      unixEpoch.AddSeconds(v)
      
    override x.Write(writer, value, _) =
      writer.WriteNumberValue(toUnix value)