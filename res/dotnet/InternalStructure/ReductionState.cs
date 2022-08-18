using System.Collections.Generic;

namespace Orkestra.InternalStructure;

public record ReductionState(
    StackLinkedListNode InitialNode, 
    StackLinkedListNode CurrentNode,
    int CurrentNodeIndex,
    IEnumerable<SubRule> Attempts,
    StackLinkedListNode ReverseParameter);