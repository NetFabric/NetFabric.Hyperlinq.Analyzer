namespace HLQ004.Diagnostic
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
