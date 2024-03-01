global using System;

using Orkestra;
using Orkestra.LexicalAnalysis;
using Orkestra.SyntacticAnalysis;
using Orkestra.Processings;
using Orkestra.Errors;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");

public class BasicSampleCompiler : Compiler
{
    Key kSUB = keyword("SUB", "\\-");
    Key kMUL = keyword("MUL", "\\*");
    Key kIDENT = key("IDENT", "[0-9]+");

    Rule rExp;
    Rule rTerm;
    Rule rFactor;

    public BasicSampleCompiler()
    {
        rFactor = rule("factor",
            sub(kIDENT)
        );
        
        rTerm = rule("term",
            sub(rFactor)
        );
        rTerm.AddSubRules(
            sub(rFactor, kMUL, rTerm)
        );

        
    }
}