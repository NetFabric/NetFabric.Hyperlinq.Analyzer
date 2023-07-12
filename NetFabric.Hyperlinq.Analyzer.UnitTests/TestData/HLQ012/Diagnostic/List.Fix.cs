using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace HLQ012.NoDiagnostic.List
{
    partial class C
    {
        void Method()
        {
            var source = new List<int>();
            foreach (var item in CollectionsMarshal.AsSpan(source))
                Console.WriteLine(item);
        }
    }
}
