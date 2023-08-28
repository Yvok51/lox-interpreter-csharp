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
            var file = File.ReadAllText(path);
            Run(file);

            return _hadError ? 65 : 0;
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

        private static void Run(string file)
        {
            var scanner = new Scanner(file);
            var tokens = scanner.ScanTokens();

            foreach (var token in tokens)
            {
                Console.WriteLine(token);
            }
        }

        public static void Error(int line, string message)
        {
            Report(line, "", message);
        }

        private static void Report(int line, string where, string message)
        {
            Console.WriteLine($"[line {line}] Error {where}: {message}");
            _hadError = true;
        }

        private static bool _hadError = false;
    }
}