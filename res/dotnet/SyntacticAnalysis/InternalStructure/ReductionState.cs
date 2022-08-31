using System.Collections.Generic;

namespace Orkestra.SyntacticAnalysis.InternalStructure;

public record ReductionState(
    StackLinkedListNode InitialNode,
    StackLinkedListNode CurrentNode,
    IEnumerable<SubRule> Attempts,
    List<IEnumerator<IRuleElement>> Iterators,
    StackLinkedListNode ReverseParameter);