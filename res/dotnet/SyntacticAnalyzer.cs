using System.Collections.Generic;

namespace Orkestra;

public class SyntacticAnalyzer
{
    public Rule StartRule { get; set; }
    
    public List<Rule> Rules { get; private set; } = new List<Rule>();
    public void Add(Rule rule) => this.Rules.Add(rule);

    public void Parse(IEnumerable<Token> tokens)
    {
        Stack<ParseState> stack = new Stack<ParseState>();


    }
}