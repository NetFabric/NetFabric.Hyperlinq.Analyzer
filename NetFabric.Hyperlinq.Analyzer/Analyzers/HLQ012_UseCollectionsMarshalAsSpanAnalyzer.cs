using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NetFabric.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace NetFabric.Hyperlinq.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class UseCollectionsMarshalAsSpanAnalyzer : DiagnosticAnalyzer
    {
        const string DiagnosticId = DiagnosticIds.UseCollectionsMarshalAsSpanId;

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
            if (!(context.Node is ForEachStatementSyntax forEachStatementSyntax))
                return;

            var semanticModel = context.SemanticModel;

            var expressionType = semanticModel.GetTypeInfo(forEachStatementSyntax.Expression).Type;
            if (expressionType is null)
                return;

            // check if it's not a List<T>
            if (expressionType.ContainingNamespace?.ToString() != "System.Collections.Generic" || expressionType.MetadataName != "List`1")
                return;

            var diagnostic = Diagnostic.Create(Rule, forEachStatementSyntax.Expression.GetLocation());
            context.ReportDiagnostic(diagnostic);
        }
    }
}