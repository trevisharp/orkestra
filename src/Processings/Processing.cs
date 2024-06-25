/* Author:  Leonardo Trevisan Silio
 * Date:    25/06/2024
 */
using System;

namespace Orkestra.Processings;

using Implementations;

/// <summary>
/// A processing code to transform text.
/// </summary>
public abstract class Processing
{
    public abstract Text Process(Text text);

    public static Processing FromFunction(Func<Text, Text> func)
        => new FuncProcessing(func);
    
    public static LineCommentProcessing LineComment(string starter)
        => new LineCommentProcessing(starter);
}