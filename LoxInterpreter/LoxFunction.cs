namespace LoxInterpreter
{
    internal class LoxFunction : ILoxCallable
    {
        private readonly Token? name;
        private readonly FunExpr declaration;
        private readonly Environment closure;
        private readonly bool isInitializer;

        public int Arity => declaration.Parameters.Count;

        public LoxFunction(Token? name, FunExpr declaration, Environment closure, bool isInitializer)
        {
            this.name = name;
            this.closure = closure;
            this.declaration = declaration;
            this.isInitializer = isInitializer;
        }

        public object? Call(Interpreter interpreter, List<object?> arguments)
        {
            Environment environment = new(closure);
            for (int i = 0; i < arguments.Count; i++)
            {
                environment.Define(declaration.Parameters[i].Lexeme, arguments[i]);
            }

            try
            {
                interpreter.ExecuteBlock(declaration.Body, environment);
            }
            catch (ReturnException e)
            {
                return isInitializer ? closure.GetAt(0, "this") : e.Value;
            }

            return isInitializer ? closure.GetAt(0, "this") : null; // default return value nil
        }

        public LoxFunction Bind(LoxInstance instance)
        {
            Environment environment = new(closure);
            environment.Define("this", instance);
            return new LoxFunction(name, declaration, environment, isInitializer);
        }

        public override string ToString()
        {
            return name is null ? "<fn>" : $"<fn {name.Lexeme}>";
        }
    }
}
