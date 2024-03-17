/* Author:  Leonardo Trevisan Silio
 * Date:    17/03/2024
 */
using System.IO;
using System.Collections.Generic;

namespace Orkestra.Projects;

/// <summary>
/// A object to select files to be processed.
/// </summary>
public class FileSelector(string query) : PathSelector
{
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