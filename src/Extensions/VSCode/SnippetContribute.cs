/* Author:  Leonardo Trevisan Silio
 * Date:    28/06/2023
 */
using System.IO;
using System.Linq;
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
        
        bool first = true;
        foreach (var fst in info.Rules.GetFirstSet())
        {
            var header = fst.RuleTokens.FirstOrDefault() as Key;
            if (header is null)
                continue;
            var headerExp = header.Expression;
            var normalForm = fst.GetNormalForm();

            await sw.WriteAsync(
                $$"""
                {{(first ? "{" : ",")}}
                    "{{headerExp}}" : {
                        "prefix": "{{headerExp}}",
                        "body": [
                            "{{normalForm}}"
                        ],
                        "descrition": "{{headerExp}} snippet."
                    }
                """
            );
            first = false;
        }

        await sw.WriteAsync("}");
        sw.Close();
    }
}