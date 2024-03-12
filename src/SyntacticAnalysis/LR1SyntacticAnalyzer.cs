/* Author:  Leonardo Trevisan Silio
 * Date:    04/11/2023
 */
using System.Collections.Generic;

namespace Orkestra.SyntacticAnalysis;

using InternalStructure;

public class LR1SyntacticAnalyzer(
    int side,
    int[] table,
    ISyntaticElement[] elements
) : ISyntacticAnalyzer
{
    public ExpressionTree Parse(IEnumerable<Token> tokens)
    {
        throw new System.NotImplementedException();
    }
}