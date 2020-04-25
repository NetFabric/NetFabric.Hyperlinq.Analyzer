using System;

namespace HLQ007.Diagnostic.Interface
{
    using System;
    using System.Collections;
    using System.Collections.Generic;

    readonly struct Enumerable<T> : IEnumerable<T>
    {
        public Enumerator GetEnumerator() => new Enumerator();
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => new Enumerator();
        IEnumerator IEnumerable.GetEnumerator() => new Enumerator();

        public struct Enumerator : IEnumerator<T>
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
