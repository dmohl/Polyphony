module ChordServerProxyCommands

open System
open System.ServiceModel

let RunPutCommand (proxy:ChordServer.IChordServer) (inputArguments:string[]) =
    proxy.PutValueByKey inputArguments.[1] inputArguments.[2]
    Some("PUT Successful" :> obj)

let RunGetCommand (proxy:ChordServer.IChordServer) (inputArguments:string[]) =
    match proxy.GetValueByKey inputArguments.[1] with
    | null -> None
    | value -> Some(value)

    