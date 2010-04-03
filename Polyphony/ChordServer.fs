module ChordServer

open System
open System.ServiceModel
open System.Collections
open System.Configuration
open System.Net

[<ServiceContract>]  
type IChordServer = interface   
    [<OperationContract>]  
    abstract PutValueByKey : key:obj -> value:obj -> unit  
    [<OperationContract>]  
    abstract GetValueByKey : value:obj -> obj  
end

[<ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)>]
type ChordServer = class
    val hashTable : Hashtable
    new () = {hashTable = new Hashtable()}
    interface IChordServer with
        member x.PutValueByKey key value =
            x.hashTable.Add(key, value)
        member x.GetValueByKey key =
            x.hashTable.Item(key)
end

let logException (ex:Exception) =
    Console.WriteLine("Error: {0}", ex.Message)
    Console.WriteLine(ex.Source)
    
let Initialize () =
    try
        let localServer = ConfigurationManager.AppSettings.Item("LocalServer")
        Console.WriteLine("Starting Server: {0}", localServer)
        let host = new ServiceHost(typeof<ChordServer>)
        host.AddServiceEndpoint(typeof<IChordServer>,
                    new NetTcpBinding(), localServer) |> ignore       
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