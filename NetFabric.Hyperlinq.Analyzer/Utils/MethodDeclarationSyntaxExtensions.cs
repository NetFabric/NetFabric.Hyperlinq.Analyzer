using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NetFabric.CodeAnalysis;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NetFabric.Hyperlinq.Analyzer
{
    static class MethodDeclarationSyntaxExtensions
    {
        public static bool ReturnsVoid(this MethodDeclarationSyntax methodDeclarationSyntax)
            => methodDeclarationSyntax.ReturnType is PredefinedTypeSyntax predefinedTypeSyntax
               && predefinedTypeSyntax.Keyword.IsKind(SyntaxKind.VoidKeyword);

        public static bool IsExtensionMethod(this MethodDeclarationSyntax methodDeclarationSyntax, [NotNullWhen(true)] out ParameterSyntax? parameterSyntax)
        {
            var parameters = methodDeclarationSyntax.ParameterList.Parameters;
            if (parameters.Count != 0)
            {
                parameterSyntax = parameters[0];
                return parameterSyntax.Modifiers.Any(modifier => modifier.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.ThisKeyword));
            }

            parameterSyntax = null;
            return false;
        }

        public static bool IsEnumerableInstanceMethod(this MethodDeclarationSyntax methodDeclarationSyntax, SyntaxNodeAnalysisContext context)
        {
            var typeDeclaration = methodDeclarationSyntax.Ancestors().OfType<TypeDeclarationSyntax>().FirstOrDefault();
            return typeDeclaration is object && typeDeclaration.IsEnumerable(context);
        }

        public static bool IsEnumerableExtensionMethod(this MethodDeclarationSyntax methodDeclarationSyntax, SyntaxNodeAnalysisContext context)
        {
            if (methodDeclarationSyntax.IsExtensionMethod(out var parameterSyntax))
            {
                if (parameterSyntax.Type is object)
                {
                    var typeSymbol = context.SemanticModel.GetTypeInfo(parameterSyntax.Type).Type;
                    if (typeSymbol is object)
                    {
                        return typeSymbol.IsEnumerable(context.Compilation, out var _);
                    }
                }
            }

            return false;
        }

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

        public static bool HasAttribute(this MethodDeclarationSyntax methodDeclarationSyntax, string name)
            => methodDeclarationSyntax.AttributeLists
                .Select(attributeList => attributeList.Target)
                .Any(target => 
                    target is object &&
                    (target.Identifier.ValueText == name || target.Identifier.ValueText == $"{name}Attribute"));
    }
}
