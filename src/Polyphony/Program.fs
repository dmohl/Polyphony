module Polyphony

open System
open System.Configuration
open System.ServiceModel
open ChordServerProxy

ChordServer.Initialize (new SettingsProvider.SettingsProvider()) (new ServiceHost(typeof<ChordServer.ChordServer>)) |> ignore
let chordServerProxy = new ChordServerProxy() :> IChordServerProxy

Console.Write "\nEnter Command:"
let mutable input = Console.ReadLine()
 
while input <> "quit"
    do
    if input <> "quit" 
    then
        ChordClient.RunCommand input chordServerProxy
        Console.Write "\nEnter Command:" 
        input <- Console.ReadLine()

