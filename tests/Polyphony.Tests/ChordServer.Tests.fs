module ChordServer.Tests

open System
open NUnit.Framework
open SpecUnit
open ChordServer
open System.ServiceModel

[<TestFixture>]      
type ChordServer__when_initializing_the_server () =   
    [<DefaultValue(false)>]  
    val mutable _result : ServiceHost option  
    inherit SpecUnit.ContextSpecification()
        override this.Because () =
            let chordServer = new ChordServer("localhost:1234", "localhost:1234")
            this._result <- ChordServer.Initialize (new SettingsProvider.SettingsProvider()) (new ServiceHost(chordServer))
            ChordServer.Stop this._result                 
        [<Test>]    
        member this.should_return_some_service_host () =    
            this._result.IsSome.ShouldBeTrue |> ignore

[<TestFixture>]
type ChordServer__when_putting_then_getting_from_the_hash_table () =
    [<DefaultValue(false)>]
    val mutable _result : obj
    inherit SpecUnit.ContextSpecification()
        override this.Context () = 
            let chordServer = new ChordServer("localhost:1234","localhost:1234")
            (chordServer  :> IChordServer).PutValueByKey 1 "testValue1" |> ignore
            (chordServer  :> IChordServer).PutValueByKey 2 "testValue2" |> ignore
            this._result <- (chordServer  :> IChordServer).GetValueByKey 2  
        [<Test>]
        member this.should_have_a_result_of_testValue2 () =
            this._result.ShouldEqual("testValue2") |> ignore

[<TestFixture>]
type ChordServer__when_updating_the_successor_node () =
    [<DefaultValue(false)>]
    val mutable _result : obj
    inherit SpecUnit.ContextSpecification()
        override this.Context () = 
            let chordServer = new ChordServer("localhost:1234","localhost:1234")
            this._result <- (chordServer  :> IChordServer).UpdateSuccessorNode "localhost:2222"  
        [<Test>]
        member this.should_have_a_result_of_localhost_2222 () =
            this._result.ShouldEqual("localhost:2222") |> ignore
