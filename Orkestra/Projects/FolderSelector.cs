/* Author:  Leonardo Trevisan Silio
 * Date:    17/03/2024
 */
using System.IO;
using System.Collections.Generic;

namespace Orkestra.Projects;

/// <summary>
/// A object to select files to be processed.
/// </summary>
public class FolderSelector(string query, FileSelector fileSelector) : PathSelector
{
    public override IEnumerable<string> GetFiles(string baseFile)
    {
        query ??= "";
        query = query.Replace("*", "");

        var folders = Directory.GetDirectories(baseFile);
        foreach (var folder in folders)
        {
            var name = Path.GetDirectoryName(folder);
            if (!name.Contains(query))
                continue;
            
            var files = fileSelector.GetFiles(folder);
            foreach (var file in files)
                yield return file;
        }
    }
}