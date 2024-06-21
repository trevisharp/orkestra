/* Author:  Leonardo Trevisan Silio
 * Date:    21/06/2023
 */
using System.IO;
using System.Threading.Tasks;

namespace Orkestra.Extensions.VSCode;

/// <summary>
/// A Language Contribute for a VSCode Extension.
/// </summary>
public class LanguageContribute(LanguageInfo info) : VSCodeContribute
{
    public override VSCodeContributeType Type 
        => VSCodeContributeType.Languages;

    public override string Declaration => 
        $$"""
        {
            "id": "{{info.Name}}",
            "aliases": [ "{{info.Name}}" ],
            "extensions": [ "{{info.Extension}}" ],
            "configuration": "./{{info.Name}}-configuration.json"
        }
        """;

    public override string Documentation => 
        """

        """;

    public override async Task GenerateFile(string dir, ExtensionArguments args)
    {
        var sw = new StreamWriter($"{dir}/{info.Name}-configuration.json");

        await sw.WriteLineAsync(
            $$"""
            {
                "comments": {
                    "lineComment": "//",
                }
            }
            """
        );
        
        sw.Close();
    }
}