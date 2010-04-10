﻿module ChordClient.Tests

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
            this._result <- ChordClient.RunCommand "put 1 test1" chordServerProxy (new SettingsProvider.SettingsProvider())
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
            this._result <- ChordClient.RunCommand "get 1" chordServerProxy (new SettingsProvider.SettingsProvider())
        [<Test>]    
        member this.should_have_a_result_of_1 () =    
            this._result.ShouldEqual("1") |> ignore

[<TestFixture>]      
type ChordClient__when_joining_the_chord_node_network_with_a_node_inbetween_two_existing_nodes () =   
    [<DefaultValue(false)>]  
    val mutable _result : obj  
    inherit SpecUnit.ContextSpecification()
        override this.Because () =
            let chordServerProxy = new FakeChordServerProxy()
            this._result <- ChordClient.JoinChordNodeNetwork 
                "localhost:2222" "localhost:1111" chordServerProxy
        [<Test>]    
        member this.should_have_a_successor_server_of__localhost_3333 () =    
            this._result.ShouldEqual("localhost:3333") |> ignore

type ChordClient__when_joining_the_chord_node_network_with_a_node_less_than_the_smallest_existing_node () =   
    [<DefaultValue(false)>]  
    val mutable _result : obj  
    inherit SpecUnit.ContextSpecification()
        override this.Because () =
            let chordServerProxy = new FakeChordServerProxy()
            this._result <- ChordClient.JoinChordNodeNetwork 
                "localhost:0001" "localhost:1111" chordServerProxy
        [<Test>]    
        member this.should_have_a_successor_server_of__localhost_1111 () =    
            this._result.ShouldEqual("localhost:1111") |> ignore
