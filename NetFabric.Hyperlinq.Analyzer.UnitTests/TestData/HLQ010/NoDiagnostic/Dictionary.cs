﻿using System;
using System.Collections.Generic;

namespace HLQ010.NoDiagnostic.Dictionary
{
    partial class C
    {
        void Method()
        {
            var source = new Dictionary<string, string>();
            foreach (var item in source)
                Console.WriteLine(item);
        }
    }
}
