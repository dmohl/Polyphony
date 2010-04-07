module ChordServer

open System
open System.ServiceModel
open System.Collections
open System.Configuration
open System.Net
open SettingsProvider

[<ServiceContract>]  
type IChordServer = interface   
    [<OperationContract>]  
    abstract PutValueByKey : key:obj -> value:obj -> unit  
    [<OperationContract>]  
    abstract GetValueByKey : value:obj -> obj  
    [<OperationContract>]  
    abstract RequestJoinChordNodeNetwork : requestorNode:string -> obj  
end

[<ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)>]
type ChordServer = class
    val hashTable : Hashtable
    [<DefaultValue(false)>]
    val mutable node : string
    [<DefaultValue(false)>]
    val mutable successorNode : string
    new () = {hashTable = new Hashtable()}
    interface IChordServer with
        member x.PutValueByKey key value =
            x.hashTable.Add(key, value)
        member x.GetValueByKey key =
            x.hashTable.Item(key)
        member x.RequestJoinChordNodeNetwork requestorNode =
            requestorNode :> obj
//            match requestorNode with
//            | _ when requestorNode > x.node && requestorNode < x.successorNode -> 
//                x.successorNode <- requestorNode
//                requestorNode
end

let logException (ex:Exception) =
    Console.WriteLine("Error: {0}", ex.Message)
    Console.WriteLine(ex.Source)
    
let Initialize (settingsProvider:ISettingsProvider) (host:ServiceHost) =
    try
        let localNode = settingsProvider.GetApplicationSetting("LocalNode")
        Console.WriteLine("Starting Node: {0}", localNode)
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