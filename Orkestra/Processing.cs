/* Author:  Leonardo Trevisan Silio
 * Date:    04/07/2024
 */
using System;

namespace Orkestra;

using Processings;

/// <summary>
/// A processing code to transform text.
/// </summary>
public abstract class Processing
{
    public abstract Text Process(Text text);

    public static Processing FromFunction(Func<Text, Text> func)
        => new FuncProcessing(func);
}