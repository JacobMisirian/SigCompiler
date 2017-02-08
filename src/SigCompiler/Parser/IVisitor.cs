using System;

using SigCompiler.Parser.Ast;

namespace SigCompiler.Parser
{
    public interface IVisitor
    {
        void Accept(ArgumentListNode node);
        void Accept(AttributeAccessNode node);
        void Accept(BinaryOperationNode node);
        void Accept(CharNode node);
        void Accept(CodeBlockNode node);
        void Accept(ExpressionStatementNode node);
        void Accept(FuncNode node);
        void Accept(FunctionCallNode node);
        void Accept(IdentifierNode node);
        void Accept(IfNode node);
        void Accept(IndexerNode node);
        void Accept(IntegerNode node);
        void Accept(LocalDeclarationNode node);
        void Accept(ReturnNode node);
        void Accept(StringNode node);
        void Accept(UnaryOperationNode node);
        void Accept(WhileNode node);
    }
}

