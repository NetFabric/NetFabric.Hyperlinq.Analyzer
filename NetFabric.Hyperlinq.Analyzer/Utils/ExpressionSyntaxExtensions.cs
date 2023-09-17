using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using System.Diagnostics.CodeAnalysis;

namespace NetFabric.Hyperlinq.Analyzer
{
    static class ExpressionSyntaxExtensions
    {
        public static bool IsIncrementAssignment(this ExpressionSyntax? expression, [NotNullWhen(true)] out string? identifier)
        {
            // check if it's something like `index++`
            if (expression is PostfixUnaryExpressionSyntax { OperatorToken.ValueText: "++" } postfixIncrementExpression)
            {
                identifier = postfixIncrementExpression.Operand.ToString();
                return true;
            }

            // check if it's something like `++index`
            if (expression is PrefixUnaryExpressionSyntax { OperatorToken.ValueText: "++" } prefixIncrementExpression)
            {
                identifier = prefixIncrementExpression.Operand.ToString();
                return true;
            }

            // check if it's something like `index += 1`
            if (expression is AssignmentExpressionSyntax assignmentExpression &&
                assignmentExpression.Kind() == SyntaxKind.AddAssignmentExpression &&
                assignmentExpression.Left is IdentifierNameSyntax identifierName &&
                assignmentExpression.Right.IsOne())
            {
                identifier = identifierName.Identifier.ValueText;
                return true;
            }

            // check if it's something like `index = index + 1` or `index = 1 + index`
            if (expression is AssignmentExpressionSyntax { Left: IdentifierNameSyntax assignmentIdentifier, Right: BinaryExpressionSyntax binaryExpression } &&
                binaryExpression.Kind() == SyntaxKind.AddExpression &&
                IsIdentifierAndOneOrViceVersa(binaryExpression.Left, binaryExpression.Right, out var binaryExpressionIdentifier)
            )
            {
                identifier = assignmentIdentifier.Identifier.ValueText;
                return binaryExpressionIdentifier == identifier;
            }

            identifier = default;
            return false;

            static bool IsIdentifierAndOneOrViceVersa(ExpressionSyntax first, ExpressionSyntax second, [NotNullWhen(true)] out string? identifier)
                => IsIdentifierAndOne(first, second, out identifier) || IsIdentifierAndOne(second, first, out identifier);

            static bool IsIdentifierAndOne(ExpressionSyntax first, ExpressionSyntax second, [NotNullWhen(true)] out string? identifier)
            {
                if(first is IdentifierNameSyntax identifierName && second.IsOne())
                {
                    identifier = identifierName.Identifier.ValueText;
                    return true;
                }

                identifier = default;
                return false;
            }
        }

        public static bool IsOne(this ExpressionSyntax? expression)
            => expression is LiteralExpressionSyntax literalExpression &&
                literalExpression.Kind() == SyntaxKind.NumericLiteralExpression &&
                literalExpression.Token.Value is int value &&
                value == 1;
    }
}
