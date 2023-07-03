/* Author:  Leonardo Trevisan Silio
 * Date:    03/07/2023
 */
using System;

namespace Orkestra.Processings;

/// <summary>
/// A processing code to transform text.
/// </summary>
public abstract class Processing
{
    public abstract Text Process(Text text);

    public static Processing FromFunction(Func<Text, Text> func)
        => new FuncProcessing(func);
}