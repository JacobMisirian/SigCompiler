using System;
using System.Collections.Generic;

using SigCompiler.Parser.Ast;
using SigCompiler.Scanner;

namespace SigCompiler.Parser
{
    public class SigParser
    {
        public SourceLocation Location { get { return tokens[position].SourceLocation; } }
        private List<Token> tokens;
        private int position;

        public SigParser(List<Token> tokens)
        {
            this.tokens = tokens;
            position = 0;
        }

        public AstNode Parse()
        {
            CodeBlockNode code = new CodeBlockNode(Location);
            while (position < tokens.Count)
                code.Children.Add(parseStatement());
            return code;
        }

        private AstNode parseStatement()
        {
            if (matchToken(TokenType.Identifier, "array"))
                return parseArrayDeclaration();
            else if (matchToken(TokenType.Identifier, "if"))
                return parseIf();
            else if (matchToken(TokenType.Identifier, "func"))
                return parseFunc();
            else if (matchToken(TokenType.Identifier, "return"))
                return parseReturn();
            else if (matchToken(TokenType.Identifier, "static"))
                return parseStatic();
            else if (matchToken(TokenType.Identifier, "while"))
                return parseWhile();
            else if (acceptToken(TokenType.OpenBracket))
            {
                CodeBlockNode codeBlock = new CodeBlockNode(Location);
                while (!acceptToken(TokenType.CloseBracket))
                    codeBlock.Children.Add(parseStatement());
                return codeBlock;
            }
            else if (matchToken(TokenType.Identifier) && tokens[position + 1].TokenType == TokenType.Identifier)
                return parseLocalDeclaration();
            else
                return parseExpressionStatement();
        }

        private ArgumentListNode parseArgumentList()
        {
            expectToken(TokenType.OpenParentheses);
            var args = new List<AstNode>();
            while (!acceptToken(TokenType.CloseParentheses))
            {
                args.Add(parseExpression());
                acceptToken(TokenType.Comma);
            }
            return new ArgumentListNode(Location, args.ToArray());
        }
        private ArrayNode parseArrayDeclaration()
        {
            expectToken(TokenType.Identifier, "array");
            expectToken(TokenType.OpenBrace);
            int size = Convert.ToInt32(expectToken(TokenType.Integer).Value);
            expectToken(TokenType.CloseBrace);

            string variable = expectToken(TokenType.Identifier).Value;

            return new ArrayNode(Location, variable, size);
        }
        private FuncNode parseFunc()
        {
            var location = Location;
            expectToken(TokenType.Identifier, "func");
            string name = expectToken(TokenType.Identifier).Value;
            expectToken(TokenType.OpenParentheses);
            List<LocalDeclarationNode> parameters = new List<LocalDeclarationNode>();
            while (!acceptToken(TokenType.CloseParentheses))
            {
                parameters.Add(parseLocalDeclaration());
                acceptToken(TokenType.Comma);
            }

            AstNode body = parseStatement();
            return new FuncNode(location, name, parameters, body);
        }
        private IfNode parseIf()
        {
            var location = Location;
            expectToken(TokenType.Identifier, "if");
            expectToken(TokenType.OpenParentheses);
            AstNode condition = parseExpression();
            expectToken(TokenType.CloseParentheses);
            AstNode ifBody = parseStatement();
            if (acceptToken(TokenType.Identifier, "else"))
                return new IfNode(location, condition, ifBody, parseStatement());
            return new IfNode(location, condition, ifBody);
        }
        private LocalDeclarationNode parseLocalDeclaration()
        {
            string type = expectToken(TokenType.Identifier).Value;
            var location = Location;
            string variable = expectToken(TokenType.Identifier).Value;

            if (acceptToken(TokenType.Assignment, "="))
                return new LocalDeclarationNode(location, type, variable, parseExpression());
            return new LocalDeclarationNode(location, type, variable);
        }
        private ReturnNode parseReturn()
        {
            expectToken(TokenType.Identifier, "return");
            if (acceptToken(TokenType.Semicolon))
                return new ReturnNode(Location);
            return new ReturnNode(Location, parseExpression());
        }
        private AstNode parseStatic()
        {
            expectToken(TokenType.Identifier, "static");
            if (matchToken(TokenType.Identifier, "struct"))
                return parseStaticStruct();
            return parseStaticVariable();
        }
        private StaticStructNode parseStaticStruct()
        {
            var location = Location;
            expectToken(TokenType.Identifier, "struct");
            string name = expectToken(TokenType.Identifier).Value;
            expectToken(TokenType.OpenBracket);
            Dictionary<string, string> members = new Dictionary<string, string>();
            while (!acceptToken(TokenType.CloseBracket))
            {
                string type = expectToken(TokenType.Identifier).Value;
                string _name = expectToken(TokenType.Identifier).Value;
                acceptToken(TokenType.Semicolon);
                members.Add(_name, type);
            }

            return new StaticStructNode(location, name, members);
        }
        private StaticVariableNode parseStaticVariable()
        {
            string type = expectToken(TokenType.Identifier).Value;
            string variable = expectToken(TokenType.Identifier).Value;

            if (acceptToken(TokenType.Assignment))
                return new StaticVariableNode(Location, type, variable, parseExpression());
            return new StaticVariableNode(Location, type, variable);
        }
        private WhileNode parseWhile()
        {
            var location = Location;
            expectToken(TokenType.Identifier, "while");
            expectToken(TokenType.OpenParentheses);
            AstNode condition = parseExpression();
            expectToken(TokenType.CloseParentheses);
            AstNode body = parseStatement();

            return new WhileNode(location, condition, body);
        }

