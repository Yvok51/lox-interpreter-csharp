namespace LoxInterpreter
{
    internal class Scanner
    {
        public Scanner(string text)
        {
            _source = text;
            _tokens = new List<Token>();
        }

        public List<Token> ScanTokens()
        {
            while (!IsAtEnd())
            {
                _start = _current;
                ScanToken();
            }

            _tokens.Add(new Token(TokenType.EOF, "", _line, null));
            return _tokens;
        }

        private void ScanToken()
        {
            char c = Advance();
            switch (c)
            {
                case '(': AddToken(TokenType.LEFT_PAREN); break;
                case ')': AddToken(TokenType.RIGHT_PAREN); break;
                case '{': AddToken(TokenType.LEFT_BRACE); break;
                case '}': AddToken(TokenType.RIGHT_BRACE); break;
                case ',': AddToken(TokenType.COMMA); break;
                case '.': AddToken(TokenType.DOT); break;
                case '-': AddToken(TokenType.MINUS); break;
                case '+': AddToken(TokenType.PLUS); break;
                case ';': AddToken(TokenType.SEMICOLON); break;
                case '*': AddToken(TokenType.STAR); break;
                case '!':
                    AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                    break;
                case '=':
                    AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                    break;
                case '<':
                    AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                    break;
                case '>':
                    AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                    break;
                case '/':
                    if (Match('/'))
                    {
                        // A comment goes until the end of the line.
                        while (Peek() != '\n' && !IsAtEnd()) Advance();
                    }
                    else
                    {
                        AddToken(TokenType.SLASH);
                    }
                    break;

                case ' ':
                case '\r':
                case '\t': break; // Ignore whitespace
                case '\n': _line++; break;

                case '"': String(); break;
                case >= '0' and <= '9': Number(); break;

                default:
                    if (IsIdentifierStartChar(c))
                    {
                        Identifier();
                    }
                    else
                    {
                        Lox.Error(_line, "Unexpected character.");
                    }
                    break;
            }
        }

        private void String()
        {
            while (Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n') _line++;
                Advance();
            }

            if (IsAtEnd())
            {
                Lox.Error(_line, "Unterminated string.");
                return;
            }

            Advance(); // Closing "
            var value = _source.Substring(_start + 1, CurrentLength() - 1); // value inside quotes
            AddToken(TokenType.STRING, value);
        }

        private void Number()
        {
            while(IsDigit(Peek())) Advance();
            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                Advance();
                while (IsDigit(Peek())) Advance();
            }
            var value = double.Parse(CurrentToken());
            AddToken(TokenType.NUMBER, value);
        }

        private void Identifier()
        {
            while (IsIdentifierChar(Peek())) Advance();

            var token = CurrentToken();
            TokenType type = keywords.GetValueOrDefault(token, TokenType.IDENTIFIER);
            AddToken(type);
        }

        private bool IsIdentifierChar(char c) => IsIdentifierStartChar(c) || IsDigit(c);

        private bool IsIdentifierStartChar(char c) =>
            (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z') || c == '_';

        private bool IsDigit(char c) => '0' <= c && c <= '9';

        private char Advance()
        {
            return _source[_current++];
        }

        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        private void AddToken(TokenType type, object? literal)
        {
            string text = CurrentToken();
            _tokens.Add(new Token(type, text, _line, literal));
        }

        private string CurrentToken() => _source.Substring(_start, CurrentLength());

        private bool IsAtEnd() => _current >= _source.Length;

        private int CurrentLength() => _current - _start;

        private bool Match(char expected)
        {
            if (Peek() != expected) return false;

            _current++;
            return true;
        }

        private char Peek()
        {
            return IsAtEnd() ? '\0' : _source[_current];
        }

        private char PeekNext()
        {
            if (_current + 1 >= _source.Length) return '\0';
            return _source[_current + 1];
        }

        private readonly string _source;
        private readonly List<Token> _tokens;

        private int _start = 0;
        private int _current = 0;
        private int _line = 1;

        private static readonly Dictionary<string, TokenType> keywords = new()
        {
            { "and",    TokenType.AND },
            { "class",  TokenType.CLASS },
            { "else",   TokenType.ELSE },
            { "false",  TokenType.FALSE },
            { "for",    TokenType.FOR },
            { "fun",    TokenType.FUN },
            { "if",     TokenType.IF },
            { "nil",    TokenType.NIL },
            { "or",     TokenType.OR },
            { "print",  TokenType.PRINT },
            { "return", TokenType.RETURN },
            { "super",  TokenType.SUPER },
            { "this",   TokenType.THIS },
            { "true",   TokenType.TRUE },
            { "var",    TokenType.VAR },
            { "while",  TokenType.WHILE }
        };
    }
}
