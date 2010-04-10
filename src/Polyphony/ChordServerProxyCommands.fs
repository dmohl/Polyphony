module ChordServerProxyCommands

open System
open System.ServiceModel
open ChordContracts
open ChordCommon

let Execute command proxy inputArguments : obj option =
    match command proxy inputArguments with
    | null -> None
    | value -> Some(value)

let RunCommand proxy operationContract inputArguments : obj option =
    let command =
        match operationContract with
        | CommandType.Put -> fun (proxy:IChordServer) (inputArguments:string[]) -> 
                                (proxy.PutValueByKey inputArguments.[1] inputArguments.[2])
        | CommandType.Get -> fun (proxy:IChordServer) (inputArguments:string[]) -> 
                                (proxy.GetValueByKey inputArguments.[1])
        | CommandType.Join -> fun (proxy:IChordServer) (inputArguments:string[]) -> 
                                (proxy.RequestJoinChordNodeNetwork inputArguments.[0] :> obj)
        | CommandType.UpdateSuccessorNode -> fun (proxy:IChordServer) (inputArguments:string[]) -> 
                                (proxy.UpdateSuccessorNode inputArguments.[0] :> obj)
        | CommandType.GetSuccessorNode -> fun (proxy:IChordServer) (inputArguments:string[]) -> 
                                (proxy.GetSuccessorNode () :> obj)
    Execute command proxy inputArguments
    



