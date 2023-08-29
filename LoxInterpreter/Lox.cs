using System.Globalization;

namespace LoxInterpreter
{
    internal class Lox
    {
        static int Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: LoxInterpreter [script]");
                return 64;
            }
            else if (args.Length == 1)
            {
                return RunFile(args[0]);
            }

            RunPrompt();
            return 0;
        }

        private static int RunFile(string path)
        {
            var code = File.ReadAllText(path);
            Run(code);

            if (_hadError) return 65;
            if (_hadRuntimeError) return 70;
            return 0;
        }

        private static void RunPrompt()
        {
            while (true)
            {
                Console.Write("> ");
                string? prompt = Console.ReadLine();
                if (prompt == null) break;
                Run(prompt);
                _hadError = false;
            }
        }

        private static void Run(string code)
        {
            var scanner = new Scanner(code);
            var tokens = scanner.ScanTokens();
            var parser = new Parser(tokens);
            var expr = parser.Parse();

            if (_hadError || expr is null) return;
            interpreter.Interpret(expr);
        }

        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        public static void Error(Token token, string message)
        {
            if (token.Type == TokenType.EOF)
            {
                Report(token.Line, "at end", message);
            }
            else
            {
                Report(token.Line, $"at '{token.Lexeme}'", message);
            }
        }

        public static void RuntimeError(RuntimeError error)
        {
            Console.Error.WriteLine($"{error.Message}\n[line {error.Token.Line}]");
            _hadRuntimeError = true;
        }

        private static void Report(int line, string where, string message)
        {
            Console.Error.WriteLine($"[line {line}] Error {where}: {message}");
            _hadError = true;
        }

        public static readonly CultureInfo Culture = new CultureInfo("en-US");


        private static bool _hadError = false;
        private static bool _hadRuntimeError = false;
        private static readonly Interpreter interpreter = new();
    }
}