module ChordCommon

type CommandType = 
    | Put
    | Get
    | Join
    | UpdateSuccessorNode
    
type NodeNeighbors = { PredecessorNode : string; SuccessorNode: string; }