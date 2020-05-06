using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.IO;
using System.Linq;
using TestHelper;
using Xunit;

namespace NetFabric.Hyperlinq.Analyzer.UnitTests
{
    public class ReadOnlyRefEnumerableAnalyzerTests : DiagnosticVerifier
    {
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() =>
            new ReadOnlyRefEnumerableAnalyzer();

        [Theory]
        [InlineData("TestData/HLQ008/NoDiagnostic/ReadOnlyValueTypeEnumerable.cs")]
        [InlineData("TestData/HLQ008/NoDiagnostic/ReferenceTypeEnumerable.cs")]
        [InlineData("TestData/HLQ008/NoDiagnostic/RefValueTypeEnumerable.cs")]
        public void Verify_NoDiagnostics(string path)
        {
            var paths = new[]
            {
                path,
            };
            VerifyCSharpDiagnostic(paths.Select(path => File.ReadAllText(path)).ToArray());
        }

        [Theory]
        [InlineData("TestData/HLQ008/Diagnostic/ValueTypeEnumerable.cs", "ValueTypeEnumerable", 5, 5)]
        public void Verify_Diagnostic(string path, string name, int line, int column)
        {
            var paths = new[]
            {
                path,
            };
            var expected = new DiagnosticResult
            {
                Id = "HLQ008",
                Message = $"'{name}' is a value type. Consider making it 'readonly'.",
                Severity = DiagnosticSeverity.Info,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", line, column)
                },
            };

            VerifyCSharpDiagnostic(paths.Select(path => File.ReadAllText(path)).ToArray(), expected);
        }
    }
}
