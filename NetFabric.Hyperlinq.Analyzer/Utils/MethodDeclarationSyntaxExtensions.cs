using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NetFabric.CodeAnalysis;
using System.Linq;

namespace NetFabric.Hyperlinq.Analyzer
{
    static class MethodDeclarationSyntaxExtensions
    {
        public static bool IsPublic(this MethodDeclarationSyntax methodDeclarationSyntax)
        {
            for (SyntaxNode? node = methodDeclarationSyntax; node is object; node = node.Parent)
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

        public static bool AllReturnsImplement(this MethodDeclarationSyntax methodDeclarationSyntax, SpecialType type, SemanticModel semanticModel)
        {
            foreach (var returnStatementSyntax in methodDeclarationSyntax.DescendantNodes().OfType<ReturnStatementSyntax>())
            {
                var expression = returnStatementSyntax.Expression;
                if (expression is null)
                    return false;

                var returnType = semanticModel.GetTypeInfo(expression).Type;
                if (returnType is null || !returnType.ImplementsInterface(type, out _))
                    return false;
            }
            return true;
        }
    }
}
