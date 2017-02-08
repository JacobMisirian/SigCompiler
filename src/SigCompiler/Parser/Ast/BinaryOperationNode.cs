using System;

namespace SigCompiler.Parser.Ast
{
    public class BinaryOperationNode : AstNode
    {
        public override SourceLocation SourceLocation { get; set; }

        public AstNode Left { get; private set; }
        public AstNode Right { get; private set; }

        public BinaryOperation BinaryOperation { get; private set; }

        public BinaryOperationNode(SourceLocation location, BinaryOperation binaryOperation, AstNode left, AstNode right)
        {
            SourceLocation = location;

            Left = left;
            Right = right;

            BinaryOperation = binaryOperation;
        }

        public override void Visit(IVisitor visitor)
        {
            visitor.Accept(this);
        }
        public override void VisitChildren(IVisitor visitor)
        {
            Left.Visit(visitor);
            Right.Visit(visitor);
        }
    }

    public enum BinaryOperation
    {
        Assignment,
        Addition,
        Subtraction,
        Multiplication,
        Division,
        Modulus,
        Equality,
        Inequality,
        Greater,
        GreaterOrEqual,
        Lesser,
        LesserOrEqual,
        LogicalAnd,
        LogicalOr,
        BitwiseAnd,
        BitwiseOr,
        BitshiftLeft,
        BitshiftRight,
        Xor
    }
}

