using System;
using Xunit;
using TestHelper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NetFabric.Hyperlinq.Analyzer.UnitTests
{
    public class AssignmentBoxingTests : CodeFixVerifier
    {
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() =>
            new AssignmentBoxingAnalyzer();

        [Fact]
        public void Verify_Field_NoDiagnostics()
        {
            var test = @"
using System.Collections.Generic;

class C
{
    List<int> field00 = new List<int>();
    IEnumerable<int> field01 = new NonOptimizedEnumerable<int>();

    public IList<int> field11 = new List<int>(); // boxes enumerator but public

    public void Method()
    {
        field00 = new List<int>();
        field01 = new NonOptimizedEnumerable<int>();

        field11 = new List<int>(); // boxes enumerator but public
    }
}" + EnumerableDefinitions;

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void Verify_Field_NoDiagnostics_Async()
        {
            var test = @"
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

class C
{
    OptimizedAsyncEnumerable<int> field00 = new OptimizedAsyncEnumerable<int>();
    IAsyncEnumerable<int> field01 = new NonOptimizedAsyncEnumerable<int>();

    public IAsyncEnumerable<int> field11 = new OptimizedAsyncEnumerable<int>(); // boxes enumerator but public

    public void Method()
    {
        field00 = new OptimizedAsyncEnumerable<int>();
        field01 = new NonOptimizedAsyncEnumerable();

        field11 = new OptimizedAsyncEnumerable<int>(); // boxes enumerator but public
    }
}" + AsyncEnumerableDefinitions;

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void Verify_Field()
        {
            var test = @"
using System.Collections.Generic;

class C
{
    IList<int> field01 = new List<int>();

    public void Method()
    {
        field01 = new List<int>();
    }
}" + EnumerableDefinitions;

            var initializer = new DiagnosticResult
            {
                Id = "HLQ001",
                Message = "'List`1' has a value type enumerator. Assigning it to 'IList`1' causes boxing of the enumerator.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 6, 5)
                },
            };

            var method = new DiagnosticResult
            {
                Id = "HLQ001",
                Message = "'List`1' has a value type enumerator. Assigning it to 'IList`1' causes boxing of the enumerator.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 10, 9)
                },
            };

            VerifyCSharpDiagnostic(test, initializer, method);
        }

        [Fact]
        public void Verify_Field_Async()
        {
            var test = @"
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

class C
{
    IAsyncEnumerable<int> field01 = new OptimizedAsyncEnumerable<int>();

    public void Method()
    {
        field01 = new OptimizedAsyncEnumerable<int>();
    }
}" + AsyncEnumerableDefinitions;

            var initializer = new DiagnosticResult
            {
                Id = "HLQ001",
                Message = "'OptimizedAsyncEnumerable`1' has a value type enumerator. Assigning it to 'IAsyncEnumerable`1' causes boxing of the enumerator.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 8, 5)
                },
            };

            var method = new DiagnosticResult
            {
                Id = "HLQ001",
                Message = "'OptimizedAsyncEnumerable`1' has a value type enumerator. Assigning it to 'IAsyncEnumerable`1' causes boxing of the enumerator.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 12, 9)
                },
            };

            VerifyCSharpDiagnostic(test, initializer, method);
        }

        [Fact]
        public void Verify_Property_NoDiagnostics()
        {
            var test = @"
using System.Collections.Generic;

class C
{
    List<int> Property00 { get; } = new List<int>();
    IEnumerable<int> Property01 { get; } = new NonOptimizedEnumerable<int>();

    public IList<int> Property11 { get; } = new List<int>(); // boxes enumerator but public

    public void Method()
    {
        Property00 = new List<int>();
        Property01 = new NonOptimizedEnumerable<int>();

        Property11 = new List<int>(); // boxes enumerator but public
    }
}" + EnumerableDefinitions;

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void Verify_Property_NoDiagnostics_Async()
        {
            var test = @"
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

class C
{
    OptimizedAsyncEnumerable<int> Property00 { get; } = new OptimizedAsyncEnumerable<int>();
    IAsyncEnumerable<int> Property01 { get; } = new NonOptimizedAsyncEnumerable<int>();

    public IAsyncEnumerable<int> Property11 { get; } = new OptimizedAsyncEnumerable<int>(); // boxes enumerator but public

    public void Method() 
    {
        Property00 = new OptimizedAsyncEnumerable<int>();
        Property01 = new NonOptimizedAsyncEnumerable<int>();

        Property11 = new OptimizedAsyncEnumerable<int>(); // boxes enumerator but public
    }
}" + EnumerableDefinitions;

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void Verify_Property()
        {
            var test = @"
using System.Collections.Generic;

class C
{
    IList<int> Property01 { get; set; } = new List<int>(); 

    public void Method()
    {
        Property01 = new List<int>(); 
    }
}" + EnumerableDefinitions;

            var initializer = new DiagnosticResult
            {
                Id = "HLQ001",
                Message = "'List`1' has a value type enumerator. Assigning it to 'IList`1' causes boxing of the enumerator.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 6, 5)
                },
            };

            var method = new DiagnosticResult
            {
                Id = "HLQ001",
                Message = "'List`1' has a value type enumerator. Assigning it to 'IList`1' causes boxing of the enumerator.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 10, 9)
                },
            };

            VerifyCSharpDiagnostic(test, initializer, method);
        }

        [Fact]
        public void Verify_Property_Async()
        {
            var test = @"
using System.Collections.Generic;

class C
{
    IAsyncEnumerable<int> Property01 { get; set; } = new OptimizedAsyncEnumerable<int>(); 

    public void Method()
    {
        Property01 = new OptimizedAsyncEnumerable<int>(); 
    }
}" + AsyncEnumerableDefinitions;

            var initializer = new DiagnosticResult
            {
                Id = "HLQ001",
                Message = "'OptimizedAsyncEnumerable`1' has a value type enumerator. Assigning it to 'IAsyncEnumerable`1' causes boxing of the enumerator.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 6, 5)
                },
            };

            var method = new DiagnosticResult
            {
                Id = "HLQ001",
                Message = "'OptimizedAsyncEnumerable`1' has a value type enumerator. Assigning it to 'IAsyncEnumerable`1' causes boxing of the enumerator.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 10, 9)
                },
            };

            VerifyCSharpDiagnostic(test, initializer, method);
        }

        [Fact]
        public void Verify_LocalVariable_NoDiagnostics()
        {
            var test = @"
using System.Collections.Generic;

class C
{
    public Method()
    {
        List<int> variable00 = new List<int>();
        IEnumerable<int> variable01 = new NonOptimizedEnumerable<int>();

        var variable10 = new List<int>();

        variable00 = new List<int>();
        variable01 = new NonOptimizedEnumerable<int>();

        variable10 = new List<int>();
    }
}" + EnumerableDefinitions;

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void Verify_LocalVariable_NoDiagnostics_Async()
        {
            var test = @"
using System.Collections.Generic;

class C
{
    public Method()
    {
        OptimizedAsyncEnumerable<int> variable00 = new OptimizedAsyncEnumerable<int>();
        IAsyncEnumerable<int> variable01 = new NonOptimizedAsyncEnumerable<int>();

        var variable10 = new OptimizedAsyncEnumerable<int>();

        variable00 = new OptimizedAsyncEnumerable<int>();
        variable01 = new NonOptimizedAsyncEnumerable<int>();

        variable10 = new OptimizedAsyncEnumerable<int>();
    }
}" + EnumerableDefinitions;

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void Verify_LocalVariable()
        {
            var test = @"
using System.Collections.Generic;

class C
{
    public Method()
    {
        IList<int> variable01 = new List<int>(); 

        variable01 = new List<int>(); 
    }
}" + EnumerableDefinitions;

            var initializer = new DiagnosticResult
            {
                Id = "HLQ001",
                Message = "'List`1' has a value type enumerator. Assigning it to 'IList`1' causes boxing of the enumerator.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 8, 9)
                },
            };

            var assignment = new DiagnosticResult
            {
                Id = "HLQ001",
                Message = "'List`1' has a value type enumerator. Assigning it to 'IList`1' causes boxing of the enumerator.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 10, 9)
                },
            };

            VerifyCSharpDiagnostic(test, initializer, assignment);
        }

        [Fact]
        public void Verify_LocalVariable_Async()
        {
            var test = @"
using System.Collections.Generic;

class C
{
    public Method()
    {
        IAsyncEnumerable<int> variable01 = new OptimizedAsyncEnumerable<int>(); 

        variable01 = new OptimizedAsyncEnumerable<int>(); 
    }
}" + AsyncEnumerableDefinitions;

            var initializer = new DiagnosticResult
            {
                Id = "HLQ001",
                Message = "'OptimizedAsyncEnumerable`1' has a value type enumerator. Assigning it to 'IAsyncEnumerable`1' causes boxing of the enumerator.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 8, 9)
                },
            };

            var assignment = new DiagnosticResult
            {
                Id = "HLQ001",
                Message = "'OptimizedAsyncEnumerable`1' has a value type enumerator. Assigning it to 'IAsyncEnumerable`1' causes boxing of the enumerator.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 10, 9)
                },
            };

            VerifyCSharpDiagnostic(test, initializer, assignment);
        }

        const string EnumerableDefinitions = @"
