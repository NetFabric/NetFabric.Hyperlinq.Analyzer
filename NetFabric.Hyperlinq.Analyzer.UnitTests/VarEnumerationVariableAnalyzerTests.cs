using System;
using Xunit;
using TestHelper;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace NetFabric.Hyperlinq.Analyzer.UnitTests
{
    public class VarEnumerationVariableAnalyzerTests : DiagnosticVerifier
    {
        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer() =>
            new RefEnumerationVariableAnalyzer();

        [Fact]
        public void Verify()
        {
            var test = @"
using System.Collections.Generic;
using System.Linq;

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

class C
{
    void Method01()
    {
        foreach(var item in RefEnumerable.GetInstance())
        {

        }
    }

    void Method02()
    {
        foreach(ref var item in RefEnumerable.GetInstance())
        {

        }
    }

    void Method03()
    {
        foreach(var item in RefReadOnlyEnumerable.GetInstance())
        {

        }
    }


    void Method05()
    {
        foreach(ref readonly var item in RefReadOnlyEnumerable.GetInstance())
        {

        }
    }

    void Method06()
    {
        foreach(var item in Enumerable.Range(0, 10))
        {

        }
    }
}";

            var method01 = new DiagnosticResult
            {
                Id = "HLQ004",
                Message = "The enumerator returns a reference to the item. Add 'ref' to the item type.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 41, 17)
                },
            };

            var method03 = new DiagnosticResult
            {
                Id = "HLQ004",
                Message = "The enumerator returns a reference to the item. Add 'ref readonly' to the item type.",
                Severity = DiagnosticSeverity.Warning,
                Locations = new[] {
                    new DiagnosticResultLocation("Test0.cs", 57, 17)
                },
            };

            VerifyCSharpDiagnostic(test, method01, method03);
        }

    }
}