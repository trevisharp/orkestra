using System.Collections.Generic;

namespace Orkestra.Packages;

public class PackageCollection : IPackage
{
    private List<IPackage> packages = new List<IPackage>();
    public IEnumerable<IPackage> Packages => packages;

    public void Add(IPackage package)
        => this.packages.Add(package);
}