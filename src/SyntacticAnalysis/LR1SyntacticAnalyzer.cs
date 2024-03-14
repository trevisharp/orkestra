/* Author:  Leonardo Trevisan Silio
 * Date:    14/03/2024
 */
using System.Collections.Generic;

namespace Orkestra.SyntacticAnalysis;

public class LR1SyntacticAnalyzer(
    int side,
    int[] table,
    Dictionary<ISyntacticElement, int> indexMap,
    Dictionary<int, int> sizeMap
) : ISyntacticAnalyzer
{
    public ExpressionTree Parse(IEnumerable<Token> tokens)
    {
        const int accept = int.MaxValue / 2;
        const int shift = int.MaxValue / 4;
        const int reduce = int.MaxValue / 8;
        const int keymod = int.MaxValue / 16;

        Stack<int> stack = new Stack<int>();
        stack.Push(0);

        var it = tokens.GetEnumerator();
        if (!it.MoveNext())
            return null;
        var token = it.Current;
        var tokenIndex = indexMap[token.Key];

        while (true)
        {
            var state = stack.Peek();

            var value = table[tokenIndex + state * side];
            var argument = value % keymod;
            var operation = value - argument;

            if (operation == shift)
            {
                stack.Push(tokenIndex);
                stack.Push(argument);

                if (!it.MoveNext())
                    return null;
                token = it.Current;
                tokenIndex = indexMap[token.Key];
            }
            else if (operation == reduce)
            {
                int reduceSize = 2 * sizeMap[argument];
                for (int i = 0; i < reduceSize; i++)
                    stack.Pop();
                
                state = stack.Peek();
                stack.Push(argument);

                value = table[tokenIndex + state * side];
                argument = value % keymod;
                stack.Push(argument);
            }
            else if (operation == accept && !it.MoveNext())
            {
                break;
            }
            else
            {
                // Errors
            }
        }

        throw new System.NotImplementedException();
    }
}