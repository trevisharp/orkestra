using System;
using System.Reflection;
using System.Collections.Generic;

namespace Orkestra;

public abstract class Compiler
{
    private LexicalAnalyzer buildLexicalAnalyzer()
    {
        LexicalAnalyzer lexicalAnalyzer =  new LexicalAnalyzer();

        var type = this.GetType();
        foreach (var filed in type.GetRuntimeFields())
        {
            if (filed.FieldType == typeof(Key))
            {
                var key = filed.GetValue(this) as Key;
                lexicalAnalyzer.Add(key);
            }
        }

        return lexicalAnalyzer;
    }

    public void Compile(string sourceCode)
    {
        var lex = buildLexicalAnalyzer();
        var tokens = lex.Parse(sourceCode);

        foreach (var token in tokens)
        {
            Console.WriteLine(token.Key.Name);
        }
    }
}