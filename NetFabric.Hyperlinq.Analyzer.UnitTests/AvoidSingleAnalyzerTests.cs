using System;
using Xunit;
using TestHelper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NetFabric.Hyperlinq.Analyzer.UnitTests
{
    public class AvoidSingleAnalyzerTests : DiagnosticVerifier
    {
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() =>
            new AvoidSingleAnalyzer();

        [Fact]
        public void Verify()
        {
            var test = @"
using System.Collections.Generic;
using System.Linq;

static class MyEnumerable
{
    public static int Single(this IEnumerable<int> source)
        => 0;
}

static class MyNotEnumerable
{
    public static int Single(this int source)
        => source;
}

class C
{
    public int Single(int source)
        => source;

    void Method()
    {
        IEnumerable<int> localVariable = new int[];

        var a = localVariable.Single();
        var b = localVariable.Single(_ => true);
        var c = Enumerable.Single(localVariable);
        var d = Enumerable.Single(localVariable, _ => true);
        var e = Single(0);
        var f = MyEnumerable.Single(localVariable);
        var g = MyNotEnumerable.Single(0);

        var aa = localVariable.SingleOrDefault();
        var ab = localVariable.SingleOrDefault(_ => true);
    }
}";

            var a = new DiagnosticResult
            {
                Id = "HLQ005",
                Message = "Avoid 'Single()'. Use 'First()' instead.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 26, 31)
                },
            };

            var b = new DiagnosticResult
            {
                Id = "HLQ005",
                Message = "Avoid 'Single()'. Use 'First()' instead.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 27, 31)
                },
            };

            var c = new DiagnosticResult
            {
                Id = "HLQ005",
                Message = "Avoid 'Single()'. Use 'First()' instead.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 28, 28)
                },
            };

            var d = new DiagnosticResult
            {
                Id = "HLQ005",
                Message = "Avoid 'Single()'. Use 'First()' instead.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 29, 28)
                },
            };

            var f = new DiagnosticResult
            {
                Id = "HLQ005",
                Message = "Avoid 'Single()'. Use 'First()' instead.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 31, 30)
                },
            };

            var aa = new DiagnosticResult
            {
                Id = "HLQ005",
                Message = "Avoid 'SingleOrDefault()'. Use 'FirstOrDefault()' instead.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 34, 32)
                },
            };

            var ab = new DiagnosticResult
            {
                Id = "HLQ005",
                Message = "Avoid 'SingleOrDefault()'. Use 'FirstOrDefault()' instead.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 35, 32)
                },
            };

            VerifyCSharpDiagnostic(test, a, b, c, d, f, aa, ab);
        }

    }
}