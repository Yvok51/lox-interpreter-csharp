namespace LoxInterpreter
{
    internal class Parser
    {
        public Parser(List<Token> tokens)
        {
            this.tokens = tokens;
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
                if (Check(TokenType.FUN) && CheckNext(TokenType.IDENTIFIER))
                {
                    Consume(TokenType.FUN, string.Empty);
                    return Function("function");
                }

                return Statement();
            }
            catch (ParseError)
            {
                Synchronize();
                return new EmptyStmt();
            }
        }

        private Stmt Function(string kind)
        {
            Token name = Consume(TokenType.IDENTIFIER, $"Expect {kind} name.");
            return new FunctionStmt(name, FunctionExpression(kind));
        }

        private Stmt VarDeclaration()
        {
            Token name = Consume(TokenType.IDENTIFIER, "Expect variable name.");

            Expr? initializer = null;
            if (Match(TokenType.EQUAL))
            {
                initializer = Expression();
            }

            Consume(TokenType.SEMICOLON, "Expect ';' after variable declaration.");
            return new VarStmt(name, initializer);
        }

        private Stmt Statement()
        {
            if (Match(TokenType.PRINT)) return PrintStatement();
            if (Match(TokenType.LEFT_BRACE)) return new BlockStmt(Block());
            if (Match(TokenType.IF)) return IfStatement();
            if (Match(TokenType.WHILE)) return WhileStatement();
            if (Match(TokenType.FOR)) return ForStatement();
            if (Match(TokenType.BREAK)) return BreakStatement();
            if (Match(TokenType.RETURN)) return ReturnStatement();

            return ExpressionStatement();
        }

        private Stmt ReturnStatement()
        {
            Token keyword = Previous();
            Expr? value = null;
            if (!Check(TokenType.SEMICOLON))
            {
                value = Expression();
            }

            Consume(TokenType.SEMICOLON, "Expect ';' after return value.");
            return new ReturnStmt(keyword, value);
        }

        private Stmt BreakStatement()
        {
            Consume(TokenType.SEMICOLON, "Expect ';' after break statement.");
            return new BreakStmt(Previous());
        }

        private Stmt ForStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'for'.");
            
            Stmt? initializer;
            if (Match(TokenType.SEMICOLON)) initializer = null;
            else if (Match(TokenType.VAR)) initializer = VarDeclaration();
            else initializer = ExpressionStatement();

            Expr condition = new LiteralExpr(true); // default empty condition
            if (!Check(TokenType.SEMICOLON)) condition = Expression();
            Consume(TokenType.SEMICOLON, "Expect ';' after loop condition");

            Expr? increment = null;
            if (!Check(TokenType.RIGHT_PAREN)) increment = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after for clauses");

            Stmt body = Statement();

            if (increment is not null)
            {
                body = new BlockStmt(new() { body, new ExpressionStmt(increment) });
            }
            var loop = new WhileStmt(condition, body);

            return initializer is not null ? new BlockStmt(new() { initializer, loop }) : loop;
        }

        private Stmt WhileStatement()
        {
            Consume(TokenType.LEFT_PAREN, "Expect '(' after 'while'.");
            Expr condition = Expression();
            Consume(TokenType.RIGHT_PAREN, "Expect ')' after while condition.");
            Stmt body = Statement();

            return new WhileStmt(condition, body);
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
            Expr expr = Expression();
            Consume(TokenType.SEMICOLON, "Expect ';' after value.");
            return new PrintStmt(expr);
        }

        private Stmt ExpressionStatement()
        {
            Expr expr = Expression();
            Consume(TokenType.SEMICOLON, "Expect ';' after value.");
            return new ExpressionStmt(expr);
        }

        private Expr Expression() => Comma();

        private Expr Comma() =>
            LeftAssocBinaryWithLeadingCheck(Assignment, TokenType.COMMA);

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
            var condition = Or();
            if (Match(TokenType.QUESTION_MARK))
            {
                var trueExpr = Expression();
                if (Match(TokenType.COLON))
                {
                    var falseExpr = TernaryConditional();
                    return new TernaryConditionExpr(condition, trueExpr, falseExpr);
                }
                throw Error(Peek(), "Expect ternary condition");
            }

            return condition;
        }

        private Expr Or() =>
            LeftAssocBinaryWithLeadingCheck(And, TokenType.OR);

        private Expr And() =>
            LeftAssocBinaryWithLeadingCheck(Equality, TokenType.AND);

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

            return Call();
        }

        private Expr Call()
        {
            var expr = Primary();

            while(Match(TokenType.LEFT_PAREN))
            {
                expr = FinishCall(expr);
            }

            return expr;
        }

        private Expr FinishCall(Expr callee)
        {
            List<Expr> args = new List<Expr>();
            if (!Check(TokenType.RIGHT_PAREN))
            {
                do
                {
                    if (args.Count >= 255)
                    {
                        Error(Peek(), "Can't have more than 255 arguments");
                    }
                    args.Add(Assignment());
                } while (Match(TokenType.COMMA));
            }

            Token paren = Consume(TokenType.RIGHT_PAREN, "Expect ')' after arguments.");
            return new CallExpr(callee, paren, args);
        }

        private Expr Primary()
        {
            if (Match(TokenType.FALSE)) return new LiteralExpr(false);
            if (Match(TokenType.TRUE)) return new LiteralExpr(true);
            if (Match(TokenType.NIL)) return new LiteralExpr(null);
            if (Match(TokenType.FUN)) return FunctionExpression("function");

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

        private FunExpr FunctionExpression(string kind)
        {
            Consume(TokenType.LEFT_PAREN, $"Expect '(' after {kind} name.");
            List<Token> parameters = new();
            if (!Check(TokenType.RIGHT_PAREN))
            {
                do
                {
                    if (parameters.Count >= 255)
                    {
                        Error(Peek(), "Can't have more than 255 arguments");
                    }
                    parameters.Add(Consume(TokenType.IDENTIFIER, "Expect parameter name."));
                } while (Match(TokenType.COMMA));
            }
            Consume(TokenType.RIGHT_PAREN, "Exppect ')' after parameters");

            Consume(TokenType.LEFT_BRACE, $"Expect '{{' before {kind} body");
            var body = Block();

            return new FunExpr(parameters, body);
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

        private bool CheckNext(TokenType type)
        {
            if (IsAtEnd()) return false;
            if (tokens[position + 1].Type == TokenType.EOF) return false;
            return tokens[position + 1].Type == type;
        }

        private bool Check(TokenType type)
        {
            if (IsAtEnd()) return false;
            return Peek().Type == type;
        }
        private Token Advance() 
        {
            if (!IsAtEnd()) position++;
            return Previous();
        }
        private bool IsAtEnd() => Peek().Type == TokenType.EOF;
        private Token Peek() => tokens[position];
        private Token Previous() => tokens[position - 1];

        private readonly List<Token> tokens;
        private int position = 0;
    }

    internal class ParseError : Exception { }
}
