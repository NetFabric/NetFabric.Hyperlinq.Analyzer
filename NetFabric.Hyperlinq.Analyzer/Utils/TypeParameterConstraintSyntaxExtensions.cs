using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NetFabric.Hyperlinq.Analyzer
{
    static class TypeParameterConstraintSyntaxExtensions
    {
        public static bool IsEnumerator(this TypeParameterConstraintSyntax typeParameterConstraintSyntax, SyntaxNodeAnalysisContext context)
        {
            if (typeParameterConstraintSyntax is TypeConstraintSyntax typeConstraintSyntax)
            {
                var typeSymbol = context.SemanticModel.GetTypeInfo(typeConstraintSyntax.Type).Type;
                if (typeSymbol is not null
                    && (typeSymbol.IsEnumerator()
                    || typeSymbol.IsAsyncEnumerator(context.Compilation)))
                    return true;
            }
            return false;
        }
    }
}
