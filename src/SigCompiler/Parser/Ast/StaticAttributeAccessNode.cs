using System;

namespace SigCompiler.Parser.Ast
{
    public class StaticAttributeAccessNode : AstNode
    {
        public override SourceLocation SourceLocation { get; set; }

        public string Left { get; private set; }

        public string Right { get; private set; }

        public StaticAttributeAccessNode(SourceLocation location, string left, string right)
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

