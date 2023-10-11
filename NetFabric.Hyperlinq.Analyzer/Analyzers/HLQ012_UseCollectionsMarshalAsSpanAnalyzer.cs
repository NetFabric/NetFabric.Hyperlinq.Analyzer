using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace NetFabric.Hyperlinq.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class UseCollectionsMarshalAsSpanAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = DiagnosticIds.UseCollectionsMarshalAsSpanId;

        static readonly LocalizableString Title =
            new LocalizableResourceString(nameof(Resources.UseCollectionsMarshalAsSpan_Title), Resources.ResourceManager, typeof(Resources));
        static readonly LocalizableString MessageFormat =
            new LocalizableResourceString(nameof(Resources.UseCollectionsMarshalAsSpan_MessageFormat), Resources.ResourceManager, typeof(Resources));
        static readonly LocalizableString Description =
            new LocalizableResourceString(nameof(Resources.UseCollectionsMarshalAsSpan_Description), Resources.ResourceManager, typeof(Resources));
        const string Category = "Performance";

        static readonly DiagnosticDescriptor Rule =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning,
                isEnabledByDefault: true, description: Description,
                helpLinkUri: "https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/tree/master/docs/reference/HLQ012_UseCollectionsMarshalAsSpanAnalyzer.md");

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
            var collectionType = context.SemanticModel.GetTypeInfo(foreachStatement.Expression).Type;

            if (collectionType.IsList(out var itemType))
            {
                var diagnostic = Diagnostic.Create(Rule, foreachStatement.Expression.GetLocation(),
                    itemType.ToDisplayString());
                context.ReportDiagnostic(diagnostic);
            }
        }
    }
}