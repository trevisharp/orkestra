using System.Collections.Generic;

namespace Orkestra.Packages;

public abstract class Package<T> : IPackage
    where T : PackageElement
{
    public IEnumerable<T> Elements { get; }
}