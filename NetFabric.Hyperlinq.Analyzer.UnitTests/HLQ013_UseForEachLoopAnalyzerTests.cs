using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.IO;
using System.Linq;
using TestHelper;
using Xunit;

namespace NetFabric.Hyperlinq.Analyzer.UnitTests
{
    public class UseForEachLoopAnalyzerTests : DiagnosticVerifier
    {
        protected override DiagnosticAnalyzer? GetCSharpDiagnosticAnalyzer() =>
            new UseForEachLoopAnalyzer();

        protected override DiagnosticAnalyzer? GetBasicDiagnosticAnalyzer()
            => null;

        [Theory]
        [InlineData("TestData/HLQ013/NoDiagnostic/List.cs")]
        [InlineData("TestData/HLQ013/NoDiagnostic/Array.cs")]
        public void Verify_NoDiagnostics(string path)
        {
            var paths = new[]
            {
                path,
            };
            VerifyCSharpDiagnostic(paths.Select(path => File.ReadAllText(path)).ToArray());
        }

        [Theory]
        [InlineData("TestData/HLQ013/Diagnostic/Array.cs", 10, 13, "int[]")]
        [InlineData("TestData/HLQ013/Diagnostic/Span.cs", 10, 13, "System.Span<int>")]
        [InlineData("TestData/HLQ013/Diagnostic/ReadOnlySpan.cs", 10, 13, "System.ReadOnlySpan<int>")]
        public void Verify_Diagnostic(string path, int line, int column, string collectionType)
        {
            var paths = new[]
            {
                path,
            };
            var sources = paths.Select(path => File.ReadAllText(path)).ToArray();
            var expected = new DiagnosticResult
            {
                Id = "HLQ013",
                Message = $"Consider using 'foreach' loop instead of 'for' loop for iterating over {collectionType}",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", line, column)
                },
            };

            VerifyCSharpDiagnostic(paths.Select(path => File.ReadAllText(path)).ToArray(), expected);
        }
    }
}
