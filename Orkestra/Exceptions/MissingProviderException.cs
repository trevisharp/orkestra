/* Author:  Leonardo Trevisan Silio
 * Date:    01/05/2025
 */
using System;

namespace Orkestra.Exceptions;

public class MissingProviderException : Exception
{
    public override string Message => "A Compiler needs a non-null value on field Provider.";
}