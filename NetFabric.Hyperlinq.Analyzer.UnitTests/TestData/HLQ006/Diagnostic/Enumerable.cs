using System;
using System.Collections;
using System.Collections.Generic;

namespace HLQ006.Diagnostic
{
    class Enumerable<T> : IEnumerable<T>
    {
        public Enumerator GetEnumerator() => new Enumerator();
        IEnumerator<T> IEnumerable<T>.GetEnumerator() => new Enumerator();
        IEnumerator IEnumerable.GetEnumerator() => new Enumerator();

        public class Enumerator : IEnumerator<T>
        {
            public T Current => default;
            object IEnumerator.Current => default;

            public bool MoveNext() => false;

            public void Reset() { }

            public void Dispose() { }
        }
    }
}
