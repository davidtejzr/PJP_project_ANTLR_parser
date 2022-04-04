using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PJP_project_ANTLR_parser
{
    public class VirtualMachine
    {
        List<string> code;
        Stack<int> stack = new Stack<int> ();
        public VirtualMachine(string code)
        {
            this.code = code.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public void Run()
        {
            foreach(var instruction in code)
            {
                if(instruction.StartsWith("PRINT"))
                {
                    Console.WriteLine(stack.Pop());
                }
                else if(instruction.StartsWith("PUSH"))
                {
                    var data = instruction.Split(" ");
                    var value = int.Parse(data[1]);
                    stack.Push(value);
                }
                else
                {
                    var right = stack.Pop();
                    var left = stack.Pop();
                    var result = instruction switch
                    {
                        "ADD" => left + right,
                        "SUB" => left - right,
                        "MUL" => left * right,
                        "DIV" => left / right,
                        _ => throw new Exception("Unexpected instruction.")
                    };
                    stack.Push(result);
                }
            }
        }
    }
}
