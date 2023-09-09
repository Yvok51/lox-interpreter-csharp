namespace LoxInterpreter
{
  internal interface IExprVisitor<T>
  {
     T Visit(BinaryExpr visitee);
     T Visit(UnaryExpr visitee);
     T Visit(LiteralExpr visitee);
     T Visit(LogicalExpr visitee);
     T Visit(GroupingExpr visitee);
     T Visit(TernaryConditionExpr visitee);
     T Visit(VariableExpr visitee);
     T Visit(AssignExpr visitee);
     T Visit(CallExpr visitee);
     T Visit(FunExpr visitee);
     T Visit(GetExpr visitee);
     T Visit(SetExpr visitee);
     T Visit(ThisExpr visitee);
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
  internal class LogicalExpr : Expr
  {
      public Expr Left { get; }
      public Token Op { get; }
      public Expr Right { get; }
      public LogicalExpr(Expr left, Token op, Expr right)
      {
         Left = left;
         Op = op;
         Right = right;
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
  internal class VariableExpr : Expr
  {
      public Token Name { get; }
      public VariableExpr(Token name)
      {
         Name = name;
      }
     public override T Accept<T>(IExprVisitor<T> visitor)
      {
          return visitor.Visit(this);
      }
  }
  internal class AssignExpr : Expr
  {
      public Token Name { get; }
      public Expr Value { get; }
      public AssignExpr(Token name, Expr value)
      {
         Name = name;
         Value = value;
      }
     public override T Accept<T>(IExprVisitor<T> visitor)
      {
          return visitor.Visit(this);
      }
  }
  internal class CallExpr : Expr
  {
      public Expr Callee { get; }
      public Token Paren { get; }
      public List<Expr> Arguments { get; }
      public CallExpr(Expr callee, Token paren, List<Expr> arguments)
      {
         Callee = callee;
         Paren = paren;
         Arguments = arguments;
      }
     public override T Accept<T>(IExprVisitor<T> visitor)
      {
          return visitor.Visit(this);
      }
  }
  internal class FunExpr : Expr
  {
      public List<Token> Parameters { get; }
      public List<Stmt> Body { get; }
      public FunExpr(List<Token> parameters, List<Stmt> body)
      {
         Parameters = parameters;
         Body = body;
      }
     public override T Accept<T>(IExprVisitor<T> visitor)
      {
          return visitor.Visit(this);
      }
  }
  internal class GetExpr : Expr
  {
      public Expr Instance { get; }
      public Token Property { get; }
      public GetExpr(Expr instance, Token property)
      {
         Instance = instance;
         Property = property;
      }
     public override T Accept<T>(IExprVisitor<T> visitor)
      {
          return visitor.Visit(this);
      }
  }
  internal class SetExpr : Expr
  {
      public Expr Instance { get; }
      public Token Property { get; }
      public Expr Value { get; }
      public SetExpr(Expr instance, Token property, Expr value)
      {
         Instance = instance;
         Property = property;
         Value = value;
      }
     public override T Accept<T>(IExprVisitor<T> visitor)
      {
          return visitor.Visit(this);
      }
  }
  internal class ThisExpr : Expr
  {
      public Token Keyword { get; }
      public ThisExpr(Token keyword)
      {
         Keyword = keyword;
      }
     public override T Accept<T>(IExprVisitor<T> visitor)
      {
          return visitor.Visit(this);
      }
  }
}
