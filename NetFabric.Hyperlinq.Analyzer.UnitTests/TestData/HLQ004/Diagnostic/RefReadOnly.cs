using System; 

namespace HLQ004.Diagnostic
{
    class RefReadOnly
    {
        void Method()
        {
            foreach (var item in new RefReadOnlyEnumerable())
            {

            }
        }
    }
}
