using System;
using System.Collections.Generic;

namespace SigCompiler.Parser.Ast
{
    public class CodeBlockNode : AstNode
    {
        public override SourceLocation SourceLocation { get; set; }

        public List<AstNode> Children { get; private set; }

        public CodeBlockNode(SourceLocation location)
        {
            SourceLocation = location;

            Children = new List<AstNode>();
        }

        public override void Visit(IVisitor visitor)
        {
            visitor.Accept(this);
        }
        public override void VisitChildren(IVisitor visitor)
        {
            foreach (var child in Children)
                child.Visit(visitor);
        }
    }
}

