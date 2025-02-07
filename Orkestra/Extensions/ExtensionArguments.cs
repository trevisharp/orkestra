/* Author:  Leonardo Trevisan Silio
 * Date:    07/02/2025
 */
using System.Collections.Generic;

namespace Orkestra.Extensions;

using VSCode;

/// <summary>
/// Represents parameters to create a item.
/// </summary>
public class ExtensionArguments
{
    public required string Name { get; set; }
    public required string[] Arguments { get; set; }
    public List<LanguageInfo> Languages { get; set; } = [];
    public Changelog Changelog { get; set; } = Changelog.Default;
}