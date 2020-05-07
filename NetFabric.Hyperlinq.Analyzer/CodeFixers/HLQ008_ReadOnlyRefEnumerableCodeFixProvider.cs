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
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ReadOnlyRefEnumerableCodeFixProvider)), Shared]
    public sealed class ReadOnlyRefEnumerableCodeFixProvider : CodeFixProvider
    {
        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(ReadOnlyRefEnumerableAnalyzer.DiagnosticId);

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

                var structDeclarationSyntax = root
                    .FindToken(diagnosticSpan.Start)
                    .Parent?.AncestorsAndSelf()
                    .OfType<StructDeclarationSyntax>().FirstOrDefault();
                if (structDeclarationSyntax is null)
                    return;

                var title = "Add 'readonly'";
                context.RegisterCodeFix(
                    CodeAction.Create(
                        title: title,
                        createChangedDocument: cancellationToken => ToRefAsync(context.Document, structDeclarationSyntax, cancellationToken),
                        equivalenceKey: title),
                    diagnostic);
            }
        }

        async Task<Document> ToRefAsync(Document document, StructDeclarationSyntax structDeclarationSyntax, CancellationToken cancellationToken)
        {
            var newStructDeclarationSyntax = structDeclarationSyntax.WithModifiers(structDeclarationSyntax.Modifiers.Insert(0, SyntaxKind.ReadOnlyKeyword.ToToken()));

            var root = await document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
            if (root is null)
                throw new NullReferenceException();

            root = root.ReplaceNode(structDeclarationSyntax, newStructDeclarationSyntax);

            return document.WithSyntaxRoot(root);
        }
    }
}