        private AstNode parseExpressionStatement()
        {
            var expr = parseExpression();
            acceptToken(TokenType.Semicolon);
            if (expr is FunctionCallNode || expr is BinaryOperationNode || expr is UnaryOperationNode)
                return new ExpressionStatementNode(Location, expr);
            return expr;
        }
        private AstNode parseExpression()
        {
            return parseAssignment();
        }
        private AstNode parseAssignment()
        {
            AstNode left = parseLogicalOr();
            if (matchToken(TokenType.Assignment))
            {
                switch (tokens[position].Value)
                {
                    case "=":
                        acceptToken(TokenType.Assignment);
                        return new BinaryOperationNode(Location, BinaryOperation.Assignment, left, parseAssignment());
                    case "+=":
                        acceptToken(TokenType.Assignment);
                        return new BinaryOperationNode(Location, BinaryOperation.Assignment, left, new BinaryOperationNode(Location, BinaryOperation.Addition, left, parseAssignment()));
                    case "-=":
                        acceptToken(TokenType.Assignment);
                        return new BinaryOperationNode(Location, BinaryOperation.Assignment, left, new BinaryOperationNode(Location, BinaryOperation.Subtraction, left, parseAssignment()));
                    case "*=":
                        acceptToken(TokenType.Assignment);
                        return new BinaryOperationNode(Location, BinaryOperation.Assignment, left, new BinaryOperationNode(Location, BinaryOperation.Multiplication, left, parseAssignment()));
                    case "/=":
                        acceptToken(TokenType.Assignment);
                        return new BinaryOperationNode(Location, BinaryOperation.Assignment, left, new BinaryOperationNode(Location, BinaryOperation.Division, left, parseAssignment()));
                    case "%=":
                        acceptToken(TokenType.Assignment);
                        return new BinaryOperationNode(Location, BinaryOperation.Assignment, left, new BinaryOperationNode(Location, BinaryOperation.Modulus, left, parseAssignment()));
                    case "<<=":
                        acceptToken(TokenType.Assignment);
                        return new BinaryOperationNode(Location, BinaryOperation.Assignment, left, new BinaryOperationNode(Location, BinaryOperation.BitshiftLeft, left, parseAssignment()));
                    case ">>=":
                        acceptToken(TokenType.Assignment);
                        return new BinaryOperationNode(Location, BinaryOperation.Assignment, left, new BinaryOperationNode(Location, BinaryOperation.BitshiftRight, left, parseAssignment()));
                    case "&=":
                        acceptToken(TokenType.Assignment);
                        return new BinaryOperationNode(Location, BinaryOperation.Assignment, left, new BinaryOperationNode(Location, BinaryOperation.LogicalAnd, left, parseAssignment()));
                    case "|=":
                        acceptToken(TokenType.Assignment);
                        return new BinaryOperationNode(Location, BinaryOperation.Assignment, left, new BinaryOperationNode(Location, BinaryOperation.LogicalOr, left, parseAssignment()));
                    case "^=":
                        acceptToken(TokenType.Assignment);
                        return new BinaryOperationNode(Location, BinaryOperation.Assignment, left, new BinaryOperationNode(Location, BinaryOperation.Xor, left, parseAssignment()));
                    default:
                        break;
                }
            }
            return left;
        }
        private AstNode parseLogicalOr()
        {
            AstNode left = parseLogicalAnd();
            while (acceptToken(TokenType.Operation, "||"))
                left = new BinaryOperationNode(Location, BinaryOperation.LogicalOr, left, parseLogicalOr());
            return left;
        }
        private AstNode parseLogicalAnd()
        {
            AstNode left = parseEquality();
            while (acceptToken(TokenType.Operation, "&&"))
                left = new BinaryOperationNode(Location, BinaryOperation.LogicalAnd, left, parseLogicalAnd());
            return left;
        }
        private AstNode parseEquality()
        {
            AstNode left = parseComparison();
            AstNode expr;
            while (matchToken(TokenType.Comparison))
            {
                switch (tokens[position].Value)
                {
                    case "==":
                        acceptToken(TokenType.Comparison);
                        expr = parseComparison();
                        if (matchToken(TokenType.Comparison))
                            return new BinaryOperationNode(Location, BinaryOperation.LogicalAnd, new BinaryOperationNode(Location, BinaryOperation.Equality, left, expr), new BinaryOperationNode(Location, stringToBinaryOperation(expectToken(TokenType.Comparison).Value), expr, parseOr()));
                        return new BinaryOperationNode(Location, BinaryOperation.Equality, left, expr);
                    case "!=":
                        acceptToken(TokenType.Comparison);
                        expr = parseComparison();
                        if (matchToken(TokenType.Comparison))
                            return new BinaryOperationNode(Location, BinaryOperation.LogicalAnd, new BinaryOperationNode(Location, BinaryOperation.Inequality, left, expr), new BinaryOperationNode(Location, stringToBinaryOperation(expectToken(TokenType.Comparison).Value), expr, parseOr()));
                        return new BinaryOperationNode(Location, BinaryOperation.Inequality, left, expr);
                    default:
                        break;
                }
                break;
            }
            return left;
        }
        private AstNode parseComparison()
        {
            AstNode left = parseXor();
            AstNode expr;
            while (matchToken(TokenType.Comparison))
            {
                switch (tokens[position].Value)
                {
                    case ">":
                        acceptToken(TokenType.Comparison);
                        expr = parseOr();
                        if (matchToken(TokenType.Comparison))
                            return new BinaryOperationNode(Location, BinaryOperation.LogicalAnd, new BinaryOperationNode(Location, BinaryOperation.Greater, left, expr), new BinaryOperationNode(Location, stringToBinaryOperation(expectToken(TokenType.Comparison).Value), expr, parseOr()));
                        return new BinaryOperationNode(Location, BinaryOperation.Greater, left, expr);
                    case ">=":
                        acceptToken(TokenType.Comparison);
                        expr = parseOr();
                        if (matchToken(TokenType.Comparison))
                            return new BinaryOperationNode(Location, BinaryOperation.LogicalAnd, new BinaryOperationNode(Location, BinaryOperation.GreaterOrEqual, left, expr), new BinaryOperationNode(Location, stringToBinaryOperation(expectToken(TokenType.Comparison).Value), expr, parseOr()));
                        return new BinaryOperationNode(Location, BinaryOperation.GreaterOrEqual, left, expr);
                    case "<":
                        acceptToken(TokenType.Comparison);
                        expr = parseOr();
                        if (matchToken(TokenType.Comparison))
                            return new BinaryOperationNode(Location, BinaryOperation.LogicalAnd, new BinaryOperationNode(Location, BinaryOperation.Lesser, left, expr), new BinaryOperationNode(Location, stringToBinaryOperation(expectToken(TokenType.Comparison).Value), expr, parseOr()));
                        return new BinaryOperationNode(Location, BinaryOperation.Lesser, left, expr);
                    case "<=":
                        acceptToken(TokenType.Comparison);
                        expr = parseOr();
                        if (matchToken(TokenType.Comparison))
                            return new BinaryOperationNode(Location, BinaryOperation.LogicalAnd, new BinaryOperationNode(Location, BinaryOperation.LesserOrEqual, left, expr), new BinaryOperationNode(Location, stringToBinaryOperation(expectToken(TokenType.Comparison).Value), expr, parseOr()));
                        return new BinaryOperationNode(Location, BinaryOperation.LesserOrEqual, left, expr);
                    default:
                        break;
                }
                break;
            }
            return left;
        }
        private AstNode parseOr()
        {
            AstNode left = parseXor();
            while (acceptToken(TokenType.Operation, "|"))
                left = new BinaryOperationNode(Location, BinaryOperation.BitwiseOr, left, parseOr());
            return left;
        }
        private AstNode parseXor()
        {
            AstNode left = parseAnd();
            while (acceptToken(TokenType.Operation, "^"))
                left = new BinaryOperationNode(Location, BinaryOperation.Xor, left, parseXor());
            return left;
        }
        private AstNode parseAnd()
        {
            AstNode left = parseBitshift();
            while (acceptToken(TokenType.Operation, "&"))
                left = new BinaryOperationNode(Location, BinaryOperation.BitwiseAnd, left, parseAnd());
            return left;
        }
        private AstNode parseBitshift()
        {
            AstNode left = parseAdditive();
            while (matchToken(TokenType.Operation))
            {
                switch (tokens[position].Value)
                {
                    case "<<":
                        acceptToken(TokenType.Operation);
                        return new BinaryOperationNode(Location, BinaryOperation.BitshiftLeft, left, parseBitshift());
                    case ">>":
                        acceptToken(TokenType.Operation);
                        return new BinaryOperationNode(Location, BinaryOperation.BitshiftRight, left, parseBitshift());
                    default:
                        break;
                }
                break;
            }
            return left;
        }
        private AstNode parseAdditive()
        {
            AstNode left = parseMultiplicative();
            while (matchToken(TokenType.Operation))
            {
                switch (tokens[position].Value)
                {
                    case "+":
                        acceptToken(TokenType.Operation);
                        return new BinaryOperationNode(Location, BinaryOperation.Addition, left, parseAdditive());
                    case "-":
                        acceptToken(TokenType.Operation);
                        return new BinaryOperationNode(Location, BinaryOperation.Subtraction, left, parseAdditive());
                    default:
                        break;
                }
                break;
            }
            return left;
        }
        private AstNode parseMultiplicative()
        {
            AstNode left = parseUnary();
            while (matchToken(TokenType.Operation))
            {
                switch (tokens[position].Value)
                {
                    case "*":
                        acceptToken(TokenType.Operation);
                        return new BinaryOperationNode(Location, BinaryOperation.Multiplication, left, parseMultiplicative());
                    case "/":
                        acceptToken(TokenType.Operation);
                        return new BinaryOperationNode(Location, BinaryOperation.Division, left, parseMultiplicative());
                    case "%":
                        acceptToken(TokenType.Operation);
                        return new BinaryOperationNode(Location, BinaryOperation.Modulus, left, parseMultiplicative());
                    default:
                        break;
                }
                break;
            }
            return left;
        }
        private AstNode parseUnary()
        {
            if (matchToken(TokenType.Operation))
            {
                switch (tokens[position].Value)
                {
                    case "~":
                        acceptToken(TokenType.Operation);
                        return new UnaryOperationNode(Location, UnaryOperation.BitwiseNot, parseUnary());
                    case "!":
                        acceptToken(TokenType.Operation);
                        return new UnaryOperationNode(Location, UnaryOperation.LogicalNot, parseUnary());
                    case "-":
                        acceptToken(TokenType.Operation);
                        return new UnaryOperationNode(Location, UnaryOperation.Negate, parseUnary());
                    case "--":
                        acceptToken(TokenType.Operation);
                        return new UnaryOperationNode(Location, UnaryOperation.PreDecrement, parseUnary());
                    case "++":
                        acceptToken(TokenType.Operation);
                        return new UnaryOperationNode(Location, UnaryOperation.PreIncrement, parseUnary());
                    case "&":
                        acceptToken(TokenType.Operation);
                        return new UnaryOperationNode(Location, UnaryOperation.Dereference, parseUnary());
                }
            }
            return parseAccess();
        }
        private AstNode parseAccess()
        {
            return parseAccess(parseTerm());
        }
        private AstNode parseAccess(AstNode left)
        {
            if (matchToken(TokenType.OpenParentheses))
                return parseAccess(new FunctionCallNode(Location, left, parseArgumentList()));
            else if (acceptToken(TokenType.OpenBrace))
            {
                var expr = parseExpression();
                expectToken(TokenType.CloseBrace);
                return new IndexerNode(Location, left, expr);
            }
            else if (acceptToken(TokenType.Operation, "--"))
                return new UnaryOperationNode(Location, UnaryOperation.PostDecrement, left);
            else if (acceptToken(TokenType.Operation, "++"))
                return new UnaryOperationNode(Location, UnaryOperation.PostIncrement, left);
            else if (acceptToken(TokenType.Dot))
                return new StaticAttributeAccessNode(Location, ((IdentifierNode)left).Identifier, expectToken(TokenType.Identifier).Value);
            return left;
        }
        private AstNode parseTerm()
        {
            if (acceptToken(TokenType.OpenParentheses))
            {
                var expr = parseExpression();
                expectToken(TokenType.CloseParentheses);
                return expr;
            }
            else if (matchToken(TokenType.Char))
                return new CharNode(Location, Convert.ToChar(expectToken(TokenType.Char).Value));
            else if (matchToken(TokenType.Identifier))
                return new IdentifierNode(Location, expectToken(TokenType.Identifier).Value);
            else if (matchToken(TokenType.Integer))
                return new IntegerNode(Location, Convert.ToInt32(expectToken(TokenType.Integer).Value));
            else if (matchToken(TokenType.String))
                return new StringNode(Location, expectToken(TokenType.String).Value);
            else if (acceptToken(TokenType.Semicolon))
                return new CodeBlockNode(Location);
            else
                throw new CompilerException(tokens[position].SourceLocation, "Unexpected token {0} at {1}", tokens[position], Location);
        }

