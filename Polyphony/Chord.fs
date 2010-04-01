module Shared

open System
open System.Collections
open System.Runtime.Remoting

type Chord = class
    val hashTable : Hashtable
    inherit MarshalByRefObject 
    new () = {hashTable = new Hashtable()}
    member x.PutValueByKey key value =
        x.hashTable.Add(key, value)
    member x.GetValueByKey key =
        x.hashTable.Item(key)
end                
