using System;
using System.Linq;
using System.Collections.Generic;

namespace Orkestra.Processings;

public class Text
{
    public Text(IEnumerable<string> lines)
    {
        this.originalLines = lines;
    }

    private IEnumerable<object> originalLines;
}