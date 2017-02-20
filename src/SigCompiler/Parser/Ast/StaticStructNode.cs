using System;
using System.Collections.Generic;

namespace SigCompiler.Parser.Ast
{
    public class StaticStructNode : AstNode
    {
        public override SourceLocation SourceLocation { get; set; }

        public string Name { get; private set; }
        public Dictionary<string, string> Members { get; private set; }

        public StaticStructNode(SourceLocation location, string name, Dictionary<string, string> members)
        {
            SourceLocation = location;

            Name = name;
            Members = members;
        }

        public override void Visit(IVisitor visitor)
        {
            visitor.Accept(this);
        }
        public override void VisitChildren(IVisitor visitor)
        {

        }
    }
}

