module Shared

open System
open System.Runtime.Remoting
open System.Runtime.Remoting.Channels

type Chord = class
    inherit MarshalByRefObject 
    val key : string
    new(key) = {key = key}
    member x.GetValueByKey(key) =
        "some value"
end                
