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
    public class NonDisposableEnumeratorAnalyzerTests : CodeFixVerifier
    {
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() =>
            new NonDisposableEnumeratorAnalyzer();

        [Fact]
        public void Verify_NoDiagnostics()
        {
            var test = @"
using System;
using System.Collections;
using System.Collections.Generic;

readonly struct Enumerable
{
    public Enumerator GetEnumerator() => new Enumerator();

    public struct Enumerator
    {
        public int Current => default;

        public bool MoveNext() => false;
    }
}

namespace NetFabric
{
    namespace Hyperlinq
    {
        readonly struct Enumerable<T> : IEnumerable<T>
        {
            public Enumerator GetEnumerator() => new Enumerator();
            IEnumerator<T> IEnumerable<T>.GetEnumerator() => new DisposableEnumerator();
            IEnumerator IEnumerable.GetEnumerator() => new DisposableEnumerator();

            public struct Enumerator
            {
                public int Current => default;

                public bool MoveNext()
                {
                    return false;
                }
            }

            class DisposableEnumerator : IEnumerator<T>
            {
                public T Current => default;
                object IEnumerator.Current => default;

                public bool MoveNext() => false;

                public void Reset() { }

                public void Dispose() { }
            }
        }
    }
}
";

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void Verify()
        {
            var test = @"
using System;
using System.Collections.Generic;

class Enumerable
{
    public Enumerator GetEnumerator() => new Enumerator();

    public class Enumerator
    {
        public int Current => default;

        public bool MoveNext() => false;

        public void Dispose() { }

        public void TestMethod() { }
    }
}

namespace NetFabric
{
    namespace Hyperlinq
    {
        readonly struct Enumerable<T>
        {
            public Enumerator GetEnumerator() => new Enumerator();

            public struct Enumerator
            {
                public T Current => default;

                public bool MoveNext() => false;

                public void Dispose() { }

                public void TestMethod() { }
            }
        }
    }
}
";

            var expected = new DiagnosticResult
            {
                Id = "HLQ007",
                Message = "'Enumerator' has an empty Dispose(). Consider returning a non-disposable enumerator.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 7, 12)
                },
            };

            var expectedGeneric = new DiagnosticResult
            {
                Id = "HLQ007",
                Message = "'Enumerator' has an empty Dispose(). Consider returning a non-disposable enumerator.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 27, 20)
                },
            };

            VerifyCSharpDiagnostic(test, expected, expectedGeneric);
        }
    }
}
