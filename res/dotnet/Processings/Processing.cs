using System;

namespace Orkestra.Processings;

using Packages;

public abstract class Processing : PackageElement
{
    public abstract TextFragment Process(TextFragment text);
}