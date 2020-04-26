using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.IO;
using System.Linq;
using TestHelper;
using Xunit;

namespace NetFabric.Hyperlinq.Analyzer.UnitTests
{
    public class NonDisposableEnumeratorAnalyzerTests : DiagnosticVerifier
    {
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() =>
            new NonDisposableEnumeratorAnalyzer();

        [Theory]
        [InlineData("TestData/HLQ007/NoDiagnostic/Enumerable.cs")]
        [InlineData("TestData/HLQ007/NoDiagnostic/IEnumerable.cs")]
        public void Verify_NoDiagnostics(string path)
        {
            var paths = new[]
            {
                path,
                "TestData/TestType.cs",
            };
            VerifyCSharpDiagnostic(paths.Select(path => File.ReadAllText(path)).ToArray());
        }

        [Theory]
        [InlineData("TestData/HLQ007/Diagnostic/Enumerable.cs", "Enumerator", 8, 16)]
        [InlineData("TestData/HLQ007/Diagnostic/IEnumerable.cs", "Enumerator", 10, 16)]
        public void Verify_Diagnostic(string path, string enumeratorName, int line, int column)
        {
            var paths = new[]
            {
                path,
                "TestData/TestType.cs",
            };
            var expected = new DiagnosticResult
            {
                Id = "HLQ007",
                Message = $"'{enumeratorName}' has an empty Dispose(). Consider returning a non-disposable enumerator.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", line, column)
                },
            };

            VerifyCSharpDiagnostic(paths.Select(path => File.ReadAllText(path)).ToArray(), expected);
        }
    }
}
