module ChordClient

open System
open System.Configuration
open System.ServiceModel

let remoteServer = ConfigurationManager.AppSettings.Item("RemoteServer")
let localServer = ConfigurationManager.AppSettings.Item("LocalServer")

let RunCommand(input:string) : unit =
    
    let inputArguments = input.Split(' ')
    let result = 
        match inputArguments.[0] with
        | "put" -> 
            let service = new ChannelFactory<ChordServer.IChordServer>(
                                new NetTcpBinding(), localServer)  
            let proxy = service.CreateChannel()        
            proxy.PutValueByKey inputArguments.[1] inputArguments.[2] |> ignore
            service.Close |> ignore
            sprintf "PUT Key:%A Value:%A" inputArguments.[1] inputArguments.[2] :> obj   
        | "get" -> 
            let rec getValue (server:string) =
                let service = new ChannelFactory<ChordServer.IChordServer>(
                                new NetTcpBinding(), server)  
                let proxy = service.CreateChannel() 
                let value = proxy.GetValueByKey inputArguments.[1]     
                service.Close |> ignore
                match value with
                | null -> getValue remoteServer    
                | _ -> value
            getValue localServer
        | _ -> "unknown command" :> obj   
    Console.WriteLine(result) |> ignore
