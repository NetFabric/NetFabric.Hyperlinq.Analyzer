using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.IO;
using System.Linq;
using TestHelper;
using Xunit;

namespace NetFabric.Hyperlinq.Analyzer.UnitTests
{
    public class UseForLoopAnalyzerTests : DiagnosticVerifier
    {
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() =>
            new UseForLoopAnalyzer();

        [Theory]
        [InlineData("TestData/HLQ010/NoDiagnostic/Array.cs")]
        [InlineData("TestData/HLQ010/NoDiagnostic/Span.cs")]
        [InlineData("TestData/HLQ010/NoDiagnostic/ReadOnlySpan.cs")]
        [InlineData("TestData/HLQ010/NoDiagnostic/Dictionary.cs")]
        public void Verify_NoDiagnostics(string path)
        {
            var paths = new[]
            {
                path,
            };
            VerifyCSharpDiagnostic(paths.Select(path => File.ReadAllText(path)).ToArray());
        }

        [Theory]
        [InlineData("TestData/HLQ010/Diagnostic/List.cs", 11, 34)]
        public void Verify_Diagnostic(string path, int line, int column)
        {
            var paths = new[]
            {
                path,
            };
            var sources = paths.Select(path => File.ReadAllText(path)).ToArray();
            var expected = new DiagnosticResult
            {
                Id = "HLQ010",
                Message = "The collection has an indexer. Consider using a 'for' loop instead.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", line, column)
                },
            };

            VerifyCSharpDiagnostic(paths.Select(path => File.ReadAllText(path)).ToArray(), expected);
        }
    }
}
