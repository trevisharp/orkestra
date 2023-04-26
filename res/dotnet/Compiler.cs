using System;
using System.Reflection;
using System.Collections.Generic;
using static System.Console;

namespace Orkestra;

using Processings;
using LexicalAnalysis;
using SyntacticAnalysis;

public abstract class Compiler
{
    public bool Verbose { get; set; }

    public void Compile(string sourceCode)
    {
        if (Verbose)
        {
            WriteLine("Build started...");
            WriteLine();
        }

        var lex = buildLexicalAnalyzer();
        var parser = buildSyntacticAnalyzer();
        var machine = buildProcessingMachine();

        var processedText = machine.ProcessAll(sourceCode);

        if (Verbose)
        {
            WriteLine("Processed Text:");
            WriteLine(processedText);
            WriteLine();
        }

        var tokens = lex.Parse(processedText);

        if (Verbose)
        {
            WriteLine("Token List:");
            foreach (var token in tokens)
                Write($"{token} ");
            WriteLine();
            WriteLine();
        }

        var tree = parser.Parse(tokens);
        
        if (Verbose)
        {
            WriteLine("Syntax Tree:");
            WriteLine(tree);
            WriteLine();
        }
    }

    private ProcessingPackage buildProcessingMachine()
    {
        ProcessingPackage package = new ProcessingPackage();

        foreach (var process in getFields<Processing>())
            package.Add(process);

        return package;
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