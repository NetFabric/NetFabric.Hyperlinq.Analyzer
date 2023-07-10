using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.IO;
using System.Linq;
using TestHelper;
using Xunit;

namespace NetFabric.Hyperlinq.Analyzer.UnitTests
{
    public class RemoveOptionalMethodsAnalyzerTests : DiagnosticVerifier
    {
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() =>
            new RemoveOptionalMethodsAnalyzer();

        [Theory]
        [InlineData("TestData/HLQ009/NoDiagnostic/AsyncEnumerable.cs")]
        [InlineData("TestData/HLQ009/NoDiagnostic/Dispose.cs")]
        [InlineData("TestData/HLQ009/NoDiagnostic/DisposeAsync.cs")]
        [InlineData("TestData/HLQ009/NoDiagnostic/Enumerable.cs")]
        [InlineData("TestData/HLQ009/NoDiagnostic/Reset.cs")]
        public void Verify_NoDiagnostics(string path)
        {
            var paths = new[]
            {
                path,
            };
            VerifyCSharpDiagnostic(paths.Select(path => File.ReadAllText(path)).ToArray());
        }

        [Theory]
        [InlineData("TestData/HLQ009/Diagnostic/Dispose.cs", "Dispose", 16, 25)]
        [InlineData("TestData/HLQ009/Diagnostic/DisposeAsync.cs", "DisposeAsync", 18, 30)]
        [InlineData("TestData/HLQ009/Diagnostic/Reset.cs", "Reset", 16, 25)]
        public void Verify_Diagnostic(string path, string name, int line, int column)
        {
            var paths = new[]
            {
                path,
            };
            var sources = paths.Select(path => File.ReadAllText(path)).ToArray();
            var expected = new DiagnosticResult
            {
                Id = "HLQ009",
                Message = $"Consider removing the empty optional enumerator method '{name}'",
                Severity = DiagnosticSeverity.Info,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", line, column)
                },
            };

            VerifyCSharpDiagnostic(paths.Select(path => File.ReadAllText(path)).ToArray(), expected);
        }
    }
}
