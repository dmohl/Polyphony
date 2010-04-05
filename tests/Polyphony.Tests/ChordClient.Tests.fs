module ChordClient.Tests

open System
open NUnit.Framework
open SpecUnit
open ChordServer
open System.ServiceModel

[<TestFixture>]      
type ChordCleint__when_initializing_the_server () =   
    [<DefaultValue(false)>]  
    val mutable _result : ServiceHost option  
    inherit SpecUnit.ContextSpecification()
        override this.Because () =
            this._result <- ChordServer.Initialize (new SettingsProvider.SettingsProvider()) (new ServiceHost(typeof<ChordServer.ChordServer>))
            ChordServer.Stop this._result                 
        [<Test>]    
        member this.should_return_some_service_host () =    
            this._result.IsSome.ShouldBeTrue |> ignore
