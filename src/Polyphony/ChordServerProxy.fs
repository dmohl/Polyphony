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
                    RunCommand proxy operationContract inputArguments
                with
                | ex -> 
                    None
            finally
                match service.State with
                | CommunicationState.Faulted -> 
                    try
                        service.Close |> ignore
                    with
                    | ex ->
                        service.Abort |> ignore
                | _ -> service.Close |> ignore                            

