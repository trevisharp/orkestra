/* Author:  Leonardo Trevisan Silio
 * Date:    21/06/2023
 */
using System.Collections.Generic;
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

    public override async Task GenerateFile(string dir)
    {
        const string schema = "https://raw.githubusercontent.com/martinring/tmlanguage/master/tmlanguage.json";
        var sw = new StreamWriter($"{dir}/{info.Name}.tmLanguage.json");

        string nums = "";
        string ids = "";

        var others = new List<string>();

        string append(string regex, string exp)
        {
            if (regex is null || regex == string.Empty)
                return exp;
            
            return regex + "|" + exp;
        }

        foreach (var key in info.Keys)
        {
            if (key.IsAuto)
                continue;
            
            if (key.IsIdentity) {
                ids = append(ids, key.Expression);
                continue;
            }
            
            if (!key.IsKeyword && key.Expression.Contains('0')) {
                nums = append(nums, key.Expression);
                continue;
            }

            others.Add(key.Expression);
        }

        string keywords = "";
        string controls = "";
        string operations = "";
        string definitions = "";

        

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
                    { "include": "#entitys" }
                ],
                "repository": {
                    "keywords": {
                        "patterns": [
                            {
                                "name": "keyword.{{info.Name}}",
                                "match": "\\b({{keywords}})\\b"
                            },
                            {
                                "name": "keyword.control.{{info.Name}}",
                                "match": "\\b({{controls}})\\b"
                            }
                        ]
                    },

                    "entitys": {
                        "patterns": [
                            {
                                "name": "entity.name.function.{{info.Name}}",
                                "match": "\\b({{operations}})\\b"
                            },
                            {
                                "name": "entity.name.class.{{info.Name}}",
                                "match": "\\b({{definitions}})\\b"
                            }
                        ]
                    },

                    "constants": {
                        "patterns": [
                            {
                                "name": "constant.numeric.{{info.Name}}",
                                "match": "\\b({{nums}})\\b"
                            }
                        ]
                    },

                    "variables": {
                        "patterns": [
                            {
                                "name": "variable.parameter.{{info.Name}}",
                                "match": "\\b({{ids}})\\b"
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