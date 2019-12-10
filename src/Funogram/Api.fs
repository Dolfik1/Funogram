module Funogram.Api
  
  open Funogram.Types
  open Funogram.Tools
  
  let api config (request: 'b when 'b :> IRequestBase<'a>) = 
    Api.makeRequestAsync config request