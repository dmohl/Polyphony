module ChordServerProxyCommands

open System
open System.ServiceModel
open ChordCommon

let RunPutCommand (proxy:ChordServer.IChordServer) (inputArguments:string[]) =
    proxy.PutValueByKey inputArguments.[1] inputArguments.[2]
    Some("PUT Successful" :> obj)

let RunGetCommand (proxy:ChordServer.IChordServer) (inputArguments:string[]) =
    match proxy.GetValueByKey inputArguments.[1] with
    | null -> None
    | value -> Some(value)

let RunJoinCommand (proxy:ChordServer.IChordServer) (inputArguments:string[]) =
    match proxy.RequestJoinChordNodeNetwork inputArguments.[0] with
    | null -> None
    | value -> Some(value)

let RunUpdateSuccessorNode (proxy:ChordServer.IChordServer) (inputArguments:string[]) =
    match proxy.RequestJoinChordNodeNetwork inputArguments.[0] with
    | null -> None
    | value -> Some(value)
    
let RunGetSuccessorNodeCommand (proxy:ChordServer.IChordServer) =
    match proxy.GetSuccessorNode () with
    | null -> None
    | value -> Some(value)

let RunCommand proxy operationContract inputArguments =
    match operationContract with
    | CommandType.Put -> RunPutCommand proxy inputArguments
    | CommandType.Get -> RunGetCommand proxy inputArguments
    | CommandType.Join -> RunJoinCommand proxy inputArguments
    | CommandType.UpdateSuccessorNode -> RunUpdateSuccessorNode proxy inputArguments
    | CommandType.GetSuccessorNode -> RunGetSuccessorNodeCommand proxy



