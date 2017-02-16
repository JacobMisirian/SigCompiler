    using System;
using System.Collections.Generic;
using System.Text;

using SigCompiler.Parser;
using SigCompiler.Parser.Ast;

namespace SigCompiler.Emit
{
    public class Compiler : IVisitor
    {
        private const int BP_INITIAL = 15000;

        private const string BP = "h";
        private const string FLAGS = "p";

        private SymbolTable table;

        private StringBuilder output;
        private Dictionary<string, string> strings;

        public string Compile(AstNode ast)
        {
            table = new SymbolTable();

            output = new StringBuilder();
            strings = new Dictionary<string, string>();

            append("li {0}, {1}", BP, BP_INITIAL);
            foreach (var child in ((CodeBlockNode)ast).Children)
                if (child is StaticVariableNode)
                    child.Visit(this);
            append("call .main");
            string end = nextSymbol();
            append("jmp .end{0}", end);
            foreach (var child in ((CodeBlockNode)ast).Children)
                if (child is FuncNode   )
                    child.Visit(this);

            foreach (var pair in strings)
            {
                append(".{0}", pair.Key);
                append("STRING \"{0}\"", pair.Value);
            }

            append(".end{0}", end);

            return output.ToString();
        }

        public void Accept(ArgumentListNode node)
        {
            for (int i = node.Arguments.Count - 1; i >= 0; i--)
                node.Arguments[i].Visit(this);
        }
        public void Accept(ArrayNode node)
        {
            table.AddSymbol(node.Variable, DataType.GetSizeByType(node.SourceLocation, "ptr"));
            table.CurrentOffset += node.Size;
            loadLocalPtr(node.SourceLocation, node.Variable, "a");
            append("push a");
            storeLocal(node.SourceLocation, node.Variable, "a");
        }
        public void Accept(AttributeAccessNode node)
        {
        }
        public void Accept(BinaryOperationNode node)
        {
            if (node.BinaryOperation != BinaryOperation.Assignment)
            {
                node.Right.Visit(this);
                node.Left.Visit(this);
                append("pop a");
                append("pop b");
            }

            switch (node.BinaryOperation)
            {
                case BinaryOperation.Assignment:
                    node.Right.Visit(this);
                    if (node.Left is IdentifierNode)
                    {
                        storeLocal(node.Left.SourceLocation, ((IdentifierNode)node.Left).Identifier, "b");
                        append("push b");
                    }
                    else if (node.Left is IndexerNode)
                    {
                        append("pop c");
                        var indexer = node.Left as IndexerNode;
                        indexer.Index.Visit(this);
                        if (indexer.Target is IdentifierNode)
                            indexer.Target.Visit(this);
                        append("pop a");
                        append("pop b");
                        append("add a, b");
                        append("stob a, c");
                        append("push c");
                    }
                    break;
                case BinaryOperation.Addition:
                    append("add a, b");
                    append("push a");
                    break;
                case BinaryOperation.Subtraction:
                    append("sub a, b");
                    append("push a");
                    break;
                case BinaryOperation.Multiplication:
                    append("mul a, b");
                    append("push a");
                    break;
                case BinaryOperation.Division:
                    append("div a, b");
                    append("push a");
                    break;
                case BinaryOperation.Modulus:
                    append("mod a, b");
                    append("push a");
                    break;
                case BinaryOperation.BitshiftLeft:
                    append("shil a, b");
                    append("push a");
                    break;
                case BinaryOperation.BitshiftRight:
                    append("shir a, b");
                    append("push a");
                    break;
                case BinaryOperation.BitwiseAnd:
                    append("and a, b");
                    append("push a");
                    break;
                case BinaryOperation.BitwiseOr:
                    append("or a, b");
                    append("push a");
                    break;
                case BinaryOperation.Xor:
                    append("xor a, b");
                    append("push a");
                    break;
                case BinaryOperation.LogicalAnd:
                    append("and a, b");
                    append("neqi a, 0");
                    append("push a");
                    break;
                case BinaryOperation.LogicalOr:

                    break;
                case BinaryOperation.Equality:
                    append("eq a, b");
                    append("push a");
                    break;
                case BinaryOperation.Inequality:
                    append("neq a, b");
                    append("push a");
                    break;
                case BinaryOperation.Greater:
                    append("gt a, b");
                    append("push a");
                    break;
                case BinaryOperation.GreaterOrEqual:
                    append("gte a, b");
                    append("push a");
                    break;
                case BinaryOperation.Lesser:
                    append("lt a, b");
                    append("push a");
                    break;
                case BinaryOperation.LesserOrEqual:
                    append("lte a, b");
                    append("push a");
                    break;
            }
        }
        public void Accept(CharNode node)
        {
            append("pushi {0}", (int)node.Char);
        }
        public void Accept(CodeBlockNode node)
        {
            table.PushScope();
            node.VisitChildren(this);
            table.PopScope();
        }
        public void Accept(ExpressionStatementNode node)
        {
            node.Expression.Visit(this);
            append("popv");
        }
        public void Accept(FuncNode node)
        {
            int preFuncLocals = table.CurrentOffset;
            table.PushScope();

            append(".{0}", node.Name);
            append("pop c");
            append("subi {0}, {1}", BP, preFuncLocals);
           
            for (int i = 0; i < node.Parameters.Count; i++)
            {
                table.AddSymbol(node.Parameters[i].Variable, DataType.GetSizeByType(node.Parameters[i].SourceLocation, node.Parameters[i].Type));
                storeLocal(node.Parameters[i].SourceLocation, node.Parameters[i].Variable, "b");
            }

            append("push c");

            append("addi {0}, {1}", BP, preFuncLocals);
            append("push {0}", BP);
            append("subi {0}, {1}", BP, preFuncLocals);

            node.VisitChildren(this);
            table.PopScope();

            append("pop {0}", BP);
            append("reti 0");
        }
        public void Accept(FunctionCallNode node)
        {
            node.Arguments.Visit(this);
            string func = ((IdentifierNode)node.Target).Identifier;
            append("call .{0}", func, node.Arguments.Arguments.Count);
        }
        public void Accept(IdentifierNode node)
        {
            if (table.ContainsGlobalSymbol(node.SourceLocation, node.Identifier))
                loadStatic(node.SourceLocation, node.Identifier);
            else
                loadLocal(node.SourceLocation, node.Identifier);
        }
        public void Accept(IfNode node)
        {
            string elseBody = nextSymbol();
            string end = nextSymbol();

            node.Condition.Visit(this);
            append("pop a");
            append("cmpi a, 0");
            append("jie .{0}", elseBody);
            node.IfBody.Visit(this);
            append("jmp .{0}", end);
            append(".{0}", elseBody);
            if (node.ElseBody != null)
                node.ElseBody.Visit(this);
            append(".{0}", end);
        }
        public void Accept(IndexerNode node)
        {
            node.Index.Visit(this);
            node.Target.Visit(this);
            append("pop a");
            append("pop b");
            append("add a, b");
            append("loadb a, a");
            append("push a");
        }
        public void Accept(IntegerNode node)
        {
            append("pushi {0}", node.Integer);
        }
        public void Accept(LocalDeclarationNode node)
        {
            table.AddSymbol(node.Variable, DataType.GetSizeByType(node.SourceLocation, node.Type));
            if (node.InitialValue != null)
            {
                node.InitialValue.Visit(this);
                storeLocal(node.InitialValue.SourceLocation, node.Variable, "b");
            }
        }
        public void Accept(ReturnNode node)
        {
            node.VisitChildren(this);
            append("pop a");
            append("pop {0}", BP);
            append("ret a");
        }
        public void Accept(StaticVariableNode node)
        {
            table.AddGlobalSymbol(node.SourceLocation, node.Variable, DataType.GetSizeByType(node.SourceLocation, node.Type));
            if (node.Expression != null)
            {
                node.Expression.Visit(this);
                storeStatic(node.SourceLocation, node.Variable, "a", "b");
            }

        }
        public void Accept(StringNode node)
        {
            string symbol = nextSymbol();
            append("pushi .{0}", symbol);
            strings.Add(symbol, node.String);
        }
        public void Accept(UnaryOperationNode node)
        {
            switch (node.UnaryOperation)
            {
                case UnaryOperation.Dereference:
                    loadLocalPtr(node.SourceLocation, ((IdentifierNode)node.Target).Identifier, "a");
                    append("push a");
                    break;
                case UnaryOperation.Reference:
                    loadLocal(node.SourceLocation, ((IdentifierNode)node.Target).Identifier);
                    append("pop a");
                    
                    break;
            }
        }
        public void Accept(WhileNode node)
        {
            string whileBody = nextSymbol();
            string end = nextSymbol();

            append(".{0}", whileBody);
            node.Condition.Visit(this);
            append("pop a");
            append("cmpi a, 0");
            append("jie .{0}", end);
            node.Body.Visit(this);
            append("jmp .{0}", whileBody);
            append(".{0}", end);
        }

