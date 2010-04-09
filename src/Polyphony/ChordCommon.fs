module ChordCommon

open System.Runtime.Serialization

type CommandType = 
    | Put
    | Get
    | Join
    | UpdateSuccessorNode
    | GetSuccessorNode

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
