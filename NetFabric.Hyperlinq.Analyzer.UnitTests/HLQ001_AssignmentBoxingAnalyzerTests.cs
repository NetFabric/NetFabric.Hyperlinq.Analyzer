using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.IO;
using System.Linq;
using TestHelper;
using Xunit;

namespace NetFabric.Hyperlinq.Analyzer.UnitTests
{
    public class AssignmentBoxingTests : DiagnosticVerifier
    {
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() =>
            new AssignmentBoxingAnalyzer();

        [Theory]
        [InlineData("TestData/HLQ001/NoDiagnostic/FieldDeclaration.cs")]
        [InlineData("TestData/HLQ001/NoDiagnostic/FieldDeclaration.Async.cs")]
        [InlineData("TestData/HLQ001/NoDiagnostic/PropertyDeclaration.cs")]
        [InlineData("TestData/HLQ001/NoDiagnostic/PropertyDeclaration.Async.cs")]
        [InlineData("TestData/HLQ001/NoDiagnostic/VariableDeclaration.cs")]
        [InlineData("TestData/HLQ001/NoDiagnostic/VariableDeclaration.Async.cs")]
        public void Verify_NoDiagnostics(string path)
        {
            var paths = new[]
            {
                path,
                "TestData/TestType.cs",
                "TestData/Enumerables.cs",
            };
            VerifyCSharpDiagnostic(paths.Select(path => File.ReadAllText(path)).ToArray());
        }

        [Theory]
        [InlineData("TestData/HLQ001/Diagnostic/EqualsValueClause/FieldDeclaration.cs", "OptimizedEnumerable`1", "IEnumerable`1", 8, 9)]
        [InlineData("TestData/HLQ001/Diagnostic/EqualsValueClause/FieldDeclaration.Async.cs", "OptimizedAsyncEnumerable`1", "IAsyncEnumerable`1", 8, 9)]
        [InlineData("TestData/HLQ001/Diagnostic/EqualsValueClause/PropertyDeclaration.cs", "OptimizedEnumerable`1", "IEnumerable`1", 8, 9)]
        [InlineData("TestData/HLQ001/Diagnostic/EqualsValueClause/PropertyDeclaration.Async.cs", "OptimizedAsyncEnumerable`1", "IAsyncEnumerable`1", 8, 9)]
        [InlineData("TestData/HLQ001/Diagnostic/EqualsValueClause/VariableDeclaration.cs", "OptimizedEnumerable`1", "IEnumerable`1", 10, 13)]
        [InlineData("TestData/HLQ001/Diagnostic/EqualsValueClause/VariableDeclaration.Async.cs", "OptimizedAsyncEnumerable`1", "IAsyncEnumerable`1", 11, 13)]
        [InlineData("TestData/HLQ001/Diagnostic/SimpleAssignment/FieldDeclaration.cs", "OptimizedEnumerable`1", "IEnumerable`1", 12, 13)]
        [InlineData("TestData/HLQ001/Diagnostic/SimpleAssignment/FieldDeclaration.Async.cs", "OptimizedAsyncEnumerable`1", "IAsyncEnumerable`1", 12, 13)]
        [InlineData("TestData/HLQ001/Diagnostic/SimpleAssignment/PropertyDeclaration.cs", "OptimizedEnumerable`1", "IEnumerable`1", 12, 13)]
        [InlineData("TestData/HLQ001/Diagnostic/SimpleAssignment/PropertyDeclaration.Async.cs", "OptimizedAsyncEnumerable`1", "IAsyncEnumerable`1", 12, 13)]
        [InlineData("TestData/HLQ001/Diagnostic/SimpleAssignment/VariableDeclaration.cs", "OptimizedEnumerable`1", "IEnumerable`1", 12, 13)]
        [InlineData("TestData/HLQ001/Diagnostic/SimpleAssignment/VariableDeclaration.Async.cs", "OptimizedAsyncEnumerable`1", "IAsyncEnumerable`1", 13, 13)]
        public void Verify_Diagnostics(string path, string type, string @interface, int line, int column)
        {
            var paths = new[]
            {
                path,
                "TestData/TestType.cs",
                "TestData/Enumerables.cs",
            };
            var expected = new DiagnosticResult
            {
                Id = "HLQ001",
                Message = $"'{type}' has a value type enumerator. Assigning it to '{@interface}' causes boxing of the enumerator.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", line, column)
                },
            };

            VerifyCSharpDiagnostic(paths.Select(path => File.ReadAllText(path)).ToArray(), expected);
        }
    }
}