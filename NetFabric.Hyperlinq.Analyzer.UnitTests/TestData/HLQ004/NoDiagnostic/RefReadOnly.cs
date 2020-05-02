using System; 

namespace HLQ004.NoDiagnostic
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
