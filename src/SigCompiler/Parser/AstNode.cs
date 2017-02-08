using System;

namespace SigCompiler.Parser
{
    public abstract class AstNode
    {
        public abstract SourceLocation SourceLocation { get; set; }
        public abstract void Visit(IVisitor visitor);
        public abstract void VisitChildren(IVisitor visitor);
    }
}

