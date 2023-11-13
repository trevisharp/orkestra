/* Author:  Leonardo Trevisan Silio
 * Date:    13/11/2023
 */
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Orkestra.SyntacticAnalysis.InternalStructure;

internal ref struct LRItemMap
{
    private Span<int> list;
}