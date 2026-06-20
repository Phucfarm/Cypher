using System;
using System.Collections.Generic;
using System.Text;

namespace CypherCompiler
{
    static class Program
    {
        static void Main(string[] args)
        {
            Lexer L = new();
            List<Token> Tokens = L.Tokenize(File.ReadAllText(@"D:\codecuabeo\Cypher\Test\Test.cy"));
            foreach (var TokenItem in Tokens)
            {
                Console.WriteLine(TokenItem.ToString());
            }
            Console.ReadKey();
        }
    }
}
