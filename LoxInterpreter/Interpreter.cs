﻿namespace LoxInterpreter
{
    internal class Interpreter : IExprVisitor<object?>, IStmtVisitor<object?>
    {
        private Environment environment = new Environment();

        public object? Visit(IfStmt visitee)
        {
            if (IsTruthy(Evaluate(visitee.Condition)))
            {
                Execute(visitee.ThenBranch);
            }
            else if (visitee.ElseBranch is not null)
            {
                Execute(visitee.ElseBranch);
            }
            return null;
        }

        public void Interpret(List<Stmt> statements)
        {
            try
            {
                foreach (var statement in statements)
                {
                    Execute(statement);
                }
            }
            catch (RuntimeError e)
            {
                Lox.RuntimeError(e);
            }
        }

        public object? Visit(VarStmt visitee)
        {
            if (visitee.Initializer is not null)
            {
                var value = Evaluate(visitee.Initializer);
                environment.Define(visitee.Name.Lexeme, value);
                return null;
            }

            environment.Define(visitee.Name.Lexeme);
            return null;
        }

        public object? Visit(ExpressionStmt visitee)
        {
            Evaluate(visitee.Expression);
            return null;
        }

        public object? Visit(PrintStmt visitee)
        {
            var val = Evaluate(visitee.Expression);
            Console.WriteLine(Stringify(val));
            return null;
        }

        public object? Visit(BlockStmt visitee)
        {
            ExecuteBlock(visitee.Statements, new Environment(environment));
            return null;
        }

        public object? Visit(AssignExpr visitee)
        {
            object? value = Evaluate(visitee.Value);
            environment.Assign(visitee.Name, value);
            return value;
        }

        public object? Visit(LogicalExpr visitee)
        {
            var left = Evaluate(visitee.Left);
            if (visitee.Op.Type == TokenType.OR)
            {
                if (IsTruthy(left)) return left;
            }
            else
            {
                if (!IsTruthy(left)) return left;
            }

            return Evaluate(visitee.Right);
        }

        public object? Visit(BinaryExpr visitee)
        {
            var left = Evaluate(visitee.Left);
            var right = Evaluate(visitee.Right);

            switch (visitee.Op.Type) {
                case TokenType.BANG_EQUAL: return !IsEqual(left, right);
                case TokenType.EQUAL_EQUAL: return IsEqual(left, right);
                case TokenType.GREATER:
                    CheckNumberOperands(visitee.Op, left, right);
                    return (double)left! > (double)right!;
                case TokenType.GREATER_EQUAL:
                    CheckNumberOperands(visitee.Op, left, right);
                    return (double)left! >= (double)right!;
                case TokenType.LESS:
                    CheckNumberOperands(visitee.Op, left, right); 
                    return (double)left! < (double)right!;
                case TokenType.LESS_EQUAL:
                    CheckNumberOperands(visitee.Op, left, right); 
                    return (double)left! <= (double)right!;
                case TokenType.MINUS:
                    CheckNumberOperands(visitee.Op, left, right); 
                    return (double)left! - (double)right!;
                case TokenType.PLUS:
                    if (left is double dLeft && right is double dRight) {
                        return dLeft + dRight;
                    }

                    if (left is string || right is string) {
                        return Stringify(left) + Stringify(right);
                    }
                    throw new RuntimeError(visitee.Op, "Operands must be two numbers or two strings.");
                case TokenType.SLASH:
                    CheckNumberOperands(visitee.Op, left, right);
                    if ((double)right! == 0)
                    {
                        throw new RuntimeError(visitee.Op, "Division by zero.");
                    }
                    return (double)left! / (double)right!;
                case TokenType.STAR:
                    CheckNumberOperands(visitee.Op, left, right); 
                    return (double)left! * (double)right!;
                case TokenType.COMMA:
                    Evaluate(visitee.Left);
                    return Evaluate(visitee.Right);
            }

            // Unreachable.
            return null;
        }

        public object? Visit(UnaryExpr visitee)
        {
            var right = Evaluate(visitee.Right);

            switch (visitee.Op.Type) {
                case TokenType.MINUS:
                    CheckNumberOperand(visitee.Op, right);
                    return -(double)right!;
                case TokenType.BANG: return !IsTruthy(right);
            }

            // Unreachable.
            return null;
        }

        public object? Visit(LiteralExpr visitee)
        {
            return visitee.Value;
        }

        public object? Visit(VariableExpr visitee)
        {
            return environment.Get(visitee.Name);
        }

        public object? Visit(GroupingExpr visitee)
        {
            return Evaluate(visitee);
        }

        public object? Visit(TernaryConditionExpr visitee)
        {
            bool condition = IsTruthy(Evaluate(visitee.Condition));
            return condition ? Evaluate(visitee.TrueExpr) : Evaluate(visitee.FalseExpr);
        }

        private object? Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        private void Execute(Stmt stmt)
        {
            stmt.Accept(this);
        }

        private void ExecuteBlock(List<Stmt> stmts, Environment environment)
        {
            var previousEnv = this.environment;
            try
            {
                this.environment = environment;
                foreach (var stmt in stmts)
                {
                    Execute(stmt);
                }
            }
            finally
            {
                this.environment = previousEnv;
            }
        }

        private string Stringify(object? value)
        {
            if (value is null) return "nil";
            if (value is double number)
            {
                if (number == Math.Floor(number))
                {
                    return ((int)number).ToString();
                }
                number.ToString(Lox.Culture);
            }
            return value.ToString()!;
        }

        private bool IsEqual(object? left, object? right)
        {
            if (left is null && right is null) return true;
            if (left is null) return false;

            return left.Equals(right);
        }

        private bool IsTruthy(object? value)
        {
            if (value is null) return false;
            if (value is bool bValue) return bValue;
            return true;
        }

        private static void CheckNumberOperand(Token op, object? operand)
        {
            if (operand is double) return;
            throw new RuntimeError(op, "Operand must be a number.");
        }

        private static void CheckNumberOperands(Token op, params object?[] operands)
        {
            foreach (var operand in operands)
            {
                if (operand is not double)
                {
                    throw new RuntimeError(op, "Operands must be numbers.");
                }
            }
        }
    }
}