class NonOptimizedEnumerable<T> : IEnumerable<T>
{
    public IEnumerator<T> GetEnumerator() => new Enumerator();
    IEnumerator IEnumerable.GetEnumerator() => new Enumerator();

    class Enumerator : IEnumerator<T>
    {
        public T Current => default;

        public bool MoveNextAsync() => false;

        public void Reset() { }

        public void Dispose() { }
    }
}";

        const string AsyncEnumerableDefinitions = @"
class OptimizedAsyncEnumerable<T> : IAsyncEnumerable<T>
{
    public Enumerator GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => new Enumerator();
    IAsyncEnumerator<T> IAsyncEnumerable<T>.GetAsyncEnumerator(CancellationToken cancellationToken)
        => new Enumerator();

    public struct Enumerator : IAsyncEnumerator<T>
    {
        public T Current => default;

        public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(false);

        public ValueTask DisposeAsync() => new ValueTask();
    }
}

class NonOptimizedAsyncEnumerable<T> : IAsyncEnumerable<T>
{
    public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken = default)
        => new Enumerator();

    class Enumerator : IAsyncEnumerator<T>
    {
        public T Current => default;

        public ValueTask<bool> MoveNextAsync() => new ValueTask<bool>(false);

        public ValueTask DisposeAsync() => new ValueTask();
    }
}
";

    }
}