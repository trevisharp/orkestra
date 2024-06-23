/* Author:  Leonardo Trevisan Silio
 * Date:    17/06/2023
 */
using System.Threading.Tasks;

namespace Orkestra.Extensions.VSCode;

/// <summary>
/// Represents a Contribute for a VSCode Extension.
/// </summary>
public abstract class VSCodeContribute
{
    public abstract VSCodeContributeType Type { get; }
    public abstract string Declaration { get; }
    public abstract string Documentation { get; }
    public abstract Task GenerateFile(string dir);
}