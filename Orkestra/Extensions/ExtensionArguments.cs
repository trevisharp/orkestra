/* Author:  Leonardo Trevisan Silio
 * Date:    11/06/2023
 */
using System.Collections.Generic;

namespace Orkestra.Extensions;

/// <summary>
/// Represents parameters to create a item.
/// </summary>
public class ExtensionArguments {
    public string Name { get; set; }
    public string[] Arguments { get; set; }
    public List<LanguageInfo> Languages { get; set; } = new();
}