using System;

namespace Orkestra.Processings;

using Packages;

public abstract class Processing : PackageElement
{
    public abstract Text Process(Text text);

    public static Processing FromFunction(Func<Text, Text> func)
        => new FuncProcessing(func);
}