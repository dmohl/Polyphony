module ChordCommon

open System

let LogException (ex:Exception) =
    Console.WriteLine("Error: {0}", ex.Message)
    Console.WriteLine(ex.Source)
    
type CommandType = 
    | Put
    | Get
    | Join
    | UpdateSuccessorNode
    | GetSuccessorNode

