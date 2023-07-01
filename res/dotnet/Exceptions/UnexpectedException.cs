/* Author:  Leonardo Trevisan Silio
 * Date:    30/06/2023
 */
using System;

namespace Orkestra.Exceptions;

[Serializable]
public class UnexpectedException : OrkestraException
{
    public override string Message =>
    $"""
        A unexpected exception occurs:
        {inner}
    """;

    private Exception inner;
    public UnexpectedException(Exception inner)
        => this.inner = inner;
}