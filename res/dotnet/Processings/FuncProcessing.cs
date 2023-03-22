using System;

namespace Orkestra.Processings;

public class FuncProcessing : Processing
{
    public FuncProcessing(Func<Text, Text> func)
        => this.func = func;

    private Func<Text, Text> func;

    public override Text Process(Text text)
        => func(text);
}