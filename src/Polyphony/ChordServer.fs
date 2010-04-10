module ChordServer

open System
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
    new (node, successorNode) = {hashTable = new Hashtable(); 
        node = node; successorNode = successorNode}
    interface IChordServer with
        member x.GetSuccessorNode () =
            x.successorNode
        member x.UpdateSuccessorNode newSuccessorNode =
            x.successorNode <- newSuccessorNode 
            x.successorNode
        member x.PutValueByKey key value =
            x.hashTable.Add(key, value)
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