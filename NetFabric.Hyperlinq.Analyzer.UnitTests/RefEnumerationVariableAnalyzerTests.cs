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
    public class RefEnumerationVariableAnalyzerTests : CodeFixVerifier
    {
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() =>
            new RefEnumerationVariableAnalyzer();

        protected override CodeFixProvider GetCSharpCodeFixProvider()
            => new RefEnumerationVariableCodeFixProvider();

        [Fact]
        public void Verify_NoDiagnostics()
        {
            var test = @"
using System.Collections.Generic;
using System.Linq;

class C
{
    void Method_NoRef()
    {
        foreach(var item in Enumerable.Range(0, 10))
        {

        }
    }

    void Method_Ref()
    {
        foreach(ref var item in RefEnumerable.GetInstance())
        {

        }
    }

    void Method_RefReadOnly()
    {
        foreach(ref readonly var item in RefReadOnlyEnumerable.GetInstance())
        {

        }
    }
}" + RefEnumerables;

            VerifyCSharpDiagnostic(test);
        }

        [Fact]
        public void Verify_MissingRef()
        {
            var test = @"
using System.Collections.Generic;
using System.Linq;

class C
{
    void Method()
    {
        foreach(var item in RefEnumerable.GetInstance())
        {

        }
    }
}" + RefEnumerables;

            var expected = new DiagnosticResult
            {
                Id = "HLQ004",
                Message = "The enumerator returns a reference to the item. Add 'ref' to the item type.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 9, 17)
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
        foreach(ref var item in RefEnumerable.GetInstance())
        {

        }
    }
}" + RefEnumerables;

            VerifyCSharpFix(test, fixtest);
        }

        [Fact]
        public void Verify_MissingRefReadOnly()
        {
            var test = @"
using System.Collections.Generic;
using System.Linq;

class C
{
    void Method()
    {
        foreach(var item in RefReadOnlyEnumerable.GetInstance())
        {

        }
    }
}" + RefEnumerables;

            var expected = new DiagnosticResult
            {
                Id = "HLQ004",
                Message = "The enumerator returns a reference to the item. Add 'ref readonly' to the item type.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 9, 17)
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
        foreach(ref readonly var item in RefReadOnlyEnumerable.GetInstance())
        {

        }
    }
}" + RefEnumerables;

            VerifyCSharpFix(test, fixtest);
        }

        const string RefEnumerables = @"
public class RefReadOnlyEnumerable
{
    public static RefReadOnlyEnumerable GetInstance() => new RefReadOnlyEnumerable();

    public Enumerator GetEnumerator() => new Enumerator();

    public class Enumerator
    {
        int[] source = new int[0];

        public ref readonly int Current => ref source[0];

        public bool MoveNext() => false;
    }
}

public class RefEnumerable
{
    public static RefEnumerable GetInstance() => new RefEnumerable();

    public Enumerator GetEnumerator() => new Enumerator();

    public class Enumerator
    {
        int[] source = new int[0];

        public ref int Current => ref source[0];

        public bool MoveNext() => false;
    }
}
";
    }
}