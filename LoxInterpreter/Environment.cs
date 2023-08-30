namespace LoxInterpreter
{
    internal class Environment
    {
        private readonly Dictionary<string, (bool assigned, object? value)> values = new();
        private readonly Environment? enclosing;

        public Environment()
        {
            enclosing = null;
        }

        public Environment(Environment enclosing)
        {
            this.enclosing = enclosing;
        }

        public void Define(string name)
        {
            values[name] = (false, null);
        }

        public void Define(string name, object? value)
        {
            values[name] = (true, value);
        }

        public object? Get(Token name)
        {
            if (values.ContainsKey(name.Lexeme))
            {
                var (assigned, value) = values[name.Lexeme];
                if (assigned)
                {
                    return value;
                }

                throw new RuntimeError(name, $"Variable '{name.Lexeme}' has not been assigned.");
            }

            if (enclosing is not null)
            {
                return enclosing.Get(name);
            }

            throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
        }

        public void Assign(Token name, object? value)
        {
            if (values.ContainsKey(name.Lexeme))
            {
                values[name.Lexeme] = (true, value);
                return;
            }

            if (enclosing is not null)
            {
                enclosing.Assign(name, value);
                return;
            }

            throw new RuntimeError(name, $"Undefined variable '{name.Lexeme}'.");
        }
    }
}