        private bool matchToken(TokenType tokenType)
        {
            return tokens[position].TokenType == tokenType;
        }
        private bool matchToken(TokenType tokenType, string value)
        {
            return tokens[position].TokenType == tokenType && tokens[position].Value == value;
        }

        private bool acceptToken(TokenType tokenType)
        {
            bool val = matchToken(tokenType);
            if (val)
                position++;
            return val;
        }
        private bool acceptToken(TokenType tokenType, string value)
        {
            bool val = matchToken(tokenType, value);
            if (val)
                position++;
            return val;
        }

        private Token expectToken(TokenType tokenType)
        {
            if (!matchToken(tokenType))
                throw new CompilerException(tokens[position].SourceLocation, "Expected token of type {0}, got {1}!", tokenType, tokens[position].TokenType);
            return tokens[position++];
        }
        private Token expectToken(TokenType tokenType, string value)
        {
            if (!matchToken(tokenType, value))
                throw new CompilerException(tokens[position].SourceLocation, "Expected token of type {0} and value {1}, instead got {2} with value {3}!", tokenType, value, tokens[position].TokenType, tokens[position].Value);
            return tokens[position++];
        }

        

        private BinaryOperation stringToBinaryOperation(string operation)
        {
            switch (operation)
            {
                case ">":
                    return BinaryOperation.Greater;
                case ">=":
                    return BinaryOperation.GreaterOrEqual;
                case "<":
                    return BinaryOperation.Lesser;
                case "<=":
                    return BinaryOperation.LesserOrEqual;
                case "!=":
                    return BinaryOperation.Inequality;
                default:
                    return BinaryOperation.Equality;
            }
        }
    }
}

