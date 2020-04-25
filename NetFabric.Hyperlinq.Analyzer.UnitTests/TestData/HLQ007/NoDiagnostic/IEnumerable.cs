using System;

namespace HLQ007.NoDiagnostic.Interface
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

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

    class C
    {
        void Test()
        {
            // make sure implementation is supported by foreach
            foreach (var item in new Enumerable<int>())
                Console.WriteLine(item);
        }
    }
}
