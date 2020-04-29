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

        static readonly DiagnosticDescriptor rule =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning,
                isEnabledByDefault: true, description: Description,
                helpLinkUri: "https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/tree/master/docs/reference/HLQ003_HighestLevelInterface.md");

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

            var returnType = context.SemanticModel.GetTypeInfo(methodDeclarationSyntax.ReturnType).Type as INamedTypeSymbol;
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

            var expressionType = context.SemanticModel.GetTypeInfo(arrowExpressionClauseSyntax.Expression).Type as INamedTypeSymbol;
            if (expressionType is null)
                return;

            switch (methodReturnType)
            {
                case SpecialType.System_Collections_IEnumerable:
                    if (expressionType.ImplementsInterface(SpecialType.System_Collections_Generic_IReadOnlyList_T, out _))
                    {
                        var diagnostic = Diagnostic.Create(rule, methodDeclarationSyntax.ReturnType.GetLocation(), "IReadOnlyList`1");
                        context.ReportDiagnostic(diagnostic);
                    }
                    else if (expressionType.ImplementsInterface(SpecialType.System_Collections_Generic_IReadOnlyCollection_T, out _))
                    {
                        var diagnostic = Diagnostic.Create(rule, methodDeclarationSyntax.ReturnType.GetLocation(), "IReadOnlyCollection`1");
                        context.ReportDiagnostic(diagnostic);
                    }
                    else if(expressionType.ImplementsInterface(SpecialType.System_Collections_Generic_IEnumerable_T, out _))
                    {
                        var diagnostic = Diagnostic.Create(rule, methodDeclarationSyntax.ReturnType.GetLocation(), "IEnumerable`1");
                        context.ReportDiagnostic(diagnostic);
                    }
                    break;
                case SpecialType.System_Collections_Generic_IEnumerable_T:
                    if (expressionType.ImplementsInterface(SpecialType.System_Collections_Generic_IReadOnlyList_T, out _))
                    {
                        var diagnostic = Diagnostic.Create(rule, methodDeclarationSyntax.ReturnType.GetLocation(), "IReadOnlyList`1");
                        context.ReportDiagnostic(diagnostic);
                    }
                    else if (expressionType.ImplementsInterface(SpecialType.System_Collections_Generic_IReadOnlyCollection_T, out _))
                    {
                        var diagnostic = Diagnostic.Create(rule, methodDeclarationSyntax.ReturnType.GetLocation(), "IReadOnlyCollection`1");
                        context.ReportDiagnostic(diagnostic);
                    }
                    break;
                case SpecialType.System_Collections_Generic_IReadOnlyCollection_T:
                    if (expressionType.ImplementsInterface(SpecialType.System_Collections_Generic_IReadOnlyList_T, out _))
                    {
                        var diagnostic = Diagnostic.Create(rule, methodDeclarationSyntax.ReturnType.GetLocation(), "IReadOnlyList`1");
                        context.ReportDiagnostic(diagnostic);
                    }
                    break;
            }
        }

        static void AnalyzeIEnumerableMethod(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax methodDeclarationSyntax)
        {
            if (AllReturnsNull(context, methodDeclarationSyntax))
                return;

            if (AllReturnsImplement(context, methodDeclarationSyntax, SpecialType.System_Collections_Generic_IReadOnlyList_T))
            {
                var diagnostic = Diagnostic.Create(rule, methodDeclarationSyntax.ReturnType.GetLocation(), "IReadOnlyList`1");
                context.ReportDiagnostic(diagnostic);
                return;
            }

            if (AllReturnsImplement(context, methodDeclarationSyntax, SpecialType.System_Collections_Generic_IReadOnlyCollection_T))
            {
                var diagnostic = Diagnostic.Create(rule, methodDeclarationSyntax.ReturnType.GetLocation(), "IReadOnlyCollection`1");
                context.ReportDiagnostic(diagnostic);
                return;
            }

            if (AllReturnsImplement(context, methodDeclarationSyntax, SpecialType.System_Collections_Generic_IEnumerable_T))
            {
                var diagnostic = Diagnostic.Create(rule, methodDeclarationSyntax.ReturnType.GetLocation(), "IEnumerable`1");
                context.ReportDiagnostic(diagnostic);
                return;
            }
        }

        static void AnalyzeIEnumerableTMethod(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax methodDeclarationSyntax)
        {
            if (AllReturnsNull(context, methodDeclarationSyntax))
                return;

            if (AllReturnsImplement(context, methodDeclarationSyntax, SpecialType.System_Collections_Generic_IReadOnlyList_T))
            {
                var diagnostic = Diagnostic.Create(rule, methodDeclarationSyntax.ReturnType.GetLocation(), "IReadOnlyList`1");
                context.ReportDiagnostic(diagnostic);
                return;
            }

            if (AllReturnsImplement(context, methodDeclarationSyntax, SpecialType.System_Collections_Generic_IReadOnlyCollection_T))
            {
                var diagnostic = Diagnostic.Create(rule, methodDeclarationSyntax.ReturnType.GetLocation(), "IReadOnlyCollection`1");
                context.ReportDiagnostic(diagnostic);
                return;
            }
        }

        static void AnalyzeIReadOnlyCollectionMethod(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax methodDeclarationSyntax)
        {
            if (AllReturnsNull(context, methodDeclarationSyntax))
                return;

            if (AllReturnsImplement(context, methodDeclarationSyntax, SpecialType.System_Collections_Generic_IReadOnlyList_T))
            {
                var diagnostic = Diagnostic.Create(rule, methodDeclarationSyntax.ReturnType.GetLocation(), "IReadOnlyList`1");
                context.ReportDiagnostic(diagnostic);
                return;
            }
        }

        static bool AllReturnsNull(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax methodDeclarationSyntax)
        {
            var semanticModel = context.SemanticModel;
            foreach(var node in methodDeclarationSyntax.DescendantNodes().OfType<ReturnStatementSyntax>())
            {
                if(!node.Expression.IsKind(SyntaxKind.NullLiteralExpression))
                    return false;
            }
            return true;
        }

        static bool AllReturnsImplement(SyntaxNodeAnalysisContext context, MethodDeclarationSyntax methodDeclarationSyntax, SpecialType type)
        {
            var semanticModel = context.SemanticModel;
            foreach(var node in methodDeclarationSyntax.DescendantNodes().OfType<ReturnStatementSyntax>())
            {
                var returnType = semanticModel.GetTypeInfo(node.Expression).Type as INamedTypeSymbol;
                if (returnType is null || !returnType.ImplementsInterface(type, out _))
                    return false;
            }
            return true;
        }
    }
}