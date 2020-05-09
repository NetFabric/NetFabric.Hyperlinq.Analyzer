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

        public static bool ReturnsEnumerable(this MethodDeclarationSyntax methodDeclarationSyntax, SyntaxNodeAnalysisContext context)
        {
            var typeSymbol = context.SemanticModel.GetTypeInfo(methodDeclarationSyntax.ReturnType).Type;
            return typeSymbol is object 
                && (typeSymbol.IsEnumerable(context.Compilation, out var _)
                || typeSymbol.IsAsyncEnumerable(context.Compilation, out var _));
        }

        public static bool ReturnsEnumerableInterface(this MethodDeclarationSyntax methodDeclarationSyntax, SyntaxNodeAnalysisContext context)
        {
            var typeSymbol = context.SemanticModel.GetTypeInfo(methodDeclarationSyntax.ReturnType).Type;
            return typeSymbol is object
                && typeSymbol.TypeKind == TypeKind.Interface
                && (typeSymbol.IsEnumerable(context.Compilation, out var _)
                || typeSymbol.IsAsyncEnumerable(context.Compilation, out var _));
        }

        public static bool IsExtensionMethod(this MethodDeclarationSyntax methodDeclarationSyntax, [NotNullWhen(true)] out ParameterSyntax? parameterSyntax)
        {
            var parameters = methodDeclarationSyntax.ParameterList.Parameters;
            if (parameters.Count != 0)
            {
                parameterSyntax = parameters[0];
                return parameterSyntax.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.ThisKeyword));
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
                        return typeSymbol.IsEnumerable(context.Compilation, out var _) || typeSymbol.IsAsyncEnumerable(context.Compilation, out var _);
                    }
                }
            }

            return false;
        }

        public static bool IsEmptyDispose(this MethodDeclarationSyntax methodDeclarationSyntax)
        {
            if (methodDeclarationSyntax.IsDispose())
            {
                return !(methodDeclarationSyntax.Body?.Statements.Any() ?? true);
            }

            if (methodDeclarationSyntax.IsAsyncDispose())
            {
                // check if it simply returns a new instance of ValueTask
                if (methodDeclarationSyntax.Body is null)
                {
                    if (methodDeclarationSyntax.ExpressionBody is null)
                        return false;
                    var expression = methodDeclarationSyntax.ExpressionBody.Expression.ToString();
                    return expression == "default" || expression == "new ValueTask()";
                }
                else
                {
                    // check if any there's any resource being disposed
                    return !methodDeclarationSyntax.Body.DescendantNodes()
                        .OfType<MemberAccessExpressionSyntax>()
                        .Any(memberAccesses =>
                            memberAccesses.Name.Identifier.ValueText == "Dispose"
                            || memberAccesses.Name.Identifier.ValueText == "DisposeAsync");
                }
            }

            return false;
        }

        public static bool IsEmptyReset(this MethodDeclarationSyntax methodDeclarationSyntax)
        {
            if (!methodDeclarationSyntax.IsReset())
                return false;

            // check if body is empty or just throws an exception
            if (methodDeclarationSyntax.Body is null)
            {
                var expression = methodDeclarationSyntax.ExpressionBody?.Expression.ToString();
                if (expression is object && expression.StartsWith("throw"))
                    return true;
            }
            else
            {
                var statements = methodDeclarationSyntax.Body.Statements;
                if (statements.Count == 0
                    || (statements.Count == 1 && statements[0].ToString().StartsWith("throw")))
                    return true;
            }

            return false;
        }

        public static bool IsDispose(this MethodDeclarationSyntax methodDeclarationSyntax)
            => methodDeclarationSyntax.Identifier.ValueText == "Dispose"
                && methodDeclarationSyntax.ReturnsVoid()
                && methodDeclarationSyntax.ParameterList.Parameters.Count == 0;

        public static bool IsAsyncDispose(this MethodDeclarationSyntax methodDeclarationSyntax)
            => methodDeclarationSyntax.Identifier.ValueText == "DisposeAsync"
                && methodDeclarationSyntax.ReturnType is IdentifierNameSyntax identifierNameSyntax
                && identifierNameSyntax.Identifier.ValueText == "ValueTask"
                && methodDeclarationSyntax.ParameterList.Parameters.Count < 2; // TODO: check if it's CancellationToken

        public static bool IsReset(this MethodDeclarationSyntax methodDeclarationSyntax)
            => methodDeclarationSyntax.Identifier.ValueText == "Reset"
                && methodDeclarationSyntax.ReturnsVoid()
                && methodDeclarationSyntax.ParameterList.Parameters.Count == 0;

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
