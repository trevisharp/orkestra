using System.Linq;
using System.Collections.Generic;

namespace Orkestra;

public class SyntacticAnalyzer
{
    public Rule StartRule { get; set; }

    public List<Rule> Rules { get; private set; } = new List<Rule>();
    public void Add(Rule rule) => this.Rules.Add(rule);

    public ExpressionTree Parse(IEnumerable<Token> tokens)
    {
        debugPrint("Starting Syntactic Analyzer...");
        ExpressionTree tree = new ExpressionTree();

        List<INode> buffer = new List<INode>(tokens);
        
        while (buffer.Count > 1)
        {
            debugPrint($"Buffer Size: {buffer.Count}");
            if (!matchRule(buffer))
            {
                break;
            }
        }

        tree.Root = buffer[0];
        return tree;
    }

    private bool matchRule(List<INode> buffer)
    {
        foreach (var rule in Rules)
        {
            if (matchRule(buffer, rule))
                return true;
        }
        return false;
    }

    private bool matchRule(List<INode> buffer, Rule rule)
    {
        debugPrint($"\tTry Matching {rule.Name} rule...");
        foreach (var subRule in rule.SubRules)
        {
            if (matchRule(buffer, subRule))
                return true;
        }
        debugPrint($"\t\tFail...");
        return false;
    }

    private bool matchRule(List<INode> buffer, SubRule attempt)
    {
        debugPrint("\t\tIn SubRule:");
        bool success = false;
        for (int i = 0; i < buffer.Count; i++)
        {
            if (matchRule(buffer, attempt, i))
            {
                debugPrint($"\t\t\tMatch!");
                success = true;
                i = 0;
            }
        }
        return success;
    }

    private bool matchRule(List<INode> buffer, SubRule attempt, int start)
    {
        debugPrint($"\t\t\t\t start = {start}");
        var len = attempt.RuleTokens.Count();
        if (buffer.Count - start < len)
            return false;
        
        int count = 0;
        foreach (var ruleToken in attempt.RuleTokens)
        {
            debugPrint($"\t\t\t\t\t {ruleToken} == {buffer[start + count]}? {buffer[start + count].Is(ruleToken)}");
            if (!buffer[start + count].Is(ruleToken))
                return false;
            count++;
        }

        RuleMatch match = new RuleMatch(
            attempt,
            buffer.Skip(start).Take(len).ToArray()
        );
        buffer.RemoveRange(start, len);
        buffer.Insert(start, match);

        return true;
    }

    private void debugPrint(object str)
    {
        #if DEBUG
        System.Console.WriteLine(str);
        #endif
    }
}