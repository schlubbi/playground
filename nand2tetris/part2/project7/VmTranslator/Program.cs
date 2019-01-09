using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;

namespace VmTranslator
{
    public class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Usage();
                return;
            }
            var vmFileName = args[0];
            if (!File.Exists(vmFileName))
            {
                Usage();
                return;
            }

            var parser = new Parser(vmFileName);
            var hackFileName = vmFileName.Replace("vm", "hack");
            using (var codeWriter = new CodeWriter(hackFileName))
            {
                while (parser.HasMoreCommands)
                {
                    Console.WriteLine(parser.CurrentCommand());
                    var command = parser.CurrentCommand();
                    if (command.CommandType == CommandType.Arithmetic)
                    {
                        codeWriter.WriteArithmetic(command);
                    }
                    else
                    {
                        codeWriter.WritePushPop(command);
                    }
                    parser.Advance();
                }
            }
        }

        static void Usage()
        {
            Console.WriteLine("VmTranslator: Translate *.vm to *.asm");
            Console.WriteLine("-------------------------------------");
            Console.WriteLine("Run: dotnet run example.vm");
        }
    }

    public class Parser
    {
        private string _vmFileName;
        private int _index = 0;
        private List<string> _lines;

        public Parser(string vmFileName)
        {
            _vmFileName = vmFileName;
            _lines = File.ReadAllLines(_vmFileName).Where(l => !l.StartsWith("//")).Where(l => !string.IsNullOrEmpty(l)).ToList();
        }

        public bool HasMoreCommands => _index + 1 <=  _lines.Count();

        public void Advance()
        {
            _index++;
        }

        public Command CurrentCommand()
        {
            var line = _lines[_index];
            var parts = line.Split(' ');
            if (parts.Length == 1)
            {
                return new Command(parts[0]);
            }
            else
            {
                return new Command(parts[0], parts[1], parts[2]);
            }
        }
    }

    public class Command
    {
        private readonly List<string> ArithmeticAndLogicalCommands = new List<string>() { "add", "sub", "neg", "eq", "gt", "lt", "and", "or", "not" };

        public Command(string name)
        {
            Name = name;
        }

        public Command(string name, string arg1, string arg2)
        {
            Name = name;
            Arg1 = arg1;
            Arg2 = arg2;
            CommandType = ParseCommandType();
        }

        public string Name { get; }
        public string Arg1 { get; }
        public string Arg2 { get; }
        public CommandType CommandType { get; }

        public override string ToString()
        {
            return $"{Name} {Arg1} {Arg2} {CommandType}";
        }

        private CommandType ParseCommandType()
        {
            if (ArithmeticAndLogicalCommands.Contains(Name))
            {
                return CommandType.Arithmetic;
            }
            else if (Name == "push")
            {
                return CommandType.Push;
            }
            else if (Name == "pop")
            {
                return CommandType.Pop;
            }
            else if (Name == "label")
            {
                return CommandType.Label;
            }
            else if (Name == "goto")
            {
                return CommandType.Goto;
            }
            else if (Name == "call")
            {
                return CommandType.Call;
            }
            else if (Name == "function")
            {
                return CommandType.Function;
            }
            else if (Name == "if-goto")
            {
                return CommandType.If;
            }
            else if (Name == "return")
            {
                return CommandType.Return;
            }
            else
            {
                throw new InvalidOperationException("Unsupported Command");
            }
        }
    }

    public enum CommandType
    {
        Arithmetic,
        Push,
        Pop,
        Label,
        Goto,
        If,
        Function,
        Return,
        Call
    }

    public class CodeWriter : IDisposable
    {
        private List<string> _lines = new List<string>();
        private string _vmFileName;

        public CodeWriter(string vmFileName)
        {
            _vmFileName = vmFileName;
        }

        public void WriteArithmetic(Command command)
        {
            string statement;
            switch(command.Name)
            {
                case "add":
                    statement = "D=D+M";
                    break;
                case "sub":
					statement = "D=D-M";
                    break;
                default:
                    throw new InvalidOperationException("unsupported arithmetic operation");
            }
			_lines.Add("@SP");
			_lines.Add("A=M");
			_lines.Add("D=M");
			_lines.Add("@SP");
			_lines.Add("M=M-1");
			_lines.Add("A=M");
			_lines.Add(statement);
			_lines.Add("@SP");
			_lines.Add("A=M");
			_lines.Add("M=D");
			_lines.Add("@SP");
			_lines.Add("M=M+1");

        }

        public void WritePushPop(Command command)
        {
            if (command.CommandType == CommandType.Push)
            {
                Push(command);
            }
            else
            {
                Pop(command);
            }
        }

        public void Dispose()
        {
            File.WriteAllLines(_vmFileName, _lines);
        }

        private void Push(Command command)
        {
            switch(command.Arg1)
            {
                case "temp":
                    PushTemp(command.Arg2);
                    break;
                case "pointer":
                    break;
                case "static":
                    break;
                case "constant":
                    PushConstant(command.Arg2);
                    break;
                default:
                    PushSegment(command.Arg1, command.Arg2);
                    break;
            }
        }

        private void Pop(Command command)
        {
            switch(command.Arg1)
            {
                case "temp":
					PopTemp(command.Arg2);
                    break;
                case "pointer":
                    break;
                case "static":
                    break;
                default:
                    PopSegment(command.Arg1, command.Arg2);
                    break;
            }
        }

		private void PopTemp(string i)
		{
			_lines.Add("@i");
			_lines.Add("D=A");
			_lines.Add("@R5");
			_lines.Add("A=A+D");
			_lines.Add("D=M");
			_lines.Add("@POPTEMP");
			_lines.Add("M=D");
			_lines.Add("@SP");
			_lines.Add("M=M-1");
			_lines.Add("A=M");
			_lines.Add("D=M");
			_lines.Add("@POPTEMP");
			_lines.Add("A=M");
			_lines.Add("M=D");
		}

        private void PopSegment(string segment, string i)
        {
            string memorySegment;
            switch(segment)
            {
                case "local":
                    memorySegment = "LCL";
                    break;
                case "this":
                    memorySegment = "THIS";
                    break;
                case "that":
                    memorySegment = "THAT";
                    break;
                case "argument":
                    memorySegment = "ARG";
                    break;
                default:
                    throw new InvalidOperationException("unsupported segment");

            }
            _lines.Add($"// pop {segment} {i}");
            _lines.Add($"@{i}");
            _lines.Add("D=A");
            _lines.Add($"@{memorySegment}");
            _lines.Add("D=M+D");
            _lines.Add("@POP");
            _lines.Add("M=D");
            _lines.Add("@SP");
            _lines.Add("M=M-1");
            _lines.Add("A=M");
            _lines.Add("D=M");
            _lines.Add("@POP");
            _lines.Add("A=M");
            _lines.Add("M=D");
        }

        private void PushSegment(string segment, string i)
        {
            string memorySegment;
            switch(segment)
            {
                case "local":
                    memorySegment = "LCL";
                    break;
                case "this":
                    memorySegment = "THIS";
                    break;
                case "that":
                    memorySegment = "THAT";
                    break;
                case "argument":
                    memorySegment = "ARG";
                    break;
                default:
                    throw new InvalidOperationException("unsupported segment");
            }

            _lines.Add($"// push {segment} {i}");
            _lines.Add($"@{i}");
            _lines.Add("D=A");
            _lines.Add($"@{memorySegment}");
            _lines.Add("A=M+D");
            _lines.Add("D=M");
            _lines.Add("@SP");
            _lines.Add("A=M");
            _lines.Add("M=D");
            _lines.Add("@SP");
            _lines.Add("M=M+1");
        }

        private void PushTemp(string i)
        {
			_lines.Add($"// push temp {i}");
			_lines.Add($"@{i}");
			_lines.Add("D=A");
			_lines.Add("@R5");
			_lines.Add("A=A+D");
			_lines.Add("D=M");
			_lines.Add("@SP");
			_lines.Add("A=M");
			_lines.Add("M=D");
			_lines.Add("@SP");
			_lines.Add("M=M+1");
        }

        private void PushConstant(string i)
        {
            _lines.Add($"// push constant {i}");
            _lines.Add($"@{i}");
            _lines.Add("D=A");
            _lines.Add("@SP");
            _lines.Add("A=M");
            _lines.Add("M=D");
            _lines.Add("@SP");
            _lines.Add("M=M+1");
        }
    }
}


// D = *p
// @p
// A=M
// D=M
//
// *SP = 17
// SP++
//
// @17
// D=A
// @SP
// A=M
// M=D
// @SP
// M = M + 1
//
// addr=LCL+2
// SP--
// *addr=*SP
//
//
// pop local i => addr = LCL+i, SP--, *addr=*SP
// push local i => addr=LCL+i, *SP=*addr, SP++o
//
// STACK POINTER == SP
// LOCAL == LCL
// ARGUMENT == ARG
// THIS == THIS
// THAT == THAT 
// STATIC == @Filename.i
// D = stack.pop
// @Filename.i
// M=D
// TEMP == RAM 5 to RAM 12 (base address 5 + i)
// POINTER push/pop pointer 0/1 => 0 == THIS 1 == THAT
// *SP = THIS/THAT, SP++
// SP--, THIS/THAT = *SP
