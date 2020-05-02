using Microsoft.CodeAnalysis;
using System;

namespace NetFabric.Hyperlinq.Analyzer.Utils
{
    static class SyntaxTokenListExtensions
    {
        // to avoid use of enumerators and heap allocations
        public static bool Any(this SyntaxTokenList list, Func<SyntaxToken, bool> predicate)
        {
            for (var index = 0; index < list.Count; index++)
            {
                if (predicate(list[index]))
                    return true;
            }
            return false;
        }
    }
}
