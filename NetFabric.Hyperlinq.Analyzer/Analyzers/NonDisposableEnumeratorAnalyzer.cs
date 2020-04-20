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
    public sealed class NonDisposableEnumeratorAnalyzer : DiagnosticAnalyzer
    {
        const string DiagnosticId = DiagnosticIds.NonDisposableEnumeratorId;

        static readonly LocalizableString Title =
            new LocalizableResourceString(nameof(Resources.NonDisposableEnumerator_Title), Resources.ResourceManager, typeof(Resources));
        static readonly LocalizableString MessageFormat =
            new LocalizableResourceString(nameof(Resources.NonDisposableEnumerator_MessageFormat), Resources.ResourceManager, typeof(Resources));
        static readonly LocalizableString Description =
            new LocalizableResourceString(nameof(Resources.NonDisposableEnumerator_Description), Resources.ResourceManager, typeof(Resources));
        const string Category = "Performance";

        static readonly DiagnosticDescriptor rule =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning,
                isEnabledByDefault: true, description: Description,
                helpLinkUri: "https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/tree/master/docs/reference/HLQ006_NonDisposableEnumerator.md");

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
            var compilation = context.Compilation;

            // check if it has an empty body
            if (methodDeclarationSyntax.Body?.Statements.Any() ?? true)
                return;

            // check if it's Dispose or DisposeAsync
            if (methodDeclarationSyntax.ParameterList.Parameters.Any())
                return;

            if (methodDeclarationSyntax.Identifier.ValueText == "Dispose")
                //&& methodDeclarationSyntax.ReturnType is PredefinedTypeSyntax predefinedTypeSyntax 
                //&& predefinedTypeSyntax.IsKind(SyntaxKind.VoidKeyword))
            {
                // do nothing
            }
            else if (methodDeclarationSyntax.Identifier.ValueText == "DisposeAsync")
            {
                var returnType = semanticModel.GetTypeInfo(methodDeclarationSyntax.ReturnType).Type;
                if (returnType.Name != "ValueTask")
                    return;

                // do nothing
            }
            else
            {
                return;
            }

            // find the disposable type
            var typeDeclaration = methodDeclarationSyntax.Ancestors().OfType<TypeDeclarationSyntax>().FirstOrDefault();
            if (typeDeclaration is null)
                return;

            var name = typeDeclaration.GetFullName();
            var declaredTypeSymbol = compilation.GetTypeByMetadataName(name);

            // check if disposable type is an enumerator
            if (!declaredTypeSymbol.IsEnumerator(compilation, out var _) && !declaredTypeSymbol.IsAsyncEnumerator(compilation, out var _))
                return;

            var enumeratorTypeSymbol = declaredTypeSymbol;

            // check if there's an outer type
            typeDeclaration = typeDeclaration.Ancestors().OfType<TypeDeclarationSyntax>().FirstOrDefault();
            if (typeDeclaration is null)
                return;

            name = typeDeclaration.GetFullName();
            declaredTypeSymbol = compilation.GetTypeByMetadataName(name);

            if (declaredTypeSymbol is null)
                throw new Exception(name);

            // check if outer type is an enumerable
            IMethodSymbol getEnumerator;
            if (declaredTypeSymbol.IsEnumerable(compilation, out var enumerableSymbols))
            {
                getEnumerator = enumerableSymbols.GetEnumerator;
            }
            else if (declaredTypeSymbol.IsAsyncEnumerable(compilation, out var asyncEnumerableSymbols))
            {
                getEnumerator = asyncEnumerableSymbols.GetAsyncEnumerator;
            }
            else
            {
                return;
            }

            // check if the public GetEnumerator() return the disposable enumerator
            if (!Equals(getEnumerator.ReturnType, enumeratorTypeSymbol))
                return;

            // find the location of GetEnumerator() return type
            var enumerableTypeDeclaration = typeDeclaration;
            var getEnumeratorDeclaration = enumerableTypeDeclaration.ChildNodes()
                .OfType<MethodDeclarationSyntax>()
                .First(method => 
                    method.Modifiers.Any(token => token.Text == "public") 
                    && (method.Identifier.Text == getEnumerator.Name));

            var diagnostic = Diagnostic.Create(rule, getEnumeratorDeclaration.ReturnType.GetLocation(), enumeratorTypeSymbol.Name);
            context.ReportDiagnostic(diagnostic);
        }

        static IEnumerable<INamedTypeSymbol> GetAllTypes(Compilation compilation)
            => GetAllTypes(compilation.GlobalNamespace);

        static IEnumerable<INamedTypeSymbol> GetAllTypes(INamespaceSymbol @namespace)
        {
            foreach (var type in @namespace.GetTypeMembers())
                foreach (var nestedType in GetNestedTypes(type))
                    yield return nestedType;

            foreach (var nestedNamespace in @namespace.GetNamespaceMembers())
                foreach (var type in GetAllTypes(nestedNamespace))
                    yield return type;
        }

        static IEnumerable<INamedTypeSymbol> GetNestedTypes(INamedTypeSymbol type)
        {
            yield return type;
            foreach (var nestedType in type.GetTypeMembers()
                .SelectMany(nestedType => GetNestedTypes(nestedType)))
                yield return nestedType;
        }
    }
}