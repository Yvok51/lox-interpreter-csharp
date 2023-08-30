namespace LoxInterpreter
{
  internal interface IStmtVisitor<T>
  {
     T Visit(ExpressionStmt visitee);
     T Visit(PrintStmt visitee);
     T Visit(VarStmt visitee);
     T Visit(BlockStmt visitee);
     T Visit(IfStmt visitee);
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
  internal class VarStmt : Stmt
  {
      public Token Name { get; }
      public Expr? Initializer { get; }
      public VarStmt(Token name, Expr? initializer)
      {
         Name = name;
         Initializer = initializer;
      }
     public override T Accept<T>(IStmtVisitor<T> visitor)
      {
          return visitor.Visit(this);
      }
  }
  internal class BlockStmt : Stmt
  {
      public List<Stmt> Statements { get; }
      public BlockStmt(List<Stmt> statements)
      {
         Statements = statements;
      }
     public override T Accept<T>(IStmtVisitor<T> visitor)
      {
          return visitor.Visit(this);
      }
  }
  internal class IfStmt : Stmt
  {
      public Expr Condition { get; }
      public Stmt ThenBranch { get; }
      public Stmt? ElseBranch { get; }
      public IfStmt(Expr condition, Stmt thenbranch, Stmt? elsebranch)
      {
         Condition = condition;
         ThenBranch = thenbranch;
         ElseBranch = elsebranch;
      }
     public override T Accept<T>(IStmtVisitor<T> visitor)
      {
          return visitor.Visit(this);
      }
  }
}
