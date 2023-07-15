using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace NetFabric.Hyperlinq.Analyzer
{
    [ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(UseCollectionsMarshalAsSpanAnalyzer)), Shared]
    public sealed class UseCollectionsMarshalAsSpanCodeFixProvider : CodeFixProvider
    {
        const string Title = "Replace with CollectionsMarshal.AsSpan()";

        public sealed override ImmutableArray<string> FixableDiagnosticIds
            => ImmutableArray.Create(UseCollectionsMarshalAsSpanAnalyzer.DiagnosticId);

        public sealed override FixAllProvider GetFixAllProvider()
            => WellKnownFixAllProviders.BatchFixer;

        public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
        {
            var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);
            var diagnostic = context.Diagnostics.First();
            var diagnosticSpan = diagnostic.Location.SourceSpan;

            var foreachStatement = root?.
                FindToken(diagnosticSpan.Start).
                Parent?.
                AncestorsAndSelf().
                OfType<ForEachStatementSyntax>().
                FirstOrDefault();
            if (foreachStatement is null)
                return;

            var codeAction = CodeAction.Create(
                title: Title,
                createChangedDocument: cancellationToken => ReplaceWithAsSpanAsync(context.Document, foreachStatement, cancellationToken),
                equivalenceKey: Title);

            context.RegisterCodeFix(codeAction, diagnostic);
        }

        static async Task<Document> ReplaceWithAsSpanAsync(Document document, ForEachStatementSyntax foreachStatement, CancellationToken cancellationToken)
        {
            var compilationUnit = (await foreachStatement.SyntaxTree.GetRootAsync(cancellationToken).ConfigureAwait(false)) as CompilationUnitSyntax;

            // Replace the foreach expression with CollectionMarshall.AsSpan(expression)
            var asSpanInvocation = SyntaxFactory.InvocationExpression(
                SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                    SyntaxFactory.IdentifierName("CollectionsMarshal"),
                    SyntaxFactory.IdentifierName("AsSpan")))
                .WithArgumentList(
                    SyntaxFactory.ArgumentList(
                        SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(foreachStatement.Expression))));

            var newForeachStatement = foreachStatement
                .WithExpression(asSpanInvocation)
                .WithAdditionalAnnotations(Formatter.Annotation);
            compilationUnit = compilationUnit!.ReplaceNode(foreachStatement, newForeachStatement);

            // Add 'System.Runtime.InteropServices' using directive if not present
            var interopServices = "System.Runtime.InteropServices";
            var usingDirective = compilationUnit?.Usings
                .FirstOrDefault(usingDirectiveSyntax => usingDirectiveSyntax.Name.ToString() == interopServices);
            if (usingDirective is null)
            {
                var newUsingDirective = SyntaxFactory
                    .UsingDirective(SyntaxFactory.ParseName(interopServices))
                    .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);
                compilationUnit = compilationUnit!.AddUsings(newUsingDirective);
            }

            return document.WithSyntaxRoot(compilationUnit!);
        }
    }
}