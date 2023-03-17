using System.Collections.Generic;

namespace Orkestra.Processings;

using Packages;

public class ProcessingPackage : Package<Processing>
{
    private List<Processing> processings = new List<Processing>();
    public override IEnumerable<Processing> Elements => processings;

    public void Add(Processing processing)
        => this.processings.Add(processing);

    public Text ProcessAll(Text text)
    {
        return null;
    }
}