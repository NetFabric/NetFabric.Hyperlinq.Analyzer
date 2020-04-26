using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
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

        [Fact]
        public void Verify_NoDiagnostics()
        {
            var test = @"
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

class C
{
    public int Single(int source)
        => source;

    public int SingleOrDefault(int source)
        => source;

    async void Method()
    {
        var a = Single(0);
        var b = SingleOrDefault(0);

        var c = MyNotEnumerable.Single(0);
        var d = MyNotEnumerable.SingleOrDefault(0);
    }
}

static class MyNotEnumerable
{
    public static int Single(this int source)
        => source;

    public static int SingleOrDefault(this int source)
        => source;
}
";

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void Verify_Single_Linq()
        {
            var test = @"
using System.Collections.Generic;
using System.Linq;

class C
{
    void Method()
    {
        IEnumerable<int> localVariable = null;

        var a = localVariable.Single();
    }
}";

            var expected = new DiagnosticResult
            {
                Id = "HLQ005",
                Message = "Avoid 'Single()'. Use 'First()' instead.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 11, 31)
                },
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
using System.Collections.Generic;
using System.Linq;

class C
{
    void Method()
    {
        IEnumerable<int> localVariable = null;

        var a = localVariable.First();
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        [Fact]
        public void Verify_SingleOrDefault_Linq()
        {
            var test = @"
using System.Collections.Generic;
using System.Linq;

class C
{
    void Method()
    {
        IEnumerable<int> localVariable = null;

        var a = localVariable.SingleOrDefault();
    }
}";

            var expected = new DiagnosticResult
            {
                Id = "HLQ005",
                Message = "Avoid 'SingleOrDefault()'. Use 'FirstOrDefault()' instead.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 11, 31)
                },
            };

            VerifyCSharpDiagnostic(test, expected);

            var fixtest = @"
using System.Collections.Generic;
using System.Linq;

class C
{
    void Method()
    {
        IEnumerable<int> localVariable = null;

        var a = localVariable.FirstOrDefault();
    }
}";

            VerifyCSharpFix(test, fixtest);
        }

        [Fact]
        public void Verify()
        {
            var test = @"
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

class C
{
    async void Method()
    {
        IEnumerable<int> localVariable = null;
        IAsyncEnumerable<int> localAsynVariable = null;

        var a = localVariable.Single();
        var b = localVariable.Single(_ => true);
        var c = Enumerable.Single(localVariable);
        var d = Enumerable.Single(localVariable, _ => true);
        var e = MyEnumerable.Single(localVariable);
        var f = await MyAsyncEnumerable.SingleAsync<int>(localAsynVariable);
    }
}
" + EnumerableDefinitions;

            var a = new DiagnosticResult
            {
                Id = "HLQ005",
                Message = "Avoid 'Single()'. Use 'First()' instead.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 13, 31)
                },
            };

            var b = new DiagnosticResult
            {
                Id = "HLQ005",
                Message = "Avoid 'Single()'. Use 'First()' instead.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 14, 31)
                },
            };

            var c = new DiagnosticResult
            {
                Id = "HLQ005",
                Message = "Avoid 'Single()'. Use 'First()' instead.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 15, 28)
                },
            };

            var d = new DiagnosticResult
            {
                Id = "HLQ005",
                Message = "Avoid 'Single()'. Use 'First()' instead.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 16, 28)
                },
            };

            var e = new DiagnosticResult
            {
                Id = "HLQ005",
                Message = "Avoid 'Single()'. Use 'First()' instead.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 17, 30)
                },
            };

            var f = new DiagnosticResult
            {
                Id = "HLQ005",
                Message = "Avoid 'SingleAsync()'. Use 'FirstAsync()' instead.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 18, 41)
                },
            };

            VerifyCSharpDiagnostic(test, a, b, c, d, e, f);
        }

        const string EnumerableDefinitions = @"
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
";

    }
}