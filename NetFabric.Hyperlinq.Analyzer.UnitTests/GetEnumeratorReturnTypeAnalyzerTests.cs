using System;
using Xunit;
using TestHelper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.CodeFixes;

namespace NetFabric.Hyperlinq.Analyzer.UnitTests
{
    public class GetEnumeratorReturnTypeAnalyzerTests : CodeFixVerifier
    {
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() =>
            new GetEnumeratorReturnTypeAnalyzer();

        [Fact]
        public void Verify_NoDiagnostics()
        {
            var test = @"
using System;
using System.Collections.Generic;

interface ICustomEnumerable
{
    IEnumerator GetEnumerator();
}   

interface IValueEnumerable<T, TEnumerator> : IEnumerable<T>
    where TEnumerator : struct, IEnumerator<T>
{
    new Enumerator GetEnumerator();
}   

readonly struct Enumerable
{
    public Enumerator GetEnumerator() => new Enumerator();

    public struct Enumerator
    {
        public int Current => 0;

        public bool MoveNext() => false;
    }
}

readonly struct Enumerable2 : IEnumerable<int>
{
    public Enumerator GetEnumerator() => new Enumerator();
    IEnumerator<int> IEnumerable<int>.GetEnumerator() => new Enumerator();
    IEnumerator IEnumerable.GetEnumerator() => new Enumerator();

    public struct Enumerator : IEnumerator<int>
    {
        public int Current => 0;

        public bool MoveNext() => false;
    }
}

readonly struct AsyncEnumerable
{
    public Enumerator GetAsyncEnumerator() => new Enumerator();

    public struct Enumerator
    {
        public int Current => 0;

        public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(false);
    }
}

readonly struct AsyncEnumerable2
{
    public Enumerator GetAsyncEnumerator(CancellationToken cancellationToken = default) => new Enumerator();

    public struct Enumerator
    {
        public int Current => 0;

        public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(false);
    }
}

readonly struct AsyncEnumerable3 : IAsyncEnumerable<int>
{
    public Enumerator GetAsyncEnumerator(CancellationToken cancellationToken = default) => new Enumerator();
    IAsyncEnumerator<int> IAsyncEnumerable<int>.GetAsyncEnumerator(CancellationToken cancellationToken = default) => new Enumerator();

    public struct Enumerator : IAsyncEnumerator<int>
    {
        public int Current => 0;

        public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(false);
    }
}
";

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void Verify_Enumerable()
        {
            var test = @"
using System;
using System.Collections.Generic;

readonly struct Enumerable : IEnumerable<int>
{
    public IEnumerator<int> GetEnumerator() => new Enumerator();
    IEnumerator IEnumerable.GetEnumerator() => new Enumerator();

    public struct Enumerator : IEnumerator<int>
    {
        public int Current => 0;

        public bool MoveNext() => false;
    }
}
";

            var expected = new DiagnosticResult
            {
                Id = "HLQ006",
                Message = "'GetEnumerator' returns an interface. Consider returning a value-type enumerator.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 7, 12)
                },
            };

            VerifyCSharpDiagnostic(test, expected);
        }


        [Fact]
        public void Verify_AsyncEnumerable()
        {
            var test = @"
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

readonly struct AsyncEnumerable : IAsyncEnumerable<int>
{
    public IAsyncEnumerator<int> GetAsyncEnumerator(CancellationToken cancellationToken = default) => new Enumerator();

    public struct Enumerator : IAsyncEnumerator<int>
    {
        public int Current => 0;

        public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(false);

        public ValueTask DisposeAsync() => new ValueTask();
    }
}
";

            var expected = new DiagnosticResult
            {
                Id = "HLQ006",
                Message = "'GetAsyncEnumerator' returns an interface. Consider returning a value-type enumerator.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 9, 12)
                },
            };

            VerifyCSharpDiagnostic(test, expected);
        }
    }
}
