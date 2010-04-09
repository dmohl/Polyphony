module ChordCommon

type CommandType = 
    | Put
    | Get
    | Join
    | UpdateSuccessorNode
    | GetSuccessorNode
    
type NodeNeighbors = { PredecessorNode : string; SuccessorNode: string; }