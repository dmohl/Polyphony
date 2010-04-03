module Polyphony

open System
open System.Configuration

ChordServer.Initialize() |> ignore

Console.Write "\nEnter Command:"
let mutable input = Console.ReadLine()
 
while input <> "quit"
    do
    if input <> "quit" 
    then
        ChordClient.RunCommand input
        Console.Write "\nEnter Command:" 
        input <- Console.ReadLine()

