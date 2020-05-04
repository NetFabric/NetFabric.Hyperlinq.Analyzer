using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using System.IO;
using System.Linq;
using TestHelper;
using Xunit;

namespace NetFabric.Hyperlinq.Analyzer.UnitTests
{
    public class AvoidSingleAnalyzerTests : CodeFixVerifier
    {
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
            => new AvoidSingleAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider()
            => new AvoidSingleCodeFixProvider();


        [Theory]
        [InlineData("TestData/HLQ005/NoDiagnostic/Single.cs")]
        [InlineData("TestData/HLQ005/NoDiagnostic/SingleAsync.cs")]
        public void Verify_NoDiagnostics(string path)
        {
            var paths = new[]
            {
                path,
                "TestData/TestType.cs",
                "TestData/Enumerable.cs",
                "TestData/AsyncEnumerable.cs",
                "TestData/HLQ005/EnumerableExtensions.cs",
                "TestData/HLQ005/AsyncEnumerableExtensions.cs",
            };
            var sources = paths.Select(path => File.ReadAllText(path)).ToArray();

            VerifyCSharpDiagnostic(sources);
        }

        [Theory]
        [InlineData("TestData/HLQ005/Diagnostic/Single.Extension.cs", "Single", "First", "TestData/HLQ005/Diagnostic/Single.Extension.Fix.cs", 8, 52)]
        [InlineData("TestData/HLQ005/Diagnostic/Single.Method.cs", "Single", "First", "TestData/HLQ005/Diagnostic/Single.Method.Fix.cs", 8, 37)]
        [InlineData("TestData/HLQ005/Diagnostic/Single.Predicate.Extension.cs", "Single", "First", "TestData/HLQ005/Diagnostic/Single.Predicate.Extension.Fix.cs", 8, 52)]
        [InlineData("TestData/HLQ005/Diagnostic/Single.Predicate.Method.cs", "Single", "First", "TestData/HLQ005/Diagnostic/Single.Predicate.Method.Fix.cs", 8, 37)]
        [InlineData("TestData/HLQ005/Diagnostic/SingleAsync.Extension.cs", "SingleAsync", "FirstAsync", "TestData/HLQ005/Diagnostic/SingleAsync.Extension.Fix.cs", 9, 57)]
        [InlineData("TestData/HLQ005/Diagnostic/SingleAsync.Method.cs", "SingleAsync", "FirstAsync", "TestData/HLQ005/Diagnostic/SingleAsync.Method.Fix.cs", 9, 42)]
        [InlineData("TestData/HLQ005/Diagnostic/SingleOrDefault.Extension.cs", "SingleOrDefault", "FirstOrDefault", "TestData/HLQ005/Diagnostic/SingleOrDefault.Extension.Fix.cs", 8, 52)]
        [InlineData("TestData/HLQ005/Diagnostic/SingleOrDefault.Method.cs", "SingleOrDefault", "FirstOrDefault", "TestData/HLQ005/Diagnostic/SingleOrDefault.Method.Fix.cs", 8, 37)]
        [InlineData("TestData/HLQ005/Diagnostic/SingleOrDefaultAsync.Extension.cs", "SingleOrDefaultAsync", "FirstOrDefaultAsync", "TestData/HLQ005/Diagnostic/SingleOrDefaultAsync.Extension.Fix.cs", 9, 57)]
        [InlineData("TestData/HLQ005/Diagnostic/SingleOrDefaultAsync.Method.cs", "SingleOrDefaultAsync", "FirstOrDefaultAsync", "TestData/HLQ005/Diagnostic/SingleOrDefaultAsync.Method.Fix.cs", 9, 42)]
        public void Verify_Diagnostics(string path, string found, string use, string fix, int line, int column)
        {
            var paths = new[]
            {
                path,
                "TestData/TestType.cs",
                "TestData/Enumerable.cs",
                "TestData/AsyncEnumerable.cs",
                "TestData/HLQ005/EnumerableExtensions.cs",
                "TestData/HLQ005/AsyncEnumerableExtensions.cs",
            };
            var sources = paths.Select(path => File.ReadAllText(path)).ToArray();
            var expected = new DiagnosticResult
            {
                Id = "HLQ005",
                Message = $"Avoid the use of '{found}'. Use '{use}' instead.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", line, column)
                },
            };

            VerifyCSharpDiagnostic(sources, expected);

            VerifyCSharpFix(sources, File.ReadAllText(fix));
        }
    }
}