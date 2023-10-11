using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NetFabric.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NetFabric.Hyperlinq.Analyzer
{
    static class FieldDeclarationSyntaxExtensions
    {
        public static bool IsPublic(this FieldDeclarationSyntax fieldDeclarationSyntax)
        {
            if (!fieldDeclarationSyntax.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PublicKeyword)))
                return false;

            for (var node = fieldDeclarationSyntax.Parent; node is not null; node = node.Parent)
            {
                if (node is TypeDeclarationSyntax type)
                {
                    if (!type.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PublicKeyword)))
                        return false;
                }
            }
            return true;
        }

        public static bool IsEnumerableValueType(this FieldDeclarationSyntax fieldDeclarationSyntax, SyntaxNodeAnalysisContext context, [NotNullWhen(true)] out ITypeSymbol? typeSymbol)
        {
            typeSymbol = context.SemanticModel.GetTypeInfo(fieldDeclarationSyntax.Declaration.Type).Type;
            if (typeSymbol is null)
                return false;

            if (typeSymbol.TypeKind == TypeKind.TypeParameter)
            {
                var typeDeclaration = fieldDeclarationSyntax.Ancestors().OfType<TypeDeclarationSyntax>().FirstOrDefault();
                if (typeDeclaration is null)
                    return false;

                var typeName = typeSymbol.Name;
                var constraintClauses = typeDeclaration.ConstraintClauses.FirstOrDefault(node => node.Name.Identifier.ValueText == typeName);
                if (constraintClauses is null || !constraintClauses.Constraints.Any(constraint => constraint.IsEnumerator(context)))
                    return false;
            }
            else
            {
                if (!typeSymbol.IsValueType
                    || !(typeSymbol.IsEnumerator()
                        || typeSymbol.IsAsyncEnumerator(context.Compilation)))
                    return false;
            }

            return true;
        }
    }
}
