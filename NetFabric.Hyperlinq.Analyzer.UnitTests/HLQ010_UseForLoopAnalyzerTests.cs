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
        protected override DiagnosticAnalyzer? GetCSharpDiagnosticAnalyzer() =>
            new UseForLoopAnalyzer();

        protected override DiagnosticAnalyzer? GetBasicDiagnosticAnalyzer()
            => null;

        [Theory]
        [InlineData("TestData/HLQ010/NoDiagnostic/Array.cs")]
        [InlineData("TestData/HLQ010/NoDiagnostic/Span.cs")]
        [InlineData("TestData/HLQ010/NoDiagnostic/List.cs")]
        [InlineData("TestData/HLQ010/NoDiagnostic/ReadOnlySpan.cs")]
        [InlineData("TestData/HLQ010/NoDiagnostic/Dictionary.cs")]
        [InlineData("TestData/HLQ010/NoDiagnostic/ImmutableArray.cs")]
        public void Verify_NoDiagnostics(string path)
        {
            var paths = new[]
            {
                path,
            };
            VerifyCSharpDiagnostic(paths.Select(path => File.ReadAllText(path)).ToArray());
        }

        [Theory]
        [InlineData("TestData/HLQ010/Diagnostic/ArraySegment.cs", 10, 34)]
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
                Message = "Consider using 'for' instead of 'foreach' for iterating over collections featuring an indexer",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", line, column)
                },
            };

            VerifyCSharpDiagnostic(paths.Select(path => File.ReadAllText(path)).ToArray(), expected);
        }
    }
}
