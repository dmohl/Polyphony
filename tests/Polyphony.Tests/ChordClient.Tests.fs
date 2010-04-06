module ChordClient.Tests

open System
open NUnit.Framework
open SpecUnit
open ChordServer
open ChordServerProxy
open FakeChordServerProxy
open System.ServiceModel

[<TestFixture>]      
type ChordClient__when_running_a_put_command () =   
    [<DefaultValue(false)>]  
    val mutable _result : obj  
    inherit SpecUnit.ContextSpecification()
        override this.Because () =
            let chordServerProxy = new FakeChordServerProxy()
            let stringList = [|"put"; "1"; "test1"|]
            this._result <- ChordClient.RunCommand "put 1 test1" chordServerProxy
        [<Test>]    
        member this.should_have_a_result_of_Put_Complete () =    
            this._result.ShouldEqual("PUT Key:\"1\" Value:\"test1\"") |> ignore

[<TestFixture>]      
type ChordClient__when_running_a_get_command () =   
    [<DefaultValue(false)>]  
    val mutable _result : obj  
    inherit SpecUnit.ContextSpecification()
        override this.Because () =
            let chordServerProxy = new FakeChordServerProxy()
            let stringList = [|"get"; "1"|]
            this._result <- ChordClient.RunCommand "get 1" chordServerProxy
        [<Test>]    
        member this.should_have_a_result_of_1 () =    
            this._result.ShouldEqual("1") |> ignore

[<TestFixture>]      
type ChordClient__when_joining_the_chord_node_network () =   
    [<DefaultValue(false)>]  
    val mutable _result : obj  
    inherit SpecUnit.ContextSpecification()
        override this.Because () =
            this._result <- ChordClient.JoinChordNodeNetwork ()
        [<Test>]    
        member this.should_have_a_successor_server_of__localhost_9876 () =    
            this._result.ShouldEqual("net.tcp://localhost:9861") |> ignore
