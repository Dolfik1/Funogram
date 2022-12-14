module Funogram.Api
  
  open Funogram.Types
  open Funogram.Tools
  
  let api config (request: IRequestBase<'a>) = 
    Api.makeRequestAsync config request