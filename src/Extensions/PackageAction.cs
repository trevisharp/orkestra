/* Author:  Leonardo Trevisan Silio
 * Date:    11/06/2023
 */
using System.Threading.Tasks;

namespace Orkestra.Extensions;

/// <summary>
/// Represents a step in the process of package a extension.
/// </summary>
public abstract class PackageAction
{
    public abstract Task Pack(ExtensionArguments args);
}