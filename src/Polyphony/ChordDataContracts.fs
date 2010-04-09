module ChordDataContracts

open System.Runtime.Serialization

[<DataContract>]    
type NodeNeighbors() = 
    let mutable predecessorNode = ""
    let mutable successorNode = ""
    [<DataMember>]
    member x.PredecessorNode
        with get() = predecessorNode
        and set value = predecessorNode <- value
    [<DataMember>]
    member x.SuccessorNode
        with get() = successorNode
        and set value = successorNode <- value
