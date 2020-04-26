using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.IO;
using System.Linq;
using TestHelper;
using Xunit;

namespace NetFabric.Hyperlinq.Analyzer.UnitTests
{
    public class NullEnumerableAnalyzerTests : DiagnosticVerifier
    {
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() =>
            new NullEnumerableAnalyzer();


        [Theory]
        [InlineData("TestData/HLQ002/NoDiagnostic/Enumerable.cs")]
        [InlineData("TestData/HLQ002/NoDiagnostic/Enumerable.Async.cs")]
        public void Verify_NoDiagnostics(string path)
        {
            var paths = new[]
            {
                path,
            };
            VerifyCSharpDiagnostic(paths.Select(path => File.ReadAllText(path)).ToArray());
        }

        [Theory]
        [InlineData("TestData/HLQ002/Diagnostic/ArrowExpression/Enumerable.cs", 10, 16)]
        [InlineData("TestData/HLQ002/Diagnostic/ArrowExpression/Enumerable.Async.cs", 11, 16)]
        [InlineData("TestData/HLQ002/Diagnostic/LiteralExpression/Enumerable.cs", 11, 13)]
        [InlineData("TestData/HLQ002/Diagnostic/LiteralExpression/Enumerable.Async.cs", 12, 13)]
        public void Verify_Diagnostics(string path, int line, int column)
        {
            var paths = new[]
            {
                path,
            };
            var expected = new DiagnosticResult
            {
                Id = "HLQ002",
                Message = "Enumerable cannot be null. Return an empty enumerable instead.",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", line, column)
                },
            };

            VerifyCSharpDiagnostic(paths.Select(path => File.ReadAllText(path)).ToArray(), expected);
        }
    }
}