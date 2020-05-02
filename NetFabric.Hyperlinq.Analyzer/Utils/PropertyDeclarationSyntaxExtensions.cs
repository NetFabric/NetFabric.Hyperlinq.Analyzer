using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace NetFabric.Hyperlinq.Analyzer.Utils
{
    static class PropertyDeclarationSyntaxExtensions
    {
        public static bool IsPublic(this PropertyDeclarationSyntax propertyDeclarationSyntax)
        {
            for (SyntaxNode? node = propertyDeclarationSyntax; node is object; node = node.Parent)
            {
                if (node is TypeDeclarationSyntax type)
                {
                    if (!type.Modifiers.Any(modifier => modifier.Text == "public"))
                        return false;
                }
                else if (node is MethodDeclarationSyntax method)
                {
                    if (!method.Modifiers.Any(modifier => modifier.Text == "public"))
                        return false;
                }
            }
            return true;
        }
    }
}
