using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using NetFabric.CodeAnalysis;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NetFabric.Hyperlinq.Analyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(RefEnumerationVariableCodeFixProvider)), Shared]
    public sealed class RefEnumerationVariableCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(RefEnumerationVariableAnalyzer.DiagnosticId);

        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
        public sealed override FixAllProvider GetFixAllProvider()
            => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

            foreach (var diagnostic in context.Diagnostics)
            {
                var diagnosticSpan = diagnostic.Location.SourceSpan;

                var forEachStatementSyntax = root
                    .FindToken(diagnosticSpan.Start)
                    .Parent.AncestorsAndSelf()
                    .OfType<ForEachStatementSyntax>().First();

                var semanticModel = await context.Document.GetSemanticModelAsync(context.CancellationToken).ConfigureAwait(false);

                var expressionType = semanticModel.GetTypeInfo(forEachStatementSyntax.Expression).Type;
                if (!expressionType.IsEnumerable(semanticModel.Compilation, out var enumerableSymbols))
                    return;

                var enumeratorSymbols = enumerableSymbols.EnumeratorSymbols;

                var title = "Add 'ref'";
                var isReadOnly = false;

                if (enumeratorSymbols.Current.ReturnsByRefReadonly)
                {
                    title = "Add 'ref readonly'";
                    isReadOnly = true;
                }

                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: title,
                        createChangedDocument: cancellationToken => AddRef(context.Document, forEachStatementSyntax, isReadOnly, cancellationToken),
                        equivalenceKey: title),
                    diagnostic);
            }
        }

        async Task<Document> AddRef(Document document, ForEachStatementSyntax forEachStatementSyntax, bool isReadOnly, CancellationToken cancellationToken)
        {
            var newForEachStatementSyntax = forEachStatementSyntax.ToRef(isReadOnly);

            var syntaxTree = await document.GetSyntaxTreeAsync(cancellationToken).ConfigureAwait(false);
            var root = syntaxTree.GetRoot();
            root = root.ReplaceNode(forEachStatementSyntax, newForEachStatementSyntax);

            return document.WithSyntaxRoot(root);
        }
    }

    // Source: https://github.com/outerminds/Entia/blob/master/Entia.Analyze/Extensions/SyntaxExtensions.cs
    static class SyntaxExtensions
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
