module Polyphony

open System
open System.Configuration
open System.ServiceModel

ChordServer.Initialize (new SettingsProvider.SettingsProvider()) (new ServiceHost(typeof<ChordServer.ChordServer>)) |> ignore

Console.Write "\nEnter Command:"
let mutable input = Console.ReadLine()
 
while input <> "quit"
    do
    if input <> "quit" 
    then
        ChordClient.RunCommand input
        Console.Write "\nEnter Command:" 
        input <- Console.ReadLine()

