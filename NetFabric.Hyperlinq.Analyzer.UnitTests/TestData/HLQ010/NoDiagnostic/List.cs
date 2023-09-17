using System;
using System.Collections.Generic;

namespace HLQ010.Diagnostic.List
{
    partial class C
    {
        void Method()
        {
            var source = new List<int>();
            foreach (var item in source)
                Console.WriteLine(item);
        }
    }
}
