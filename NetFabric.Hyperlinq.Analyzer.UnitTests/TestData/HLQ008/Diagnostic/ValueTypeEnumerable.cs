namespace HLQ008.Diagnostic
{
    struct ValueTypeEnumerable<T>
    {
        public Enumerator GetEnumerator() => new Enumerator();

        public struct Enumerator
        {
            public T Current => default;

            public bool MoveNext() => false;
        }
    }
}
