using System;

namespace SigCompiler.Parser.Ast
{
    public class IntegerNode : AstNode
    {
        public override SourceLocation SourceLocation { get; set; }

        public int Integer { get; private set; }

        public IntegerNode(SourceLocation location, int integer)
        {
            SourceLocation = location;

            Integer = integer;
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

