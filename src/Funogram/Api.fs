module Funogram.Api

  open Funogram.Types
  open Funogram.Tools

  let apiAsync config (request: IRequestBase<'a>) =
    Api.makeRequestAsync config request