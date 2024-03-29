﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using NetFabric.CodeAnalysis;
using System.Collections.Immutable;

namespace NetFabric.Hyperlinq.Analyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class AvoidSingleAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = DiagnosticIds.AvoidSingleId;

        static readonly LocalizableString Title =
            new LocalizableResourceString(nameof(Resources.AvoidSingle_Title), Resources.ResourceManager, typeof(Resources));
        static readonly LocalizableString MessageFormat =
            new LocalizableResourceString(nameof(Resources.AvoidSingle_MessageFormat), Resources.ResourceManager, typeof(Resources));
        static readonly LocalizableString Description =
            new LocalizableResourceString(nameof(Resources.AvoidSingle_Description), Resources.ResourceManager, typeof(Resources));
        const string Category = "Performance";

        static readonly DiagnosticDescriptor Rule =
            new(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning,
                isEnabledByDefault: true, description: Description,
                helpLinkUri: "https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/tree/master/docs/reference/HLQ005_AvoidSingle.md");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.Analyze | GeneratedCodeAnalysisFlags.ReportDiagnostics);
            context.RegisterSyntaxNodeAction(AnalyzeInvocationExpression, SyntaxKind.InvocationExpression);
        }

        static void AnalyzeInvocationExpression(SyntaxNodeAnalysisContext context)
        {
            if (context.Node is not InvocationExpressionSyntax invocationExpressionSyntax)
                return;

            if (invocationExpressionSyntax.Expression is not MemberAccessExpressionSyntax memberAccessExpressionSyntax)
                return;

            if (memberAccessExpressionSyntax.Expression is null)
                return;

            string methodFound;
            string methodReplace;
            bool mustBeAsync;
            switch (memberAccessExpressionSyntax.Name.Identifier.Text)
            {
                case "Single":
                    methodFound = "Single";
                    methodReplace = "First";
                    mustBeAsync = false;
                    break;
                case "SingleOrDefault":
                    methodFound = "SingleOrDefault";
                    methodReplace = "FirstOrDefault";
                    mustBeAsync = false;
                    break;
                case "SingleAsync":
                    methodFound = "SingleAsync";
                    methodReplace = "FirstAsync";
                    mustBeAsync = true;
                    break;
                case "SingleOrDefaultAsync":
                    methodFound = "SingleOrDefaultAsync";
                    methodReplace = "FirstOrDefaultAsync";
                    mustBeAsync = true;
                    break;
                default:
                    return;
            }

            ExpressionSyntax syntaxNode;
            if (memberAccessExpressionSyntax.Expression is IdentifierNameSyntax) 
            {
                // check if first parameter is enumerable
                var arguments = invocationExpressionSyntax.ArgumentList.Arguments;
                if (arguments.Count == 0)
                    return;

                syntaxNode = arguments[0].Expression;
            }
            else 
            {
                // check if caller expression is enumerable
                syntaxNode = memberAccessExpressionSyntax.Expression;
            }

            var type = context.SemanticModel.GetTypeInfo(syntaxNode).Type;
            if (type is null)
                return;

            if (mustBeAsync)
            {
                if (!type.IsAsyncEnumerable(context.Compilation, out _))
                    return;
            }
            else
            {
                if (!type.IsEnumerable(context.Compilation, out _))
                    return;
            }

            var diagnostic = Diagnostic.Create(Rule, memberAccessExpressionSyntax.Name.GetLocation(), methodFound, methodReplace);
            context.ReportDiagnostic(diagnostic);
        }
    }
}