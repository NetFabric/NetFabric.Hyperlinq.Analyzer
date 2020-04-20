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
    public sealed class GetEnumeratorReturnTypeAnalyzer : DiagnosticAnalyzer
    {
        const string DiagnosticId = DiagnosticIds.GetEnumeratorReturnTypeId;

        static readonly LocalizableString Title =
            new LocalizableResourceString(nameof(Resources.GetEnumeratorReturnType_Title), Resources.ResourceManager, typeof(Resources));
        static readonly LocalizableString MessageFormat =
            new LocalizableResourceString(nameof(Resources.GetEnumeratorReturnType_MessageFormat), Resources.ResourceManager, typeof(Resources));
        static readonly LocalizableString Description =
            new LocalizableResourceString(nameof(Resources.GetEnumeratorReturnType_Description), Resources.ResourceManager, typeof(Resources));
        const string Category = "Performance";

        static readonly DiagnosticDescriptor rule =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning,
                isEnabledByDefault: true, description: Description,
                helpLinkUri: "https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/tree/master/docs/reference/HLQ006_GetEnumeratorReturnType.md");

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

            var semanticModel = context.SemanticModel;

            // check if it does not return a value type
            var returnType = methodDeclarationSyntax.ReturnType;
            if (semanticModel.GetTypeInfo(returnType).Type.TypeKind == TypeKind.Struct)
                return;

            // check if it's public
            if (!methodDeclarationSyntax.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PublicKeyword)))
                return;

            // check if it's "GetEnumerator" or "GetAsyncEnumerator"
            var identifier = methodDeclarationSyntax.Identifier.ValueText;
            if (identifier == "GetEnumerator" && methodDeclarationSyntax.ParameterList.Parameters.Count == 0)
            {
                // check if it returns an enumerator
                var type = semanticModel.GetTypeInfo(returnType).Type;
                if (!type.IsEnumerator(semanticModel.Compilation, out var enumerableSymbols))
                    return;
            }
            else if (identifier == "GetAsyncEnumerator" && methodDeclarationSyntax.ParameterList.Parameters.Count < 2)
            {
                // check if it returns an async enumerator
                var type = semanticModel.GetTypeInfo(returnType).Type;
                if (!type.IsAsyncEnumerator(semanticModel.Compilation, out var enumerableSymbols))
                    return;
            }
            else
            {
                return;
            }

            var diagnostic = Diagnostic.Create(rule, methodDeclarationSyntax.ReturnType.GetLocation(), identifier);
            context.ReportDiagnostic(diagnostic);
        }
    }
}