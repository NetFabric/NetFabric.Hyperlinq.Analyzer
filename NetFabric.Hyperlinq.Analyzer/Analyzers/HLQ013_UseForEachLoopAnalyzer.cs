using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;

namespace NetFabric.Hyperlinq.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class UseForEachLoopAnalyzer : DiagnosticAnalyzer
    {
        const string DiagnosticId = DiagnosticIds.UseForEachLoopId;

        static readonly LocalizableString Title =
            new LocalizableResourceString(nameof(Resources.UseForEachLoop_Title), Resources.ResourceManager, typeof(Resources));
        static readonly LocalizableString MessageFormat =
            new LocalizableResourceString(nameof(Resources.UseForEachLoop_MessageFormat), Resources.ResourceManager, typeof(Resources));
        static readonly LocalizableString Description =
            new LocalizableResourceString(nameof(Resources.UseForEachLoop_Description), Resources.ResourceManager, typeof(Resources));
        const string Category = "Performance";

        static readonly DiagnosticDescriptor Rule =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning,
                isEnabledByDefault: true, description: Description,
                helpLinkUri: "https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/tree/master/docs/reference/HLQ010_UseForEachLoop.md");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(AnalyzeForLoop, SyntaxKind.ForStatement);
        }

        private static void AnalyzeForLoop(SyntaxNodeAnalysisContext context)
        {
            var forStatement = (ForStatementSyntax)context.Node;

            var statement = forStatement.Statement;
            if (statement is null)
                return;

            if (!forStatement.IsIncrementalStep(context, out var forIdentifier))
                return;

            // check if the variable is used in the statement only for accessing indexer of one single array or span
            var indexedCollection = default(string);
            var indexedCollectionType = default(ITypeSymbol);
            foreach (var identifierNameSyntax in statement.DescendantNodes().OfType<IdentifierNameSyntax>())
            {
                // check if the identifier is using the variable
                if (identifierNameSyntax.Identifier.ToString() != forIdentifier)
                    continue;

                // check if the identifier is used for something other than to access indexer
                var elementAccessExpressionSyntax = identifierNameSyntax.FirstAncestorOrSelf<ElementAccessExpressionSyntax>();
                if (elementAccessExpressionSyntax is null)
                    return;

                var arguments = elementAccessExpressionSyntax.ArgumentList.Arguments;
                if (arguments.Count != 1)
                    continue;

                // check if the indexer variable is the same as the one used in the for loop
                if (arguments[0].ToString() != forIdentifier)
                    return;

                var expression = elementAccessExpressionSyntax.Expression;
                if (expression is not IdentifierNameSyntax elementAccessIdentifierNameSyntax)
                    return;

                // check if the indexed collection is the same for all accesses
                var expressionString = expression.ToString();
                if (!string.IsNullOrEmpty(indexedCollection) && indexedCollection != expressionString)
                    return;

                // check if foreach is optimized for this type
                var typeInfo = context.SemanticModel.GetTypeInfo(elementAccessIdentifierNameSyntax);
                var expressionType = typeInfo.ConvertedType;
                if (!expressionType.IsForEachOptimized())
                    return;

                indexedCollection = expressionString;
                indexedCollectionType = expressionType;
            }
            if (indexedCollection is null)
                return;

            var diagnostic2 = Diagnostic.Create(Rule, forStatement.ForKeyword.GetLocation(), indexedCollectionType!.ToString());
            context.ReportDiagnostic(diagnostic2);
        }
    }
}