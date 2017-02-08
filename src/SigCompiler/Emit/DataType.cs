using System;
using System.Collections.Generic;

namespace SigCompiler.Emit
{
    public class DataType
    {
        public static Dictionary<string, int> Types = new Dictionary<string, int>()
        {
            { "byte", 1 },
            { "short", 2 },
            { "ptr", 2 }
        };
    }
}

