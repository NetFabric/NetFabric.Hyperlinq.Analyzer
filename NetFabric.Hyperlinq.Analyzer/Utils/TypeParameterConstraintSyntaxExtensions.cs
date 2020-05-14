using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NetFabric.CodeAnalysis;
using System;

namespace NetFabric.Hyperlinq.Analyzer
{
    static class TypeParameterConstraintSyntaxExtensions
    {
        public static bool IsEnumerator(this TypeParameterConstraintSyntax typeParameterConstraintSyntax, SyntaxNodeAnalysisContext context)
        {
            if (typeParameterConstraintSyntax is TypeConstraintSyntax typeConstraintSyntax)
            {
                var typeSymbol = context.SemanticModel.GetTypeInfo(typeConstraintSyntax.Type).Type;
                if (typeSymbol is object
                    && (typeSymbol.IsEnumerator(context.Compilation, out var _)
                    || typeSymbol.IsAsyncEnumerator(context.Compilation, out var _)))
                    return true;
            }
            return false;
        }
    }
}
