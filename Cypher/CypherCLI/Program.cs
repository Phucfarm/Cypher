using System;
using System.IO;
using System.CommandLine;
using CypherCompiler;
namespace CypherCLI
{
    class Program
    {
        static int Main(string[] args)
        {
            var fileArgument = new Argument<FileInfo>("file")
            {
                Description = "The cypher source file to build"
            };
            var buildCommand = new Command("build", "Build a cypher source file")
            {
                fileArgument
            };
            buildCommand.SetAction(parseResult =>
            {
                var file = parseResult.GetValue(fileArgument);
                if (file == null || !file.Exists)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.Error.WriteLine($"Error: File '{file?.FullName}' does not exist.");
                    Console.ResetColor();
                    return 1;
                }
                Console.WriteLine($"Building: {file.FullName}");
                Lexer lexer = new Lexer();
                List<Token> T = lexer.Tokenize(File.ReadAllText(file.FullName));
                foreach (var item in T)
                {
                    Console.WriteLine(item.ToString());
                }
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Build completed successfully.");
                Console.ResetColor();
                return 0;
            });
            var rootCommand = new RootCommand("Cypher Compiler")
            {
                buildCommand
            };
            return rootCommand.Parse(args).Invoke();
        }
    }
}
