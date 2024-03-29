namespace LoxInterpreter
{
  internal interface IStmtVisitor<T>
  {
     T Visit(EmptyStmt visitee);
     T Visit(ExpressionStmt visitee);
     T Visit(PrintStmt visitee);
     T Visit(VarStmt visitee);
     T Visit(BlockStmt visitee);
     T Visit(IfStmt visitee);
     T Visit(WhileStmt visitee);
     T Visit(BreakStmt visitee);
     T Visit(FunctionStmt visitee);
     T Visit(ReturnStmt visitee);
     T Visit(ClassStmt visitee);
  }

  internal abstract class Stmt
  {
  public abstract T Accept<T>(IStmtVisitor<T> visitor);
  }
  internal class EmptyStmt : Stmt
  {
      public EmptyStmt()
      {
      }
     public override T Accept<T>(IStmtVisitor<T> visitor)
      {
          return visitor.Visit(this);
      }
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
  internal class WhileStmt : Stmt
  {
      public Expr Condition { get; }
      public Stmt Body { get; }
      public WhileStmt(Expr condition, Stmt body)
      {
         Condition = condition;
         Body = body;
      }
     public override T Accept<T>(IStmtVisitor<T> visitor)
      {
          return visitor.Visit(this);
      }
  }
  internal class BreakStmt : Stmt
  {
      public Token Keyword { get; }
      public BreakStmt(Token keyword)
      {
         Keyword = keyword;
      }
     public override T Accept<T>(IStmtVisitor<T> visitor)
      {
          return visitor.Visit(this);
      }
  }
  internal class FunctionStmt : Stmt
  {
      public Token Name { get; }
      public FunExpr Function { get; }
      public FunctionStmt(Token name, FunExpr function)
      {
         Name = name;
         Function = function;
      }
     public override T Accept<T>(IStmtVisitor<T> visitor)
      {
          return visitor.Visit(this);
      }
  }
  internal class ReturnStmt : Stmt
  {
      public Token Keyword { get; }
      public Expr? Value { get; }
      public ReturnStmt(Token keyword, Expr? value)
      {
         Keyword = keyword;
         Value = value;
      }
     public override T Accept<T>(IStmtVisitor<T> visitor)
      {
          return visitor.Visit(this);
      }
  }
  internal class ClassStmt : Stmt
  {
      public Token Name { get; }
      public List<FunctionStmt> Methods { get; }
      public ClassStmt(Token name, List<FunctionStmt> methods)
      {
         Name = name;
         Methods = methods;
      }
     public override T Accept<T>(IStmtVisitor<T> visitor)
      {
          return visitor.Visit(this);
      }
  }
}
