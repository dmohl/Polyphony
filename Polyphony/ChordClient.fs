module ChordClient

open System
open System.Configuration
open System.Runtime.Remoting
open System.Runtime.Remoting.Channels
open System.Runtime.Remoting.Channels.Tcp

let serverInformation = ConfigurationManager.AppSettings.Item("ClientServer")
let selfServerInformation = ConfigurationManager.AppSettings.Item("SelfServer")

let Initialize() =
    Console.WriteLine("Polyphony Client started and pointing to server {0}...", serverInformation)    
    let localObject = Activator.GetObject(typeof<Shared.Chord>, selfServerInformation) :?> Shared.Chord 
    localObject.PutValueByKey "test" "testValue"

let RunCommand(input:string) : unit =
    
    let inputArguments = input.Split(' ')
    let result = match inputArguments.[0] with
                 | "put" -> 
                     let localObject = Activator.GetObject(typeof<Shared.Chord>, selfServerInformation) :?> Shared.Chord     
                     localObject.PutValueByKey inputArguments.[1] inputArguments.[2] |> ignore
                     sprintf "PUT Key:%A Value:%A" inputArguments.[1] inputArguments.[2] :> obj   
                 | "get" -> 
                     let remoteObject = Activator.GetObject(typeof<Shared.Chord>, serverInformation) :?> Shared.Chord     
                     remoteObject.GetValueByKey inputArguments.[1]
                 | _ -> "unknown command" :> obj   
    Console.WriteLine(result) |> ignore
