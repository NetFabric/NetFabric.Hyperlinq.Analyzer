using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HLQ009.NoDiagnostics
{
    readonly struct ResetEnumerable<T> : IEnumerable
    {
        public Enumerator GetEnumerator() => new Enumerator();
        IEnumerator IEnumerable.GetEnumerator() => new Enumerator();

        public struct Enumerator : IEnumerator
        {
            public object Current => default;

            public bool MoveNext() => false;

            public void Reset() => throw new NotImplementedException();
        }
    }

    partial class C
    {
        public void Test_ResetEnumerable()
        {
            foreach (var _ in new ResetEnumerable<TestType>())
            {

            }
        }
    }
}
