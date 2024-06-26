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
                "Print to console": {
                    "prefix": "print",
                    "body": [
                    "print($1);"
                    ],
                    "description": "Insere um comando de impressão no console."
                },
                "Loop for": {
                    "prefix": "for",
                    "body": [
                    "for (int i = 0; i < ${1:count}; i++) {",
                    "\t$2",
                    "}"
                    ],
                    "description": "Estrutura de loop for."
                }
            }
            """
        );

        sw.Close();
    }
}