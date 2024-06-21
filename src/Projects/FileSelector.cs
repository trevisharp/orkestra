/* Author:  Leonardo Trevisan Silio
 * Date:    21/06/2024
 */
using System.IO;
using System.Collections.Generic;

namespace Orkestra.Projects;

/// <summary>
/// A object to select files to be processed.
/// </summary>
public class FileSelector(string query) : PathSelector
{
    public string Extension
    {
        get
        {
            var parts = query.Split(".");
            var final = parts[^1];
            return $".{final}";
        }
    }

    public override IEnumerable<string> GetFiles(string baseFile)
    {
        query ??= "";
        query = query.Replace("*", "");

        var files = Directory.GetFiles(baseFile);
        foreach (var file in files)
        {
            var name = Path.GetFileName(file);
            if (!name.Contains(query))
                continue;
            
            yield return file;
        }
    }
}