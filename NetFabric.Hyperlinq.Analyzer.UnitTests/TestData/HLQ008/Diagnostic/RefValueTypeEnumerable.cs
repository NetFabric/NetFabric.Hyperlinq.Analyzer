using System;

namespace HLQ008.Diagnostic
{
    ref struct RefValueTypeEnumerable<T>
    {
        public Enumerator GetEnumerator() => new Enumerator();

        public struct Enumerator
        {
            public T Current => default;

            public bool MoveNext() => false;
        }
    }
}
