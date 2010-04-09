module FakeChordServerProxy

open ChordServerProxy
open ChordCommon

type FakeChordServerProxy() = 
    interface IChordServerProxy with
        member this.CallServer server operationContract inputArguments =
            try
                let result = 
                    match operationContract with
                    | CommandType.Put -> Some("Put Complete" :> obj)
                    | CommandType.Get -> Some(inputArguments.[1] :> obj)
                    | CommandType.Join -> 
                        let nodeNeighbors = new NodeNeighbors()
                        nodeNeighbors.PredecessorNode <- "localhost:1111"
                        nodeNeighbors.SuccessorNode <- "localhost:3333"
                        Some(nodeNeighbors :> obj)
                    | CommandType.UpdateSuccessorNode -> Some(inputArguments.[0] :> obj)
                    | CommandType.GetSuccessorNode -> Some("localhost:3333" :> obj)
                result             
            with
            | ex -> 
                None
