using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SigCompiler.Emit
{
    public class Struct
    {
        public string Name { get; private set; }
        public bool IsStatic { get; private set; }
        public int Size { get; private set; }

        private Dictionary<string, int> memberOffsets = new Dictionary<string, int>();
        private Dictionary<string, int> memberSizes = new Dictionary<string, int>();

        public Struct(string name, bool isStatic)
        {
            Name = name;
            IsStatic = isStatic;
        }

        public void AddMember(string name, int size)
        {
            memberOffsets.Add(name, Size);
            memberSizes.Add(name, size);
            Size += size;
        }

        public bool ContainsMember(string member)
        {
            return memberOffsets.ContainsKey(member);
        }

        public int GetOffset(string name)
        {
            return memberOffsets[name];
        }

        public int GetSize(string name)
        {
            return memberSizes[name];
        }
    }
}
