namespace HLQ004
{
    public class RefReadOnlyEnumerable
    {
        public static RefReadOnlyEnumerable GetInstance() => new RefReadOnlyEnumerable();

        public Enumerator GetEnumerator() => new Enumerator();

        public class Enumerator
        {
            int[] source = new int[0];

            public ref readonly int Current => ref source[0];

            public bool MoveNext() => false;
        }
    }

    public class RefEnumerable
    {
        public static RefEnumerable GetInstance() => new RefEnumerable();

        public Enumerator GetEnumerator() => new Enumerator();

        public class Enumerator
        {
            int[] source = new int[0];

            public ref int Current => ref source[0];

            public bool MoveNext() => false;
        }
    }
}
