module ChordClient.Tests

open System
open NUnit.Framework
open SpecUnit
open ChordServer
open ChordServerProxy
open System.ServiceModel
open Rhino.Mocks

[<TestFixture>]      
type ChordClient__when_running_a_put_command () =   
    [<DefaultValue(false)>]  
    val mutable _mocks : MockRepository  
    [<DefaultValue(false)>]  
    val mutable _result : obj  
    override this.Context () =   
        this._mocks <- new MockRepository()  
    inherit SpecUnit.ContextSpecification()
        override this.Because () =
            let chordServerProxy = this._mocks.DynamicMock<IChordServerProxy>([||])
            let stringList = [|"put"; "1"; "test1"|]
            Expect.Call(chordServerProxy.CallServer "" "" stringList).IgnoreArguments().Return("Put Complete" :> obj) |> ignore           
            this._result <- ChordClient.RunCommand "put 1 test1" chordServerProxy
        [<Test>]    
        member this.should_have_a_result_of_Put_Complete () =    
            this._result.ShouldEqual("PUT Key:\"1\" Value:\"test1\"") |> ignore
