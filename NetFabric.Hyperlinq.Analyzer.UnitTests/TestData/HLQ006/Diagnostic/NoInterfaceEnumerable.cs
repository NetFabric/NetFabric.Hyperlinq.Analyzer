namespace HLQ006.Diagnostic
{
    class NoInterfaceEnumerable<T>
    {
        public Enumerator GetEnumerator() => new Enumerator();

        public class Enumerator
        {
            public T Current => default;

            public bool MoveNext() => false;
        }
    }
}
