namespace LoxInterpreter
{
    internal class LoxClass : ILoxCallable
    {
        private readonly string name;
        private readonly Dictionary<string, LoxFunction> methods;

        public LoxClass(string name, Dictionary<string, LoxFunction> methods)
        {
            this.name = name;
            this.methods = methods;
        }

        public int Arity { get
            {
                return TryFindMethod("init", out var constructor) ? constructor.Arity : 0;
            }
        }

        public object? Call(Interpreter interpreter, List<object?> arguments)
        {
            var instance = new LoxInstance(this);
            if (TryFindMethod("init", out var constructor))
            {
                constructor.Bind(instance).Call(interpreter, arguments);
            }
            return instance;
        }

        public bool TryFindMethod(string name, out LoxFunction method)
        {
            return methods.TryGetValue(name, out method);
        }

        public override string ToString()
        {
            return name;
        }
    }
}
