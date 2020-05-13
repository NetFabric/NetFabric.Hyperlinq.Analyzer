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
    public sealed class ReadOnlyEnumeratorFieldAnalyzer : DiagnosticAnalyzer
    {
        const string DiagnosticId = DiagnosticIds.ReadOnlyEnumeratorFieldId;

        static readonly LocalizableString Title =
            new LocalizableResourceString(nameof(Resources.ReadOnlyEnumeratorField_Title), Resources.ResourceManager, typeof(Resources));
        static readonly LocalizableString MessageFormat =
            new LocalizableResourceString(nameof(Resources.ReadOnlyEnumeratorField_MessageFormat), Resources.ResourceManager, typeof(Resources));
        static readonly LocalizableString Description =
            new LocalizableResourceString(nameof(Resources.ReadOnlyEnumeratorField_Description), Resources.ResourceManager, typeof(Resources));
        const string Category = "Compiler";

        static readonly DiagnosticDescriptor Rule =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Error,
                isEnabledByDefault: true, description: Description,
                helpLinkUri: "https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/tree/master/docs/reference/HLQ011_ReadOnlyEnumeratorField.md");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(AnalyzeFieldDeclaration, SyntaxKind.FieldDeclaration);
        }

        static void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is FieldDeclarationSyntax fieldDeclaration))
                return;

            if (!fieldDeclaration.Modifiers.Any(modifier => modifier.IsKind(SyntaxKind.ReadOnlyKeyword))
                || !fieldDeclaration.IsEnumerableValueType(context))
                return;

            var diagnostic = Diagnostic.Create(Rule, fieldDeclaration.Declaration.Type.GetLocation(), fieldDeclaration.Declaration.Type.ToString());
            context.ReportDiagnostic(diagnostic);
        }
    }
}