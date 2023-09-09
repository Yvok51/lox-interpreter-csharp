namespace LoxInterpreter
{
    internal class LoxInstance
    {
        private readonly LoxClass @class;
        private readonly Dictionary<string, object?> fields;
        
        public LoxInstance(LoxClass @class)
        {
            this.@class = @class;
            fields = new();
        }

        public object? Get(Token property)
        {
            if (fields.ContainsKey(property.Lexeme))
            {
                return fields[property.Lexeme];
            }

            if (@class.TryFindMethod(property.Lexeme, out var method))
            {
                return method;
            }

            throw new RuntimeError(property, $"Undefined property '{property.Lexeme}'.");
        }

        public void Set(Token property, object? value)
        {
            fields[property.Lexeme] = value;
        }

        public override string ToString()
        {
            return @class.ToString() + " instance";
        }
    }
}
