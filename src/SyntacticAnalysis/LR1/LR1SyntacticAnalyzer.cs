/* Author:  Leonardo Trevisan Silio
 * Date:    21/04/2024
 */
using System.Collections.Generic;

namespace Orkestra.SyntacticAnalysis.LR1;

public class LR1SyntacticAnalyzer(
    int rowSize,
    int[] table,
    Dictionary<ISyntacticElement, int> indexMap
) : ISyntacticAnalyzer
{
    public ExpressionTree Parse(IEnumerable<Token> tokens)
    {
        const int accept = 1 << 28;
        const int shift = 1 << 29;
        const int reduce = 1 << 30;
        const int keymod = 1 << 27;
        const int sizeParam = 1 << 16;

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

            var value = table[tokenIndex + state * rowSize];
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
                int rule = argument % sizeParam;
                int size = argument / sizeParam;
                int reduceSize = 2 * size;
                for (int i = 0; i < reduceSize; i++)
                    stack.Pop();
                
                state = stack.Peek();
                stack.Push(rule);

                value = table[tokenIndex + state * rowSize];
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