module ChordServerHelper

open ChordDataContracts

let BuildNodeNeighbors predecessor successor = 
    let nodeNeighbors = new NodeNeighbors()
    nodeNeighbors.PredecessorNode <- predecessor
    nodeNeighbors.SuccessorNode <- successor
    nodeNeighbors
