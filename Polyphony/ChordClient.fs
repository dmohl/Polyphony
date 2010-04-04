module ChordClient

open System
open System.Configuration
open System.ServiceModel

let remoteServer = ConfigurationManager.AppSettings.Item("RemoteServer")
let localServer = ConfigurationManager.AppSettings.Item("LocalServer")

let CallServer (server:string) (operationContract:string) (inputArguments:string[]) =
    let service = new ChannelFactory<ChordServer.IChordServer>(
                        new NetTcpBinding(), server)  
    try                    
        try
            let proxy = service.CreateChannel()        
            let result = match operationContract with
                         | "put" ->
                             proxy.PutValueByKey inputArguments.[1] inputArguments.[2] 
                             "Put Complete" :> obj
                         | "get" -> 
                             proxy.GetValueByKey inputArguments.[1]     
                         | _ -> "Unknown" :> obj 
            result             
        with
        | ex -> 
            Console.WriteLine ex.Message
            "Unknown" :> obj
    finally                 
        service.Close |> ignore

let RunCommand(input:string) : unit =
    let inputArguments = input.Split(' ')
    let result = 
        match inputArguments.[0] with
        | "put" ->
            CallServer localServer inputArguments.[0] inputArguments |> ignore
            sprintf "PUT Key:%A Value:%A" inputArguments.[1] inputArguments.[2] :> obj   
        | "get" -> 
            let rec getValue server =
                let value = CallServer server inputArguments.[0] inputArguments
                match value with
                | null -> getValue remoteServer    
                | _ -> value
            getValue localServer
        | _ -> "unknown command" :> obj   
    Console.WriteLine(result) |> ignore
