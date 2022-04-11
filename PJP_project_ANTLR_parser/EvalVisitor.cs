using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PJP_project_ANTLR_parser
{
    public class EvalVisitor : MyGrammarBaseVisitor<string>
    {
        public override string VisitInt([NotNull] MyGrammarParser.IntContext context)
        {
            var value = Convert.ToInt32(context.INT().GetText(), 10);
            return $"PUSH {value}\n";
        }
        public override string VisitHexa([NotNull] MyGrammarParser.HexaContext context)
        {
            var value = Convert.ToInt32(context.HEXA().GetText(), 16);
            return $"PUSH {value}\n";
        }
        public override string VisitOct([NotNull] MyGrammarParser.OctContext context)
        {
            var value = Convert.ToInt32(context.OCT().GetText(), 8);
            return $"PUSH {value}\n";
        }
        public override string VisitFloat([NotNull] MyGrammarParser.FloatContext context)
        {
            var value = Convert.ToDecimal(context.FLOAT().GetText());
            return $"PUSH {value}\n";
        }
        public override string VisitString([NotNull] MyGrammarParser.StringContext context)
        {
            var value = context.STRING().GetText();
            return $"PUSH {value}\n";
        }
        public override string VisitPar([NotNull] MyGrammarParser.ParContext context)
        {
            return Visit(context.expr());
        }
        public override string VisitAdd([NotNull] MyGrammarParser.AddContext context)
        {
            var left = Visit(context.expr()[0]);
            var right = Visit(context.expr()[1]);
            if (context.op.Text.Equals("+"))
            {
                return left + right + "ADD\n";
            }
            else
            {
                return left + right + "SUB\n";
            }
        }
        public override string VisitMul([NotNull] MyGrammarParser.MulContext context)
        {
            var left = Visit(context.expr()[0]);
            var right = Visit(context.expr()[1]);
            if (context.op.Text.Equals("*"))
            {
                return left + right + "MUL\n";
            }
            else
            {
                return left + right + "DIV\n";
            }
        }
        public override string VisitProg([NotNull] MyGrammarParser.ProgContext context)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var expr in context.statement())
            {
                var code = Visit(expr);
                sb.Append(code);
                sb.AppendLine("PRINT");
            }
            return sb.ToString();
        }
        public override string VisitIdentifier([NotNull] MyGrammarParser.IdentifierContext context)
        {
            var value = context.IDENTIFIER().GetText();
            return value;
        }
    }
}
