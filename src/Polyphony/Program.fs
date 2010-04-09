module Polyphony

open System
open System.Configuration
open System.ServiceModel
open ChordServer
open ChordServerProxy
open SettingsProvider

let settingsProvider = new SettingsProvider() :> ISettingsProvider

let remoteNode = settingsProvider.GetApplicationSetting("RemoteNode")
let localNode = settingsProvider.GetApplicationSetting("LocalNode")

let chordServer = new ChordServer(localNode, localNode)
ChordServer.Initialize (new SettingsProvider.SettingsProvider()) (new ServiceHost(chordServer)) |> ignore

let chordServerProxy = new ChordServerProxy() :> IChordServerProxy
let nodeSuccessor = ChordClient.JoinChordNodeNetwork localNode remoteNode chordServerProxy 

Console.Write("\n{0}>", localNode)
let mutable input = Console.ReadLine()
 
while input <> "quit"
    do
    if input <> "quit" 
    then
        ChordClient.RunCommand input chordServerProxy settingsProvider |> ignore
        Console.Write("\n{0}>", localNode)
        input <- Console.ReadLine()

