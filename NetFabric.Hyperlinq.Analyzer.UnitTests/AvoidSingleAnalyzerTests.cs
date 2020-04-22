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
using System.Threading.Tasks;
using System.Linq;

class C
{
    public int Single(int source)
        => source;

    async void Method()
    {
        IEnumerable<int> localVariable = null;
        IAsyncEnumerable<int> localAsynVariable = null;

        var a = localVariable.Single();
        var b = localVariable.Single(_ => true);
        var c = Enumerable.Single(localVariable);
        var d = Enumerable.Single(localVariable, _ => true);
        var e = Single(0);
        var f = MyEnumerable.Single(localVariable);
        var g = await MyAsyncEnumerable.SingleAsync<int>(localAsynVariable);
        var h = MyNotEnumerable.Single(0);

        var aa = localVariable.SingleOrDefault();
        var ab = localVariable.SingleOrDefault(_ => true);
        var ac = await MyAsyncEnumerable.SingleOrDefaultAsync(localAsynVariable);
    }
}

static class MyEnumerable
{
    public static T Single<T>(this IEnumerable<T> source)
        => default;
}

static class MyAsyncEnumerable
{
    public static ValueTask<T> SingleAsync<T>(this IAsyncEnumerable<T> source)
        => new ValueTask<T>(default(T));

    public static ValueTask<T> SingleOrDefaultAsync<T>(this IAsyncEnumerable<T> source)
        => new ValueTask<T>(default(T));
}

static class MyNotEnumerable
{
    public static int Single(this int source)
        => source;
}
";

            var a = new DiagnosticResult
            {
                Id = "HLQ005",
                Message = "Avoid 'Single()'. Use 'First()' instead.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 16, 31)
                },
            };

            var b = new DiagnosticResult
            {
                Id = "HLQ005",
                Message = "Avoid 'Single()'. Use 'First()' instead.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 17, 31)
                },
            };

            var c = new DiagnosticResult
            {
                Id = "HLQ005",
                Message = "Avoid 'Single()'. Use 'First()' instead.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 18, 28)
                },
            };

            var d = new DiagnosticResult
            {
                Id = "HLQ005",
                Message = "Avoid 'Single()'. Use 'First()' instead.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 19, 28)
                },
            };

            var f = new DiagnosticResult
            {
                Id = "HLQ005",
                Message = "Avoid 'Single()'. Use 'First()' instead.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 21, 30)
                },
            };

            var g = new DiagnosticResult
            {
                Id = "HLQ005",
                Message = "Avoid 'SingleAsync()'. Use 'FirstAsync()' instead.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 22, 41)
                },
            };

            var aa = new DiagnosticResult
            {
                Id = "HLQ005",
                Message = "Avoid 'SingleOrDefault()'. Use 'FirstOrDefault()' instead.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 25, 32)
                },
            };

            var ab = new DiagnosticResult
            {
                Id = "HLQ005",
                Message = "Avoid 'SingleOrDefault()'. Use 'FirstOrDefault()' instead.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 26, 32)
                },
            };

            var ac = new DiagnosticResult
            {
                Id = "HLQ005",
                Message = "Avoid 'SingleOrDefaultAsync()'. Use 'FirstOrDefaultAsync()' instead.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 27, 42)
                },
            };

            VerifyCSharpDiagnostic(test, a, b, c, d, f, g, aa, ab, ac);
        }

    }
}