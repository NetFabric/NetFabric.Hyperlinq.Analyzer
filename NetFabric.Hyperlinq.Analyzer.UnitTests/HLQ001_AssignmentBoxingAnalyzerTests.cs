using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using System.IO;
using System.Linq;
using TestHelper;
using Xunit;

namespace NetFabric.Hyperlinq.Analyzer.UnitTests
{
    public class AssignmentBoxingTests : DiagnosticVerifier
    {
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() =>
            new AssignmentBoxingAnalyzer();

        [Theory]
        [InlineData("TestData/HLQ001/NoDiagnostic/FieldDeclaration.cs")]
        [InlineData("TestData/HLQ001/NoDiagnostic/FieldDeclaration.Async.cs")]
        [InlineData("TestData/HLQ001/NoDiagnostic/PropertyDeclaration.cs")]
        [InlineData("TestData/HLQ001/NoDiagnostic/PropertyDeclaration.Async.cs")]
        [InlineData("TestData/HLQ001/NoDiagnostic/VariableDeclaration.cs")]
        [InlineData("TestData/HLQ001/NoDiagnostic/VariableDeclaration.Async.cs")]
        public void Verify_NoDiagnostics(string path)
        {
            var paths = new[]
            {
                path,
                "TestData/TestType.cs",
                "TestData/Enumerables.cs",
            };
            VerifyCSharpDiagnostic(paths.Select(path => File.ReadAllText(path)).ToArray());
        }

        [Theory]
        [InlineData("TestData/HLQ001/Diagnostic/EqualsValueClause/FieldDeclaration.cs", "OptimizedEnumerable`1", "IEnumerable`1", 8, 9)]
        [InlineData("TestData/HLQ001/Diagnostic/EqualsValueClause/FieldDeclaration.Async.cs", "OptimizedAsyncEnumerable`1", "IAsyncEnumerable`1", 8, 9)]
        [InlineData("TestData/HLQ001/Diagnostic/EqualsValueClause/PropertyDeclaration.cs", "OptimizedEnumerable`1", "IEnumerable`1", 8, 9)]
        [InlineData("TestData/HLQ001/Diagnostic/EqualsValueClause/PropertyDeclaration.Async.cs", "OptimizedAsyncEnumerable`1", "IAsyncEnumerable`1", 8, 9)]
        [InlineData("TestData/HLQ001/Diagnostic/EqualsValueClause/VariableDeclaration.cs", "OptimizedEnumerable`1", "IEnumerable`1", 10, 13)]
        [InlineData("TestData/HLQ001/Diagnostic/EqualsValueClause/VariableDeclaration.Async.cs", "OptimizedAsyncEnumerable`1", "IAsyncEnumerable`1", 11, 13)]
        [InlineData("TestData/HLQ001/Diagnostic/SimpleAssignment/FieldDeclaration.cs", "OptimizedEnumerable`1", "IEnumerable`1", 12, 13)]
        [InlineData("TestData/HLQ001/Diagnostic/SimpleAssignment/FieldDeclaration.Async.cs", "OptimizedAsyncEnumerable`1", "IAsyncEnumerable`1", 12, 13)]
        [InlineData("TestData/HLQ001/Diagnostic/SimpleAssignment/PropertyDeclaration.cs", "OptimizedEnumerable`1", "IEnumerable`1", 12, 13)]
        [InlineData("TestData/HLQ001/Diagnostic/SimpleAssignment/PropertyDeclaration.Async.cs", "OptimizedAsyncEnumerable`1", "IAsyncEnumerable`1", 12, 13)]
        [InlineData("TestData/HLQ001/Diagnostic/SimpleAssignment/VariableDeclaration.cs", "OptimizedEnumerable`1", "IEnumerable`1", 12, 13)]
        [InlineData("TestData/HLQ001/Diagnostic/SimpleAssignment/VariableDeclaration.Async.cs", "OptimizedAsyncEnumerable`1", "IAsyncEnumerable`1", 13, 13)]
        public void Verify_Diagnostics(string path, string type, string @interface, int line, int column)
        {
            var paths = new[]
            {
                path,
                "TestData/TestType.cs",
                "TestData/Enumerables.cs",
            };
            var expected = new DiagnosticResult
            {
                Id = "HLQ001",
                Message = $"'{type}' has a value type enumerator. Assigning it to '{@interface}' causes boxing of the enumerator.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", line, column)
                },
            };

            VerifyCSharpDiagnostic(paths.Select(path => File.ReadAllText(path)).ToArray(), expected);
        }

        [Fact]
        public void Verify_Field_Async()
        {
            var test = @"
using System;
using System.Collections.Generic;
using System.Threading;
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
                    new DiagnosticResultLocation("Test0.cs", 9, 5)
                },
            };

            var method = new DiagnosticResult
            {
                Id = "HLQ001",
                Message = "'OptimizedAsyncEnumerable`1' has a value type enumerator. Assigning it to 'IAsyncEnumerable`1' causes boxing of the enumerator.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 13, 9)
                },
            };

            VerifyCSharpDiagnostic(test, initializer, method);
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
using System.Threading;
using System.Threading.Tasks;

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
        public void Verify_LocalVariable()
        {
            var test = @"
using System.Collections.Generic;

class C
{
    public void Method()
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
using System.Threading;
using System.Threading.Tasks;

class C
{
    public void Method()
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
                    new DiagnosticResultLocation("Test0.cs", 10, 9)
                },
            };

            var assignment = new DiagnosticResult
            {
                Id = "HLQ001",
                Message = "'OptimizedAsyncEnumerable`1' has a value type enumerator. Assigning it to 'IAsyncEnumerable`1' causes boxing of the enumerator.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 12, 9)
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