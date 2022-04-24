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
        Dictionary<string, MachineData> variables = new Dictionary<string, MachineData>();
        Dictionary<int, int> labels = new Dictionary<int, int>();
        int blockId = 0;
        public VirtualMachine(string code)
        {
            this.code = code.Split("\n\r".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        public void Run()
        {
            //foreach (var instruction in code)
            for(; blockId < code.Count; blockId++)
            {
                var instruction = code[blockId];
                if (instruction.StartsWith("label"))
                {
                    var splitted = instruction.Split(" ");
                    label(int.Parse(splitted[1]));
                }
            }

            blockId = 0;

            //foreach(var instruction in code)
            for(; blockId < code.Count; blockId++)
            {
                var instruction = code[blockId];
                string[] data;
                if(instruction.Contains("\""))
                {
                    var tmp = instruction.Split("\"");
                    data = tmp[0].Split(" ");
                    data[data.Length - 1] = tmp[1];
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
                    case "read":
                        read(data[1]);
                        break;
                    case "push":
                        push(data);
                        break;
                    case "pop":
                        pop();
                        break;
                    case "save":
                        save(data[1]);
                        break;
                    case "load":
                        load(data[1]);
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
                    case "uminus":
                        uminus();
                        break;
                    case "itof":
                        itof();
                        break;
                    case "concat":
                        concat();
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
                    case "not":
                        not();
                        break;
                    case "or":
                        logic("||");
                        break;
                    case "and":
                        logic("&&");
                        break;
                    case "label":
                        label(int.Parse(data[1]));
                        break;
                    case "jmp":
                        jmp(int.Parse(data[1]));
                        break;
                    case "fjmp":
                        fjmp(int.Parse(data[1]));
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
                var value = stack.Pop();
                if(value.DataType == "S")
                    Console.WriteLine("\t" + value.Value);
                else
                    Console.WriteLine(value.Value);
            }
        }

        public void read(string type)
        {
            MachineData md = new MachineData();

            switch (type)
            {
                case "I":
                    md.DataType = "I";
                    break;
                case "F":
                    md.DataType = "F";
                    break;
                case "S":
                    md.DataType = "S";
                    break;
                case "B":
                    md.DataType = "B";
                    break;
                default:
                    return;
            }

            Console.Write("input " + type + ": ");
            md.Value = Console.ReadLine();
            stack.Push(md);
        }

        public void push(string[] data)
        {
            MachineData md = new MachineData();
            md.DataType = data[1];
            if (data.Length > 2)
            {
                md.Value = data[2];
            }
            stack.Push(md);
        }

        public void pop()
        {
            //stack.Pop();
        }

        public void save(string variable)
        {
            var value = stack.Pop();

            if(variables.ContainsKey(variable))
                variables[variable] = value;
            else
                variables.Add(variable, value);
        }
        public void load(string variable)
        {
            if(variables.ContainsKey(variable))
            {
                stack.Push(variables[variable]);
            }
            else
            {
                throw new Exception("Variable '" + variable + "' not exist!");
            }
        }

        public void calc(string oper)
        {
            MachineData md = new MachineData();
            md.DataType = "F";
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

        public void uminus()
        {
            MachineData md = new MachineData();
            var value = stack.Pop();
            if (value.DataType == "I")
            {
                int result = int.Parse(value.Value);
                result *= -1;
                md.Value = result.ToString();
                stack.Push(md);
            }
            else if (value.DataType == "F")
            {
                float result = float.Parse(value.Value);
                result *= -1;
                md.Value = result.ToString();
                stack.Push(md);
            }
        }

        public void itof()
        {
            var value = stack.Pop();
            if(value.DataType == "I")
            {
                float floatVal = float.Parse(value.Value);
                value.Value = String.Format("{0:0.0}", floatVal);
                stack.Push(value);
            }
        }

        public void concat()
        {
            MachineData md = new MachineData();
            var second = stack.Pop();
            var first = stack.Pop();

            string concated = first.Value.ToString() + second.Value.ToString();
            md.Value = concated;
            stack.Push(md);
        }

        public void compare(string comp)
        {
            MachineData md = new MachineData();
            md.DataType = "B";
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

            stack.Push(md);
        }

        public void not()
        {
            var value = stack.Pop();
            if(value.DataType == "B")
            {
                bool bVal = bool.Parse(value.Value);
                value.Value = (!bVal).ToString();
                stack.Push(value);
            }
        }

        public void logic(string log)
        {
            MachineData md = new MachineData();
            md.DataType = "B";
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

        public void label(int id)
        {
            if(labels.ContainsKey(id))
                labels[id] = blockId;
            else
                labels.Add(id, blockId);
        }

        public void jmp(int id)
        {
            if(labels.ContainsKey(id))
                labels.TryGetValue(id, out blockId);
            else
                throw new Exception("Label ID not found!");
        }

        public void fjmp(int id)
        {
            var value = stack.Pop();
            if(value.DataType == "B")
            {
                if (value.Value == "False")
                {
                    if (labels.ContainsKey(id))
                        labels.TryGetValue(id, out blockId);
                    else
                        throw new Exception("Label ID not found!");
                }
            }
            else
            {
                throw new Exception("Condition type error!");
            }
        }
    }
}
