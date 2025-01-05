/* Author:  Leonardo Trevisan Silio
 * Date:    05/01/2024
 */
using System.Collections.Generic;

namespace Orkestra.Extensions;

/// <summary>
/// Represents info of a specific language.
/// </summary>
public class LanguageInfo
{
    public required string Name { get; init; }
    public required Rule InitialRule { get; init; }
    public required List<Rule> Rules { get; init; }
    public required List<Key> Keys { get; init; }
    public required List<Processing> Processings { get; init; }
}