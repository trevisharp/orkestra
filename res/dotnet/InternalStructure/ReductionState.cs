using System.Collections.Generic;

namespace Orkestra.InternalStructure;

public record ReductionState(
    StackLinkedListNode InitialNode, 
    StackLinkedListNode CurrentNode, 
    IEnumerable<SubRule> Attempts);