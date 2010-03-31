module Shared

open System
open System.Runtime.Remoting

type Chord = class
    inherit MarshalByRefObject 
    new () = {}
    member x.GetValueByKey(key) =
        "some value"
end                
