using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using System.IO;
using System.Linq;
using TestHelper;
using Xunit;

namespace NetFabric.Hyperlinq.Analyzer.UnitTests
{
    public class ReadOnlyRefEnumerableAnalyzerTests : CodeFixVerifier
    {
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() =>
            new ReadOnlyRefEnumerableAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider()
            => new ReadOnlyRefEnumerableCodeFixProvider();

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
        [InlineData("TestData/HLQ008/Diagnostic/ValueTypeEnumerable.cs", "ValueTypeEnumerable", "TestData/HLQ008/Diagnostic/ValueTypeEnumerable.Fix.cs", 5, 5)]
        [InlineData("TestData/HLQ008/Diagnostic/RefValueTypeEnumerable.cs", "RefValueTypeEnumerable", "TestData/HLQ008/Diagnostic/RefValueTypeEnumerable.Fix.cs", 5, 9)]
        public void Verify_Diagnostic(string path, string name, string fix, int line, int column)
        {
            var paths = new[]
            {
                path,
            };
            var sources = paths.Select(path => File.ReadAllText(path)).ToArray();
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

            VerifyCSharpFix(sources, File.ReadAllText(fix));
        }
    }
}
