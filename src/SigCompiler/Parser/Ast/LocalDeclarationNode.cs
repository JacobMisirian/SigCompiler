using System;

namespace SigCompiler.Parser.Ast
{
    public class LocalDeclarationNode : AstNode
    {
        public override SourceLocation SourceLocation { get; set; }
        
        public AstNode InitialValue { get; private set; }

        public string Type { get; private set; }
        public string Variable { get; private set; }

        public LocalDeclarationNode(SourceLocation location, string type, string variable, AstNode initialValue = null)
        {
            SourceLocation = location;

            InitialValue = initialValue;

            Type = type;
            Variable = variable;
        }

        public override void Visit(IVisitor visitor)
        {
            visitor.Accept(this);
        }
        public override void VisitChildren(IVisitor visitor)
        {
            if (InitialValue != null)
                InitialValue.Visit(visitor);
        }
    }
}

