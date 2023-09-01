using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxInterpreter
{
    internal interface ILoxCallable
    {
        object? Call(Interpreter interpreter, List<object?> arguments);

        int Arity { get; }
    }
}
