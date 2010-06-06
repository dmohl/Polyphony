module ChordClient

open System
open System.Configuration
open ChordServerProxy
open ChordCommon
open ChordContracts
open SettingsProvider

let GetLocalNode (settingsProvider:ISettingsProvider) = settingsProvider.GetApplicationSetting("LocalNode")

let (|PossiblePredecessorIsPredecessor|_|) possiblePredecessorNode predecessorNode =
    match predecessorNode with 
    | _ when predecessorNode = possiblePredecessorNode -> 
        Some PossiblePredecessorIsPredecessor 
    | _ -> None
let (|StartingNodeEqualsSuccessor|_|) startingNode successorNode =
    match startingNode with 
    | _ when startingNode = successorNode -> Some StartingNodeEqualsSuccessor
    | _ -> None
let (|StartingNodeIsEmpty|_|) startingNode =
    match startingNode with 
    | "" -> Some StartingNodeIsEmpty
    | _ -> None
let (|OptionStartingNodeIsEmpty|_|) startingNode optionSuccessorNode =
    match optionSuccessorNode with 
    | Some successorNode when startingNode = "" -> 
        Some (OptionStartingNodeIsEmpty successorNode)
    | _ -> None
let (|OptionStartingNodeIsNotSuccessor|_|) startingNode optionSuccessorNode =
    match optionSuccessorNode with 
    | Some successorNode when startingNode <> (string successorNode) -> 
        Some (OptionStartingNodeIsNotSuccessor successorNode)
    | _ -> None

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
            | OptionStartingNodeIsEmpty startingNode successorNode -> 
                getValue (successorNode :?> string) localNode
            | OptionStartingNodeIsNotSuccessor startingNode successorNode -> 
                getValue (successorNode :?> string) startingNode
            | None -> "Node not found" :> obj
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

let rec FindSuccessorNode possiblePredecessorNode startingNode localNode (chordServerProxy:IChordServerProxy) =
    let valueOption = chordServerProxy.CallServer possiblePredecessorNode CommandType.Join [|localNode|]
    match valueOption with
    | Some value -> 
        let nodeNeighbors = value :?> NodeNeighbors
        let successorNode = nodeNeighbors.SuccessorNode
        match nodeNeighbors.PredecessorNode with
        | PossiblePredecessorIsPredecessor possiblePredecessorNode -> nodeNeighbors.SuccessorNode
        | StartingNodeEqualsSuccessor startingNode successorNode -> 
            chordServerProxy.CallServer possiblePredecessorNode CommandType.UpdateSuccessorNode [|localNode|] |> ignore
            nodeNeighbors.SuccessorNode
        | StartingNodeIsEmpty startingNode ->
            FindSuccessorNode nodeNeighbors.SuccessorNode possiblePredecessorNode localNode chordServerProxy
        | _ -> FindSuccessorNode nodeNeighbors.SuccessorNode startingNode localNode chordServerProxy
    | None -> localNode

let JoinChordNodeNetwork localNode remoteNode (chordServerProxy:IChordServerProxy) =
    let successorNode = FindSuccessorNode remoteNode "" localNode chordServerProxy
    chordServerProxy.CallServer localNode 
        CommandType.UpdateSuccessorNode [|successorNode|] |> ignore
    successorNode    
