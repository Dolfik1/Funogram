module Funogram.StringUtils

open System
open System.Linq.Expressions
open System.Reflection
open System.Runtime.Serialization
open Microsoft.FSharp.Reflection

let toSnakeCase =
  let assembly = Assembly.Load("System.Text.Json")
  let fullName = "System.Text.Json" + "." + "JsonSnakeCaseLowerNamingPolicy"
  let t = assembly.GetType(fullName, true, true)
  let instance = Activator.CreateInstance(t)

  let createConvertNameFunc instance =
    let t = instance.GetType()
    let method = t.GetMethod("ConvertName", BindingFlags.Public ||| BindingFlags.Instance)
    let instanceExpr = Expression.Constant(instance)
    let paramExpr = Expression.Parameter(typeof<string>)
    let callExpr = Expression.Call(instanceExpr, method, paramExpr)
    let lambda = Expression.Lambda<Func<string, string>>(callExpr, paramExpr)
    lambda.Compile()

  // not good solution, but fastest
  let fn = createConvertNameFunc instance
  fn.Invoke

let inline caseName (caseInfo: UnionCaseInfo) =
  let dataMember =
    caseInfo.GetCustomAttributes(typeof<DataMemberAttribute>)
    |> Seq.cast<DataMemberAttribute>
    |> Seq.filter (fun x -> String.IsNullOrEmpty(x.Name) |> not)
    |> Seq.toArray
  
  if dataMember.Length > 0
  then dataMember[0].Name
  else caseInfo.Name |> toSnakeCase