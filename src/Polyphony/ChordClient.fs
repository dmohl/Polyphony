module ChordClient

open System
open System.Configuration
open ChordServerProxy

let remoteServer = ConfigurationManager.AppSettings.Item("RemoteServer")
let localServer = ConfigurationManager.AppSettings.Item("LocalServer")

let RunCommand(input:string) (chordServerProxy:IChordServerProxy) : obj =
    let inputArguments = input.Split(' ')
    let result = 
        match inputArguments.[0] with
        | "put" ->
            let valueOption = chordServerProxy.CallServer localServer inputArguments.[0] inputArguments 
            match valueOption with
            | Some _ -> sprintf "PUT Key:%A Value:%A" inputArguments.[1] inputArguments.[2] :> obj       
            | None -> sprintf "The PUT was not successful" :> obj
        | "get" -> 
            let rec getValue server =
                let valueOption = chordServerProxy.CallServer server inputArguments.[0] inputArguments 
                match valueOption with
                | Some value -> value    
                | None -> getValue remoteServer
            getValue localServer
        | _ -> "unknown command" :> obj   
    Console.WriteLine(result) |> ignore
    result

let JoinChordNodeNetwork () =
    // Join the network
    // Return this nodes successor
    remoteServer