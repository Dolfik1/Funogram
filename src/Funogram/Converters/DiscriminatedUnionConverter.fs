namespace Funogram.Converters

open System
open System.Collections.Generic
open System.Collections.ObjectModel
open System.IO
open System.Reflection
open System.Runtime.CompilerServices
open System.Text.Json
open System.Text.Json.Serialization
open Funogram.Types
open TypeShape.Core
open Funogram.StringUtils

[<assembly:InternalsVisibleTo("Funogram.Tests")>]
do ()

[<Interface>]
type internal IUnionDeserializer<'T> =
  abstract member Deserialize: reader: byref<Utf8JsonReader> * options: JsonSerializerOptions -> 'T
  
[<RequireQualifiedAccess>]
type private CaseShape =
  | Nullary
  | Scalar of clrType: Type
  | Array
  | Object of ObjectShape
and private ObjectShape =
  { PayloadType: Type
    Props: Map<string, string option>
    Discriminator: Set<string * string> }

type private CaseState<'a> =
  { Tag: int
    Name: string
    Shape: CaseShape
    Serialize: Utf8JsonWriter -> 'a -> JsonSerializerOptions -> unit
    Deserializer: IUnionDeserializer<'a> }

[<RequireQualifiedAccess>]
type private Resolution<'a> =
  | Exact of state: CaseState<'a>
  | Guessed of state: CaseState<'a>
  | UnknownDiscriminator of found: Set<string * string>
  | NoMatch

[<RequireQualifiedAccess>]
type private JsonShape =
  | String of value: string
  | Scalar of candidateTypes: Type list
  | Array
  | Object of props: ReadOnlyCollection<string * string option>

module private DiscriminatedUnionConverter =
  let mkCachedConverter<'T> () =
    let cell = ref (null: JsonConverter<'T>)
    fun (options: JsonSerializerOptions) ->
      if isNull cell.Value then
        cell.Value <- options.GetConverter(typeof<'T>) :?> JsonConverter<'T>
      cell.Value

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
          let converter = mkCachedConverter<'Field> ()
          fun writer value options ->
            if isFile then
              let str = box (shape.Get value) |> unbox<string>
              writer.WriteStringValue($"attach://{str}")
            else
              (converter options).Write(writer, shape.Get value, options)
      }
  
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
            let converter = mkCachedConverter<'Field> ()
            { new IUnionDeserializer<'DeclaringType> with
                member x.Deserialize(reader, options) =
                  (converter options).Read(&reader, typeof<'Field>, options) |> shape.Set (init ())
            }
      }

  let EmptyProps = List<string * string option>().AsReadOnly()
  
  [<Literal>]
  let private PropsBindingFlags = BindingFlags.Public ||| BindingFlags.Instance
  
  let private fsharpListTypeDef = typedefof<_ list>
  
  let isArrayLike (t: Type) =
    t.IsArray
    || (t.IsGenericType && t.GetGenericTypeDefinition() = fsharpListTypeDef)

  let mkObjectShape (payloadType: Type) =
    let clrProps =
      payloadType.GetProperties(PropsBindingFlags)
      |> Array.filter (fun p -> p.CanRead && p.GetIndexParameters().Length = 0)

    let props =
      clrProps
      |> Seq.map (fun p ->
        toSnakeCase p.Name,
        p.GetCustomAttribute<AlwaysAttribute>() |> Option.ofObj |> Option.map _.Value)
      |> Map.ofSeq

    { PayloadType = payloadType
      Props = props
      Discriminator =
        props
        |> Seq.choose (fun kv -> kv.Value |> Option.map (fun v -> kv.Key, v))
        |> Set.ofSeq }

  let mkCaseShape (c: ShapeFSharpUnionCase<'a>): CaseShape =
    match c.Fields with
    | [||] -> CaseShape.Nullary
    | fields ->
      let tp = fields[0].Member.Type
      if tp.IsPrimitive || tp = typeof<string> then CaseShape.Scalar tp
      elif isArrayLike tp then CaseShape.Array
      else CaseShape.Object (mkObjectShape tp)

  let mkCaseStates (union: ShapeFSharpUnion<'a>) =
    union.UnionCases
    |> Array.map (fun c ->
      { Tag = c.CaseInfo.Tag
        Name = caseName c.CaseInfo
        Shape = mkCaseShape c
        Serialize = mkMemberSerializer c
        Deserializer = mkMemberDeserializer c c.CreateUninitialized })

  let describeFound (found: Set<string * string>) =
    found |> Seq.map (fun (n, v) -> $"{n}={v}") |> String.concat ", "

open DiscriminatedUnionConverter

type internal DiscriminatedUnionConverter<'a>() =
  inherit JsonConverter<'a>()
  
  let union =
    match shapeof<'a> with
    | Shape.FSharpUnion (:? ShapeFSharpUnion<'a> as union) -> union
    | _ -> failwith $"Unsupported type: {typeof<'a>.FullName}"
  
  let cases = mkCaseStates union |> Array.sortBy _.Tag

  let alwaysFields =
    cases
    |> Seq.collect (fun s ->
      match s.Shape with
      | CaseShape.Object o -> o.Discriminator |> Seq.map fst
      | _ -> Seq.empty)
    |> Set.ofSeq

  let objectShapes =
    cases
    |> Array.choose (fun s ->
      match s.Shape with
      | CaseShape.Object o -> Some (s, o)
      | _ -> None)

  let hasDiscriminator = not alwaysFields.IsEmpty

  let byDiscriminator =
    objectShapes
    |> Seq.filter (fun (_, o) -> not o.Discriminator.IsEmpty)
    |> Seq.map (fun (s, o) -> o.Discriminator, s)
    |> Map.ofSeq

  let knownDiscriminators =
    byDiscriminator.Keys
    |> Seq.map describeFound
    |> Seq.sort
    |> String.concat " | "
  
  let resolveObject (props: ReadOnlyCollection<string * string option>) : Resolution<'a> =
    let found =
      if not hasDiscriminator then Set.empty
      else
        props
        |> Seq.choose (fun (name, value) -> value |> Option.map (fun v -> name, v))
        |> Set.ofSeq

    if not found.IsEmpty then
      match byDiscriminator.TryGetValue found with
      | true, state -> Resolution.Exact state
      | _ -> Resolution.UnknownDiscriminator found
    else
      let subset =
        objectShapes
        |> Array.tryFind (fun (_, o) -> props |> Seq.forall (fun (name, _) -> o.Props.ContainsKey name))
      match subset with
      | Some (state, _) -> Resolution.Exact state
      | None ->
        let scored =
          objectShapes
          |> Array.map (fun (state, o) ->
            state, props |> Seq.sumBy (fun (name, _) -> if o.Props.ContainsKey name then 1 else 0))
        if Array.isEmpty scored then Resolution.NoMatch
        else
          let state, best = scored |> Array.maxBy snd
          if best = 0 then Resolution.NoMatch else Resolution.Guessed state

  member private _.ReadShape(reader: byref<Utf8JsonReader>) : JsonShape =
    let mutable reader = reader
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
          if alwaysFields.Contains name && loop && reader.TokenType = JsonTokenType.String then
            props.Add(name, Some (reader.GetString()))
            loop <- reader.Read()
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
  
  member private _.ResolveCase(shape: JsonShape) : CaseState<'a> =
    match shape with
    | JsonShape.String value ->
      let scalar =
        cases |> Array.tryFind (fun s ->
          match s.Shape with CaseShape.Scalar t -> t = typeof<string> | _ -> false)
      let nullary () =
        cases |> Array.tryFind (fun s ->
          match s.Shape with CaseShape.Nullary -> s.Name = value | _ -> false)
      match scalar |> Option.orElseWith nullary with
      | Some s -> s
      | None ->
        raise (JsonException($"No case of union {typeof<'a>.Name} accepts the string '{value}'"))

    | JsonShape.Scalar candidateTypes ->
      let found =
        cases |> Array.tryFind (fun s ->
          match s.Shape with
          | CaseShape.Scalar t -> candidateTypes |> List.contains t
          | _ -> false)
      match found with
      | Some s -> s
      | None ->
        raise (JsonException($"No scalar case of union {typeof<'a>.Name} matches this JSON value"))

    | JsonShape.Array ->
      match cases |> Array.tryFind (fun s -> s.Shape = CaseShape.Array) with
      | Some s -> s
      | None ->
        raise (JsonException($"No array case of union {typeof<'a>.Name}"))

    | JsonShape.Object props ->
      match resolveObject props with
      | Resolution.Exact state -> state
      | Resolution.Guessed state when not hasDiscriminator -> state
      | Resolution.Guessed _ ->
        raise (JsonException($"Union {typeof<'a>.Name} requires a discriminator, but the payload has none"))
      | Resolution.UnknownDiscriminator found ->
        raise (JsonException($"Unknown discriminator {describeFound found} for union {typeof<'a>.Name}; known values: {knownDiscriminators}"))
      | Resolution.NoMatch ->
        raise (JsonException($"Unable to match JSON to any case of union {typeof<'a>.Name}"))

  override this.Read(reader, _, options) =
    let state = this.ResolveCase(this.ReadShape(&reader))
    state.Deserializer.Deserialize(&reader, options)

  override x.Write(writer, value, options) =
    cases[union.GetTag value].Serialize writer value options

type internal DiscriminatedUnionConverterFactory() =
  inherit JsonConverterFactory()

  override x.CreateConverter(typeToConvert, _) =
    let g = typedefof<DiscriminatedUnionConverter<_>>.MakeGenericType(typeToConvert)
    Activator.CreateInstance(g) :?> JsonConverter
   
  override x.CanConvert(typeToConvert) =
    match TypeShape.Create(typeToConvert) with
    | Shape.FSharpOption _ -> false
    | Shape.FSharpUnion _ -> true
    | _ -> false