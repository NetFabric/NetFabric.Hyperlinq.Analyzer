﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace NetFabric.Hyperlinq.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class ReadOnlyRefEnumerableAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = DiagnosticIds.ReadOnlyRefEnumerableId;

        static readonly LocalizableString Title =
            new LocalizableResourceString(nameof(Resources.ReadOnlyRefEnumerable_Title), Resources.ResourceManager, typeof(Resources));
        static readonly LocalizableString MessageFormat =
            new LocalizableResourceString(nameof(Resources.ReadOnlyRefEnumerable_MessageFormat), Resources.ResourceManager, typeof(Resources));
        static readonly LocalizableString Description =
            new LocalizableResourceString(nameof(Resources.ReadOnlyRefEnumerable_Description), Resources.ResourceManager, typeof(Resources));
        const string Category = "Performance";

        static readonly DiagnosticDescriptor Rule =
            new(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Info,
                isEnabledByDefault: true, description: Description,
                helpLinkUri: "https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/tree/master/docs/reference/HLQ008_ReadOnlyRefEnumerable.md");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(AnalyzeStructDeclaration, SyntaxKind.StructDeclaration);
        }

        static void AnalyzeStructDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not StructDeclarationSyntax structDeclarationSyntax)
                return;

            if (structDeclarationSyntax.Modifiers
                .Any(modifier => modifier.IsKind(SyntaxKind.ReadOnlyKeyword)))
                return;

            if (!structDeclarationSyntax.IsEnumerableOrAsyncEnumerable(context))
                return;

            var diagnostic = Diagnostic.Create(Rule, structDeclarationSyntax.Keyword.GetLocation(), structDeclarationSyntax.Identifier.Text);
            context.ReportDiagnostic(diagnostic);
        }
    }
}