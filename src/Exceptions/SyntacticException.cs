/* Author:  Leonardo Trevisan Silio
 * Date:    29/04/2024
 */
using System.Linq;
using System.Collections.Generic;

namespace Orkestra.Exceptions;

public class SyntacticException(IEnumerable<string> messages) : OrkestraException
{
    public override string Message => 
        messages.Aggregate("", (acc, str) => acc + str + "\n");
}