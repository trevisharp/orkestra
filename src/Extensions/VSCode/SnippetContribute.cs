using System.IO;
using System.Threading.Tasks;

namespace Orkestra.Extensions.VSCode;

public class SnippetContribute(LanguageInfo info) : VSCodeContribute
{
    public override VSCodeContributeType Type => VSCodeContributeType.Snippets;

    public override string Declaration =>
        $$"""
        {
            "language": "{{info.Name}}",
            "path": "./{{info.Name}}-snippets.json"
        }
        """;

    public override string Documentation => 
        """

        """;

    public override async Task GenerateFile(string dir)
    {
        var sw = new StreamWriter($"{dir}/{info.Name}-snippets.json");

        await sw.WriteAsync(
            $$"""
            {
                "Define": {
                    "prefix": "define",
                    "body": [
                        "define ${1:setname} as ${2:subset of nat}",
                    ],
                    "description": "Define Snippet."
                }
            }
            """
        );

        sw.Close();
    }
}