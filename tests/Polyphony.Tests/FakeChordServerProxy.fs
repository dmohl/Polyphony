module FakeChordServerProxy

open ChordServerProxy

type FakeChordServerProxy() = 
    interface IChordServerProxy with
        member this.CallServer server operationContract inputArguments =
            try
                let result = match operationContract with
                             | "put" ->
                                 Some("Put Complete" :> obj)
                             | "get" -> 
                                 Some(inputArguments.[1] :> obj)     
                             | _ -> None 
                result             
            with
            | ex -> 
                None
