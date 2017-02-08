using System;

namespace SigCompiler.Parser.Ast
{
    public class ReturnNode : AstNode
    {
        public override SourceLocation SourceLocation { get; set; }

        public AstNode Expression { get; private set; }

        public bool IsVoidReturn { get { return Expression == null; } }

        public ReturnNode(SourceLocation location, AstNode expression = null)
        {
            SourceLocation = location;

            Expression = expression;
        }

        public override void Visit(IVisitor visitor)
        {
            visitor.Accept(this);
        }
        public override void VisitChildren(IVisitor visitor)
        {
            Expression.Visit(visitor);
        }
    }
}

