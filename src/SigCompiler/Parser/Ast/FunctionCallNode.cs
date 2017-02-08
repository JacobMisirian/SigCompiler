using System;

namespace SigCompiler.Parser.Ast
{
    public class FunctionCallNode : AstNode
    {
        public override SourceLocation SourceLocation { get; set; }

        public AstNode Target { get; private set; }
        public ArgumentListNode Arguments { get; private set; }

        public FunctionCallNode(SourceLocation location, AstNode target, ArgumentListNode arguments)
        {
            SourceLocation = location;

            Target = target;
            Arguments = arguments;
        }

        public override void Visit(IVisitor visitor)
        {
            visitor.Accept(this);
        }
        public override void VisitChildren(IVisitor visitor)
        {
            Target.Visit(visitor);
            Arguments.Visit(visitor);
        }
    }
}

