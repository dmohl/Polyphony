module ChordClient

open System
open System.Configuration
open ChordServerProxy
open ChordCommon

let remoteNode = ConfigurationManager.AppSettings.Item("RemoteNode")
let localNode = ConfigurationManager.AppSettings.Item("LocalNode")

let RunPutCommand node (inputArguments:string[]) (chordServerProxy:IChordServerProxy) = 
    let valueOption = chordServerProxy.CallServer localNode CommandType.Put inputArguments 
    match valueOption with
    | Some _ -> sprintf "PUT Key:%A Value:%A" inputArguments.[1] inputArguments.[2] :> obj       
    | None -> sprintf "The PUT was not successful" :> obj

let RunGetCommand node (inputArguments:string[]) (chordServerProxy:IChordServerProxy) = 
    let rec getValue node localServerForRecursionStopCheck =
        let valueOption = chordServerProxy.CallServer node CommandType.Get inputArguments 
        match valueOption with
        | Some value -> value   
        | None when localServerForRecursionStopCheck = localNode -> "The Key was not found" :> obj
        | None -> getValue remoteNode localNode
    getValue localNode ""

let RunCommand(input:string) (chordServerProxy:IChordServerProxy) : obj =
    let inputArguments = input.Split(' ')
    let result = 
        match inputArguments.[0] with
        | "put" ->
            RunPutCommand localNode inputArguments chordServerProxy
        | "get" -> 
            RunGetCommand localNode inputArguments chordServerProxy
        | _ -> "Unknown command" :> obj   
    Console.WriteLine(result) |> ignore
    result

let JoinChordNodeNetwork localNode remoteNode (chordServerProxy:IChordServerProxy) =
    let rec joinTheNetwork possiblePredecessorNode =
        let valueOption = chordServerProxy.CallServer possiblePredecessorNode CommandType.Join [|localNode|]
        match valueOption with
        | Some value -> 
            let nodeNeighbors = value :?> NodeNeighbors
            match nodeNeighbors.PredecessorNode with
            | possiblePredecessorNode -> nodeNeighbors.SuccessorNode
            | _ -> joinTheNetwork nodeNeighbors.SuccessorNode
        | None -> "" // throw exception?  failwith?
    let successorNode = joinTheNetwork remoteNode
    chordServerProxy.CallServer localNode 
        CommandType.UpdateSuccessorNode [|successorNode|] |> ignore
    successorNode