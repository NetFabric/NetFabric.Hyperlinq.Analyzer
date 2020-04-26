using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using TestHelper;
using Xunit;

namespace NetFabric.Hyperlinq.Analyzer.UnitTests
{
    public class NullEnumerableAnalyzerTests : DiagnosticVerifier
    {
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() =>
            new NullEnumerableAnalyzer();

        [Fact]
        public void Verify_Enumerable_NoDiagnostics()
        {
            var test = @"
using System;
using System.Collections.Generic;

class C
{
    IEnumerable<int> Method_Iterator()
    {
        yield return 0;
    }

    IEnumerable<int> Method()
    {
        return Method_Iterator();
    }

    IEnumerable<int> MethodArrow() => Method_Iterator();
}";

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void Verify_AsyncEnumerable_NoDiagnostics()
        {
            var test = @"
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

class C
{
    async IAsyncEnumerable<int> MethodAsync_Iterator()
    {
        yield return await Task.FromResult(0);
    }

    IAsyncEnumerable<int> MethodAsync()
    {
        return MethodAsync_Iterator();
    }

    IAsyncEnumerable<int> MethodArrowAsync() => MethodAsync_Iterator();
}";

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void Verify_Enumerable()
        {
            var test = @"
using System;
using System.Collections.Generic;

class C
{
    IEnumerable<int> Method()
    {
        return null;
    }

    IEnumerable<int> MethodArrow() => null;
}";

            var expectedMethod = new DiagnosticResult
            {
                Id = "HLQ002",
                Message = "Enumerable cannot be null. Return an empty enumerable instead.",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 9, 9)
                },
            };

            var expectedMethodArrow = new DiagnosticResult
            {
                Id = "HLQ002",
                Message = "Enumerable cannot be null. Return an empty enumerable instead.",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 12, 39)
                },
            };

            VerifyCSharpDiagnostic(test, expectedMethod, expectedMethodArrow);
        }

        [Fact]
        public void Verify_AsyncEnumerable()
        {
            var test = @"
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

class C
{
    IAsyncEnumerable<int> Method()
    {
        return null;
    }

    IAsyncEnumerable<int> MethodArrow() => null;
}";

            var expectedMethod = new DiagnosticResult
            {
                Id = "HLQ002",
                Message = "Enumerable cannot be null. Return an empty enumerable instead.",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 10, 9)
                },
            };

            var expectedMethodArrow = new DiagnosticResult
            {
                Id = "HLQ002",
                Message = "Enumerable cannot be null. Return an empty enumerable instead.",
                Severity = DiagnosticSeverity.Error,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 13, 44)
                },
            };

            VerifyCSharpDiagnostic(test, expectedMethod, expectedMethodArrow);
        }

    }
}