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

        // https://macromates.com/manual/en/language_grammars
        await sw.WriteAsync(
            $$"""
            {
                "$schema": "{{schema}}",
                "name": "{{info.Name}}",
                "patterns": [
                    { "include": "#keywords" },
                    { "include": "#constants" },
                    { "include": "#variables" },
                    { "include": "#storages" },
                    { "include": "#entitys" }
                ],
                "repository": {
                    "keywords": {
                        "patterns": [
                            {
                                "name": "keyword.control.{{info.Name}}",
                                "match": "\\b(for|if)\\b"
                            },
                            {
                                "name": "keyword.{{info.Name}}",
                                "match": "\\b(given|define|check)\\b"
                            },
                            {
                                "name": "keyword.operator.{{info.Name}}",
                                "match": "\\b(\\+)\\b"
                            },
                            {
                                "name": "keyword.other.{{info.Name}}",
                                "match": "\\b(all|some)\\b"
                            }
                        ]
                    },

                    "constants": {
                        "patterns": [
                            {
                                "name": "constant.numeric.{{info.Name}}",
                                "match": "-?[0-9][0-9\\.]*"
                            }
                        ]
                    },

                    "variables": {
                        "patterns": [
                            {
                                "name": "variable.parameter.{{info.Name}}",
                                "match": "\\b(nat|real|rat|int)\\b"
                            }
                        ]
                    },

                    "storages": {
                        "patterns": [
                            {
                                "name": "storage.type.{{info.Name}}",
                                "match": "\\b(is|contains|as|of|then|in)\\b"
                            }
                        ]
                    },

                    "entitys": {
                        "patterns": [
                            {
                                "name": "entity.name.function.{{info.Name}}",
                                "match": "\\b(subset)\\b"
                            },
                            {
                                "name": "entity.name.class.{{info.Name}}",
                                "match": "\\b([a-z]+)\\b"
                            }
                        ]
                    }
                },
                "scopeName": "source{{info.Extension}}"
            }
            """
        );

        sw.Close();
    }
}