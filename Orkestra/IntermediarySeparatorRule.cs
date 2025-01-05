/* Author:  Leonardo Trevisan Silio
 * Date:    05/01/2025
 */
namespace Orkestra;

/// <summary>
/// Intermediary object to improve expressiveness.
/// Do not use out of the "rule1 / rule2" context.
/// </summary>
public class IntermediarySeparatorRule(Rule main, params Key[] separators)
{
    readonly Rule main = main;
    readonly Key[] separators = separators;

    public static IntermediarySeparatorRule operator /(IntermediarySeparatorRule fst, Key scn)
        => new (fst.main, [ ..fst.separators, scn ]);

    public static Rule operator +(IntermediarySeparatorRule intermediary)
    {
        var main = intermediary.main;
        var rule = new Rule();
        rule.AddSubRules([ main ]);
        foreach (var separator in intermediary.separators)
            rule.AddSubRules([ main, separator, rule ]);
        return rule;
    }

}