using System;

namespace HLQ010.NoDiagnostic.ArraySegment
{
    partial class C
    {
        void Method()
        {
            var source = new ArraySegment<int>();
            foreach (var item in source)
                Console.WriteLine(item);
        }
    }
}
