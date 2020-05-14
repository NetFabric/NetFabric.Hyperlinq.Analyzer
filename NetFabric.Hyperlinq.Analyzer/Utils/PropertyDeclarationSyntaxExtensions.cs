using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NetFabric.Hyperlinq.Analyzer.Utils
{
    static class PropertyDeclarationSyntaxExtensions
    {
        public static bool IsPublic(this PropertyDeclarationSyntax propertyDeclarationSyntax)
        {
            if (!propertyDeclarationSyntax.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PublicKeyword)))
                return false;

            for (var node = propertyDeclarationSyntax.Parent; node is object; node = node.Parent)
            {
                if (node is TypeDeclarationSyntax type)
                {
                    if (!type.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PublicKeyword)))
                        return false;
                }
            }
            return true;
        }
    }
}
