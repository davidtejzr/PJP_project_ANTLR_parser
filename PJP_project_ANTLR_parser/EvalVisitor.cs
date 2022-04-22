using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PJP_project_ANTLR_parser
{
    /*public struct Data
    {
        public string DataType;
        public string Value;
    }*/

    public class EvalVisitor : MyGrammarBaseVisitor<string>
    {
        StringBuilder sb = new StringBuilder();
        public override string VisitProg([NotNull] MyGrammarParser.ProgContext context)
        {
            foreach (var expr in context.line())
            {
                var code = Visit(expr);
            }
            //Data data = new Data();
            //data.Value = sb.ToString();
            //return data;
            return sb.ToString();
        }

        public override string VisitFormatedWrite([NotNull] MyGrammarParser.FormatedWriteContext context)
        {
            int exprCount = 0;
            foreach(var item in context.children)
            {
                if (item.GetText() == ",")
                    break;

                sb.AppendLine("push S " + item);

                exprCount = context.expr().Length;
                for (int i = 0; i < exprCount; i++)
                {
                    var result = Visit(context.expr(0));
                    var dd = result.GetTypeCode();
                    sb.AppendLine("push " + result);
                }
            }

            sb.AppendLine("print " + ++exprCount);
            return "";
        }

        public override string VisitInt([NotNull] MyGrammarParser.IntContext context)
        {
            var value = Convert.ToInt32(context.INT().GetText());
            return "I " + value.ToString();
        }
        public override string VisitFloat([NotNull] MyGrammarParser.FloatContext context)
        {
            var value = Convert.ToDecimal(context.FLOAT().GetText());
            return "F " + value.ToString();
        }
        public override string VisitPar([NotNull] MyGrammarParser.ParContext context)
        {
            return Visit(context.expr());
        }
        public override string VisitAssignment([NotNull] MyGrammarParser.AssignmentContext context)
        {
            var koko = context.assignment();
            return koko.ToString();
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
        public override string VisitIdentifier([NotNull] MyGrammarParser.IdentifierContext context)
        {
            //var value = context.IDENTIFIER().GetText();
            var value = "test";
            return value;
        }

        public override string VisitConcat([NotNull] MyGrammarParser.ConcatContext context)
        {
            //var value = context.IDENTIFIER().GetText();
            var value = "v";
            return value;
        }
    }
}
