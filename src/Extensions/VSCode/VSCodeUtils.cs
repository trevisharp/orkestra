/* Author:  Leonardo Trevisan Silio
 * Date:    18/06/2023
 */
namespace Orkestra.Extensions.VSCode;

/// <summary>
/// Util extension methods for VSCode convertions.
/// </summary>
public static class VSCodeUtils
{
    public static string ToCamelCasePlural(this VSCodeContributeType type)
    {
        var name = type.ToString();
        var chars = name.ToCharArray();
        chars[0] = char.ToLowerInvariant(chars[0]);
        return new string(chars) + "s";
    }
}