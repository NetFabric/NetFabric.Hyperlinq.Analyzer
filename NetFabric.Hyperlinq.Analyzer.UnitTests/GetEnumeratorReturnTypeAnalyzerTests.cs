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
using System.Collections;
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
using System.Collections;
using System.Collections.Generic;

readonly struct Enumerable
{
    public Enumerator GetEnumerator() => new Enumerator();

    public class Enumerator
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

    public class Enumerator : IEnumerator<int>
    {
        public int Current => 0;

        public bool MoveNext() => false;
    }
}

readonly struct Enumerable3 : IEnumerable<int>
{
    public IEnumerator<int> GetEnumerator() => new Enumerator();
    IEnumerator IEnumerable.GetEnumerator() => new Enumerator();

    public struct Enumerator : IEnumerator<int>
    {
        public int Current => 0;

        object IEnumerator.Current => throw new NotImplementedException();

        public bool MoveNext() => false;

        public void Reset() => throw new NotImplementedException();

        public void Dispose() { }
    }
}
";

            var expected1 = new DiagnosticResult
            {
                Id = "HLQ006",
                Message = "'GetEnumerator' returns a reference type. Consider returning a value type.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 8, 12)
                },
            };

            var expected2 = new DiagnosticResult
            {
                Id = "HLQ006",
                Message = "'GetEnumerator' returns a reference type. Consider returning a value type.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 20, 12)
                },
            };

            var expected3 = new DiagnosticResult
            {
                Id = "HLQ006",
                Message = "'GetEnumerator' returns a reference type. Consider returning a value type.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 34, 12)
                },
            };

            VerifyCSharpDiagnostic(test, expected1, expected2, expected3);
        }

        [Fact]
        public void Verify_AsyncEnumerable()
        {
            var test = @"
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

readonly struct AsyncEnumerable
{
    public Enumerator GetAsyncEnumerator() => new Enumerator();

    public class Enumerator
    {
        public int Current => 0;

        public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(false);
    }
}

readonly struct AsyncEnumerable2
{
    public Enumerator GetAsyncEnumerator(CancellationToken cancellationToken = default) => new Enumerator();

    public class Enumerator
    {
        public int Current => 0;

        public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(false);
    }
}

readonly struct AsyncEnumerable3 : IAsyncEnumerable<int>
{
    public Enumerator GetAsyncEnumerator(CancellationToken cancellationToken = default) => new Enumerator();
    IAsyncEnumerator<int> IAsyncEnumerable<int>.GetAsyncEnumerator(CancellationToken cancellationToken = default) => new Enumerator();

    public class Enumerator : IAsyncEnumerator<int>
    {
        public int Current => 0;

        public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(false);
    }
}

readonly struct AsyncEnumerable4 : IAsyncEnumerable<int>
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

            var expected1 = new DiagnosticResult
            {
                Id = "HLQ006",
                Message = "'GetAsyncEnumerator' returns a reference type. Consider returning a value type.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 9, 12)
                },
            };

            var expected2 = new DiagnosticResult
            {
                Id = "HLQ006",
                Message = "'GetAsyncEnumerator' returns a reference type. Consider returning a value type.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 21, 12)
                },
            };

            var expected3 = new DiagnosticResult
            {
                Id = "HLQ006",
                Message = "'GetAsyncEnumerator' returns a reference type. Consider returning a value type.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 33, 12)
                },
            };

            var expected4 = new DiagnosticResult
            {
                Id = "HLQ006",
                Message = "'GetAsyncEnumerator' returns a reference type. Consider returning a value type.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 46, 12)
                },
            };

            VerifyCSharpDiagnostic(test, expected1, expected2, expected3, expected4);
        }
    }
}
