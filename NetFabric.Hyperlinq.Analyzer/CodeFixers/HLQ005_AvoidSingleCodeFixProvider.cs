using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NetFabric.Hyperlinq.Analyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AvoidSingleCodeFixProvider)), Shared]
    public sealed class AvoidSingleCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(AvoidSingleAnalyzer.DiagnosticId);

        // See https://github.com/dotnet/roslyn/blob/master/docs/analyzers/FixAllProvider.md for more information on Fix All Providers
        public sealed override FixAllProvider GetFixAllProvider()
            => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            if (root is null)
                return;

            foreach (var diagnostic in context.Diagnostics)
            {
                var diagnosticSpan = diagnostic.Location.SourceSpan;

                var memberAccessExpressionSyntax = root
                    .FindToken(diagnosticSpan.Start)
                    .Parent?.Ancestors()
                    .OfType<MemberAccessExpressionSyntax>().FirstOrDefault();
                if (memberAccessExpressionSyntax is null)
                    return;

                var replacement = memberAccessExpressionSyntax.Name.Identifier.Text switch
                {
                    "Single" => "First",
                    "SingleOrDefault" => "FirstOrDefault",
                    "SingleAsync" => "FirstAsync",
                    "SingleOrDefaultAsync" => "FirstOrDefaultAsync",
                    _ => null
                };
                if (replacement is null)
                    return;

                var title = $"Use '{replacement}' instead";
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: title,
                        createChangedDocument: cancellationToken => ReplaceAsync(context.Document, memberAccessExpressionSyntax, replacement, cancellationToken),
                        equivalenceKey: title),
                    diagnostic);
            }
        }

        async Task<Document> ReplaceAsync(Document document, MemberAccessExpressionSyntax memberAccessExpressionSyntax, string replacement, CancellationToken cancellationToken)
        {
            var newMemberAccessExpressionSyntax = memberAccessExpressionSyntax.WithName(SyntaxFactory.IdentifierName(replacement));

            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root is null)
                throw new NullReferenceException();

            root = root.ReplaceNode(memberAccessExpressionSyntax, newMemberAccessExpressionSyntax);

            return document.WithSyntaxRoot(root);
        }
    }
}
