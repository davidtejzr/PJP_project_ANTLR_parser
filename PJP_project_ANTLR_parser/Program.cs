using Antlr4.Runtime;
using Antlr4.Runtime.Tree;
using System.Globalization;

namespace PJP_project_ANTLR_parser
{
    internal class Program
    {
        public static void Main(string[] args)
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            var fileName = "input1.txt";
            Console.WriteLine("Parsing: " + fileName);
            var inputFile = new StreamReader(fileName);
            AntlrInputStream input = new AntlrInputStream(inputFile);
            MyGrammarLexer lexer = new MyGrammarLexer(input);
            CommonTokenStream tokens = new CommonTokenStream(lexer);
            MyGrammarParser parser = new MyGrammarParser(tokens);

            parser.AddErrorListener(new VerboseErrorListener());

            IParseTree tree = parser.prog();

            if (parser.NumberOfSyntaxErrors == 0)
            {
                //Console.WriteLine(tree.ToStringTree(parser));

                var result = new EvalVisitor().Visit(tree);
                Console.WriteLine(result);

                VirtualMachine virtualMachine = new VirtualMachine(result);
                virtualMachine.Run();
            }
        }
    }
}