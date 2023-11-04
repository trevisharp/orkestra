using System.Collections.Generic;

namespace Orkestra.SyntacticAnalysis;

using InternalStructure;

public class LR0SyntacticAnalyzerBuilder : ISyntacticAnalyzerBuilder
{
    public Rule StartRule { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public void Add(Rule rule)
    {
        throw new System.NotImplementedException();
    }

    public ISyntacticAnalyzer Build()
    {
        throw new System.NotImplementedException();
    }

    public void Load()
    {
        throw new System.NotImplementedException();
    }

    public bool LoadCache()
    {
        throw new System.NotImplementedException();
    }

    public void SaveCache()
    {
        throw new System.NotImplementedException();
    }
}