using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NetFabric.CodeAnalysis;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace NetFabric.Hyperlinq.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class HighestLevelInterfaceAnalyzer : DiagnosticAnalyzer
    {
        const string DiagnosticId = DiagnosticIds.HighestLevelInterfaceId;

        static readonly LocalizableString Title =
            new LocalizableResourceString(nameof(Resources.HighestLevelInterface_Title), Resources.ResourceManager, typeof(Resources));
        static readonly LocalizableString MessageFormat =
            new LocalizableResourceString(nameof(Resources.HighestLevelInterface_MessageFormat), Resources.ResourceManager, typeof(Resources));
        static readonly LocalizableString Description =
            new LocalizableResourceString(nameof(Resources.HighestLevelInterface_Description), Resources.ResourceManager, typeof(Resources));
        const string Category = "Performance";

        static readonly DiagnosticDescriptor Rule =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning,
                isEnabledByDefault: true, description: Description,
                helpLinkUri: "https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/tree/master/docs/reference/HLQ003_HighestLevelInterface.md");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Rule);

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

            if (methodDeclarationSyntax.ReturnsVoid())
                return;

            var returnType = context.SemanticModel.GetTypeInfo(methodDeclarationSyntax.ReturnType).Type;
            if (returnType is null)
                return;

            var returnSpecialType = returnType.OriginalDefinition.SpecialType;
            if (returnSpecialType == SpecialType.None)
                return;

            if (methodDeclarationSyntax.Body is null)
                AnalyzeArrowExpressionClause(context, methodDeclarationSyntax, returnSpecialType);
            else
                AnalyzeMethodDeclaration(context, methodDeclarationSyntax, returnSpecialType);
        }

        static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax methodDeclarationSyntax, SpecialType methodReturnType)
        {
            switch (methodReturnType)
            {
                case SpecialType.System_Collections_IEnumerable:
                    AnalyzeIEnumerableMethod(context, methodDeclarationSyntax);
                    break;
                case SpecialType.System_Collections_Generic_IEnumerable_T:
                    AnalyzeIEnumerableTMethod(context, methodDeclarationSyntax);
                    break;
                case SpecialType.System_Collections_Generic_IReadOnlyCollection_T:
                    AnalyzeIReadOnlyCollectionMethod(context, methodDeclarationSyntax);
                    break;
            }
        }

        static void AnalyzeArrowExpressionClause(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax methodDeclarationSyntax, SpecialType methodReturnType)
        {
            var arrowExpressionClauseSyntax = methodDeclarationSyntax.DescendantNodes()
                .OfType<ArrowExpressionClauseSyntax>()
                .FirstOrDefault();
            if (arrowExpressionClauseSyntax is null)
                return;

            var expressionType = context.SemanticModel.GetTypeInfo(arrowExpressionClauseSyntax.Expression).Type;
            if (expressionType is null)
                return;

            switch (methodReturnType)
            {
                case SpecialType.System_Collections_IEnumerable:
                    if (expressionType.ImplementsInterface(SpecialType.System_Collections_Generic_IReadOnlyList_T, out _))
                    {
                        var diagnostic = Diagnostic.Create(Rule, methodDeclarationSyntax.ReturnType.GetLocation(), "IReadOnlyList`1");
                        context.ReportDiagnostic(diagnostic);
                    }
                    else if (expressionType.ImplementsInterface(SpecialType.System_Collections_Generic_IReadOnlyCollection_T, out _))
                    {
                        var diagnostic = Diagnostic.Create(Rule, methodDeclarationSyntax.ReturnType.GetLocation(), "IReadOnlyCollection`1");
                        context.ReportDiagnostic(diagnostic);
                    }
                    else if(expressionType.ImplementsInterface(SpecialType.System_Collections_Generic_IEnumerable_T, out _))
                    {
                        var diagnostic = Diagnostic.Create(Rule, methodDeclarationSyntax.ReturnType.GetLocation(), "IEnumerable`1");
                        context.ReportDiagnostic(diagnostic);
                    }
                    break;
                case SpecialType.System_Collections_Generic_IEnumerable_T:
                    if (expressionType.ImplementsInterface(SpecialType.System_Collections_Generic_IReadOnlyList_T, out _))
                    {
                        var diagnostic = Diagnostic.Create(Rule, methodDeclarationSyntax.ReturnType.GetLocation(), "IReadOnlyList`1");
                        context.ReportDiagnostic(diagnostic);
                    }
                    else if (expressionType.ImplementsInterface(SpecialType.System_Collections_Generic_IReadOnlyCollection_T, out _))
                    {
                        var diagnostic = Diagnostic.Create(Rule, methodDeclarationSyntax.ReturnType.GetLocation(), "IReadOnlyCollection`1");
                        context.ReportDiagnostic(diagnostic);
                    }
                    break;
                case SpecialType.System_Collections_Generic_IReadOnlyCollection_T:
                    if (expressionType.ImplementsInterface(SpecialType.System_Collections_Generic_IReadOnlyList_T, out _))
                    {
                        var diagnostic = Diagnostic.Create(Rule, methodDeclarationSyntax.ReturnType.GetLocation(), "IReadOnlyList`1");
                        context.ReportDiagnostic(diagnostic);
                    }
                    break;
            }
        }

        static void AnalyzeIEnumerableMethod(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax methodDeclarationSyntax)
        {
            if (methodDeclarationSyntax.AllReturnsImplement(SpecialType.System_Collections_Generic_IReadOnlyList_T, context.SemanticModel))
            {
                var diagnostic = Diagnostic.Create(Rule, methodDeclarationSyntax.ReturnType.GetLocation(), "IReadOnlyList`1");
                context.ReportDiagnostic(diagnostic);
                return;
            }

            if (methodDeclarationSyntax.AllReturnsImplement(SpecialType.System_Collections_Generic_IReadOnlyCollection_T, context.SemanticModel))
            {
                var diagnostic = Diagnostic.Create(Rule, methodDeclarationSyntax.ReturnType.GetLocation(), "IReadOnlyCollection`1");
                context.ReportDiagnostic(diagnostic);
                return;
            }

            if (methodDeclarationSyntax.AllReturnsImplement(SpecialType.System_Collections_Generic_IEnumerable_T, context.SemanticModel))
            {
                var diagnostic = Diagnostic.Create(Rule, methodDeclarationSyntax.ReturnType.GetLocation(), "IEnumerable`1");
                context.ReportDiagnostic(diagnostic);
                return;
            }
        }

        static void AnalyzeIEnumerableTMethod(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax methodDeclarationSyntax)
        {
            if (methodDeclarationSyntax.AllReturnsImplement(SpecialType.System_Collections_Generic_IReadOnlyList_T, context.SemanticModel))
            {
                var diagnostic = Diagnostic.Create(Rule, methodDeclarationSyntax.ReturnType.GetLocation(), "IReadOnlyList`1");
                context.ReportDiagnostic(diagnostic);
                return;
            }

            if (methodDeclarationSyntax.AllReturnsImplement(SpecialType.System_Collections_Generic_IReadOnlyCollection_T, context.SemanticModel))
            {
                var diagnostic = Diagnostic.Create(Rule, methodDeclarationSyntax.ReturnType.GetLocation(), "IReadOnlyCollection`1");
                context.ReportDiagnostic(diagnostic);
                return;
            }
        }

        static void AnalyzeIReadOnlyCollectionMethod(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax methodDeclarationSyntax)
        {
            if (methodDeclarationSyntax.AllReturnsImplement(SpecialType.System_Collections_Generic_IReadOnlyList_T, context.SemanticModel))
            {
                var diagnostic = Diagnostic.Create(Rule, methodDeclarationSyntax.ReturnType.GetLocation(), "IReadOnlyList`1");
                context.ReportDiagnostic(diagnostic);
                return;
            }
        }
    }
}