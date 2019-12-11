using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NetFabric.CodeAnalysis;
using System;
using System.Collections.Immutable;
using System.Linq;

namespace NetFabric.Hyperlinq.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class AvoidSingleAnalyzer : DiagnosticAnalyzer
    {
        const string DiagnosticId = DiagnosticIds.AvoidSingleId;

        static readonly LocalizableString Title =
            new LocalizableResourceString(nameof(Resources.AvoidSingle_Title), Resources.ResourceManager, typeof(Resources));
        static readonly LocalizableString MessageFormat =
            new LocalizableResourceString(nameof(Resources.AvoidSingle_MessageFormat), Resources.ResourceManager, typeof(Resources));
        static readonly LocalizableString Description =
            new LocalizableResourceString(nameof(Resources.AvoidSingle_Description), Resources.ResourceManager, typeof(Resources));
        const string Category = "Performance";

        static readonly DiagnosticDescriptor rule =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning,
                isEnabledByDefault: true, description: Description,
                helpLinkUri: "https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/tree/master/docs/reference/HLQ005_AvoidSingle.md");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);
        }

        static void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is InvocationExpressionSyntax invocationExpressionSyntax))
                return;

            var memberAccessExpressionSyntax = invocationExpressionSyntax.Expression as MemberAccessExpressionSyntax;
            if (memberAccessExpressionSyntax is null)
                return;

            var methodFound = String.Empty;
            var methodReplace = String.Empty;
            switch (memberAccessExpressionSyntax.Name.ToString())
            {
                case "Single":
                    methodFound = "Single";
                    methodReplace = "First";
                    break;
                case "SingleOrDefault":
                    methodFound = "SingleOrDefault";
                    methodReplace = "FirstOrDefault";
                    break;
                default:
                    return;
            }

            if (memberAccessExpressionSyntax.Expression is IdentifierNameSyntax identifierName)
            {
                var semanticModel = context.SemanticModel;

                var type = semanticModel.GetTypeInfo(identifierName).Type;
                if (!type.IsEnumerable(context.Compilation, out _))
                {
                    var arguments = invocationExpressionSyntax.ArgumentList.Arguments;
                    if (arguments.Count == 0 || arguments.Count > 2)
                        return;

                    var firstArgument = arguments[0];
                    var argumentType = semanticModel.GetTypeInfo(firstArgument.Expression).Type;
                    if (argumentType is null || !argumentType.IsEnumerable(context.Compilation, out _))
                        return;
                }
            }

            var diagnostic = Diagnostic.Create(rule, memberAccessExpressionSyntax.Name.GetLocation(), methodFound, methodReplace);
            context.ReportDiagnostic(diagnostic);
        }
    }
}