using System;
using Xunit;
using TestHelper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CodeFixes;
using System.IO;

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
            VerifyCSharpDiagnostic(File.ReadAllText(path));
        }

        [Theory]
        [InlineData("TestData/HLQ007/Diagnostic/Enumerable.cs", "Enumerator", 7, 16)]
        [InlineData("TestData/HLQ007/Diagnostic/IEnumerable.cs", "Enumerator", 11, 16)]
        public void Verify_Diagnostic(string path, string enumeratorName, int line, int column)
        {
            var expected = new DiagnosticResult
            {
                Id = "HLQ007",
                Message = $"'{enumeratorName}' has an empty Dispose(). Consider returning a non-disposable enumerator.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", line, column)
                },
            };

            VerifyCSharpDiagnostic(File.ReadAllText(path), expected);
        }
    }
}
