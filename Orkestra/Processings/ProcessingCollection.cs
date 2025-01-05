/* Author:  Leonardo Trevisan Silio
 * Date:    03/07/2023
 */
using System.Collections.Generic;

namespace Orkestra.Processings;

/// <summary>
/// A package of processing functions.
/// </summary>
public class ProcessingCollection
{
    private List<Processing> processings = new List<Processing>();
    public IEnumerable<Processing> Elements => processings;

    public void Add(Processing processing)
        => this.processings.Add(processing);

    public Text ProcessAll(Text text)
    {
        foreach (var processing in this.processings)
            text = processing.Process(text);
        
        return text;
    }
}