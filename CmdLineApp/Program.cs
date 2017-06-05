using CmdLineLib;
using CmdLineLib.Attributes;
using System;
using System.Linq;

namespace CmdLineApp
{
    class Program
    {
        public static bool Verbose { get; set; }

        static void Main(string[] args)
        {
            AppGuard.Invoke(() =>
            {
                CmdLine<Program>.Validate();
                CmdLine<Program>.Execute(args);
            });
        }

        static public void RunWithOne([CmdLineArg(helpText: "Value to pass")]int value)
        {
            Console.WriteLine($"RunWithOne: value={value}");
        }

        static public void RunWithOptional(int value = 0)
        {
            Console.WriteLine($"RunWithOptional: value={value}");
        }

        static public void RunWithTwo(int value, string str)
        {
            Console.WriteLine($"RunWithTwo: value={value} str={str}");
        }

        static public void RunWithTwo(int value, bool stuff, string something = null)
        {
            Console.WriteLine($"RunWithTwo: value={value} stuff={stuff} something={something}");
        }

        static public void RunWithNullable(int? value)
        {
            Console.WriteLine($"RunWithNullable: hasvalue={value.HasValue} value={value.Value}");
        }

        static public void RunWithNullableWithDefault(int? value = null)
        {
            Console.WriteLine("RunWithNullableWithDefault: hasvalue={0} value={1}", value.HasValue, value.HasValue ? value.Value.ToString() : "null");
        }

        static public void RunWithArray(int value, int[] array)
        {
            var str = array.Aggregate((string)null, (s, i) => (s == null ? $"{i}" : $"{s},{i}"));
            Console.WriteLine($"RunWithArray: value={value} array={str}");
        }

        [CmdLineMethod(helpText: "Method with help text")]
        static public void RunWithHelpText(int value, int[] array)
        {
            var str = array.Aggregate((string)null, (s, i) => (s == null ? $"{i}" : $"{s},{i}"));
            Console.WriteLine($"RunWithHelpText: value={value} array={str}");
        }

        [CmdLineMethod(command: "run", helpText: "Method with help text")]
        static public void RunWithCrazyAssNameAndHelpTextAndWhatnots(int value, int[] array)
        {
            var str = array.Aggregate((string)null, (s, i) => (s == null ? $"{i}" : $"{s},{i}"));
            Console.WriteLine($"RunWithCrazyAssNameAndHelpTextAndWhatnots (aka. run): value={value} array={str}");
        }

        static public void Run2([CmdLineArg(argName: "x")]int value)
        {
            Console.WriteLine($"Run2: value={value}");
        }

    }
}
