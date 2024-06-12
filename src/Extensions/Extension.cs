/* Author:  Leonardo Trevisan Silio
 * Date:    11/06/2023
 */
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Orkestra.Extensions;

/// <summary>
/// Represents a language extension.
/// </summary>
public abstract class Extension
{
    public abstract Task Generate(ExtensionArguments args);
}