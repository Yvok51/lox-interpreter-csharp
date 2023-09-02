using System.Text;

namespace GenerateAst
{
    internal class Program
    {
        static int Main(string[] args)
        {
            if (args.Length != 1)
            {
                Console.Error.WriteLine("Usage: generate_ast <output directory>");
                return 64;
            }

            var outputDir = args[0];
            var exprCode = DefineAst("Expr", new()
            {
                ("BinaryExpr", new() { ("Expr", "Left"), ("Expr", "Right"), ("Token", "Op") }),
                ("UnaryExpr", new() { ("Expr", "Right"), ("Token", "Op") }),
                ("LiteralExpr", new() { ("object?", "Value") }),
                ("LogicalExpr", new() { ("Expr", "Left"), ("Token", "Op"), ("Expr", "Right") }),
                ("GroupingExpr", new() { ("Expr", "Expression") }),
                ("TernaryConditionExpr", new() { ("Expr", "Condition"), ("Expr", "TrueExpr"), ("Expr", "FalseExpr") }),
                ("VariableExpr", new() { ("Token", "Name") }),
                ("AssignExpr", new() { ("Token", "Name"), ("Expr", "Value") }),
                ("CallExpr", new() {("Expr", "Callee"), ("Token", "Paren"), ("List<Expr>", "Arguments")}),
                ("FunExpr", new() {("List<Token>", "Parameters"), ("List<Stmt>", "Body")})
            });
            var exprFile = outputDir + "/" + "Expr.cs";
            File.WriteAllText(exprFile, exprCode);

            var stmtCode = DefineAst("Stmt", new()
            {
                ("EmptyStmt", new() {}),
                ("ExpressionStmt", new() { ("Expr", "Expression") }),
                ("PrintStmt", new() { ("Expr", "Expression") }),
                ("VarStmt", new() { ("Token", "Name"), ("Expr?", "Initializer") }),
                ("BlockStmt", new() { ("List<Stmt>", "Statements") }),
                ("IfStmt", new() { ("Expr", "Condition"), ("Stmt", "ThenBranch"), ("Stmt?", "ElseBranch") }),
                ("WhileStmt", new() { ("Expr", "Condition"), ("Stmt", "Body")}),
                ("BreakStmt", new() {}),
                ("FunctionStmt", new() { ("Token", "Name"), ("FunExpr", "Function")}),
                ("ReturnStmt", new() { ("Token", "Keyword"), ("Expr?", "Value") }),
            });
            var stmtFile = outputDir + "/" + "Stmt.cs";
            File.WriteAllText(stmtFile, stmtCode);

            return 0;
        }

        static string DefineAst(
            string baseClass,
            List<(string className, List<(string type, string name)> properties)> subclasses,
            string namespaceName = "LoxInterpreter"
        )
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"namespace {namespaceName}");
            stringBuilder.AppendLine("{");

            stringBuilder.AppendLine(defineVisitor(baseClass, subclasses.Select(subclass => subclass.className)));

            stringBuilder.AppendLine($"  internal abstract class {baseClass}");
            stringBuilder.AppendLine("  {");
            stringBuilder.AppendLine($"  public abstract T Accept<T>(I{baseClass}Visitor<T> visitor);");
            stringBuilder.AppendLine("  }");
            
            foreach (var (className, properties) in subclasses)
            {
                stringBuilder.AppendLine($"  internal class {className} : {baseClass}");
                stringBuilder.AppendLine("  {");
                // Properties
                foreach (var property in properties)
                {
                    stringBuilder.AppendLine($"      public {property.type} {property.name} {{ get; }}");
                }
                // Constructor
                var parameters = string.Join(", ", properties.Select(property => $"{property.type} {property.name.ToLower()}"));
                stringBuilder.AppendLine($"      public {className}({parameters})");
                stringBuilder.AppendLine("      {");
                foreach (var property in properties)
                {
                    stringBuilder.AppendLine($"         {property.name} = {property.name.ToLower()};");
                }
                stringBuilder.AppendLine("      }");
                // Accept
                stringBuilder.AppendLine($"     public override T Accept<T>(I{baseClass}Visitor<T> visitor)");
                stringBuilder.AppendLine("      {");
                stringBuilder.AppendLine("          return visitor.Visit(this);");
                stringBuilder.AppendLine("      }");

                stringBuilder.AppendLine("  }");
            }
            stringBuilder.AppendLine("}");

            return stringBuilder.ToString();
        }

        private static string defineVisitor(string baseClass, IEnumerable<string> classes)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine($"  internal interface I{baseClass}Visitor<T>");
            stringBuilder.AppendLine("  {");
            foreach (var className in classes)
            {
                stringBuilder.AppendLine($"     T Visit({className} visitee);");
            }
            stringBuilder.AppendLine("  }");

            return stringBuilder.ToString();
        }
    }
}