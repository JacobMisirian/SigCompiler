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

        public static int GetSizeByType(SourceLocation location, string type)
        {
            if (!Types.ContainsKey(type))
                throw new CompilerException(location, "Unknown type {0}!", type);
            return Types[type];
        }
    }
}

