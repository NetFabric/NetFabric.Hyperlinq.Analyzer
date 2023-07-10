using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.IO;
using System.Linq;
using TestHelper;
using Xunit;

namespace NetFabric.Hyperlinq.Analyzer.UnitTests
{
    public class HighestLevelInterfaceAnalyzerTests : DiagnosticVerifier
    {
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() =>
            new HighestLevelInterfaceAnalyzer();


        [Theory]
        [InlineData("TestData/HLQ003/NoDiagnostic/ArrowExpression/Enumerable.cs")]
        [InlineData("TestData/HLQ003/NoDiagnostic/ArrowExpression/ReadOnlyCollection.cs")]
        [InlineData("TestData/HLQ003/NoDiagnostic/ArrowExpression/ReadOnlyList.cs")]
        [InlineData("TestData/HLQ003/NoDiagnostic/MethodDeclaration/Enumerable.cs")]
        [InlineData("TestData/HLQ003/NoDiagnostic/MethodDeclaration/ReadOnlyCollection.cs")]
        [InlineData("TestData/HLQ003/NoDiagnostic/MethodDeclaration/ReadOnlyList.cs")]
        [InlineData("TestData/HLQ003/NoDiagnostic/MethodDeclaration/YieldReturn.cs")]
        public void Verify_NoDiagnostics(string path)
        {
            var paths = new[]
            {
                path,
                "TestData/TestType.cs",
                "TestData/Enumerable.cs",
                "TestData/ReadOnlyCollection.cs",
                "TestData/ReadOnlyList.cs",
                "TestData/AsyncEnumerable.cs",
            };
            VerifyCSharpDiagnostic(paths.Select(path => File.ReadAllText(path)).ToArray());
        }

        [Theory]
        [InlineData("TestData/HLQ003/Diagnostic/ArrowExpression/ReadOnlyCollection/Enumerable.cs", "IReadOnlyCollection`1", 9, 16)]
        [InlineData("TestData/HLQ003/Diagnostic/ArrowExpression/ReadOnlyList/Enumerable.cs", "IReadOnlyList`1", 9, 16)]
        [InlineData("TestData/HLQ003/Diagnostic/ArrowExpression/ReadOnlyList/ReadOnlyCollection.cs", "IReadOnlyList`1", 9, 16)]
        [InlineData("TestData/HLQ003/Diagnostic/MethodDeclaration/ReadOnlyCollection/Enumerable.cs", "IReadOnlyCollection`1", 9, 16)]
        [InlineData("TestData/HLQ003/Diagnostic/MethodDeclaration/ReadOnlyList/Enumerable.cs", "IReadOnlyList`1", 9, 16)]
        [InlineData("TestData/HLQ003/Diagnostic/MethodDeclaration/ReadOnlyList/ReadOnlyCollection.cs", "IReadOnlyList`1", 9, 16)]
        public void Verify_Diagnostics(string path, string @interface, int line, int column)
        {
            var paths = new[]
            {
                path,
                "TestData/TestType.cs",
                "TestData/Enumerable.cs",
                "TestData/ReadOnlyCollection.cs",
                "TestData/ReadOnlyList.cs",
                "TestData/AsyncEnumerable.cs",
            };
            var expected = new DiagnosticResult
            {
                Id = "HLQ003",
                Message = $"Consider returning '{@interface}' instead",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", line, column)
                },
            };

            VerifyCSharpDiagnostic(paths.Select(path => File.ReadAllText(path)).ToArray(), expected);
        }
    }
}