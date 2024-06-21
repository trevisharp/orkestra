/* Author:  Leonardo Trevisan Silio
 * Date:    21/06/2023
 */
using System.Threading.Tasks;

namespace Orkestra.Extensions.VSCode;

/// <summary>
/// A Grammar Contribute for VSCode Extension
/// </summary>
public class GrammarContribute(LanguageInfo info) : VSCodeContribute
{
    public override VSCodeContributeType Type 
        => VSCodeContributeType.Grammars;

    public override string Declaration => 
        $$"""
        {
            "language": {{info.Name}},
            "scopeName": source{{info.Extension}}
        }
        """;

    public override string Documentation => throw new System.NotImplementedException();

    public override Task GenerateFile(string dir, ExtensionArguments args)
    {
        throw new System.NotImplementedException();
    }
}