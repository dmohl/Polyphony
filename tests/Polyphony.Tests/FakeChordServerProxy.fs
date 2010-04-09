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
                    | CommandType.Join -> Some({PredecessorNode = "localhost:1111"; SuccessorNode = "localhost:3333"} :> obj)
                    | CommandType.UpdateSuccessorNode -> Some(inputArguments.[0] :> obj)
                result             
            with
            | ex -> 
                None
