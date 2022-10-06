using System;

namespace Orkestra.Processings;

public abstract class Processing
{
    public abstract TextFragment Process(string[] lines);
}