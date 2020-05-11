using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HLQ009.NoDiagnostics
{
    readonly struct DisposeEnumerable<T> : IEnumerable<T>
    {
        public Enumerator GetEnumerator() => new Enumerator();
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => new Enumerator();
        IEnumerator IEnumerable.GetEnumerator() => new Enumerator();

        public struct Enumerator : IEnumerator<T>
        {
            public T Current => default;
            object IEnumerator.Current => default;

            public bool MoveNext() => false;

            public void Reset() => throw new NotImplementedException();

            public void Dispose() { }
        }
    }

    partial class C
    {
        public void Test_DisposeEnumerable()
        {
            foreach (var _ in new DisposeEnumerable<TestType>())
            {

            }
        }
    }
}
