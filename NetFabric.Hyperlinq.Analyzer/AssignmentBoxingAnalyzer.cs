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
    public sealed class AssignmentBoxingAnalyzer : DiagnosticAnalyzer
    {
        const string DiagnosticId = DiagnosticIds.AssignmentBoxingId;

        static readonly LocalizableString Title =
            new LocalizableResourceString(nameof(Resources.AssignmentBoxing_Title), Resources.ResourceManager, typeof(Resources));
        static readonly LocalizableString MessageFormat =
            new LocalizableResourceString(nameof(Resources.AssignmentBoxing_MessageFormat), Resources.ResourceManager, typeof(Resources));
        static readonly LocalizableString Description =
            new LocalizableResourceString(nameof(Resources.AssignmentBoxing_Description), Resources.ResourceManager, typeof(Resources));
        const string Category = "Performance";

        static readonly DiagnosticDescriptor rule =
            new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, 
                isEnabledByDefault: true, description: Description,
                helpLinkUri: "https://github.com/NetFabric/NetFabric.Hyperlinq.Analyzer/tree/master/docs/reference/HLQ001_AssignmentBoxing.md");

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
            ImmutableArray.Create(rule);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxNodeAction(AnalyzeSimpleAssignment, SyntaxKind.SimpleAssignmentExpression);
            context.RegisterSyntaxNodeAction(AnalyzeEqualsValueClause, SyntaxKind.EqualsValueClause);
        }

        static void AnalyzeSimpleAssignment(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is AssignmentExpressionSyntax assignmentExpression))
                return;

            var semanticModel = context.SemanticModel;

            var rightTypeSymbol = semanticModel.GetTypeInfo(assignmentExpression.Right).Type;
            if (!GetsValueTypeEnumerator(rightTypeSymbol, context.Compilation))
                return;

            var leftSymbol = semanticModel.GetSymbolInfo(assignmentExpression.Left).Symbol;
            if (leftSymbol is null)
                return;

            switch (leftSymbol)
            {
                case IFieldSymbol fieldSymbol:
                    if (fieldSymbol.DeclaredAccessibility == Accessibility.Public)
                        return;
                    break;
                case IPropertySymbol propertySymbol:
                    if (propertySymbol.DeclaredAccessibility == Accessibility.Public)
                        return;
                    break;
            }

            var leftTypeSymbol = semanticModel.GetTypeInfo(assignmentExpression.Left).Type;
            if (!GetsReferenceTypeEnumerator(leftTypeSymbol, context.Compilation))
                return;

            var diagnostic = Diagnostic.Create(rule, assignmentExpression.Left.GetLocation(), rightTypeSymbol.Name, leftTypeSymbol.Name);
            context.ReportDiagnostic(diagnostic);
        }

        static void AnalyzeEqualsValueClause(SyntaxNodeAnalysisContext context)
        {
            if (!(context.Node is EqualsValueClauseSyntax equalsValueClauseSyntax))
                return;

            var semanticModel = context.SemanticModel;

            var typeSymbol = semanticModel.GetTypeInfo(equalsValueClauseSyntax.Value).Type; 
            if (!GetsValueTypeEnumerator(typeSymbol, context.Compilation))
                return;

            if (equalsValueClauseSyntax.Parent is PropertyDeclarationSyntax propertyDeclarationSyntax)
            {
                AnalyzePropertyDeclaration(context, propertyDeclarationSyntax, typeSymbol);
                return;
            }

            if (!(equalsValueClauseSyntax.Parent is VariableDeclaratorSyntax variableDeclaratorSyntax))
                return;

            if (!(variableDeclaratorSyntax.Parent is VariableDeclarationSyntax variableDeclarationSyntax))
                return;

            switch (variableDeclarationSyntax.Parent)
            {
                case LocalDeclarationStatementSyntax localDeclarationStatementSyntax:
                    AnalyzeLocalDeclaration(context, localDeclarationStatementSyntax, typeSymbol);
                    break;

                case FieldDeclarationSyntax fieldDeclarationSyntax:
                    AnalyzeFieldDeclaration(context, fieldDeclarationSyntax, typeSymbol);
                    break;
            }
        }

        static void AnalyzePropertyDeclaration(SyntaxNodeAnalysisContext context, PropertyDeclarationSyntax propertyDeclarationSyntax, ITypeSymbol enumerableTypeSymbol)
        {
            if (propertyDeclarationSyntax.Modifiers.Any(token => token.Text == "public"))
                return;

            var semanticModel = context.SemanticModel;

            var typeSymbol = semanticModel.GetDeclaredSymbol(propertyDeclarationSyntax).Type;
            if (!GetsReferenceTypeEnumerator(typeSymbol, context.Compilation))
                return;

            var diagnostic = Diagnostic.Create(rule, propertyDeclarationSyntax.GetLocation(), enumerableTypeSymbol.Name, typeSymbol.Name);
            context.ReportDiagnostic(diagnostic);
        }

        static void AnalyzeLocalDeclaration(SyntaxNodeAnalysisContext context, LocalDeclarationStatementSyntax localDeclarationStatementSyntax, ITypeSymbol enumerableTypeSymbol)
        {
            var typeSyntax = localDeclarationStatementSyntax.Declaration.Type;
            var semanticModel = context.SemanticModel;
            var typeSymbol = semanticModel.GetTypeInfo(typeSyntax).Type;
            if (!GetsReferenceTypeEnumerator(typeSymbol, context.Compilation))
                return;

            var diagnostic = Diagnostic.Create(rule, typeSyntax.GetLocation(), enumerableTypeSymbol.Name, typeSymbol.Name);
            context.ReportDiagnostic(diagnostic);
        }

        static void AnalyzeFieldDeclaration(SyntaxNodeAnalysisContext context, FieldDeclarationSyntax fieldDeclarationSyntax, ITypeSymbol enumerableTypeSymbol)
        {
            if (fieldDeclarationSyntax.Modifiers.Any(token => token.Text == "public"))
                return;

            var typeSyntax = fieldDeclarationSyntax.Declaration.Type;
            var semanticModel = context.SemanticModel;
            var typeSymbol = semanticModel.GetTypeInfo(typeSyntax).Type;
            if (!GetsReferenceTypeEnumerator(typeSymbol, context.Compilation))
                return;

            var diagnostic = Diagnostic.Create(rule, typeSyntax.GetLocation(), enumerableTypeSymbol.Name, typeSymbol.Name);
            context.ReportDiagnostic(diagnostic);
        }

        static bool GetsValueTypeEnumerator(ITypeSymbol typeSymbol, Compilation compilation)
            => typeSymbol is object &&
                typeSymbol.IsEnumerable(compilation, out var enumerableSymbols) &&
                enumerableSymbols.GetEnumerator.ReturnType.IsValueType;

        static bool GetsReferenceTypeEnumerator(ITypeSymbol typeSymbol, Compilation compilation)
            => typeSymbol is object &&
                typeSymbol.IsEnumerable(compilation, out var enumerableSymbols) &&
                enumerableSymbols.GetEnumerator.ReturnType.IsReferenceType;
    }
}