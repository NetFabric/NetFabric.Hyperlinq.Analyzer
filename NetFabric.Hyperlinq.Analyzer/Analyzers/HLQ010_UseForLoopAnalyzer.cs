using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;

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
            if (context.Node is not ForEachStatementSyntax forEachStatementSyntax)
                return;

            var semanticModel = context.SemanticModel;

            var expressionType = semanticModel.GetTypeInfo(forEachStatementSyntax.Expression).Type;
            if (expressionType is null)
                return;

            // check if it has an indexer
            if (!expressionType.GetMembers().OfType<IPropertySymbol>()
                .Any(property => property.IsIndexer 
                && property.Parameters.Length == 1 
                && property.Parameters[0].Type.SpecialType == SpecialType.System_Int32))
                return;

            // check if it has a Count or a Length property
            if (!expressionType.GetMembers().OfType<IPropertySymbol>()
                .Any(property => property.IsReadOnly
                && (property.Name == "Length" || property.Name == "Count")))
                return;

            var diagnostic = Diagnostic.Create(Rule, forEachStatementSyntax.Expression.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}