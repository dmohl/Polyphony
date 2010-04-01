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
    let remoteObject = Activator.GetObject(typeof<Shared.Chord>, serverInformation) :?> Shared.Chord 
    let result = remoteObject.GetValueByKey input
    Console.WriteLine(result) |> ignore
