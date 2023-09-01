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

        public int Arity => declaration.Parameters.Count;

        public LoxFunction(FunctionStmt declaration)
        {
            this.declaration = declaration;
        }

        public object? Call(Interpreter interpreter, List<object?> arguments)
        {
            Environment environment = new(interpreter.Globals);
            for (int i = 0; i < arguments.Count; i++)
            {
                environment.Define(declaration.Parameters[i].Lexeme, arguments[i]);
            }

            interpreter.ExecuteBlock(declaration.Body, environment);
            return null;
        }

        public override string ToString()
        {
            return $"<fn {declaration.Name.Lexeme}>";
        }
    }
}
