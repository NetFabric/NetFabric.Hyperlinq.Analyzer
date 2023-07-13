using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.IO;
using System.Linq;
using TestHelper;
using Xunit;

namespace NetFabric.Hyperlinq.Analyzer.UnitTests
{
    public class GetEnumeratorReturnTypeAnalyzerTests : DiagnosticVerifier
    {
        protected override DiagnosticAnalyzer? GetCSharpDiagnosticAnalyzer() =>
            new GetEnumeratorReturnTypeAnalyzer();

        protected override DiagnosticAnalyzer? GetBasicDiagnosticAnalyzer()
            => null;

        [Theory]
        [InlineData("TestData/HLQ006/NoDiagnostic/AsyncEnumerable.cs")]
        [InlineData("TestData/HLQ006/NoDiagnostic/Enumerable.cs")]
        [InlineData("TestData/HLQ006/NoDiagnostic/NoInterfaceAsyncEnumerable.cs")]
        [InlineData("TestData/HLQ006/NoDiagnostic/NoInterfaceEnumerable.cs")]
        public void Verify_NoDiagnostics(string path)
        {
            var paths = new[]
            {
                path,
            };
            VerifyCSharpDiagnostic(paths.Select(path => File.ReadAllText(path)).ToArray());
        }

        [Theory]
        [InlineData("TestData/HLQ006/Diagnostic/AsyncEnumerable.cs", "GetAsyncEnumerator", 10, 16)]
        [InlineData("TestData/HLQ006/Diagnostic/Enumerable.cs", "GetEnumerator", 9, 16)]
        [InlineData("TestData/HLQ006/Diagnostic/NoInterfaceAsyncEnumerable.cs", "GetAsyncEnumerator", 9, 16)]
        [InlineData("TestData/HLQ006/Diagnostic/NoInterfaceEnumerable.cs", "GetEnumerator", 7, 16)]
        public void Verify_Diagnostic(string path, string name, int line, int column)
        {
            var paths = new[]
            {
                path,
            };
            var expected = new DiagnosticResult
            {
                Id = "HLQ006",
                Message = $"'{name}' returns a reference type. Consider returning a value type.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", line, column)
                },
            };

            VerifyCSharpDiagnostic(paths.Select(path => File.ReadAllText(path)).ToArray(), expected);
        }
    }
}
