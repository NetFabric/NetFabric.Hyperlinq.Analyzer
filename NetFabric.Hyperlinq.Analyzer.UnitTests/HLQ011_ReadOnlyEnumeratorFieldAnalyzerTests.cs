using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.IO;
using System.Linq;
using TestHelper;
using Xunit;

namespace NetFabric.Hyperlinq.Analyzer.UnitTests
{
    public class ReadOnlyEnumeratorFieldAnalyzerTests : DiagnosticVerifier
    {
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() =>
            new ReadOnlyEnumeratorFieldAnalyzer();

        [Theory]
        [InlineData("TestData/HLQ011/NoDiagnostic/Explicit.cs")]
        [InlineData("TestData/HLQ011/NoDiagnostic/ExplicitReadOnlyStruct.cs")]
        [InlineData("TestData/HLQ011/NoDiagnostic/Generic.cs")]
        public void Verify_NoDiagnostics(string path)
        {
            var paths = new[]
            {
                path,
                "TestData/Enumerable.cs",
                "TestData/AsyncEnumerable.cs",
            };
            VerifyCSharpDiagnostic(paths.Select(path => File.ReadAllText(path)).ToArray());
        }

        [Theory]
        [InlineData("TestData/HLQ011/Diagnostic/Explicit.cs", "OptimizedEnumerable<T>.Enumerator", 9, 18)]
        [InlineData("TestData/HLQ011/Diagnostic/Generic.cs", "TEnumerator", 9, 18)]
        public void Verify_Diagnostic(string path, string name, int line, int column)
        {
            var paths = new[]
            {
                path,
                "TestData/Enumerable.cs",
                "TestData/AsyncEnumerable.cs",
            };
            var sources = paths.Select(path => File.ReadAllText(path)).ToArray();
            var expected = new DiagnosticResult
            {
                Id = "HLQ011",
                Message = $"'{name}' is a mutable value-type enumerator. It cannot be stored in a 'readonly' field.",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", line, column)
                },
            };

            VerifyCSharpDiagnostic(paths.Select(path => File.ReadAllText(path)).ToArray(), expected);
        }
    }
}
