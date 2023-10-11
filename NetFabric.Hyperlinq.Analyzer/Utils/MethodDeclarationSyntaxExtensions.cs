using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NetFabric.CodeAnalysis;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace NetFabric.Hyperlinq.Analyzer
{
    static class MethodDeclarationSyntaxExtensions
    {
        public static bool ReturnsVoid(this MethodDeclarationSyntax methodDeclarationSyntax)
            => methodDeclarationSyntax.ReturnType is PredefinedTypeSyntax predefinedTypeSyntax
               && predefinedTypeSyntax.Keyword.IsKind(SyntaxKind.VoidKeyword);

        public static bool Returns(this MethodDeclarationSyntax methodDeclarationSyntax, string valueText)
            => methodDeclarationSyntax.ReturnType is IdentifierNameSyntax identifierNameSyntax
                && identifierNameSyntax.Identifier.ValueText == valueText;

        public static bool ReturnsEnumerable(this MethodDeclarationSyntax methodDeclarationSyntax, SyntaxNodeAnalysisContext context)
        {
            var typeSymbol = context.SemanticModel.GetTypeInfo(methodDeclarationSyntax.ReturnType).Type;
            return !(typeSymbol is null)
                && (typeSymbol.IsEnumerable(context.Compilation, out _)
                || typeSymbol.IsAsyncEnumerable(context.Compilation, out _));
        }

        public static bool ReturnsEnumerableInterface(this MethodDeclarationSyntax methodDeclarationSyntax, SyntaxNodeAnalysisContext context)
        {
            var typeSymbol = context.SemanticModel.GetTypeInfo(methodDeclarationSyntax.ReturnType).Type;
            return !(typeSymbol is null)
                && typeSymbol.TypeKind == TypeKind.Interface
                && (typeSymbol.IsEnumerable(context.Compilation, out _)
                || typeSymbol.IsAsyncEnumerable(context.Compilation, out _));
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
            return typeDeclaration is not null && typeDeclaration.IsEnumerable(context);
        }

        public static bool IsEnumerableExtensionMethod(this MethodDeclarationSyntax methodDeclarationSyntax, SyntaxNodeAnalysisContext context)
        {
            if (methodDeclarationSyntax.IsExtensionMethod(out var parameterSyntax))
            {
                if (parameterSyntax.Type is not null)
                {
                    var typeSymbol = context.SemanticModel.GetTypeInfo(parameterSyntax.Type).Type;
                    if (typeSymbol is not null)
                    {
                        return typeSymbol.IsEnumerable(context.Compilation, out _) || typeSymbol.IsAsyncEnumerable(context.Compilation, out _);
                    }
                }
            }

            return false;
        }

        public static bool IsDispose(this MethodDeclarationSyntax methodDeclarationSyntax)
            => methodDeclarationSyntax.Identifier.ValueText == "Dispose"
                && methodDeclarationSyntax.ReturnsVoid()
                && methodDeclarationSyntax.ParameterList.Parameters.Count == 0;

        public static bool IsAsyncDispose(this MethodDeclarationSyntax methodDeclarationSyntax)
            => methodDeclarationSyntax.Identifier.ValueText == "DisposeAsync"
                && methodDeclarationSyntax.Returns("ValueTask")
                && methodDeclarationSyntax.ParameterList.Parameters.Count == 0; 

        public static bool IsReset(this MethodDeclarationSyntax methodDeclarationSyntax)
            => methodDeclarationSyntax.Identifier.ValueText == "Reset"
                && methodDeclarationSyntax.ReturnsVoid()
                && methodDeclarationSyntax.ParameterList.Parameters.Count == 0;

        public static bool IsEmptyMethod(this MethodDeclarationSyntax methodDeclarationSyntax)
        {
            if (methodDeclarationSyntax.Body is null)
            {
                if (methodDeclarationSyntax.ExpressionBody is null)
                    return false;

                var expression = methodDeclarationSyntax.ExpressionBody.Expression.ToString().Trim();
                if (expression.StartsWith("throw"))
                    return true;
            }
            else
            {
                var statements = methodDeclarationSyntax.Body.Statements;
                if (statements.Count == 0)
                    return true;

                if (statements.Count == 1)
                {
                    var statement = statements[0].ToString().Trim();

                    if (statement.StartsWith("throw"))
                        return true;
                }
            }

            return false;
        }

        public static bool IsEmptyAsyncMethod(this MethodDeclarationSyntax methodDeclarationSyntax)
        {
            if (methodDeclarationSyntax.Body is null)
            {
                if (methodDeclarationSyntax.ExpressionBody is null)
                    return false;

                var expression = methodDeclarationSyntax.ExpressionBody.Expression.ToString().Trim();
                if (expression.StartsWith("default")
                    || expression.StartsWith("new ValueTask")
                    || expression.StartsWith("throw"))
                    return true;
            }
            else
            {
                var statements = methodDeclarationSyntax.Body.Statements;
                if (statements.Count == 0)
                    return true;

                if (statements.Count == 1)
                {
                    var statement = statements[0].ToString().Trim();

                    if (statement.StartsWith("return default") 
                        || statement.StartsWith("return new ValueTask") 
                        || statement.StartsWith("throw"))
                        return true;
                }
            }

            return false;
        }

        public static bool IsPublic(this MethodDeclarationSyntax methodDeclarationSyntax)
        {
            for (SyntaxNode? node = methodDeclarationSyntax; node is not null; node = node.Parent)
            {
                if (node is TypeDeclarationSyntax type)
                {
                    if (!type.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PublicKeyword)))
                        return false;
                }
                else if (node is MethodDeclarationSyntax method)
                {
                    if (!method.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PublicKeyword)))
                        return false;
                }
            }
            return true;
        }

        public static bool AllReturnsImplement(this MethodDeclarationSyntax methodDeclarationSyntax, SpecialType type, SyntaxNodeAnalysisContext context)
        {
            foreach (var returnStatementSyntax in methodDeclarationSyntax.DescendantNodes().OfType<ReturnStatementSyntax>())
            {
                var expression = returnStatementSyntax.Expression;
                if (expression is null)
                    return false;

                var returnType = context.SemanticModel.GetTypeInfo(expression).Type;
                if (returnType is null || !returnType.ImplementsInterface(type, out _))
                    return false;
            }
            return true;
        }

        public static bool HasAttribute(this MethodDeclarationSyntax methodDeclarationSyntax, string name)
            => methodDeclarationSyntax.AttributeLists
                .Select(attributeList => attributeList.Target)
                .Any(target => 
                    target is not null &&
                    (target.Identifier.ValueText == name || target.Identifier.ValueText == $"{name}Attribute"));
    }
}
