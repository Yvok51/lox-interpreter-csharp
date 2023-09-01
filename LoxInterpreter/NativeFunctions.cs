using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxInterpreter
{
    internal class NativeFunctions
    {
        public static ILoxCallable Clock = new ClockImpl();
        private class ClockImpl : ILoxCallable
        {
            public int Arity { get { return 0; } }
            public object? Call(Interpreter interpreter, List<object?> arguments)
            {
                return DateTime.Now.Ticks / TimeSpan.TicksPerSecond;
            }
        }
    }
}
