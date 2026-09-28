namespace Funogram.Tests

module internal Extensions =
  open Xunit
        
  let inline shouldEqual (expected: 'a) (actual: 'a) = Assert.Equal<'a>(expected, actual)
  let inline shouldThrow<'a when 'a :> exn>(y: unit -> unit) = Assert.Throws<'a>(y)