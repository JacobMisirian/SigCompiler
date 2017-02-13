using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SigCompiler.Parser.Ast
{
    public class ArrayNode : AstNode
    {
        public override SourceLocation SourceLocation { get; set; }

        public string Variable { get; private set; }
        public int Size { get; private set; }

        public ArrayNode(SourceLocation location, string variable, int size)
        {
            SourceLocation = location;

            Variable = variable;
            Size = size;
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
