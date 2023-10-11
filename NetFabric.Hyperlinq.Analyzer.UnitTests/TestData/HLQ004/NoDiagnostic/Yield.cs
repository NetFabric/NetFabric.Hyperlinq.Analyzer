using System.Collections.Generic;

namespace HLQ004.NoDiagnostic
{
    class Yield
    {
        void Method()
        {
            foreach (var item in GetEnumerable())
            {
            }
        }

        IEnumerable<int> GetEnumerable()
        {
            yield return 1;
        }   
    }
}
