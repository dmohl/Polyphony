module Polyphony

open System
open System.Configuration

let port = ConfigurationManager.AppSettings.Item("ServerPort")

ChordServer.Initialize(Convert.ToInt32(port)) |> ignore
ChordClient.Initialize() |> ignore

Console.Write "\nEnter Command:"
let mutable input = Console.ReadLine()
 
while input <> "quit"
    do
    if input <> "quit" 
    then
        ChordClient.RunCommand input
        Console.Write "\nEnter Command:" 
        input <- Console.ReadLine()

