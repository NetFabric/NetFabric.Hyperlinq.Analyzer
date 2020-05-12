using System;
using System.Collections.Generic;

namespace HLQ010.NoDiagnostic.List
{
    partial class C
    {
        void Method()
        {
            foreach (var item in new List<int>())
                Console.WriteLine(item);
        }
    }
}
