/* Author:  Leonardo Trevisan Silio
 * Date:    29/04/2024
 */
using System.Collections.Generic;

namespace Orkestra.Errors;

using Packages;

/// <summary>
/// Represents a package import error.
/// </summary>
public class PackageError : Package<Error>
{
    private List<Error> errors = new List<Error>();
    public override IEnumerable<Error> Elements => errors;
    public void Add(Error error)
        => this.errors.Add(error);
}