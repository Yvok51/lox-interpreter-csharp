using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxInterpreter
{
    internal class LoxFunction : ILoxCallable
    {
        private readonly FunctionStmt declaration;
        private readonly Environment closure;

        public int Arity => declaration.Parameters.Count;

        public LoxFunction(FunctionStmt declaration, Environment closure)
        {
            this.closure = closure;
            this.declaration = declaration;
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
                return e.Value;
            }

            return null; // default return value nil
        }

        public override string ToString()
        {
            return $"<fn {declaration.Name.Lexeme}>";
        }
    }
}
