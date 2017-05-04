namespace Funogram.Tests

module internal Extensions =
    open Xunit
        
    let inline shouldEqual (x: 'a) (y: 'a) = Assert.Equal<'a>(x, y)
    let inline shouldThrow<'a when 'a :> exn>(y: unit -> unit) = Assert.Throws<'a>(y)