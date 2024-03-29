﻿using System.Text;

namespace LoxInterpreter
{
    internal class AstPrinter : IExprVisitor<string>
    {
        public string Print(Expr expr)
        {
            return expr.Accept(this);
        }

        public string Visit(BinaryExpr visitee)
        {
            return Parenthesize(visitee.Op.Lexeme, visitee.Left, visitee.Right);
        }

        public string Visit(UnaryExpr visitee)
        {
            return Parenthesize(visitee.Op.Lexeme, visitee.Right);
        }

        public string Visit(LiteralExpr visitee)
        {
            if (visitee.Value is null) return "nil";
            return visitee.Value.ToString()!;
        }

        public string Visit(GroupingExpr visitee)
        {
            return Parenthesize("group", visitee.Expression);
        }

        public string Visit(TernaryConditionExpr visitee)
        {
            return Parenthesize("?:", visitee.Condition, visitee.TrueExpr, visitee.FalseExpr);
        }

        public string Visit(VariableExpr visitee)
        {
            return Parenthesize("var " + visitee.Name.Lexeme);
        }

        public string Visit(AssignExpr visitee)
        {
            return Parenthesize(visitee.Name.Lexeme + " =", visitee.Value);
        }

        public string Visit(LogicalExpr visitee)
        {
            return Parenthesize(visitee.Op.Lexeme, visitee.Left, visitee.Right);
        }

        public string Visit(CallExpr visitee)
        {

            return Parenthesize($"call {visitee.Callee.Accept(this)} with", visitee.Arguments.ToArray());
        }

        public string Visit(FunExpr visitee)
        {
            return Parenthesize("anonymous function");
        }

        public string Visit(GetExpr visitee)
        {
            return Parenthesize($"get {visitee.Property} of", visitee.Instance);
        }

        public string Visit(SetExpr visitee)
        {
            return Parenthesize($"set {visitee.Property} of", visitee.Instance, visitee.Value);
        }

        public string Visit(ThisExpr visitee)
        {
            return visitee.Keyword.Lexeme;
        }

        private string Parenthesize(string name, params Expr[] exprs)
        {
            StringBuilder builder = new StringBuilder();

            builder.Append("(").Append(name);
            foreach (Expr expr in exprs)
            {
                builder.Append(" ");
                builder.Append(expr.Accept(this));
            }
            builder.Append(")");

            return builder.ToString();
        }
    }
}
