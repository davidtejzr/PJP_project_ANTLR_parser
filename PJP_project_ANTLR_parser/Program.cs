using Antlr4.Runtime;
using Antlr4.Runtime.Tree;

namespace PJP_project_ANTLR_parser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var inputFile = new StreamReader("input1.txt");
            AntlrInputStream stream = new AntlrInputStream(inputFile);
            ITokenSource lexer = new MyGrammarLexer(stream);
            ITokenStream tokens = new CommonTokenStream(lexer);
            MyGrammarParser parser = new MyGrammarParser(tokens);
            parser.BuildParseTree = true;
            IParseTree tree = parser.prog();
            Console.WriteLine(tree.ToStringTree(parser));
        }
    }
}