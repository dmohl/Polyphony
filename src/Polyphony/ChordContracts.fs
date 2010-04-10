module ChordContracts

open System.Collections.Generic
open System.ServiceModel
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

[<ServiceContract>]  
type IChordServer = interface   
    [<OperationContract>]
    abstract GetFingerNodes : unit -> KeyValuePair<string, string> list
    [<OperationContract>]  
    abstract GetSuccessorNode : unit -> string
    [<OperationContract>]  
    abstract UpdateSuccessorNode : newSuccessorNode:string -> string  
    [<OperationContract>]  
    abstract PutValueByKey : key:obj -> value:obj -> obj  
    [<OperationContract>]  
    abstract GetValueByKey : value:obj -> obj  
    [<OperationContract>]  
    abstract RequestJoinChordNodeNetwork : requestorNode:string -> NodeNeighbors  
end
