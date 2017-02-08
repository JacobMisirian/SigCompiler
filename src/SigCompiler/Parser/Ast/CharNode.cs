using System;

namespace SigCompiler.Parser.Ast
{
    public class CharNode : AstNode
    {
        public override SourceLocation SourceLocation { get; set; }

        public Char Char { get; private set; }

        public CharNode(SourceLocation location, char _char)
        {
            SourceLocation = location;
            Char = _char;
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

