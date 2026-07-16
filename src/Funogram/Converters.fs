namespace Funogram

open System
open System.Collections.Generic
open System.Collections.ObjectModel
open System.IO
open System.Linq
open System.Reflection
open System.Runtime.CompilerServices
open System.Text.Json
open System.Text.Json.Serialization
open Funogram.Types
open TypeShape.Core
open TypeShape.Core.SubtypeExtensions

[<assembly:InternalsVisibleTo("Funogram.Tests")>]
do ()
module internal Converters =
  open Funogram.StringUtils

  let private EmptyProps = List<string * string option>().AsReadOnly()
  let private unixEpoch = DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
  let toUnix (x: DateTime) = (x.ToUniversalTime() - unixEpoch).TotalSeconds |> int64
  
  let mkMemberSerializer (case: ShapeFSharpUnionCase<'DeclaringType>) =
    let isFile =
      case.Fields.Length = 2
        && case.Fields[0].Member.Type = typeof<string>
        && (case.Fields[1].Member.Type = typeof<Stream> || case.Fields[1].Member.Type = typeof<byte[]>)
    
    if case.Fields.Length = 0 then
      let name = caseName case.CaseInfo
      fun (writer: Utf8JsonWriter) _ _ ->
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

  [<RequireQualifiedAccess>]
  type private CaseDescriptor =
    | Nullary of name: string
    | Scalar  of clrType: Type
    | Array
    | Object of Map<string, string option> // key is prop name and value is prop "always" value

  [<RequireQualifiedAccess>]
  type private JsonShape =
    | String of value: string
    | Scalar of candidateTypes: Type list
    | Array
    | Object of props: ReadOnlyCollection<string * string option>

  let private fsharpListTypeDef = typedefof<_ list>

  let private isArrayLike (t: Type) =
    t.IsArray
    || (t.IsGenericType && t.GetGenericTypeDefinition() = fsharpListTypeDef)

  type DiscriminatedUnionConverter<'a>() =
    inherit JsonConverter<'a>()
    
    let shape = shapeof<'a>
    let union =
      match shape with
      | Shape.FSharpUnion (:? ShapeFSharpUnion<'a> as union) -> union
      | _ -> failwith "Unsupported type"

    let cases =
      union.UnionCases
      |> Seq.map (fun c ->
        if c.Fields.Length = 0 then
          CaseDescriptor.Nullary (caseName c.CaseInfo)
        else
          let tp = c.Fields[0].Member.Type
          if tp.IsPrimitive || tp = typeof<string> then
            CaseDescriptor.Scalar tp
          elif isArrayLike tp then
            CaseDescriptor.Array
          else
            tp.GetProperties()
            |> Seq.map (fun x ->
                (x.Name |> toSnakeCase, x.GetCustomAttribute<AlwaysAttribute>() |> Option.ofObj |> Option.map _.Value)
            )
            |> Map.ofSeq
            |> CaseDescriptor.Object)
      |> Seq.toArray
    
    let alwaysFields =
      cases
      |> Seq.collect (fun x ->
        match x with
        | CaseDescriptor.Object o -> o |> Seq.choose (fun x -> if x.Value.IsSome then Some x.Key else None)
        | _ -> Seq.empty
      )
      |> Set.ofSeq
    
    let casesAlwaysIndices =
      cases
      |> Seq.mapi (fun i x ->
        match x with
        | CaseDescriptor.Object props ->
          let requiredFields =
            props
            |> Seq.choose (fun kv -> if kv.Value.IsSome then Some(kv.Key, kv.Value.Value) else None)
            |> Set.ofSeq
          (requiredFields, i)
        | _ -> (Set.empty, i)
      ) |> Map.ofSeq
    
    let serializers =
      union.UnionCases
      |> Seq.map mkMemberSerializer
      |> Seq.toArray
      
    let deserializers =
      union.UnionCases
      |> Seq.map (fun case -> mkMemberDeserializer case case.CreateUninitialized)
      |> Seq.toArray
    
    let resolveObject (props: ReadOnlyCollection<string * string option>): int option =
      let requiredFields =
        if alwaysFields.IsEmpty then
          Set.empty
        else
          props
          |> Seq.choose (fun (name, value) -> if value.IsSome then Some(name, value.Value) else None)
          |> Set.ofSeq

      let exact =
        if requiredFields.IsEmpty |> not then
          match casesAlwaysIndices.TryGetValue(requiredFields) with
          | true, idx -> Some idx
          | _ -> None
        else
          cases
          |> Array.tryFindIndex (function
             | CaseDescriptor.Object known -> props.All(fun (name, _) -> known.ContainsKey(name))
             | _ -> false)
        
      match exact with
      | Some _ -> exact
      | None ->
        let scored =
          cases
          |> Array.mapi (fun i c ->
            match c with
            | CaseDescriptor.Object known ->
              i, props.Sum(fun (n, _) -> if known.ContainsKey n then 1 else 0)
            | _ -> i, -1)
        let i, best = scored |> Array.maxBy snd
        if best < 0 then None else Some i

    
    override x.Write(writer, value, options) =
      let serialize = serializers[union.GetTag value] // all union cases
      serialize writer value options

    member private _.ReadShape(reader: byref<Utf8JsonReader>) : JsonShape =
      let reader = reader
      match reader.TokenType with
      | JsonTokenType.String ->
        JsonShape.String (reader.GetString())
      | JsonTokenType.True
      | JsonTokenType.False ->
        JsonShape.Scalar [ typeof<bool> ]
      | JsonTokenType.Number ->
        JsonShape.Scalar [ typeof<int>; typeof<int64>; typeof<float32>; typeof<float> ]
      | JsonTokenType.StartArray ->
        JsonShape.Array
      | JsonTokenType.StartObject ->
        let props = List<string * string option>()
        let mutable loop = reader.Read()
        while loop do
          match reader.TokenType with
          | JsonTokenType.PropertyName ->
            let name = reader.GetString()
            loop <- reader.Read()
            if alwaysFields.Contains(name) && loop then
              match reader.TokenType with
              | JsonTokenType.String ->
                props.Add(name, Some (reader.GetString()))
                loop <- reader.Read()
              | _ ->
                props.Add(name, None)
            else
              props.Add(name, None)
          | JsonTokenType.StartObject
          | JsonTokenType.StartArray ->
            reader.Skip()
            loop <- reader.Read()
          | JsonTokenType.EndObject ->
            loop <- false
          | _ ->
            loop <- reader.Read()
        JsonShape.Object (props.AsReadOnly())
      | _ ->
        JsonShape.Object EmptyProps
    
    override x.Read(reader, _, options) =

      let idx =
        match x.ReadShape(&reader) with
        | JsonShape.String value ->
          cases
          |> Array.tryFindIndex (function CaseDescriptor.Scalar t -> t = typeof<string> | _ -> false)
          |> Option.orElseWith (fun () ->
            cases |> Array.tryFindIndex (function CaseDescriptor.Nullary n -> n = value | _ -> false))
        | JsonShape.Scalar candidateTypes ->
          cases
          |> Array.tryFindIndex (function
             | CaseDescriptor.Scalar t -> candidateTypes |> List.contains t
             | _ -> false)
        | JsonShape.Array ->
          cases |> Array.tryFindIndex (function CaseDescriptor.Array -> true | _ -> false)
        | JsonShape.Object props ->
          resolveObject props

      match idx with
      | Some i ->
        deserializers[i].Deserialize(&reader, options)
      | None ->
        raise (JsonException($"Unable to match JSON to any case of union {typeof<'a>.Name}"))
    
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
  
  type OptionConverter<'T>() =
    inherit JsonConverter<Option<'T>>()

    override x.Read(reader, _, options) =
      match reader.TokenType with
      | JsonTokenType.Null ->
        None
      | _ ->
        let converter = options.GetConverter(typeof<'T>)
        let c = converter :?> JsonConverter<'T>
        c.Read(&reader, typeof<'T>, options) |> Some

    override x.Write(writer, value, options) =
      match value with
      | Some v ->
        let converter = options.GetConverter(typeof<'T>)
        let c = converter :?> JsonConverter<'T>
        c.Write(writer, v, options)
      | None ->
        writer.WriteNullValue()

  // The FSharpOptionTypeConverter in STJ seems to be broken
  // There is no stable repro to check this, so I used my own Option converter
  type OptionConverterFactory() =
    inherit JsonConverterFactory()

    override x.CreateConverter(typeToConvert, _) =
      let innerType = typeToConvert.GetGenericArguments()[0]
      let optionConverterType = typedefof<OptionConverter<_>>.MakeGenericType(innerType)
      Activator.CreateInstance(optionConverterType) :?> JsonConverter
      
    override x.CanConvert(typeToConvert) =
      match TypeShape.Create(typeToConvert) with
      | Shape.FSharpOption _ -> true
      | _ -> false