using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SigCompiler.Parser.Ast
{
    public class StaticVariableNode : AstNode
    {
        public override SourceLocation SourceLocation { get; set; }

        public string Type { get; private set; }
        public string Variable { get; private set; }
        
        public AstNode Expression { get; private set; }

        public StaticVariableNode(SourceLocation location, string type, string variable, AstNode expression = null)
        {
            SourceLocation = location;

            Type = type;
            Variable = variable;

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
