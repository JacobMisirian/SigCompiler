using System;

namespace SigCompiler.Parser.Ast
{
    public class UnaryOperationNode : AstNode
    {
        public override SourceLocation SourceLocation { get; set; }

        public AstNode Target { get; private set; }

        public UnaryOperation UnaryOperation { get; private set; }

        public UnaryOperationNode(SourceLocation location, UnaryOperation unaryOperation, AstNode target)
        {
            SourceLocation = location;

            Target = target;

            UnaryOperation = unaryOperation;
        }

        public override void Visit(IVisitor visitor)
        {
            visitor.Accept(this);
        }
        public override void VisitChildren(IVisitor visitor)
        {
            Target.Visit(visitor);
        }
    }

    public enum UnaryOperation
    {
        BitwiseNot,
        Dereference,
        LogicalNot,
        Negate,
        PostDecrement,
        PostIncrement,
        PreDecrement,
        PreIncrement,
        Reference
    }
}

