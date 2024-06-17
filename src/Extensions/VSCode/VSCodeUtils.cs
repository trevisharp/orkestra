/* Author:  Leonardo Trevisan Silio
 * Date:    17/06/2023
 */
using System;

namespace Orkestra.Extensions.VSCode;

/// <summary>
/// Util extension methods for VSCode convertions.
/// </summary>
public static class VSCodeUtils
{
    public static string ToCamelCase(this VSCodeContributeType type)
    {
        var name = type.ToString();
        var chars = name.ToCharArray();
        chars[0] = char.ToLowerInvariant(chars[0]);
        return new string(chars);
    }
}