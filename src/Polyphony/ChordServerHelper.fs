module ChordServerHelper

open ChordContracts

let BuildNodeNeighbors predecessor successor = 
    let nodeNeighbors = new NodeNeighbors()
    nodeNeighbors.PredecessorNode <- predecessor
    nodeNeighbors.SuccessorNode <- successor
    nodeNeighbors
