using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PJP_project_ANTLR_parser
{
    public struct MachineData
    {
        public string DataType;
        public string Value;
    }

    public class VirtualMachine
    {
        List<string> code;
        Stack<MachineData> stack = new Stack<MachineData>();
        public VirtualMachine(string code)
        {
            this.code = code.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public void Run()
        {
            foreach(var instruction in code)
            {
                //var data = instruction.Split(" ");
                string[] data;
                if(instruction.Contains("\""))
                {
                    var tmp = instruction.Split("\"");
                    data = tmp[0].Split(" ");
                    data[data.Length - 1] = "\t" + tmp[1];
                }
                else
                {
                    data = instruction.Split(" ");
                }

                switch (data[0])
                {
                    case "print":
                        print(data[1]);
                        break;
                    case "push":
                        push(data);
                        break;
                    case "add":
                        calc("+");
                        break;
                    case "sub":
                        calc("-");
                        break;
                    case "mul":
                        calc("*");
                        break;
                    case "div":
                        calc("/");
                        break;
                    case "mod":
                        calc("%");
                        break;
                    case "lt":
                        compare("<");
                        break;
                    case "gt":
                        compare(">");
                        break;
                    case "eq":
                        compare("==");
                        break;
                    /*case "not":
                        compare("!=");
                        break;*/
                    case "or":
                        logic("||");
                        break;
                    case "and":
                        logic("&&");
                        break;

                    default:
                        break;
                };
            }
        }

        public void print(string count)
        {
            for(int i = 0; i < int.Parse(count); i++)
            {
                Console.WriteLine(stack.Pop().Value);
            }
        }

        public void push(string[] data)
        {
            MachineData md = new MachineData();
            md.DataType = data[1];
            if(data.Length > 2)
                md.Value = data[2];
            stack.Push(md);
        }

        public void calc(string oper)
        {
            MachineData md = new MachineData();
            var right = stack.Pop();
            var left = stack.Pop();
            float result = 0;

            if (((left.DataType == "I") || (left.DataType == "F")) && ((right.DataType == "I") || (right.DataType == "F")))
            {
                if(oper == "+")
                    result = float.Parse(left.Value) + float.Parse(right.Value);
                else if(oper == "-")
                    result = float.Parse(left.Value) - float.Parse(right.Value);
                else if(oper == "*")
                    result = float.Parse(left.Value) * float.Parse(right.Value);
                else if(oper == "/")
                    result = float.Parse(left.Value) / float.Parse(right.Value);

                if ((right.DataType == "I") && (left.DataType == "I"))
                {
                    if (oper == "%")
                        result = int.Parse(left.Value) % int.Parse(right.Value);

                    md.Value = ((int)result).ToString();
                    md.DataType = "I";
                }
                else
                {
                    md.Value = ((float)result).ToString();
                    md.DataType = "F";
                }
                stack.Push(md);
            }
        }

        public void compare(string comp)
        {
            MachineData md = new MachineData();
            var right = stack.Pop();
            var left = stack.Pop();

            if(comp == "<")
            {
                if (((left.DataType == "I") || (left.DataType == "F")) && ((right.DataType == "I") || (right.DataType == "F")))
                {
                    if (float.Parse(left.Value) < float.Parse(right.Value))
                        md.Value = "True";
                    else
                        md.Value = "False";
                }
            }
            else if(comp == ">")
            {
                if (((left.DataType == "I") || (left.DataType == "F")) && ((right.DataType == "I") || (right.DataType == "F")))
                {
                    if (float.Parse(left.Value) > float.Parse(right.Value))
                        md.Value = "True";
                    else
                        md.Value = "False";
                }
            }
            else if(comp == "==")
            {
                if (left.Value == right.Value)
                    md.Value = "True";
                else
                    md.Value = "False";
            }
            else if (comp == "!=")
            {
                if (left.Value != right.Value)
                    md.Value = "True";
                else
                    md.Value = "False";
            }

            stack.Push(md);
        }

        public void logic(string log)
        {
            MachineData md = new MachineData();
            var right = stack.Pop();
            var left = stack.Pop();

            if (log == "||")
            {
                if ((left.DataType == "B") && (right.DataType == "B"))
                {
                    if ((bool.Parse(left.Value)) || (bool.Parse(right.Value)))
                        md.Value = "True";
                    else
                        md.Value = "False";
                }
            }
            else if (log == "&&")
            {
                if ((left.DataType == "B") && (right.DataType == "B"))
                {
                    if ((bool.Parse(left.Value)) && (bool.Parse(right.Value)))
                        md.Value = "True";
                    else
                        md.Value = "False";
                }
            }

            stack.Push(md);
        }
    }
}
