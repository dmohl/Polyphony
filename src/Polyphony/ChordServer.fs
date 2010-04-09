module ChordServer

open System
open System.ServiceModel
open System.Collections
open System.Configuration
open System.Net
open SettingsProvider
open ChordCommon

[<ServiceContract>]  
type IChordServer = interface   
    [<OperationContract>]  
    abstract GetSuccessorNode : unit -> obj
    [<OperationContract>]  
    abstract PutValueByKey : key:obj -> value:obj -> unit  
    [<OperationContract>]  
    abstract GetValueByKey : value:obj -> obj  
    [<OperationContract>]  
    abstract UpdateSuccessorNode : newSuccessorNode:string -> obj  
    [<OperationContract>]  
    abstract RequestJoinChordNodeNetwork : requestorNode:string -> obj  
end

[<ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)>]
type ChordServer = class
    val hashTable : Hashtable
    val node : string
    val mutable successorNode : string
    new (node, successorNode) = {hashTable = new Hashtable(); 
        node = node; successorNode = successorNode}
    interface IChordServer with
        member x.GetSuccessorNode () =
            x.successorNode :> obj
        member x.PutValueByKey key value =
            x.hashTable.Add(key, value)
        member x.GetValueByKey key =
            x.hashTable.Item(key)
        member x.UpdateSuccessorNode newSuccessorNode =
            x.successorNode <- newSuccessorNode 
            x.successorNode :> obj
        member x.RequestJoinChordNodeNetwork requestorNode =
            let result = 
                match requestorNode with
                // If the requestorNode equals the current node (meaning 
                // there is only one node on the network) or if current node
                // is equal to the current nodes successor (meaning there is only
                // one other node on the network) then we should
                // return the current node as both the predecessor and the successor.
                | _ when requestorNode = x.node || x.node = x.successorNode ->
                    x.successorNode <- requestorNode 
                    {PredecessorNode = x.node; SuccessorNode = x.node}
                // If the requestor node is in between the current node and the 
                // current node's successor, then make the current node's successor
                // the requestor node and return the current node as the requestor
                // nodes predeccessor and the current node's successor as the 
                // requestor nodes successor
                | _ when requestorNode > x.node && requestorNode < x.successorNode -> 
                    let requestorsSuccessor = x.successorNode 
                    x.successorNode <- requestorNode
                    {PredecessorNode = x.node; SuccessorNode = requestorsSuccessor}
                // We don't know who the predecessor or successor is.  
                // Try the current nodes successor and see if they know what to do.   
                | _ -> {PredecessorNode = x.successorNode; SuccessorNode = x.successorNode}   
            result :> obj        
end

let logException (ex:Exception) =
    Console.WriteLine("Error: {0}", ex.Message)
    Console.WriteLine(ex.Source)
    
let Initialize (settingsProvider:ISettingsProvider) (host:ServiceHost) =
    try
        let localNode = settingsProvider.GetApplicationSetting("LocalNode")
        host.AddServiceEndpoint(typeof<IChordServer>,
                    new NetTcpBinding(), localNode) |> ignore       
        host.Open()
        Some(host)
    with
    | ex -> logException ex
            None
           
let Stop (host: ServiceHost option)  =
    try
        match host with
        | None -> ()
        | Some(host) ->
            if host.State <> CommunicationState.Closed then
                host.Close()
                Console.WriteLine("Stopping Server")               
    with
    | ex -> logException ex           