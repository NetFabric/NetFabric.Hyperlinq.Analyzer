using System; 

namespace HLQ004.Diagnostic
{
    class RefReadOnly
    {
        void Method()
        {
            foreach (ref readonly var item in new RefReadOnlyEnumerable())
            {

            }
        }
    }
}
