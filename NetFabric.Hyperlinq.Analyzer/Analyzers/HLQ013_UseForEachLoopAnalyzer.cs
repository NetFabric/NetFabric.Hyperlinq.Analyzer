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

            var declaration = forStatement.Declaration;
            if (declaration is null || declaration.Variables.Count != 1)
                return;

            var statement = forStatement.Statement;
            if (statement is null)
                return;

            // check if declaration declares a variable of type int
            var variableType = context.SemanticModel.GetTypeInfo(declaration.Type).Type;
            if (variableType is null || variableType.SpecialType != SpecialType.System_Int32)
                return;

            // check if the variable is used in the statement only for accessing indexer of one single array or span
            var variableIdentifier = declaration.Variables[0].Identifier.ToString();
            var indexedCollection = default(string);
            var indexedCollectionType = default(ITypeSymbol);
            foreach (var identifierNameSyntax in statement.DescendantNodes().OfType<IdentifierNameSyntax>())
            {
                // check if the identifier is using the variable
                if (identifierNameSyntax.Identifier.ToString() != variableIdentifier)
                    continue;

                // check if the identifier is used for something other than to access indexer
                var elementAccessExpressionSyntax = identifierNameSyntax.FirstAncestorOrSelf<ElementAccessExpressionSyntax>();
                if (elementAccessExpressionSyntax is null)
                    return;

                if (elementAccessExpressionSyntax.ArgumentList.Arguments.Count != 1)
                    continue;

                var expression = elementAccessExpressionSyntax.Expression;

                // check if the indexed collection is an array or a span
                var expressionType = context.SemanticModel.GetTypeInfo(expression).Type;
                if (!(expressionType.IsArrayType() || expressionType.IsSpanType()))
                    return;

                // check if the indexed collection is the same for all accesses
                var expressionString = expression.ToString();
                if (!string.IsNullOrEmpty(indexedCollection) && indexedCollection != expressionString)
                    return;

                indexedCollection = expressionString;
                indexedCollectionType = expressionType;
            }
            if (indexedCollection is null)
                return;

            var diagnostic = Diagnostic.Create(Rule, forStatement.ForKeyword.GetLocation(), indexedCollectionType!.ToDisplayString());
            context.ReportDiagnostic(diagnostic);
        }
    }
}