using System;
using System.Reflection;
using System.Collections.Generic;

#if DEBUG
using static System.Console;
#endif

namespace Orkestra;

public abstract class Compiler
{
    public void Compile(string sourceCode)
    {
        var lex = buildLexicalAnalyzer();
        var parser = buildSyntacticAnalyzer();
        var machine = buildProcessingMachine();

        var tokens = lex.Parse(sourceCode);

        #if DEBUG
        foreach (var token in tokens)
            Write($"{token} ");
        WriteLine();
        #endif

        var tree = parser.Parse(tokens);
        
        #if DEBUG
        WriteLine(tree);
        #endif
    }

    private ProcessingMachine buildProcessingMachine()
    {
        ProcessingMachine machine = new ProcessingMachine();

        foreach (var process in getFields<Processing>())
            machine.Add(process);

        return machine;
    }

    private LexicalAnalyzer buildLexicalAnalyzer()
    {
        LexicalAnalyzer lexicalAnalyzer = new LexicalAnalyzer();

        foreach (var key in getFields<Key>())
            lexicalAnalyzer.Add(key);

        return lexicalAnalyzer;
    }

    private SyntacticAnalyzer buildSyntacticAnalyzer()
    {
        SyntacticAnalyzer syntacticAnalyzer = new SyntacticAnalyzer();

        foreach (var rule in getFields<Rule>())
        {
            if (rule.IsStartRule)
                syntacticAnalyzer.StartRule = rule;
            syntacticAnalyzer.Add(rule);
        }

        return syntacticAnalyzer;
    }

    private IEnumerable<T> getFields<T>()
        where T : class
    {
        var type = this.GetType();
        foreach (var filed in type.GetRuntimeFields())
        {
            if (filed.FieldType == typeof(T))
                yield return filed.GetValue(this) as T;
        }
    }
}