module ChordServerProxy

open System
open System.ServiceModel

type IChordServerProxy = interface   
    abstract CallServer : server:string -> operationContract:string -> inputArguments:string[] -> obj option
end

type ChordServerProxy() = 
    interface IChordServerProxy with
        member this.CallServer server operationContract inputArguments =
            let service = new ChannelFactory<ChordServer.IChordServer>(
                                new NetTcpBinding(), server)  
            try                    
                try
                    let proxy = service.CreateChannel()        
                    let result = match operationContract with
                                 | "put" ->
                                     proxy.PutValueByKey inputArguments.[1] inputArguments.[2] 
                                     Some("Put Complete" :> obj)
                                 | "get" -> 
                                     Some(proxy.GetValueByKey inputArguments.[1])     
                                 | _ -> None 
                    result             
                with
                | ex -> 
                    Console.WriteLine ex.Message
                    None
            finally                 
                service.Close |> ignore
