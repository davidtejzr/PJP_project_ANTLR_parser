using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PJP_project_ANTLR_parser
{
    public struct Data
    {
        public string DataType;
        public string Value;
    }

    public class EvalVisitor : MyGrammarBaseVisitor<Data>
    {
        StringBuilder sb = new StringBuilder();
        VerboseErrorListener err = new VerboseErrorListener();
        Dictionary<string, string> variables = new Dictionary<string, string>();
        Dictionary<string, string> values = new Dictionary<string, string>();

        public override Data VisitProg([NotNull] MyGrammarParser.ProgContext context)
        {
            Data data = new Data();
            foreach (var expr in context.line())
            {
                var code = Visit(expr);
            }

            data.Value = sb.ToString();
            return data;
        }

        /*public override Data VisitWrite([NotNull] MyGrammarParser.WriteContext context)
        {
            int exprCount = 0;
            foreach (var item in context.formatedWrite())
            {
                exprCount++;
                VisitFormatedWrite(item);
            }

            sb.AppendLine("print " + exprCount);

            return base.VisitWrite(context);
        }*/

        public override Data VisitFormatedWrite([NotNull] MyGrammarParser.FormatedWriteContext context)
        {
            /*var item = context.STRING();

            sb.AppendLine("push S " + item);
            if (context.expr(0) != null)
            {
                var result = Visit(context.expr()[0]);
                if ((result.DataType == "I") || (result.DataType == "F") || (result.DataType == "B"))
                    sb.AppendLine("push " + result.DataType + " " + result.Value);
            }

            return base.VisitFormatedWrite(context);*/
            Data data = new Data();
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
                    if ((result.DataType == "I") || (result.DataType == "F") || (result.DataType == "B"))
                        sb.AppendLine("push " + result.DataType + " " + result.Value);
                }
            }

            sb.AppendLine("print " + ++exprCount);
            return data;
        }

        public override Data VisitInt([NotNull] MyGrammarParser.IntContext context)
        {
            Data data = new Data();
            data.Value = Convert.ToInt32(context.INT().GetText()).ToString();
            data.DataType = "I";
            return data;
        }
        public override Data VisitFloat([NotNull] MyGrammarParser.FloatContext context)
        {
            Data data = new Data();
            data.Value = Convert.ToDecimal(context.FLOAT().GetText()).ToString();
            data.DataType = "F";
            return data;
        }
        public override Data VisitBool([NotNull] MyGrammarParser.BoolContext context)
        {
            Data data = new Data();
            data.Value = Convert.ToBoolean(context.BOOL().GetText()).ToString();
            data.DataType = "B";
            return data;
        }
        public override Data VisitPar([NotNull] MyGrammarParser.ParContext context)
        {
            Data data = new Data();

            return Visit(context.expr());
        }

        public override Data VisitDeclaration([NotNull] MyGrammarParser.DeclarationContext context)
        {
            Data data = new Data();
            if (context.children[0] != null)
            {
                string varName = context.children[0].ToString();
                if(varName == "string")
                {
                    data.DataType = "S";
                    data.Value = "\"\"";
                }
                else if(varName == "float")
                {
                    data.DataType = "F";
                    data.Value = "0.0";
                }
                else if(varName == "int")
                {
                    data.DataType = "I";
                    data.Value = "0";
                }
                else if(varName == "bool")
                {
                    data.DataType = "B";
                    data.Value = "false";
                }
                else
                    err.datatypeUnknownError(context.children[0].ToString());

                sb.AppendLine("push " + data.DataType + " " + data.Value.ToString());
                sb.AppendLine("save " + context.children[1].ToString());
                variables[data.DataType] = context.children[1].ToString();
            }

            return data;
        }

        public override Data VisitAssignment([NotNull] MyGrammarParser.AssignmentContext context)
        {
            Data data = new Data();

            if (context.children[0] != null)
            {
                if (variables.ContainsValue(context.children[0].ToString()))
                {
                    var searchValue = context.children[2].GetChild(0).GetChild(0);
                    if (variables.ContainsValue(context.children[0].ToString()))
                    {
                        var searchType = variables.FirstOrDefault(x => x.Value == context.children[0].ToString()).Key;
                        sb.AppendLine("push " + searchType + " " + searchValue);
                    }

                    values[context.children[0].ToString()] = context.children[2].ToString();
                    sb.AppendLine("save " + context.children[0].ToString());
                    sb.AppendLine("load " + context.children[0].ToString());
                    sb.AppendLine("pop");
                }
                else
                    err.variableNotexistError(context.children[0].ToString());
            }

            return data;
        }
        public override Data VisitAdd([NotNull] MyGrammarParser.AddContext context)
        {
            Data data = new Data();
            var left = Visit(context.expr()[0]);
            var right = Visit(context.expr()[1]);
            if (context.op.Text.Equals("+"))
            {
                data.Value = left.ToString() + right.ToString() + "ADD\n";
                sb.AppendLine("add");
            }
            else
            {
                data.Value = left.ToString() + right.ToString() + "SUB\n";
                sb.AppendLine("sub");
            }

            return data;
        }
        public override Data VisitMul([NotNull] MyGrammarParser.MulContext context)
        {
            Data data = new Data();
            var left = Visit(context.expr()[0]);
            var right = Visit(context.expr()[1]);
            if (context.op.Text.Equals("*"))
            {
                data.Value = left.ToString() + right.ToString() + "MUL\n";
                sb.AppendLine("mul");

            }
            else if (context.op.Text.Equals("%"))
            {
                data.Value = left.ToString() + right.ToString() + "MOD\n";
                sb.AppendLine("mod");

            }
            else
            {
                data.Value = left.ToString() + right.ToString() + "DIV\n";
                sb.AppendLine("div");
            }

            return data;
        }
        public override Data VisitIdentifier([NotNull] MyGrammarParser.IdentifierContext context)
        {
            Data data = new Data();

            sb.AppendLine("load " + context.IDENTIFIER().GetText());

            return data;
        }

        public override Data VisitConcat([NotNull] MyGrammarParser.ConcatContext context)
        {
            Data data = new Data();

            string first = context.children[0].ToString();
            string second = context.children[2].ToString();

            sb.AppendLine("push S " + first);
            sb.AppendLine("push S " + second);
            sb.AppendLine("concat");

            return data;
        }
    }
}
