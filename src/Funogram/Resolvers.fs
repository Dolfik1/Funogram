namespace Funogram

open System
open System.IO
open System.Runtime.CompilerServices
open System.Runtime.Serialization
open System.Text
open Utf8Json

[<assembly:InternalsVisibleTo("Funogram.Tests")>]
do ()
module internal Resolvers =

  open TypeShape.Core

  let getSnakeCaseName (name: string) =
    let chars =
      seq {
        let chars = name.ToCharArray()
        for i in 0 .. chars.Length - 1 do
          let c = chars.[i]
          if Char.IsUpper(c) then
            if i = 0 then
              yield Char.ToLowerInvariant(c)
            else if (Char.IsUpper(chars.[i - 1])) then
              yield Char.ToLowerInvariant(c)
            else
              yield '_'
              yield Char.ToLowerInvariant(c)
          else yield c
    }
    String.Concat(chars).ToLower()

  let mkMemberSerializer (case: ShapeFSharpUnionCase<'DeclaringType>) =
    let isFile = case.Fields |> Array.map (fun x -> x.Member.Type) = [|typeof<string>; typeof<Stream>|]


    if case.Fields.Length = 0 then
      let dataMember =
          case.CaseInfo.GetCustomAttributes(typeof<DataMemberAttribute>)
          |> Seq.cast<DataMemberAttribute>
          |> Seq.filter (fun x -> String.IsNullOrEmpty(x.Name) |> not)
          |> Seq.toArray
      if dataMember.Length > 0 then fun _ _ ->
        Encoding.UTF8.GetBytes(dataMember.[0].Name |> sprintf "\"%s\"")
      else fun _ _ ->
        Encoding.UTF8.GetBytes(getSnakeCaseName case.CaseInfo.Name |> sprintf "\"%s\"")
    else
      case.Fields.[0].Accept { new IMemberVisitor<'DeclaringType, 'DeclaringType -> IJsonFormatterResolver -> byte[]> with
        member __.Visit (shape : ShapeMember<'DeclaringType, 'Field>) =
          fun value resolver ->
            let mutable myWriter = JsonWriter()

            if isFile then
              let str = box (shape.Get value) |> unbox<string>
              myWriter.WriteString(sprintf "attach://%s" str)
            else
              resolver.GetFormatterWithVerify<'Field>()
                .Serialize(&myWriter, shape.Get value, resolver)
            myWriter.ToUtf8ByteArray()
      }

  let mkMemberDeserializer (case: ShapeFSharpUnionCase<'DeclaringType>) (init: unit -> 'DeclaringType) =
    if case.Fields.Length = 0 then
      fun value offset _ ->
        let mutable myReader = JsonReader(value, offset)
        myReader.ReadNext()
        let offset = myReader.GetCurrentOffsetUnsafe()
        (init(), offset)
    else
      case.Fields.[0].Accept { new IMemberVisitor<'DeclaringType, byte[] -> int -> IJsonFormatterResolver -> ('DeclaringType * int)> with
        member __.Visit (shape : ShapeMember<'DeclaringType, 'Field>) =
          fun value offset resolver ->
            let mutable myReader = JsonReader(value, offset)
            let v =
              resolver.GetFormatterWithVerify<'Field>()
                .Deserialize(&myReader, resolver)
            let offset = myReader.GetCurrentOffsetUnsafe()
            (shape.Set (init()) v, offset)
    }

  type FunogramDiscriminatedUnionFormatter<'a>() =

    let shape = Core.shapeof<'a>
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
            then dataMember.[0].Name
            else c.CaseInfo.Name |> getSnakeCaseName
          (Set.ofList [name], None)
        else
          let tp = c.Fields.[0].Member.Type
          if tp.IsPrimitive then (Set.empty, Some tp)
          else (tp.GetProperties()
                |> Seq.map(fun x -> x.Name |> getSnakeCaseName)
                |> Set.ofSeq,
                None))
      |> Seq.toArray

    let serializers =
      union.UnionCases
      |> Seq.map (fun case -> mkMemberSerializer case)
      |> Seq.toArray

    let deserializers =
      union.UnionCases
      |> Seq.map (fun case -> mkMemberDeserializer case case.CreateUninitialized)
      |> Seq.toArray

    // this serializer/deserializer is used to match union case by set of fields
    interface IJsonFormatter<'a> with
      member x.Serialize(writer, value, resolver) =
        let serialize = serializers.[union.GetTag value] // all union cases
        writer.WriteRaw(serialize value resolver)

      member x.Deserialize(reader, resolver) =
        // read list of properties
        let mutable loop = true

        let buffer = reader.GetBufferUnsafe()
        let offset = reader.GetCurrentOffsetUnsafe()

        let propReader = JsonReader(buffer, offset)
        let mutable readProperty = false
        let mutable level = 0
        let mutable types: Type list = []

        let (jsonCaseNames, jsonCaseTypes) =
          (seq {
            while loop do
              let token = propReader.GetCurrentJsonToken()

              if level = 0 && token <> JsonToken.BeginObject && enumUnion |> not then
                types <-
                  match token with
                  | JsonToken.True | JsonToken.False -> [typeof<bool>]
                  | JsonToken.String ->
                    [typeof<string>]
                  | JsonToken.Number -> [typeof<int>;typeof<int64>;typeof<float32>;typeof<float>]
                  | _ -> failwith "Unknown type!"
                loop <- false
              else if enumUnion then
                yield propReader.ReadString()
                loop <- false
              else
                match token with
                | JsonToken.BeginObject ->
                  level <- level + 1
                  readProperty <- true
                | JsonToken.EndObject ->
                  level <- level - 1
                | JsonToken.ValueSeparator ->
                  readProperty <- true
                | JsonToken.None -> loop <- false
                | _ -> ()

                if readProperty && level = 1 then
                  propReader.ReadNext()
                  yield propReader.ReadPropertyName()
                  readProperty <- false
                else if level = 0 then
                  loop <- false
                else
                  readProperty <- false
                  propReader.ReadNext()
          } |> Seq.toList, types)

        let idx =
          cases
          |> Array.tryFindIndex (fun (caseNames, tp) ->
            (jsonCaseTypes.Length = 0 || (tp.IsSome && jsonCaseTypes |> Seq.contains tp.Value))
            && jsonCaseNames |> Seq.forall (fun n ->
               caseNames |> Set.contains n))
        match idx with
        | Some idx ->
          let value, newOffset = deserializers.[idx] buffer offset resolver
          reader.AdvanceOffset(newOffset - offset)
          value
        | None ->
          failwithf "Internal error: Cannot match type \"%s\" by fields. Please create issue on https://github.com/Dolfik1/Funogram/issues" typeof<'a>.FullName

  let private unixEpoch = DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
  let toUnix (x: DateTime) = (x.ToUniversalTime() - unixEpoch).TotalSeconds |> int64

  type FunogramUnixTimestampDateTimeFormatter() =
    let unixEpoch = DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)
    interface IJsonFormatter<DateTime> with
      member x.Serialize(writer, value, _) =
        writer.WriteInt64(toUnix value)

      member x.Deserialize(reader, _) =
        let v = reader.ReadInt64() |> float
        unixEpoch.AddSeconds(v)

  type FunogramResolver() =
    interface IJsonFormatterResolver with
      member x.GetFormatter<'a>(): IJsonFormatter<'a> =
        match shapeof<'a> with
        | Shape.FSharpOption _ -> null
        | Shape.FSharpUnion _ ->
          FunogramDiscriminatedUnionFormatter<'a>() :> IJsonFormatter<'a>
        | _ -> null

    static member Instance = FunogramResolver()