/* Author:  Leonardo Trevisan Silio
 * Date:    28/06/2023
 */
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Orkestra.Extensions.VSCode;

using Processings.Implementations;

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

    public override async Task GenerateFile(string dir)
    {
        var sw = new StreamWriter($"{dir}/{info.Name}-configuration.json");

        var lineComment = info.Processings
            .FirstOrDefault(p => p is LineCommentProcessing)
            as LineCommentProcessing;

        await sw.WriteLineAsync(
            $$"""
            {
                "comments": {
                    {{(
                        lineComment is null ? "" : $"\"lineComment\": \"{lineComment.CommentStarter}\""
                    )}}
                }
            }
            """
        );
        
        sw.Close();
    }
}