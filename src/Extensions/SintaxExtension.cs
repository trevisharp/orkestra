/* Author:  Leonardo Trevisan Silio
 * Date:    28/06/2023
 */
using System;
using System.Linq;
using System.Text;
using System.Data;
using System.Collections.Generic;

namespace Orkestra.Extensions;

public static class SintaxExtension
{
    /// <summary>
    /// Returns the dictionary of all Subrules of a language that start with a keyword.
    /// </summary>
    public static IEnumerable<IGrouping<string, SubRule>> GetHeaders(this LanguageInfo language)
    {
        if (language is null)
            return [];
        
        var rules = language.Rules;
        var keyGroups = 
            from subRule in rules.SelectMany(r => r)
            where hasKeywordHeader(subRule)
            group subRule by subRule.FirstOrDefault().Name;
        
        return keyGroups;
    }

    static bool hasKeywordHeader(SubRule rule)
    {
        if (rule?.FirstOrDefault() is not Key key)
            return false;

        if (!key.IsKeyword)
            return false;
        
        return true;
    }

    
}