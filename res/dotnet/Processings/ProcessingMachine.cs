using System;
using System.Collections.Generic;

namespace Orkestra.Processings;

public class ProcessingMachine
{
    public List<Processing> Processings { get; private set; } = new List<Processing>();

    public void Add(Processing processing)
        => this.Processings.Add(processing);

    public string Process(string text)
    {
        throw new NotImplementedException();
    }
}