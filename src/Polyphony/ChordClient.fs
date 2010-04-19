module ChordClient

open System
open System.Configuration
open ChordServerProxy
open ChordCommon
open ChordContracts
open SettingsProvider

let GetLocalNode (settingsProvider:ISettingsProvider) = settingsProvider.GetApplicationSetting("LocalNode")

let RunPutCommand node (inputArguments:string[]) (chordServerProxy:IChordServerProxy) (settingsProvider:ISettingsProvider) = 
    let valueOption = chordServerProxy.CallServer (GetLocalNode settingsProvider) CommandType.Put inputArguments 
    match valueOption with
    | Some _ -> sprintf "PUT Key:%A Value:%A" inputArguments.[1] inputArguments.[2] :> obj       
    | None -> sprintf "The PUT was not successful" :> obj

let RunGetCommand node (inputArguments:string[]) (chordServerProxy:IChordServerProxy) (settingsProvider:ISettingsProvider) = 
    let localNode = (GetLocalNode settingsProvider)
    let rec getValue node startingNode =
        let valueOption = chordServerProxy.CallServer node CommandType.Get inputArguments 
        match valueOption with
        | Some value -> value   
        | None -> 
            let successorNodeOption = chordServerProxy.CallServer node CommandType.GetSuccessorNode inputArguments 
            match successorNodeOption with
            | Some successorNode when startingNode = "" -> getValue (successorNode :?> string) localNode
            | Some successorNode when startingNode <> (successorNode :?> string) -> getValue (successorNode :?> string) startingNode
            | _ -> "The Key was not found" :> obj
    getValue localNode ""

let RunGetSuccessorCommand node (chordServerProxy:IChordServerProxy) (settingsProvider:ISettingsProvider) = 
    let valueOption = chordServerProxy.CallServer (GetLocalNode settingsProvider) CommandType.GetSuccessorNode [||] 
    match valueOption with
    | Some successor -> sprintf "Successor: %A" successor :> obj       
    | None -> sprintf "The request did not complete successfully" :> obj

let RunCommand(input:string) (chordServerProxy:IChordServerProxy) (settingsProvider:ISettingsProvider) : obj =
    let inputArguments = input.Split(' ')
    let localNode = (GetLocalNode settingsProvider)
    let result = 
        match inputArguments.[0] with
        | "put" ->
            RunPutCommand localNode inputArguments chordServerProxy settingsProvider
        | "get" -> 
            RunGetCommand localNode inputArguments chordServerProxy settingsProvider
        | "getsuccessor" -> 
            RunGetSuccessorCommand localNode chordServerProxy settingsProvider
        | _ -> "Unknown command" :> obj   
    Console.WriteLine(result) |> ignore
    result

let JoinChordNodeNetwork localNode remoteNode (chordServerProxy:IChordServerProxy) =
    let rec joinTheNetwork possiblePredecessorNode startingNode =
        let valueOption = chordServerProxy.CallServer possiblePredecessorNode CommandType.Join [|localNode|]
        match valueOption with
        | Some value -> 
            let nodeNeighbors = value :?> NodeNeighbors
            match nodeNeighbors.PredecessorNode with
            | predecessorNode when possiblePredecessorNode = predecessorNode -> nodeNeighbors.SuccessorNode
            | _ when startingNode = "" ->
                joinTheNetwork nodeNeighbors.SuccessorNode possiblePredecessorNode
            | _ when startingNode = nodeNeighbors.SuccessorNode -> 
                chordServerProxy.CallServer possiblePredecessorNode CommandType.UpdateSuccessorNode [|localNode|] |> ignore
                nodeNeighbors.SuccessorNode
            | _ -> joinTheNetwork nodeNeighbors.SuccessorNode startingNode
        | None -> localNode
    let successorNode = joinTheNetwork remoteNode ""
    chordServerProxy.CallServer localNode 
        CommandType.UpdateSuccessorNode [|successorNode|] |> ignore
    successorNode    
