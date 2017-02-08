using System;
using System.Collections.Generic;

namespace SigCompiler.Parser.Ast
{
    public class FuncNode : AstNode
    {
        public override SourceLocation SourceLocation { get; set; }

        public AstNode Body { get; private set; }
        public List<LocalDeclarationNode> Parameters { get; private set; }

        public string Name { get; private set; }


        public FuncNode(SourceLocation location, string name, List<LocalDeclarationNode> parameters, AstNode body)
        {
            SourceLocation = location;

            Body = body;
            Parameters = parameters;

            Name = name;
        }

        public override void Visit(IVisitor visitor)
        {
            visitor.Accept(this);
        }
        public override void VisitChildren(IVisitor visitor)
        {
            Body.Visit(visitor);
        }
    }
}

