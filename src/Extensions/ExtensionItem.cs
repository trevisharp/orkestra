/* Author:  Leonardo Trevisan Silio
 * Date:    11/06/2023
 */
using System.Threading.Tasks;

namespace Orkestra.Extensions;

/// <summary>
/// Represents a item of a extension that can be provide your content.
/// </summary>
public abstract class ExtensionItem
{
    public string Name { get; set; }
    public abstract Task Create(ExtensionArguments args);
}