using System;
using System.Collections.Generic;

namespace SigCompiler.Parser.Ast
{
    public class ArgumentListNode : AstNode
    {
        public override SourceLocation SourceLocation { get; set; }

        public List<AstNode> Arguments { get; private set; }

        public ArgumentListNode(SourceLocation location, params AstNode[] args)
        {
            SourceLocation = location;
            Arguments = new List<AstNode>(args);
        }

        public override void Visit(IVisitor visitor)
        {
            visitor.Accept(this);
        }
        public override void VisitChildren(IVisitor visitor)
        {
            foreach (var arg in Arguments)
                arg.Visit(visitor);
        }
    }
}

