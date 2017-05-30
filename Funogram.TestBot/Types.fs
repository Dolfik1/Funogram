namespace Funogram.TestBot

module Types = 
    type Bot = { Token: string; Me: Funogram.Types.User }

[<AutoOpen>]
module Extend = 
    let (|?) = defaultArg