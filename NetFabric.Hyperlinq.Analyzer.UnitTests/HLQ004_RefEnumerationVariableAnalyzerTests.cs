using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using System.IO;
using System.Linq;
using TestHelper;
using Xunit;

namespace NetFabric.Hyperlinq.Analyzer.UnitTests
{
    public class RefEnumerationVariableAnalyzerTests : CodeFixVerifier
    {
        protected override DiagnosticAnalyzer? GetCSharpDiagnosticAnalyzer() 
            => new RefEnumerationVariableAnalyzer();

        protected override DiagnosticAnalyzer? GetBasicDiagnosticAnalyzer()
            => null;

        protected override CodeFixProvider? GetCSharpCodeFixProvider()
            => new RefEnumerationVariableCodeFixProvider();

        protected override CodeFixProvider? GetBasicCodeFixProvider()
            => null;

        [Theory]
        [InlineData("TestData/HLQ004/NoDiagnostic/NoRef.cs")]
        [InlineData("TestData/HLQ004/NoDiagnostic/Ref.cs")]
        [InlineData("TestData/HLQ004/NoDiagnostic/RefReadOnly.cs")]
        [InlineData("TestData/HLQ004/NoDiagnostic/Yield.cs")]
        public void Verify_NoDiagnostics(string path)
        {
            var paths = new[]
            {
                path,
                "TestData/TestType.cs",
                "TestData/Enumerable.cs",
                "TestData/HLQ004/RefEnumerables.cs",
            };
            var sources = paths.Select(path => File.ReadAllText(path)).ToArray();

            VerifyCSharpDiagnostic(sources);
        }

        [Theory]
        [InlineData("TestData/HLQ004/Diagnostic/Ref.cs", "ref", "TestData/HLQ004/Diagnostic/Ref.Fix.cs", 7, 22)]
        [InlineData("TestData/HLQ004/Diagnostic/RefReadOnly.cs", "ref readonly", "TestData/HLQ004/Diagnostic/RefReadOnly.Fix.cs", 9, 22)]
        public void Verify_Diagnostics(string path, string message, string fix, int line, int column)
        {
            var paths = new[]
            {
                path,
                "TestData/TestType.cs",
                "TestData/Enumerable.cs",
                "TestData/HLQ004/RefEnumerables.cs",
            };
            var sources = paths.Select(path => File.ReadAllText(path)).ToArray();
            var expected = new DiagnosticResult
            {
                Id = "HLQ004",
                Message = $"The enumerator returns a reference to the item. Add '{message}' to the item type.",
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