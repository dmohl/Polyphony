module ChordServerProxy

open System
open System.ServiceModel
open ChordCommon
open ChordServerProxyCommands

type IChordServerProxy = interface   
    abstract CallServer : server:string -> operationContract:CommandType -> inputArguments:string[] -> obj option
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
                                 | CommandType.Put -> RunPutCommand proxy inputArguments
                                 | CommandType.Get -> RunGetCommand proxy inputArguments
                                 | _ -> None 
                    result             
                with
                | ex -> 
                    Console.WriteLine ex.Message
                    None
            finally                 
                service.Close |> ignore
