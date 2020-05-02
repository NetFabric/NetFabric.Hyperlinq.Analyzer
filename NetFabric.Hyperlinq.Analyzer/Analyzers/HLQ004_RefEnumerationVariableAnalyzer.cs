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
    public sealed class RefEnumerationVariableAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = DiagnosticIds.RefEnumerationVariableId;

        static readonly LocalizableString Title =
            new LocalizableResourceString(nameof(Resources.RefEnumerationVariable_Title), Resources.ResourceManager, typeof(Resources));
        static readonly LocalizableString MessageFormat =
            new LocalizableResourceString(nameof(Resources.RefEnumerationVariable_MessageFormat), Resources.ResourceManager, typeof(Resources));
        static readonly LocalizableString Description =
            new LocalizableResourceString(nameof(Resources.RefEnumerationVariable_Description), Resources.ResourceManager, typeof(Resources));
        const string Category = "Performance";

        static readonly DiagnosticDescriptor rule =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning,
                isEnabledByDefault: true, description: Description,
                helpLinkUri: "https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/tree/master/docs/reference/HLQ004_RefEnumerationVariable.md");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(rule);

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
            if (expressionType is null || !expressionType.IsEnumerable(context.Compilation, out var enumerableSymbols))
                return;

            var enumeratorSymbols = enumerableSymbols.EnumeratorSymbols;

            if (forEachStatementSyntax.Type is RefTypeSyntax refTypeSyntax)
            {
                if (refTypeSyntax.ReadOnlyKeyword.Value is null && enumeratorSymbols.Current.ReturnsByRefReadonly)
                {
                    var diagnostic = Diagnostic.Create(rule, forEachStatementSyntax.Type.GetLocation(), "ref readonly");
                    context.ReportDiagnostic(diagnostic);
                }
            }
            else
            {
                if(enumeratorSymbols.Current.ReturnsByRef)
                {
                    var diagnostic = Diagnostic.Create(rule, forEachStatementSyntax.Type.GetLocation(), "ref");
                    context.ReportDiagnostic(diagnostic);
                }
                else if (enumeratorSymbols.Current.ReturnsByRefReadonly)
                {
                    var diagnostic = Diagnostic.Create(rule, forEachStatementSyntax.Type.GetLocation(), "ref readonly");
                    context.ReportDiagnostic(diagnostic);
                }
            }
        }
    }
}