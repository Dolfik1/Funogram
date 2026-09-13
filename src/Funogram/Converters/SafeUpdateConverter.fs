namespace Funogram.Converters

open System
open System.Reflection
open System.Text.Json
open System.Text.Json.Serialization

type SafeUpdateConverter<'T> internal (innerOptions: JsonSerializerOptions) =
  inherit JsonConverter<'T>()

  static let createMethod : MethodInfo =
    typeof<'T>.GetMethods(BindingFlags.Public ||| BindingFlags.Static)
    |> Array.tryFind (fun m ->
         m.Name = "Create"
         && m.ReturnType = typeof<'T>
         && m.GetParameters() |> Array.exists (fun p -> p.Name = "updateId"))
    |> Option.defaultWith (fun () ->
         failwith $"Static method Create(updateId, ...) not found on {typeof<'T>.FullName}")

  static let createParams = createMethod.GetParameters()

  static let updateIdIndex =
    createParams |> Array.findIndex (fun p -> p.Name = "updateId")

  static let createEmpty (updateId: int64) : 'T =
    let args : obj[] = Array.zeroCreate createParams.Length
    args[updateIdIndex] <- box updateId
    createMethod.Invoke(null, args) :?> 'T

  static let tryGetUpdateId (element: JsonElement) (options: JsonSerializerOptions) =
    if element.ValueKind <> JsonValueKind.Object then None
    else
      [ "update_id"
        if not (isNull options.PropertyNamingPolicy) then
          options.PropertyNamingPolicy.ConvertName "UpdateId"
        "updateId"; "UpdateId" ]
      |> List.tryPick (fun name ->
           match element.TryGetProperty name with
           | true, p when p.ValueKind = JsonValueKind.Number ->
               match p.TryGetInt64() with
               | true, v -> Some v
               | _ -> None
           | _ -> None)

  override _.Read(reader: byref<Utf8JsonReader>, _typeToConvert: Type, options: JsonSerializerOptions) =
    let mutable copy = reader
    let mutable result = Unchecked.defaultof<'T>
    let mutable error : exn = null

    try
      result <- JsonSerializer.Deserialize<'T>(&copy, innerOptions)
    with ex ->
      error <- ex

    if isNull error then
      reader <- copy
      result
    else
      let element = JsonElement.ParseValue(&reader)
      let updateId = tryGetUpdateId element options
      let idText = updateId |> Option.map string |> Option.defaultValue "<missing>"
      let raw = element.GetRawText()
      let rawShort = if raw.Length > 2000 then raw.Substring(0, 2000) + "..." else raw
      Console.Error.WriteLine(
        $"[SafeUpdateConverter] Failed to parse {typeof<'T>.Name} (update_id={idText}): {error.GetType().Name}: {error.Message}{Environment.NewLine}JSON: {rawShort}")
      createEmpty (defaultArg updateId 0L)

  override _.Write(writer: Utf8JsonWriter, value: 'T, _options: JsonSerializerOptions) =
    JsonSerializer.Serialize<'T>(writer, value, innerOptions)

type SafeUpdateConverterFactory(targetTypeFullName: string) =
  inherit JsonConverterFactory()

  new() = SafeUpdateConverterFactory("Funogram.Telegram.Types+Update")

  override _.CanConvert(t: Type) =
    t.FullName = targetTypeFullName

  override _.CreateConverter(t: Type, options: JsonSerializerOptions) =
    let inner = JsonSerializerOptions(options)
    inner.Converters
    |> Seq.filter (fun c -> c :? SafeUpdateConverterFactory)
    |> Seq.toList
    |> List.iter (inner.Converters.Remove >> ignore)

    let convType = typedefof<SafeUpdateConverter<_>>.MakeGenericType(t)
    Activator.CreateInstance(
      convType,
      BindingFlags.Instance ||| BindingFlags.Public ||| BindingFlags.NonPublic,
      null,
      [| box inner |],
      null) :?> JsonConverter