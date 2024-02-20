module Funogram.StringUtils

open System
open System.Linq.Expressions
open System.Reflection

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