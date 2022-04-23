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
        int exprCount = 0;
        public string operators = "+-*/%";

        public override Data VisitProg([NotNull] MyGrammarParser.ProgContext context)
        {
            Data data = new Data();
            foreach (var expr in context.line())
            {
                Visit(expr);
            }

            data.Value = sb.ToString();
            return data;
        }

        public override Data VisitWrite([NotNull] MyGrammarParser.WriteContext context)
        {
            Data data = new Data();
            exprCount = 0;
            foreach (var item in context.writePart())
            {
                exprCount++;
                VisitWritePart(item);
            }

            sb.AppendLine("print " + exprCount);

            return data;
        }

        public override Data VisitWritePart([NotNull] MyGrammarParser.WritePartContext context)
        {
            Data data = new Data();
            var item = context.STRING();

            sb.AppendLine("push S " + item);
            if (context.expr(0) != null)
            {
                var result = Visit(context.expr()[0]);
                if ((result.DataType == "I") || (result.DataType == "F") || (result.DataType == "B"))
                    sb.AppendLine("push " + result.DataType + " " + result.Value);
                exprCount++;
            }

            return data;
        }

        public override Data VisitRead([NotNull] MyGrammarParser.ReadContext context)
        {
            Data data = new Data();
            foreach (var item in context.IDENTIFIER())
            {
                if (item.GetText() == ",")
                    break;

                if (variables.ContainsKey(item.ToString()))
                {
                    var searchType = variables[item.ToString()];
                    //var searchType = variables.FirstOrDefault(x => x.Value == item.ToString()).Key;
                    sb.AppendLine("read " + searchType);
                    sb.AppendLine("save " + item.ToString());
                }
                else
                    err.variableNotexistError(item.ToString());
            }
            return data;
        }

        /*public override Data VisitStatement([NotNull] MyGrammarParser.StatementContext context)
        {
            if(context.declaration() != null)
                return VisitDeclaration(context.declaration());
            else if(context.assignment() != null)
            {
                sb.AppendLine("pop");
                return VisitAssignment(context.assignment());
            }

            return VisitExpr(context.expr());
        }*/

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
            data.Value = bool.Parse(context.BOOL().ToString()).ToString();
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
                foreach (var child in context.children)
                {
                    if ((child.ToString() == ",") || (child.ToString() == varName))
                        continue;

                    if (varName == "string")
                    {
                        data.DataType = "S";
                        data.Value = "\"\"";
                    }
                    else if (varName == "float")
                    {
                        data.DataType = "F";
                        data.Value = "0.0";
                    }
                    else if (varName == "int")
                    {
                        data.DataType = "I";
                        data.Value = "0";
                    }
                    else if (varName == "bool")
                    {
                        data.DataType = "B";
                        data.Value = "true";
                    }
                    else
                        err.datatypeUnknownError(context.children[0].ToString());

                    sb.AppendLine("push " + data.DataType + " " + data.Value.ToString());
                    sb.AppendLine("save " + child.ToString());
                    variables[child.ToString()] = data.DataType;
                }
            }

            return data;
        }

        public override Data VisitAssignment([NotNull] MyGrammarParser.AssignmentContext context)
        {
            Data data = new Data();
            bool isUminus = false;
            bool isItof = false;

            //funguje ale robije jine veci
            if (context.expr() != null)
            {
                return Visit(context.expr());
            }
            var value = VisitAssignment(context.assignment());

            if (context.children[0] != null)
            {
                if (variables.ContainsKey(context.children[0].ToString()))
                {
                    string searchValue = null;
           
                    if (context.children[2].GetChild(0).GetChild(0) != null)
                        searchValue = context.children[2].GetChild(0).GetChild(0).ToString();
                    var searchType = variables[context.children[0].ToString()];

                    //kontrola zapornych hodnot
                    if ((searchType == "I") && (searchValue != null))
                    {
                        int result;
                        int.TryParse(searchValue, out result);
                        if(result < 0)
                        {
                            int val = int.Parse(searchValue);
                            val *= -1;
                            searchValue = val.ToString();
                            isUminus = true;
                        }
                    }
                        
                    if ((searchType == "F") && (searchValue != null))
                    {
                        float result;
                        float.TryParse(searchValue, out result);
                        if (result < 0)
                        {
                            int val = int.Parse(searchValue);
                            val *= -1;
                            searchValue = val.ToString();
                            isUminus = true;
                        }
                    }

                    if((value.DataType == "I") && (searchType == "F"))
                    {
                        searchType = "I";
                        searchValue = int.Parse(searchValue).ToString();  
                        isItof = true;
                    }

                    data.DataType = searchType;
                    data.Value = searchValue;
                    sb.AppendLine("push " + searchType + " " + searchValue);

                    if (isItof)
                        sb.AppendLine("itof");

                    if (isUminus)
                        sb.AppendLine("uminus");

                    values[context.children[0].ToString()] = searchValue;
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

            if ((left.DataType != null) && (left.Value != null))
                sb.AppendLine("push " + left.DataType + " " + left.Value.ToString());
            if ((right.DataType != null) && (right.Value != null))
                sb.AppendLine("push " + right.DataType + " " + right.Value.ToString());

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

            if((left.DataType != null) && (left.Value != null))
                sb.AppendLine("push " + left.DataType + " " + left.Value.ToString());
            if ((right.DataType != null) && (right.Value != null))
                sb.AppendLine("push " + right.DataType + " " + right.Value.ToString());

            if ((left.DataType != null) && (right.DataType != null))
                if (((left.DataType == "I") && (right.DataType == "F")) || ((left.DataType == "F") && (right.DataType == "I")))
                    sb.AppendLine("itof");

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
