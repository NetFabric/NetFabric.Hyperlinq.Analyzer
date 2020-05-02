namespace HLQ004.NoDiagnostic
{
    class Ref
    {
        void Method()
        {
            foreach (ref var item in new RefEnumerable())
            {

            }
        }
    }
}
