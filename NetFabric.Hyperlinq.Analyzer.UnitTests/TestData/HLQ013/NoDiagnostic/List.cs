using System;
using System.Collections.Generic;

namespace HLQ013.NoDiagnostic.ListTest
{
    partial class C
    {
        void Method()
        {
            var source = new List<int>();
            for (var index = 0; index < source.Count; index++)
            {
                var item = source[index];
                Console.WriteLine(item);
            }
        }
    }
}
