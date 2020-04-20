using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;

namespace NetFabric.Hyperlinq.Analyzer
{
    // Source: https://github.com/outerminds/Entia/blob/master/Entia.Analyze/Extensions/SyntaxExtensions.cs
    static partial class SyntaxExtensions
    {
        public static bool IsSome(this SyntaxToken token) => !token.IsNone();
        public static bool IsNone(this SyntaxToken token) => token == default;
        public static SyntaxToken ToToken(this SyntaxKind kind) => SyntaxFactory.Token(kind);

        public static ForEachStatementSyntax ToRef(this ForEachStatementSyntax statement, bool @readonly = false)
            => statement.WithType(statement.Type.ToRef(@readonly));

        public static RefTypeSyntax ToRef(this TypeSyntax type, bool @readonly = false)
        {
            var token = @readonly ? SyntaxKind.ReadOnlyKeyword.ToToken() : default;
            if (type is RefTypeSyntax @ref)
            {
                if (@ref.ReadOnlyKeyword.IsSome() == @readonly) return @ref;

                return @ref.WithReadOnlyKeyword(token);
            }

            return SyntaxFactory.RefType(SyntaxKind.RefKeyword.ToToken(), token, type).WithTriviaFrom(type);
        }
    }
}
