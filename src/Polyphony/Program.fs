module Polyphony

open System
open System.Configuration
open System.ServiceModel
open ChordServerProxy

let remoteNode = ConfigurationManager.AppSettings.Item("RemoteNode")
let localNode = ConfigurationManager.AppSettings.Item("RemoteNode")

ChordServer.Initialize (new SettingsProvider.SettingsProvider()) (new ServiceHost(typeof<ChordServer.ChordServer>)) |> ignore
ChordClient.JoinChordNodeNetwork remoteNode |> ignore

let chordServerProxy = new ChordServerProxy() :> IChordServerProxy

Console.Write "\nEnter Command:"
let mutable input = Console.ReadLine()
 
while input <> "quit"
    do
    if input <> "quit" 
    then
        ChordClient.RunCommand input chordServerProxy |> ignore
        Console.Write "\nEnter Command:" 
        input <- Console.ReadLine()

