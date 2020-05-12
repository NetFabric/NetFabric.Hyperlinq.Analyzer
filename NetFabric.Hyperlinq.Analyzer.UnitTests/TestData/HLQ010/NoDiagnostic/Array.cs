using System;

namespace HLQ010.NoDiagnostic.Array
{
    partial class C
    {
        void Method()
        {
            foreach (var item in new int[0])
                Console.WriteLine(item);
        }
    }
}
