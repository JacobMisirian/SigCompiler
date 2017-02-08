using System;

namespace SigCompiler.Parser.Ast
{
    public class AttributeAccessNode : AstNode
    {
        public override SourceLocation SourceLocation { get; set; }

        public AstNode Left { get; private set; }

        public string Right { get; private set; }

        public AttributeAccessNode(SourceLocation location, AstNode left, string right)
        {
            SourceLocation = location;

            Left = left;

            Right = right;
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

