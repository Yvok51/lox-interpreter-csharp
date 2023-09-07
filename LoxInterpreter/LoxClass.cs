namespace LoxInterpreter
{
    internal class LoxClass : ILoxCallable
    {
        private readonly string name;
        public LoxClass(string name)
        {
            this.name = name;
        }

        public int Arity => 0;

        public object? Call(Interpreter interpreter, List<object?> arguments)
        {
            var instance = new LoxInstance(this);
            return instance;
        }

        public override string ToString()
        {
            return name;
        }
    }
}
