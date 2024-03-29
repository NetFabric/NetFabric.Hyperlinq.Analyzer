﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NetFabric.CodeAnalysis;
using System.Collections.Immutable;
using System.Linq;

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

        static readonly DiagnosticDescriptor Rule =
            new(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning,
                isEnabledByDefault: true, description: Description,
                helpLinkUri: "https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/tree/master/docs/reference/HLQ007_NonDisposableEnumerator.md");

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
            var compilation = context.Compilation;

            // check if it's an empty Dispose or DisposeAsync
            if (methodDeclarationSyntax.IsDispose())
            {
                if (!methodDeclarationSyntax.IsEmptyMethod())
                    return;
            }
            else if (methodDeclarationSyntax.IsAsyncDispose())
            {
                if (!methodDeclarationSyntax.IsEmptyAsyncMethod())
                    return;
            }
            else
            {
                return;
            }

            // find the disposable type
            var typeDeclaration = methodDeclarationSyntax.Ancestors().OfType<TypeDeclarationSyntax>().FirstOrDefault();
            if (typeDeclaration is null)
                return;

            var name = typeDeclaration.GetMetadataName();
            var declaredTypeSymbol = compilation.GetTypeByMetadataName(name);
            if (declaredTypeSymbol is null)
                return;

            var enumeratorTypeSymbol = declaredTypeSymbol;
            IMethodSymbol getEnumerator;

            // check if disposable type is an enumerator
            if (declaredTypeSymbol.IsEnumerator())
            {
                // check if there's an outer type
                typeDeclaration = typeDeclaration.Ancestors().OfType<TypeDeclarationSyntax>().FirstOrDefault();
                if (typeDeclaration is null)
                    return;

                name = typeDeclaration.GetMetadataName();
                declaredTypeSymbol = compilation.GetTypeByMetadataName(name);
                if (declaredTypeSymbol is null)
                    return;

                // check if outer type is an enumerable
                if (!declaredTypeSymbol.IsEnumerable(compilation, out var enumerableSymbols))
                    return;

                getEnumerator = enumerableSymbols.GetEnumerator;
            }
            else if (declaredTypeSymbol.IsAsyncEnumerator(compilation))
            {
                // check if there's an outer type
                typeDeclaration = typeDeclaration.Ancestors().OfType<TypeDeclarationSyntax>().FirstOrDefault();
                if (typeDeclaration is null)
                    return;

                name = typeDeclaration.GetMetadataName();
                declaredTypeSymbol = compilation.GetTypeByMetadataName(name);
                if (declaredTypeSymbol is null)
                    return;

                // check if outer type is an async enumerable
                if (!declaredTypeSymbol.IsAsyncEnumerable(compilation, out var asyncEnumerableSymbols))
                    return;

                getEnumerator = asyncEnumerableSymbols.GetAsyncEnumerator;
            }
            else
            {
                return;
            }

            // check if the public GetEnumerator() returns the disposable enumerator
            if (getEnumerator.ReturnType.TypeKind != TypeKind.Interface 
                && !SymbolEqualityComparer.Default.Equals(getEnumerator.ReturnType, enumeratorTypeSymbol))
                return;

            // find the location of GetEnumerator() return type
            var enumerableTypeDeclaration = typeDeclaration;
            var getEnumeratorDeclaration = enumerableTypeDeclaration.ChildNodes()
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(method => 
                    method.Modifiers.Any(token => token.IsKind(SyntaxKind.PublicKeyword)) 
                    && (method.Identifier.Text == getEnumerator.Name));
            if (getEnumeratorDeclaration is null)
                return;

            var diagnostic = Diagnostic.Create(Rule, getEnumeratorDeclaration.ReturnType.GetLocation(), enumeratorTypeSymbol.Name);
            context.ReportDiagnostic(diagnostic);
        }
    }
}