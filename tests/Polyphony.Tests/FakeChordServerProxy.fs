module FakeChordServerProxy

open ChordServerProxy
open ChordCommon

type FakeChordServerProxy() = 
    interface IChordServerProxy with
        member this.CallServer server operationContract inputArguments =
            try
                let result = match operationContract with
                             | CommandType.Put -> Some("Put Complete" :> obj)
                             | CommandType.Get -> Some(inputArguments.[1] :> obj)     
                             | _ -> None 
                result             
            with
            | ex -> 
                None
