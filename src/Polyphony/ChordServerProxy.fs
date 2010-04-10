module ChordServerProxy

open System
open System.Configuration
open System.ServiceModel
open System.Runtime.Serialization
open ChordCommon
open ChordServerProxyCommands
open ChordContracts
open SettingsProvider

type IChordServerProxy = interface   
    abstract CallServer : server:string -> operationContract:CommandType -> inputArguments:string[] -> obj option
end

let GetOperationTimeout() =
    let settingsProvider = new SettingsProvider() :> ISettingsProvider
    let serviceOperationTimeout = settingsProvider.GetApplicationSetting("ServiceOperationTimeout")
    let mutable operationTimeout = 5
    Int32.TryParse(serviceOperationTimeout, ref operationTimeout) |> ignore
    new TimeSpan(0, 0, operationTimeout)

type ChordServerProxy() = 
    interface IChordServerProxy with
        member this.CallServer server operationContract inputArguments =
            let binding = new NetTcpBinding()
            let service = new ChannelFactory<IChordServer>(binding, server)  
            try                    
                try
                    let proxy = service.CreateChannel()
                    (proxy :?> IContextChannel).OperationTimeout <- GetOperationTimeout()
                    RunCommand proxy operationContract inputArguments
                with
                | ex -> 
                    None
            finally
                match service.State with
                | serviceState when serviceState <> CommunicationState.Faulted -> 
                    try
                        service.Close |> ignore
                    with
                    | ex ->
                        service.Abort |> ignore
                | _ -> service.Abort |> ignore 
                                           

