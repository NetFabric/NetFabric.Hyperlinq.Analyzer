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
        protected override DiagnosticAnalyzer? GetCSharpDiagnosticAnalyzer() =>
            new NonDisposableEnumeratorAnalyzer();

        protected override DiagnosticAnalyzer? GetBasicDiagnosticAnalyzer()
            => null;

        [Theory]
        [InlineData("TestData/HLQ007/NoDiagnostic/IAsyncEnumerable.Arrow.cs")]
        [InlineData("TestData/HLQ007/NoDiagnostic/IAsyncEnumerable.Block.cs")]
        [InlineData("TestData/HLQ007/NoDiagnostic/IAsyncEnumerable.Empty.cs")]
        [InlineData("TestData/HLQ007/NoDiagnostic/IEnumerable.Arrow.cs")]
        [InlineData("TestData/HLQ007/NoDiagnostic/IEnumerable.Block.cs")]
        [InlineData("TestData/HLQ007/NoDiagnostic/IEnumerable.Empty.cs")]
        public void Verify_NoDiagnostics(string path)
        {
            var paths = new[]
            {
                path,
                "TestData/TestType.cs",
                "TestData/HLQ007/Disposable.cs",
            };
            VerifyCSharpDiagnostic(paths.Select(path => File.ReadAllText(path)).ToArray());
        }

        [Theory]
        [InlineData("TestData/HLQ007/Diagnostic/IAsyncEnumerable.Arrow.cs", "Enumerator", 11, 16)]
        [InlineData("TestData/HLQ007/Diagnostic/IAsyncEnumerable.Block.cs", "Enumerator", 13, 16)]
        [InlineData("TestData/HLQ007/Diagnostic/IEnumerable.cs", "Enumerator", 10, 16)]
        public void Verify_Diagnostic(string path, string enumeratorName, int line, int column)
        {
            var paths = new[]
            {
                path,
                "TestData/TestType.cs",
                "TestData/HLQ007/Disposable.cs",
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
