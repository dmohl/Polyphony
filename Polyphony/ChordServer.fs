module ChordServer

open System
open System.Runtime.Remoting
open System.Runtime.Remoting.Channels
open System.Runtime.Remoting.Channels.Tcp

let Initialize port =
    let tcpChannel = new TcpChannel(port)
    Console.WriteLine("Polyphony Server started on port {0}...", port)    
    ChannelServices.RegisterChannel(tcpChannel, false) |> ignore
    let commonType = typeof<Shared.Chord>
    RemotingConfiguration.RegisterWellKnownServiceType(commonType,
        "chord", WellKnownObjectMode.SingleCall) |> ignore
   
