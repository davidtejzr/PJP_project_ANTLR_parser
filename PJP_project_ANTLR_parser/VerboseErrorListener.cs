﻿using Antlr4.Runtime;
using Antlr4.Runtime.Misc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PJP_project_ANTLR_parser
{
    public class VerboseErrorListener : BaseErrorListener
    {
       public override void SyntaxError([NotNull] IRecognizer recognizer, [Nullable] IToken offendingSymbol, int line, int charPositionInLine, [NotNull] string msg, [Nullable] RecognitionException e)
       {

            IList<string> stack = ((Parser)recognizer).GetRuleInvocationStack();
            stack.Reverse();

            Console.Error.WriteLine("rule stack: " + String.Join(", ", stack));
            Console.Error.WriteLine("line " + line + ":" + charPositionInLine + " at " + offendingSymbol + ": " + msg);
       }

        public void datatypeUnknownError(string datatype)
        {
            Console.Error.WriteLine("datatype '" + datatype + "' unknown");
        }
        public void variableNotexistError(string variable)
        {
            Console.Error.WriteLine("variable '" + variable + "' not exist");
        }

    }
}
