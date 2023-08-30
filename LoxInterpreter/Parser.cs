﻿namespace LoxInterpreter
{
    internal class Parser
    {
        public Parser(List<Token> tokens)
        {
            _tokens = tokens;
        }

        public List<Stmt> Parse()
        {
            List<Stmt> statements = new();
            while (!IsAtEnd())
            {
                statements.Add(Declaration());
            }
            return statements;
        }

        private Stmt Declaration()
        {
            try
            {
                if (Match(TokenType.VAR)) return VarDeclaration();

                return Statement();
            }
            catch (ParseError)
            {
                Synchronize();
                return null;
            }
        }

        private Stmt VarDeclaration()
        {
            Token name = Consume(TokenType.IDENTIFIER, "Expect variable name.");

            Expr initializer = null;
            if (Match(TokenType.EQUAL))
            {
                initializer = ExpressionList();
            }

            Consume(TokenType.SEMICOLON, "Expect ';' after variable declaration.");
            return new VarStmt(name, initializer);
        }

        private Stmt Statement()
        {
            if (Match(TokenType.PRINT)) return PrintStatement();
            if (Match(TokenType.LEFT_BRACE)) return new BlockStmt(Block());
            if (Match(TokenType.IF)) return IfStatement();

            return ExpressionStatement();
        }

        private Stmt IfStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'if'.");
            Expr condition = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after if condition.");

            Stmt thenBranch = Statement();
            Stmt? elseBranch = Match(TokenType.ELSE) ? Statement() : null;

            return new IfStmt(condition, thenBranch, elseBranch);
        }

        private List<Stmt> Block()
        {
            List<Stmt> statements = new();
            while (!Check(TokenType.RIGHT_BRACE) && !IsAtEnd())
            {
                statements.Add(Declaration());
            }
            Consume(TokenType.RIGHT_BRACE, "Expect '}' after block.");
            return statements;
        }

        private Stmt PrintStatement()
        {
            Expr expr = ExpressionList();
            Consume(TokenType.SEMICOLON, "Expect ';' after value.");
            return new PrintStmt(expr);
        }

        private Stmt ExpressionStatement()
        {
            Expr expr = ExpressionList();
            Consume(TokenType.SEMICOLON, "Expect ';' after value.");
            return new ExpressionStmt(expr);
        }

        private Expr ExpressionList() =>
            LeftAssocBinaryWithLeadingCheck(Expression, TokenType.COMMA);

        private Expr Expression()
        {
            return Assignment();
        }

        private Expr Assignment()
        {
            Expr expr = TernaryConditional();

            if (Match(TokenType.EQUAL))
            {
                Token equals = Previous();
                Expr value = Assignment();

                if (expr is VariableExpr varExpr) {
                    Token name = varExpr.Name;
                    return new AssignExpr(name, value);
                }

                Error(equals, "Invalid assignment target.");
            }

            return expr;
        }

        private Expr TernaryConditional()
        {
            var condition = Equality();
            if (Match(TokenType.QUESTION_MARK))
            {
                var trueExpr = ExpressionList();
                if (Match(TokenType.COLON))
                {
                    var falseExpr = TernaryConditional();
                    return new TernaryConditionExpr(condition, trueExpr, falseExpr);
                }
                throw Error(Peek(), "Expect ternary condition");
            }

            return condition;
        }

        private Expr Equality() =>
            LeftAssocBinaryWithLeadingCheck(Comparison, TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL);

        private Expr Comparison() =>
            LeftAssocBinaryWithLeadingCheck(Term, TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL);

        private Expr Term() =>
            LeftAssocBinary(Factor, TokenType.MINUS, TokenType.PLUS);

        private Expr Factor() =>
            LeftAssocBinaryWithLeadingCheck(Unary, TokenType.STAR, TokenType.SLASH);

        private Expr Unary()
        {
            if (Match(TokenType.BANG, TokenType.MINUS))
            {
                Token op = Previous();
                Expr right = Unary();
                return new UnaryExpr(right, op);
            }

            return Primary();
        }

        private Expr Primary()
        {
            if (Match(TokenType.FALSE)) return new LiteralExpr(false);
            if (Match(TokenType.TRUE)) return new LiteralExpr(true);
            if (Match(TokenType.NIL)) return new LiteralExpr(null);

            if (Match(TokenType.NUMBER, TokenType.STRING))
            {
                return new LiteralExpr(Previous().Literal);
            }

            if (Match(TokenType.IDENTIFIER))
            {
                return new VariableExpr(Previous());
            }

            if (Match(TokenType.LEFT_PAREN))
            {
                Expr expr = Expression();
                Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");
                return new GroupingExpr(expr);
            }

            throw Error(Peek(), "Expect expression.");
        }

        private Expr LeftAssocBinaryWithLeadingCheck(Func<Expr> operand, params TokenType[] operators)
        {
            if (Match(operators))
            {
                throw Error(Previous(), $"Binary operator '{Previous().Lexeme}' without left operand.");
            }

            return LeftAssocBinary(operand, operators);
        }

        private Expr LeftAssocBinary(Func<Expr> operand, params TokenType[] operators)
        {
            Expr expr = operand();
            while (Match(operators))
            {
                Token op = Previous();
                Expr right = operand();
                expr = new BinaryExpr(expr, right, op);
            }

            return expr;
        }

        private Token Consume(TokenType type, string message)
        {
            if (Check(type)) return Advance();
            
            throw Error(Peek(), message);
        }

        private ParseError Error(Token token, string message)
        {
            Lox.Error(token, message);
            return new ParseError();
        }

        private void Synchronize()
        {
            Advance();
            while (!IsAtEnd())
            {
                if (Previous().Type == TokenType.SEMICOLON) return;
                switch (Previous().Type)
                {
                    case TokenType.CLASS:
                    case TokenType.FUN:
                    case TokenType.VAR:
                    case TokenType.FOR:
                    case TokenType.IF:
                    case TokenType.WHILE:
                    case TokenType.PRINT:
                    case TokenType.RETURN:
                        return;
                }
                Advance();
            }
        }

        private bool Match(params TokenType[] types)
        {
            foreach (var type in types)
            {
                if (Check(type))
                {
                    Advance();
                    return true;
                }
            }
            return false;
        }

        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Peek().Type == type;
        }
        private Token Advance() 
        {
            if (!IsAtEnd()) _position++;
            return Previous();
        }
        private bool IsAtEnd() => Peek().Type == TokenType.EOF;
        private Token Peek() => _tokens[_position];
        private Token Previous() => _tokens[_position - 1];

        private readonly List<Token> _tokens;
        private int _position = 0;
    }

    internal class ParseError : Exception { }
}
