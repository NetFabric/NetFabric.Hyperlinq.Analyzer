using Microsoft.CodeAnalysis;
using System;

namespace NetFabric.Hyperlinq.Analyzer
{
    static class SyntaxListExtensions
    {
        // to avoid use of enumerators and heap allocations
        public static bool Any<TNode>(this SyntaxList<TNode> list, Func<TNode, bool> predicate) 
            where TNode : SyntaxNode
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
