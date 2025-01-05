/* Author:  Leonardo Trevisan Silio
 * Date:    28/06/2023
 */
using System.Collections.Generic;

namespace Orkestra.Extensions;

using Processings;

/// <summary>
/// Represents info of a specific language.
/// </summary>
public class LanguageInfo
{
    public string Name { get; set; }
    public string Extension { get; set; }
    public List<Rule> Rules { get; set; }
    public List<Key> Keys { get; set; }
    public List<Processing> Processings { get; set; }
}