        private void storeLocal(SourceLocation location, string variable, string register)
        {
            append("pop {0}", register);
            loadLocalPtr(location, variable, "a");
            switch (table.GetSize(location, variable))
            {
                case 1:
                    append("stob a, {0}", register);
                    break;
                case 2:
                    append("stow a, {0}", register);
                    break;
            }
        }

        private void loadLocal(SourceLocation location, string variable)
        {
            loadLocalPtr(location, variable, "a");

            switch (table.GetSize(location, variable))
            {
                case 1:
                    append("loadb a, a");
                    break;
                case 2:
                    append("loadw a, a");
                    break;
            }
            append("push a");
        }

        private void storeStatic(SourceLocation location, string variable, string register1, string register2)
        {
            append("pop {0}", register1);
            append("li {0}, {1}", register2, getStaticPtr(location, variable));
            
            switch (table.GetGlobalSize(location, variable))
            {
                case 1:
                    append("stob {0}, {1}", register2, register1);
                    break;
                case 2:
                    append("stow {0}, {1}", register2, register1);
                    break;
            }
        }

        private void loadStatic(SourceLocation location, string variable)
        {
            append("li a, {0}", getStaticPtr(location, variable));

            switch (table.GetGlobalSize(location, variable))
            {
                case 1:
                    append("loadb a, a");
                    break;
                case 2:
                    append("loadw a, a");
                    break;
            }
            append("push a");
        }

        private void loadLocalPtr(SourceLocation location, string variable, string register)
        {
            append("mov {0}, {1}", register, BP);
            append("subi {0}, {1}", register, table.GetOffset(location, variable));
        }

        private int getStaticPtr(SourceLocation location, string variable)
        {
            return BP_INITIAL + table.GetGlobalOffset(location, variable);
        }

        private void append(string strf, params object[] args)
        {
            output.AppendFormat(strf + "\n", args);
        }

        private int currentSymbol = 0;
        private string nextSymbol()
        {
            return "L" + currentSymbol++.ToString();
        }
    }
}