using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using System.IO;
using System.Linq;
using TestHelper;
using Xunit;

namespace NetFabric.Hyperlinq.Analyzer.UnitTests
{
    public class UseCollectionsMarshalAsSpanAnalyzerTests : CodeFixVerifier
    {
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() =>
            new UseCollectionsMarshalAsSpanAnalyzer();

        protected override DiagnosticAnalyzer? GetBasicDiagnosticAnalyzer()
            => null;

        protected override CodeFixProvider? GetCSharpCodeFixProvider()
            => new UseCollectionsMarshalAsSpanCodeFixProvider();

        protected override CodeFixProvider? GetBasicCodeFixProvider()
            => null;

        [Theory]
        [InlineData("TestData/HLQ012/NoDiagnostic/MarshalCollectionAsSpan.cs")]
        public void Verify_NoDiagnostics(string path)
        {
            var paths = new[]
            {
                path,
            };
            VerifyCSharpDiagnostic(paths.Select(path => File.ReadAllText(path)).ToArray());
        }

        [Theory]
        [InlineData("TestData/HLQ012/Diagnostic/List.cs", "int", "TestData/HLQ012/Diagnostic/List.Fix.cs", 11, 34)]
        public void Verify_Diagnostic(string path, string type, string fix, int line, int column)
        {
            var paths = new[]
            {
                path,
            };
            var sources = paths.Select(path => File.ReadAllText(path)).ToArray();
            var expected = new DiagnosticResult
            {
                Id = "HLQ012",
                Message = $"Consider using CollectionsMarshal.AsSpan() instead of foreach with List<{type}>",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", line, column)
                },
            };

            VerifyCSharpDiagnostic(paths.Select(path => File.ReadAllText(path)).ToArray(), expected);

            VerifyCSharpFix(sources, File.ReadAllText(fix));
        }
    }
}
