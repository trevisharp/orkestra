/* Author:  Leonardo Trevisan Silio
 * Date:    11/06/2023
 */
using System.Collections.Generic;

namespace Orkestra.Extensions;

/// <summary>
/// Represents parameters to create a item.
/// </summary>
public class ExtensionArguments {
    public string[] Arguments { get; set; }
    public List<Rule> Rules { get; set; }
    public List<Key> Keys { get; set; }
}