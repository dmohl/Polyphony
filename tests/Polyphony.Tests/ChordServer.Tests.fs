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
            this._result <- ChordServer.Initialize (new SettingsProvider.SettingsProvider()) (new ServiceHost(typeof<ChordServer.ChordServer>))
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
            let chordServer = new ChordServer()
            (chordServer  :> IChordServer).PutValueByKey 1 "testValue1" |> ignore
            (chordServer  :> IChordServer).PutValueByKey 2 "testValue2" |> ignore
            this._result <- (chordServer  :> IChordServer).GetValueByKey 2  
        [<Test>]
        member this.should_have_a_result_of_testValue2 () =
            this._result.ShouldEqual("testValue2") |> ignore
