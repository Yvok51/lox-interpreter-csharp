using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxInterpreter
{
    internal enum ResolvingState
    {
        Resolving, Resolved, None
    }

    internal enum FunctionType
    {
        None, Function
    }

    internal class Resolver : IExprVisitor<Null?>, IStmtVisitor<Null?>
    {
        private readonly Interpreter interpreter;
        private readonly List<Dictionary<string, ResolvingState>> scopes = new();
        private FunctionType currentFunction = FunctionType.None;
        private int loopDepth = 0;

        public Resolver(Interpreter interpreter)
        {
            this.interpreter = interpreter;
        }

        public Null? Visit(EmptyStmt visitee)
        {
            return null;
        }

        public Null? Visit(ExpressionStmt visitee)
        {
            Resolve(visitee.Expression);
            return null;
        }

        public Null? Visit(PrintStmt visitee)
        {
            Resolve(visitee.Expression);
            return null;
        }

        public Null? Visit(VarStmt visitee)
        {
            Declare(visitee.Name);
            if (visitee.Initializer is not null)
            {
                Resolve(visitee.Initializer);
            }
            Define(visitee.Name);
            return null;
        }

        public Null? Visit(BlockStmt visitee)
        {
            BeginScope();
            Resolve(visitee.Statements);
            EndScope();
            return null;
        }

        public Null? Visit(IfStmt visitee)
        {
            Resolve(visitee.Condition);
            Resolve(visitee.ThenBranch);
            if (visitee.ElseBranch is not null) Resolve(visitee.ElseBranch);
            return null;
        }

        public Null? Visit(WhileStmt visitee)
        {
            Resolve(visitee.Condition);
            loopDepth++;
            Resolve(visitee.Body);
            loopDepth--;
            return null;
        }

        public Null? Visit(BreakStmt visitee)
        {
            if (loopDepth == 0)
            {
                Lox.Error(visitee.Keyword, "Break statement not allowed outside loops");
            }
            return null;
        }

        public Null? Visit(FunctionStmt visitee)
        {
            Declare(visitee.Name);
            Define(visitee.Name);

            ResolveFunction(visitee.Function, FunctionType.Function);
            return null;
        }

        public Null? Visit(ReturnStmt visitee)
        {
            if (currentFunction == FunctionType.None)
            {
                Lox.Error(visitee.Keyword, "Can't return from top-level code.");
            }
            if (visitee.Value is not null) Resolve(visitee.Value);
            return null;
        }

        public Null? Visit(ClassStmt visitee)
        {
            Declare(visitee.Name);
            Define(visitee.Name);
            return null;
        }

        public Null? Visit(BinaryExpr visitee)
        {
            Resolve(visitee.Left);
            Resolve(visitee.Right);
            return null;
        }

        public Null? Visit(UnaryExpr visitee)
        {
            Resolve(visitee.Right);
            return null;
        }

        public Null? Visit(LiteralExpr visitee)
        {
            return null;
        }

        public Null? Visit(LogicalExpr visitee)
        {
            Resolve(visitee.Left);
            Resolve(visitee.Right);
            return null;
        }

        public Null? Visit(GroupingExpr visitee)
        {
            Resolve(visitee.Expression);
            return null;
        }

        public Null? Visit(TernaryConditionExpr visitee)
        {
            Resolve(visitee.Condition);
            Resolve(visitee.TrueExpr);
            Resolve(visitee.FalseExpr);
            return null;
        }

        public Null? Visit(VariableExpr visitee)
        {
            if (!InGlobalScope && scopes.Last().GetValueOrDefault(visitee.Name.Lexeme, ResolvingState.None) == ResolvingState.Resolving)
            {
                Lox.Error(visitee.Name, "Can't read local variable in its own initializer.");
            }

            ResolveLocal(visitee, visitee.Name);
            return null;
        }

        public Null? Visit(AssignExpr visitee)
        {
            Resolve(visitee.Value);
            ResolveLocal(visitee, visitee.Name);
            return null;
        }

        public Null? Visit(CallExpr visitee)
        {
            Resolve(visitee.Callee);
            foreach (var arg in visitee.Arguments)
            {
                Resolve(arg);
            }
            return null;
        }

        public Null? Visit(GetExpr visitee)
        {
            Resolve(visitee.Instance);
            return null;
        }

        public Null? Visit(SetExpr visitee)
        {
            Resolve(visitee.Value);
            Resolve(visitee.Instance);
            return null;
        }

        public Null? Visit(FunExpr visitee)
        {
            ResolveFunction(visitee, FunctionType.Function);
            return null;
        }

        private bool InGlobalScope => scopes.Count == 0;

        private void ResolveFunction(FunExpr function, FunctionType functionType)
        {
            FunctionType encopassingFunction = currentFunction;
            currentFunction = functionType;
            BeginScope();
            foreach (var param in function.Parameters)
            {
                Declare(param);
                Define(param);
            }
            Resolve(function.Body);
            EndScope();
            currentFunction = encopassingFunction;
        }

        private void ResolveLocal(Expr expr, Token name)
        {
            for (int i = scopes.Count - 1; i >= 0; i--)
            {
                if (scopes[i].ContainsKey(name.Lexeme))
                {
                    interpreter.Resolve(expr, scopes.Count - 1 - i);
                    return;
                }
            }
        }

        private void Declare(Token name)
        {
            if (scopes.Count == 0) return;
            var scope = scopes.Last();
            if (scope.ContainsKey(name.Lexeme))
            {
                Lox.Error(name, "Already a variable with this name in this scope.");
            }
            scope.Add(name.Lexeme, ResolvingState.Resolving);
        }

        private void Define(Token name)
        {
            if (scopes.Count == 0) return;
            scopes.Last()[name.Lexeme] = ResolvingState.Resolved;
        }

        private void BeginScope()
        {
            scopes.Add(new());
        }

        private void EndScope()
        {
            scopes.RemoveAt(scopes.Count - 1);
        }

        public void Resolve(List<Stmt> stmts)
        {
            foreach (var stmt in stmts)
            {
                Resolve(stmt);
            }
        }

        private void Resolve(Stmt stmt)
        {
            stmt.Accept(this);
        }

        private void Resolve(Expr stmt)
        {
            stmt.Accept(this);
        }
    }
}
