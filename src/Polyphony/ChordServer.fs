module ChordServer

open System
open System.Collections.Generic
open System.ServiceModel
open System.Collections
open System.Configuration
open System.Net
open SettingsProvider
open ChordCommon
open ChordServerHelper
open ChordContracts

[<ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)>]
type ChordServer = class
    val hashTable : Hashtable
    val node : string
    val mutable successorNode : string
    val fingerNodes : IDictionary<string,string>
    new (node, successorNode) = {hashTable = new Hashtable(); 
        node = node; successorNode = successorNode; fingerNodes = new Dictionary<string,string>()}
    member x.AddToFingerNodes node =
        match x.fingerNodes.ContainsKey node with
        | false -> x.fingerNodes.Add(node, node)
        | _ -> node |> ignore // do nothing    
    interface IChordServer with
        member x.GetFingerNodes () =
            [ for nodeKeyValue in x.fingerNodes
              -> nodeKeyValue ]
        member x.GetSuccessorNode () =
            x.successorNode
        member x.UpdateSuccessorNode newSuccessorNode =
            x.successorNode <- newSuccessorNode 
            x.successorNode
        member x.PutValueByKey key value =
            x.hashTable.Add(key, value)
            key
        member x.GetValueByKey key =
            x.hashTable.Item(key)
        member x.RequestJoinChordNodeNetwork requestorNode =
            let result = 
                match requestorNode with
                | _ when requestorNode = x.node || x.node = x.successorNode ->
                    x.successorNode <- requestorNode 
                    BuildNodeNeighbors x.node x.node
                | _ when requestorNode > x.node && requestorNode < x.successorNode -> 
                    let requestorsSuccessor = x.successorNode 
                    x.successorNode <- requestorNode
                    BuildNodeNeighbors x.node requestorsSuccessor
                | _ -> 
                    BuildNodeNeighbors x.successorNode x.successorNode
            x.AddToFingerNodes requestorNode        
            result
end
   
let Initialize (settingsProvider:ISettingsProvider) (host:ServiceHost) =
    try
        let localNode = settingsProvider.GetApplicationSetting("LocalNode")
        host.AddServiceEndpoint(typeof<IChordServer>,
                    new NetTcpBinding(), localNode) |> ignore       
        host.Open()
        Some(host)
    with
    | ex -> LogException ex
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
    | ex -> LogException ex           