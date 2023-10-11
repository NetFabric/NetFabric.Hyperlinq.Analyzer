using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NetFabric.CodeAnalysis;
using System.Collections.Immutable;

namespace NetFabric.Hyperlinq.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class UseForLoopAnalyzer : DiagnosticAnalyzer
    {
        const string DiagnosticId = DiagnosticIds.UseForLoopId;

        static readonly LocalizableString Title =
            new LocalizableResourceString(nameof(Resources.UseForLoop_Title), Resources.ResourceManager, typeof(Resources));
        static readonly LocalizableString MessageFormat =
            new LocalizableResourceString(nameof(Resources.UseForLoop_MessageFormat), Resources.ResourceManager, typeof(Resources));
        static readonly LocalizableString Description =
            new LocalizableResourceString(nameof(Resources.UseForLoop_Description), Resources.ResourceManager, typeof(Resources));
        const string Category = "Performance";

        static readonly DiagnosticDescriptor Rule =
            new(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning,
                isEnabledByDefault: true, description: Description,
                helpLinkUri: "https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/tree/master/docs/reference/HLQ010_UseForLoop.md");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(AnalyzeForEachStatement, SyntaxKind.ForEachStatement);
        }

        static void AnalyzeForEachStatement(SyntaxNodeAnalysisContext context)
        {
            var foreachStatement = (ForEachStatementSyntax)context.Node;

            var expressionType = context.SemanticModel.GetTypeInfo(foreachStatement.Expression).Type;
            if (expressionType is null)
                return;

            // if foreach uses indexer for this type then let it be
            if (expressionType.IsForEachOptimized())
                return;

            // if it's List<T> then should be using foreach with CollectionsMarshal.AsSpan()
            if (expressionType.IsList(out _))
                return;

            if (expressionType.IsIndexable(out _))
            {
                var diagnostic = Diagnostic.Create(Rule, foreachStatement.Expression.GetLocation());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}