namespace LoxInterpreter
{
  internal interface IStmtVisitor<T>
  {
     T Visit(ExpressionStmt visitee);
     T Visit(PrintStmt visitee);
  }

  internal abstract class Stmt
  {
  public abstract T Accept<T>(IStmtVisitor<T> visitor);
  }
  internal class ExpressionStmt : Stmt
  {
      public Expr Expression { get; }
      public ExpressionStmt(Expr expression)
      {
         Expression = expression;
      }
     public override T Accept<T>(IStmtVisitor<T> visitor)
      {
          return visitor.Visit(this);
      }
  }
  internal class PrintStmt : Stmt
  {
      public Expr Expression { get; }
      public PrintStmt(Expr expression)
      {
         Expression = expression;
      }
     public override T Accept<T>(IStmtVisitor<T> visitor)
      {
          return visitor.Visit(this);
      }
  }
}
