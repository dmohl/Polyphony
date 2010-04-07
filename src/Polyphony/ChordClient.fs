module ChordClient

open System
open System.Configuration
open ChordServerProxy

let remoteServer = ConfigurationManager.AppSettings.Item("RemoteServer")
let localServer = ConfigurationManager.AppSettings.Item("LocalServer")

let RunPutCommand server (inputArguments:string[]) (chordServerProxy:IChordServerProxy) = 
    let valueOption = chordServerProxy.CallServer localServer inputArguments.[0] inputArguments 
    match valueOption with
    | Some _ -> sprintf "PUT Key:%A Value:%A" inputArguments.[1] inputArguments.[2] :> obj       
    | None -> sprintf "The PUT was not successful" :> obj

let RunGetCommand server (inputArguments:string[]) (chordServerProxy:IChordServerProxy) = 
    let rec getValue server localServerForRecursionStopCheck =
        let valueOption = chordServerProxy.CallServer server inputArguments.[0] inputArguments 
        match valueOption with
        | Some value -> value   
        | None when localServerForRecursionStopCheck = localServer -> "The Key was not found" :> obj
        | None -> getValue remoteServer localServer
    getValue localServer ""

let RunCommand(input:string) (chordServerProxy:IChordServerProxy) : obj =
    let inputArguments = input.Split(' ')
    let result = 
        match inputArguments.[0] with
        | "put" ->
            RunPutCommand localServer inputArguments chordServerProxy
        | "get" -> 
            RunGetCommand localServer inputArguments chordServerProxy
        | _ -> "Unknown command" :> obj   
    Console.WriteLine(result) |> ignore
    result

let JoinChordNodeNetwork () =
    // Join the network
    // Return this nodes successor
    remoteServer