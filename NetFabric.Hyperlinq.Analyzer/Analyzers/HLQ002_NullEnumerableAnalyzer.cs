using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NetFabric.CodeAnalysis;
using System.Collections.Immutable;
using System.Linq;

namespace NetFabric.Hyperlinq.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class NullEnumerableAnalyzer : DiagnosticAnalyzer
    {
        const string DiagnosticId = DiagnosticIds.NullReturnId;

        static readonly LocalizableString Title =
            new LocalizableResourceString(nameof(Resources.NullEnumerable_Title), Resources.ResourceManager, typeof(Resources));
        static readonly LocalizableString MessageFormat =
            new LocalizableResourceString(nameof(Resources.NullEnumerable_MessageFormat), Resources.ResourceManager, typeof(Resources));
        static readonly LocalizableString Description =
            new LocalizableResourceString(nameof(Resources.NullEnumerable_Description), Resources.ResourceManager, typeof(Resources));
        const string Category = "Compiler";

        static readonly DiagnosticDescriptor rule =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error,
                isEnabledByDefault: true, description: Description,
                helpLinkUri: "https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/tree/master/docs/reference/HLQ002_NullEnumerable.md");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
        }

        static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is MethodDeclarationSyntax methodDeclarationSyntax))
                return;

            if (methodDeclarationSyntax.ReturnType.IsKind(SyntaxKind.VoidKeyword))
                return;

            if (methodDeclarationSyntax.Body is null)
                AnalyzeArrowExpressionClause(context, methodDeclarationSyntax);
            else
                AnalyzeMethodDeclaration(context, methodDeclarationSyntax);
        }

        static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax methodDeclarationSyntax)
        {
            foreach (var returnStatementSyntax in methodDeclarationSyntax.DescendantNodes().OfType<ReturnStatementSyntax>())
            {
                if (returnStatementSyntax.Expression.IsKind(SyntaxKind.NullLiteralExpression))
                {
                    var diagnostic = Diagnostic.Create(rule, returnStatementSyntax.GetLocation());
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }

        static void AnalyzeArrowExpressionClause(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax methodDeclarationSyntax)
        {
            var arrowExpressionClauseSyntax = methodDeclarationSyntax.DescendantNodes()
                .OfType<ArrowExpressionClauseSyntax>()
                .FirstOrDefault();
            if (arrowExpressionClauseSyntax is null)
                return;

            if (arrowExpressionClauseSyntax.Expression.IsKind(SyntaxKind.NullLiteralExpression))
            {
                var diagnostic = Diagnostic.Create(rule, arrowExpressionClauseSyntax.Expression.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}