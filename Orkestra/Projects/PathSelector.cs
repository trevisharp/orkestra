/* Author:  Leonardo Trevisan Silio
 * Date:    17/03/2024
 */
using System.Collections.Generic;

namespace Orkestra.Projects;

/// <summary>
/// A object to select files to be processed.
/// </summary>
public abstract class PathSelector
{
    public abstract IEnumerable<string> GetFiles(string baseFile);
}