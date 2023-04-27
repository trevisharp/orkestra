using System.Collections.Generic;

namespace Orkestra.Errors;

using Packages;

public class ErrorPackage : Package<Error>
{
    private List<Error> errors = new List<Error>();
    public override IEnumerable<Error> Elements => errors;
    public void Add(Error error)
        => this.errors.Add(error);
}