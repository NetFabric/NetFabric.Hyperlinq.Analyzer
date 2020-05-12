using System;
using System.Collections.Generic;

namespace HLQ010.NoDiagnostic.Dictionary
{
    partial class C
    {
        void Method()
        {
            foreach (var item in new Dictionary<string, string>())
                Console.WriteLine(item);
        }
    }
}
