using System;

namespace SigCompiler.Parser.Ast
{
    public class IfNode : AstNode
    {
        public override SourceLocation SourceLocation { get; set; }

        public AstNode Condition { get; private set; }
        public AstNode IfBody { get; private set; }
        public AstNode ElseBody { get; private set; }

        public bool HasElseBody { get { return ElseBody != null; } }

        public IfNode(SourceLocation location, AstNode condition, AstNode ifBody, AstNode elseBody = null)
        {
            SourceLocation = location;

            Condition = condition;
            IfBody = ifBody;
            ElseBody = elseBody;
        }

        public override void Visit(IVisitor visitor)
        {
            visitor.Accept(this);
        }
        public override void VisitChildren(IVisitor visitor)
        {
            Condition.Visit(visitor);
            IfBody.Visit(visitor);
            ElseBody.Visit(visitor);
        }
    }
}

