namespace Orkestra.Processings;

using Packages;

public abstract class Processing : PackageElement
{
    public abstract Text Process(Text text);
}