/* Author:  Leonardo Trevisan Silio
 * Date:    29/04/2024
 */
using System.Collections;
using System.Collections.Generic;

namespace Orkestra.Errors;

public class ErrorQueue : IEnumerable<Error>
{
    private static ErrorQueue main = new ErrorQueue();
    public static ErrorQueue Main => main;

    public static void New()
        => main = new ErrorQueue();

    private Queue<Error> queue = new Queue<Error>();

    public void Enqueue(Error error)
        => queue.Enqueue(error);

    public Error Dequeue()
        => queue.Dequeue();
    
    public IEnumerator<Error> GetEnumerator()
        => queue.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
        => GetEnumerator();
}