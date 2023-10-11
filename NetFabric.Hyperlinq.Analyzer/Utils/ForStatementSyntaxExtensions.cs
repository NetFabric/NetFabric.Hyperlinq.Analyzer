using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace NetFabric.Hyperlinq.Analyzer
{
    static class ForStatementSyntaxExtensions
    {
        public static bool IsIncrementalStep(this ForStatementSyntax forStatement, SyntaxNodeAnalysisContext context, [NotNullWhen(true)] out string? identifier)
        {
            identifier = default;

            var incrementors = forStatement.Incrementors;
            if (incrementors.Count != 1)
                return false;

            // check if incrementor is incrementing the variable by one
            var incrementor = incrementors[0];
            if (!incrementor.IsIncrementAssignment(out identifier))
                return false;

            var declaration = forStatement.Declaration;
            if (declaration is null || declaration.Variables.Count != 1)
                return false;

            // check if the variable is the same used in the incrementor
            if (identifier != declaration.Variables[0].Identifier.ToString())
                return false;

            // check if declaration declares a variable of type int
            var variableType = context.SemanticModel.GetTypeInfo(declaration.Type).Type;
            if (variableType is null || variableType.SpecialType != SpecialType.System_Int32)
                return false;

            return true;
        }
    }
}
