using NetFabric.Hyperlinq.Analyzer.UnitTests.TestData;
using System;
using System.Collections;
using System.Collections.Generic;

namespace HLQ007.NoDiagnostic.Arrow
{
    readonly struct Enumerable<T> : IEnumerable<T>
    {
        public IEnumerator<T> GetEnumerator() => new Enumerator();
        IEnumerator IEnumerable.GetEnumerator() => new Enumerator();

        class Enumerator: IEnumerator<T>
        {
            readonly IDisposable disposable = new Disposable();

            public T Current => default;
            object IEnumerator.Current => default;

            public bool MoveNext() => false;

            public void Reset() { }

            public void Dispose() => disposable.Dispose();
        }
    }

    partial class Tests
    {
        void Test_Enumerable()
        {
            // make sure implementation is supported by foreach
            foreach (var item in new Enumerable<TestType>())
                Console.WriteLine(item);
        }
    }
}
