namespace LoxInterpreter
{
  internal interface IExprVisitor<T>
  {
     T Visit(BinaryExpr visitee);
     T Visit(UnaryExpr visitee);
     T Visit(LiteralExpr visitee);
     T Visit(GroupingExpr visitee);
     T Visit(TernaryConditionExpr visitee);
  }

  internal abstract class Expr
  {
  public abstract T Accept<T>(IExprVisitor<T> visitor);
  }
  internal class BinaryExpr : Expr
  {
      public Expr Left { get; }
      public Expr Right { get; }
      public Token Op { get; }
      public BinaryExpr(Expr left, Expr right, Token op)
      {
         Left = left;
         Right = right;
         Op = op;
      }
     public override T Accept<T>(IExprVisitor<T> visitor)
      {
          return visitor.Visit(this);
      }
  }
  internal class UnaryExpr : Expr
  {
      public Expr Right { get; }
      public Token Op { get; }
      public UnaryExpr(Expr right, Token op)
      {
         Right = right;
         Op = op;
      }
     public override T Accept<T>(IExprVisitor<T> visitor)
      {
          return visitor.Visit(this);
      }
  }
  internal class LiteralExpr : Expr
  {
      public object? Value { get; }
      public LiteralExpr(object? value)
      {
         Value = value;
      }
     public override T Accept<T>(IExprVisitor<T> visitor)
      {
          return visitor.Visit(this);
      }
  }
  internal class GroupingExpr : Expr
  {
      public Expr Expression { get; }
      public GroupingExpr(Expr expression)
      {
         Expression = expression;
      }
     public override T Accept<T>(IExprVisitor<T> visitor)
      {
          return visitor.Visit(this);
      }
  }
  internal class TernaryConditionExpr : Expr
  {
      public Expr Condition { get; }
      public Expr TrueExpr { get; }
      public Expr FalseExpr { get; }
      public TernaryConditionExpr(Expr condition, Expr trueexpr, Expr falseexpr)
      {
         Condition = condition;
         TrueExpr = trueexpr;
         FalseExpr = falseexpr;
      }
     public override T Accept<T>(IExprVisitor<T> visitor)
      {
          return visitor.Visit(this);
      }
  }
}
