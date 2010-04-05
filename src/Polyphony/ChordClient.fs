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
            chordServerProxy.CallServer localServer inputArguments.[0] inputArguments |> ignore
            sprintf "PUT Key:%A Value:%A" inputArguments.[1] inputArguments.[2] :> obj   
        | "get" -> 
            let rec getValue server =
                let value = chordServerProxy.CallServer server inputArguments.[0] inputArguments
                match value with
                | null -> getValue remoteServer    
                | _ -> value
            getValue localServer
        | _ -> "unknown command" :> obj   
    Console.WriteLine(result) |> ignore
    result
