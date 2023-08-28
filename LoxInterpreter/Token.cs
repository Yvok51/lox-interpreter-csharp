namespace LoxInterpreter
{
    internal class Token
    {
        public TokenType Type { get; }
        public string Lexeme { get; }
        public int Line { get; }
        public object? Literal { get; }

        public Token(TokenType type, string lexeme, int line, object? literal)
        {
            Type = type;
            Lexeme = lexeme;
            Line = line;
            Literal = literal;
        }

        public override string ToString()
        {
            return $"{Type} {Lexeme} {Literal}";
        }
    }
}
