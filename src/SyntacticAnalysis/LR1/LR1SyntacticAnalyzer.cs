/* Author:  Leonardo Trevisan Silio
 * Date:    29/04/2024
 */
using System.Collections.Generic;

namespace Orkestra.SyntacticAnalysis.LR1;

public class LR1SyntacticAnalyzer(
    int rowSize,
    int[] table,
    Dictionary<ISyntacticElement, int> elementMap,
    Dictionary<int, ISyntacticElement> indexMap
) : ISyntacticAnalyzer
{
    public ExpressionTree Parse(IEnumerable<Token> tokens)
    {
        const int accept = 1 << 28;
        const int shift = 1 << 29;
        const int reduce = 1 << 30;
        const int keymod = 1 << 27;
        const int sizeParam = 1 << 16;

        Stack<ExpressionTree> treeStack = new Stack<ExpressionTree>();
        Stack<int> stack = new Stack<int>();
        stack.Push(0);

        var it = tokens.GetEnumerator();
        var token = 
            it.MoveNext() ? 
            it.Current : 
            null;
        var tokenIndex =
            token is not null ?
            elementMap[token.Key] :
            0;

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

                treeStack.Push(
                    new ExpressionTree(token.Key, [], token.Value)
                );

                token = 
                    it.MoveNext() ? 
                    it.Current : 
                    null;
                tokenIndex =
                    token is not null ?
                    elementMap[token.Key] :
                    0;
            }
            else if (operation == reduce)
            {
                int rule = argument % sizeParam;
                int size = argument / sizeParam;

                ExpressionTree[] children = new ExpressionTree[size];
                for (int i = 0, j = size - 1; i < size; i++, j--)
                {
                    children[j] = treeStack.Pop();
                    stack.Pop();
                    stack.Pop();
                }

                var ruleObj = indexMap[rule];
                treeStack.Push(
                    new ExpressionTree(ruleObj, children)
                );
                
                state = stack.Peek();
                stack.Push(rule);

                value = table[rule + state * rowSize];
                argument = value % keymod;
                stack.Push(argument);
            }
            else if (operation == accept && !it.MoveNext())
            {
                break;
            }
            else
            {
                throw 
            }
        }

        return treeStack.Peek();
    }
}