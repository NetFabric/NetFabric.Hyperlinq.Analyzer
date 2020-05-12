using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System;

namespace HLQ009.Diagnostics
{
    readonly struct DisposeEnumerable<T>
    {
        public Enumerator GetEnumerator() => new Enumerator();

        public struct Enumerator : IDisposable
        {
            public T Current => default;

            public bool MoveNext() => false;

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
