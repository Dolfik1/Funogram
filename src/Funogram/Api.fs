module Funogram.Api
  
  open Funogram.Types
  open Funogram.Tools
  
  let api config (request: IRequestBase<'a>) = 
    Api.makeRequestAsync<'a> config request
    
  let apiRaw config (request: IBotRequest) = 
    Api.makeRequestAsync<obj> config request