using System;

namespace Orkestra;

public abstract class Processing
{
    public abstract TextFragment Process(ProcessingParameters input);
}