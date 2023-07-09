using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NetFabric.CodeAnalysis;
using System.Collections.Immutable;

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

        static readonly DiagnosticDescriptor Rule =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning,
                isEnabledByDefault: true, description: Description,
                helpLinkUri: "https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/tree/master/docs/reference/HLQ006_GetEnumeratorReturnType.md");

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
            if (context.Node is not MethodDeclarationSyntax methodDeclarationSyntax)
                return;

            var semanticModel = context.SemanticModel;

            // check if it does not return a value type
            var returnType = methodDeclarationSyntax.ReturnType;
            var returnTypeInfo = semanticModel.GetTypeInfo(returnType).Type;
            if (returnTypeInfo is null || returnTypeInfo.TypeKind == TypeKind.Struct)
                return;

            // check if it's public
            if (!methodDeclarationSyntax.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.PublicKeyword)))
                return;

            // check if it's "GetEnumerator" or "GetAsyncEnumerator"
            var identifier = methodDeclarationSyntax.Identifier.ValueText;
            if (identifier == "GetEnumerator" && !methodDeclarationSyntax.ParameterList.Parameters.Any())
            {
                // check if it returns an enumerator
                var type = semanticModel.GetTypeInfo(returnType).Type;
                if (type is null || !type.IsEnumerator(semanticModel.Compilation, out _))
                    return;
            }
            else if (identifier == "GetAsyncEnumerator" && methodDeclarationSyntax.ParameterList.Parameters.Count < 2)
            {
                // check if it returns an async enumerator
                var type = semanticModel.GetTypeInfo(returnType).Type;
                if (type is null || !type.IsAsyncEnumerator(semanticModel.Compilation, out _))
                    return;
            }
            else
            {
                return;
            }

            var diagnostic = Diagnostic.Create(Rule, returnType.GetLocation(), identifier);
            context.ReportDiagnostic(diagnostic);
        }
    }
}