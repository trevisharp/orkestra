/* Author:  Leonardo Trevisan Silio
 * Date:    21/06/2023
 */
using System.IO;
using System.Threading.Tasks;

namespace Orkestra.Extensions.VSCode;

/// <summary>
/// A Grammar Contribute for a VSCode Extension
/// </summary>
public class GrammarContribute(LanguageInfo info) : VSCodeContribute
{

    public override VSCodeContributeType Type 
        => VSCodeContributeType.Grammars;

    public override string Declaration => 
        $$"""
        {
            "language": "{{info.Name}}",
            "scopeName": "source{{info.Extension}}",
            "path" : "./{{info.Name}}.tmLanguage.json"
        }
        """;

    public override string Documentation
    {
        get
        {
            return "";
        }
    }

    public override async Task GenerateFile(string dir, ExtensionArguments args)
    {
        const string schema = "https://raw.githubusercontent.com/martinring/tmlanguage/master/tmlanguage.json";
        var sw = new StreamWriter($"{dir}/{info.Name}.tmLanguage.json");

        await sw.WriteAsync(
            $$"""
            {
                "$schema": "{{schema}}",
                "name": "{{info.Name}}",
                "patterns": [
                    { "include": "#keywords" },
                    { "include": "#strings" }
                ],
                "repository": {
                    "keywords": {
                        "patterns": [{
                            "name": "keyword.control.{{info.Name}}",
                            "match": "\\b(given|define|for|if)\\b"
                        }]
                    }
                },
                "scopeName": "source{{info.Extension}}"
            }
            """
        );

        sw.Close();
    }
}