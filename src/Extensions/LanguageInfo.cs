/* Author:  Leonardo Trevisan Silio
 * Date:    21/06/2023
 */
using System.Collections.Generic;

namespace Orkestra.Extensions;

using Projects;

/// <summary>
/// Represents info of a specific language.
/// </summary>
public class LanguageInfo
{
    public string Name { get; set; }
    public PathSelector ExtensionPath { get; set; }
    public List<Rule> Rules { get; set; }
    public List<Key> Keys { get; set; }
}