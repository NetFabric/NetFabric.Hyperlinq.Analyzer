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

namespace NetFabric.Hyperlinq.Analyzer;

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
            First();
        if (foreachStatement is null)
            return;

        var collectionExpression = foreachStatement.Expression;
        if(collectionExpression is null)
            return;

        var codeAction = CodeAction.Create(
            title: Title,
            createChangedDocument: cancellationToken => ReplaceWithAsSpanAsync(context.Document, foreachStatement, collectionExpression, cancellationToken),
            equivalenceKey: Title);

        context.RegisterCodeFix(codeAction, diagnostic);
    }

    static async Task<Document> ReplaceWithAsSpanAsync(Document document, ForEachStatementSyntax foreachStatement, ExpressionSyntax collectionExpression, CancellationToken cancellationToken)
    {
        var compilationUnit = (await foreachStatement.SyntaxTree.GetRootAsync(cancellationToken).ConfigureAwait(false)) as CompilationUnitSyntax;

        // Add 'System.Runtime.InteropServices' using directive if not present
        var usingDirective = compilationUnit?.Usings
            .FirstOrDefault(usingDirectiveSyntax => usingDirectiveSyntax.Name.ToString() == "System.Runtime.InteropServices");
        if (usingDirective is null)
        {
            var newUsingDirective = SyntaxFactory
                .UsingDirective(SyntaxFactory.ParseName("System.Runtime.InteropServices"))
                .WithTrailingTrivia(SyntaxFactory.CarriageReturnLineFeed);
            compilationUnit = compilationUnit!.AddUsings(newUsingDirective);
        }

        // Replace the foreach expression with CollectionsMarshal.AsSpan()
        var asSpanInvocation = SyntaxFactory.InvocationExpression(
            SyntaxFactory.MemberAccessExpression(SyntaxKind.SimpleMemberAccessExpression,
                SyntaxFactory.IdentifierName("CollectionsMarshal"),
                SyntaxFactory.IdentifierName("AsSpan")))
            .WithArgumentList(SyntaxFactory.ArgumentList(SyntaxFactory.SingletonSeparatedList(SyntaxFactory.Argument(collectionExpression))));

        var newForeachStatement = foreachStatement
            .WithExpression(asSpanInvocation)
            .WithAdditionalAnnotations(Formatter.Annotation);

        var newRoot = compilationUnit?.ReplaceNode(foreachStatement, newForeachStatement);
        return newRoot is null 
            ? document
            : document.WithSyntaxRoot(newRoot);
    }
}
