module ChordClient

open System
open System.Configuration
open System.Runtime.Remoting
open System.Runtime.Remoting.Channels
open System.Runtime.Remoting.Channels.Tcp

let serverInformation = ConfigurationManager.AppSettings.Item("ClientServer")

let Initialize() =
    Console.WriteLine("Polyphony Client started and pointing to server {0}...", serverInformation)    
    let tcpChannel = new TcpChannel()
    ChannelServices.RegisterChannel(tcpChannel, false) |> ignore

let RunCommand(input:string) : unit =
    let requiredType = typeof<Shared.Chord>
    let remoteObject = Activator.GetObject(requiredType, serverInformation) :?> Shared.Chord 
    let result = remoteObject.GetValueByKey(input)
    Console.WriteLine(result) |> ignore
