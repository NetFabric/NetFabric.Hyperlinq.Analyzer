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
    public sealed class RemoveOptionalMethodsAnalyzer : DiagnosticAnalyzer
    {
        const string DiagnosticId = DiagnosticIds.RemoveOptionalMethodsId;

        static readonly LocalizableString Title =
            new LocalizableResourceString(nameof(Resources.RemoveOptionalMethods_Title), Resources.ResourceManager, typeof(Resources));
        static readonly LocalizableString MessageFormat =
            new LocalizableResourceString(nameof(Resources.RemoveOptionalMethods_MessageFormat), Resources.ResourceManager, typeof(Resources));
        static readonly LocalizableString Description =
            new LocalizableResourceString(nameof(Resources.RemoveOptionalMethods_Description), Resources.ResourceManager, typeof(Resources));
        const string Category = "Performance";

        static readonly DiagnosticDescriptor Rule =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Info,
                isEnabledByDefault: true, description: Description,
                helpLinkUri: "https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/tree/master/docs/reference/HLQ009_RemoveOptionalMethods.md");

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

            if (methodDeclarationSyntax.IsReset() && methodDeclarationSyntax.IsEmptyMethod())
            {
                var enumeratorDeclaration = methodDeclarationSyntax.Ancestors().OfType<TypeDeclarationSyntax>().FirstOrDefault();
                if (enumeratorDeclaration is null || enumeratorDeclaration.ImplementsInterface(SpecialType.System_Collections_IEnumerator, context))
                    return;
            }
            else if (methodDeclarationSyntax.IsDispose() && methodDeclarationSyntax.IsEmptyMethod())
            {
                var enumeratorDeclaration = methodDeclarationSyntax.Ancestors().OfType<TypeDeclarationSyntax>().FirstOrDefault();
                if (enumeratorDeclaration is null || enumeratorDeclaration.ImplementsInterface(SpecialType.System_Collections_Generic_IEnumerator_T, context))
                    return;
            }
            else if (methodDeclarationSyntax.IsAsyncDispose() && methodDeclarationSyntax.IsEmptyAsyncMethod())
            {
                var enumeratorDeclaration = methodDeclarationSyntax.Ancestors().OfType<TypeDeclarationSyntax>().FirstOrDefault();
                if (enumeratorDeclaration is null)
                    return;

                var asyncEnumeratorType = context.Compilation.GetTypeByMetadataName("System.Collections.Generic.IAsyncEnumerator`1");
                if (asyncEnumeratorType is null || enumeratorDeclaration.ImplementsInterface(asyncEnumeratorType, context))
                    return;
            }
            else
            {
                return;
            }

            var diagnostic = Diagnostic.Create(Rule, methodDeclarationSyntax.Identifier.GetLocation(), methodDeclarationSyntax.Identifier.ValueText);
            context.ReportDiagnostic(diagnostic);
        }
    }
}