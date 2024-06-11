/* Author:  Leonardo Trevisan Silio
 * Date:    11/06/2023
 */
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orkestra.Extensions;

/// <summary>
/// Represents a language extension.
/// </summary>
public abstract class Extension : List<ExtensionItem>
{
    public List<PackageAction> PackageActions { get; private set; } = new();

    public async Task Generate(ExtensionArguments args) {
        foreach (var item in this) {
            await item.Create(args);
        }

        foreach (var action in this.PackageActions) {
            await action.Pack(args);
        }
    }
}