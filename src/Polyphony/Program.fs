module Polyphony

open System
open System.Configuration
open System.ServiceModel
open ChordServer
open ChordServerProxy

let remoteNode = ConfigurationManager.AppSettings.Item("RemoteNode")
let localNode = ConfigurationManager.AppSettings.Item("LocalNode")

let chordServer = new ChordServer(localNode, localNode)
ChordServer.Initialize (new SettingsProvider.SettingsProvider()) (new ServiceHost(chordServer)) |> ignore

let chordServerProxy = new ChordServerProxy() :> IChordServerProxy
let nodeSuccessor = ChordClient.JoinChordNodeNetwork localNode remoteNode chordServerProxy 
match nodeSuccessor with
| "" -> ChordClient.JoinChordNodeNetwork localNode localNode chordServerProxy |> ignore
| _ -> nodeSuccessor |> ignore

Console.Write("\n{0}>", localNode)
let mutable input = Console.ReadLine()
 
while input <> "quit"
    do
    if input <> "quit" 
    then
        ChordClient.RunCommand input chordServerProxy |> ignore
        Console.Write("\n{0}>", localNode)
        input <- Console.ReadLine()

