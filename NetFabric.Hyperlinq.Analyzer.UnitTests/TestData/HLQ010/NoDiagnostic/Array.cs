using System;

namespace HLQ010.NoDiagnostic.Array
{
    partial class C
    {
        void Method()
        {
            var source = new int[0];
            foreach (var item in source)
                Console.WriteLine(item);
        }
    }
}
