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
            AppGuard.Invoke(() => CmdLine<Program>.Execute(args, new CmdLineConfig { ArgStartsWith = '-' }));
            AppGuard.DebugReadLine();
        }

        static void WriteForegroundColors()
        {
            DoColor(false);
        }

        static void WriteBackgroundColors()
        {
            DoColor(true);
        }

        static void DoColor(bool doBackground)
        {
            var c2 = new AnsiColorConsole();
            Console.WriteLine(doBackground ? "Background" : "Foreground");

            int ansix = doBackground ? 4 : 3;
            string text = doBackground ? "ab" : "\u2588\u2588";
            Console.Write("ANSI [0-7] :");
            for (int i = 0; i < 8; i++)
            {
                var c = i;
                Console.Write($"\u001b[{ansix}{c}m{c,2}{text}");
            }
            Console.WriteLine("\u001b[0m");
            Console.Write("DOS  [0-7] :");
            for (int i = 0; i < 8; i++)
            {
                if (doBackground)
                    Console.BackgroundColor = (ConsoleColor)i;
                else
                    Console.ForegroundColor = (ConsoleColor)i;
                Console.Write($"{i,2}{text}");
            }
            Console.WriteLine();
            Console.ResetColor();
            Console.Write("ANSI [8-15]:");
            for (int i = 0; i < 8; i++)
            {
                var c = i;
                Console.Write($"\u001b[{ansix}{c};1m{c,2}{text}");
            }
            Console.WriteLine("\u001b[0m");
            if (doBackground)
            {
                Console.Write("ANSIx[8-15]:");
                for (int i = 0; i < 8; i++)
                {
                    var c = i;
                    Console.Write($"\u001b[48;5;{c + 8}m{c,2}{text}");
                }
                Console.WriteLine("\u001b[0m");
            }

            Console.Write("DOS  [8-15]:");
            for (int i = 8; i < 16; i++)
            {
                if (doBackground)
                    Console.BackgroundColor = (ConsoleColor)i;
                else
                    Console.ForegroundColor = (ConsoleColor)i;
                Console.Write($"{i,2}{text}");
            }
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("IColorConsole implementations:");

            WriteConsoleImplementation(new ColorConsole(), "DOS ", doBackground);
            WriteConsoleImplementation(new AnsiColorConsole(), "ANSI", doBackground);
            WriteConsoleImplementation(SystemConsole.CreateColorConsole(), "AUTO", doBackground);
        }

        static void WriteConsoleImplementation(IColorConsole con, string prefix, bool doBackground)
        {
            string text = doBackground ? "ab" : "\u2588\u2588";
            Console.Write("{0}:", prefix);
            for (int i = 0; i < 16; i++)
                if (doBackground)
                    con.w((ConsoleColor)(-1), (ConsoleColor)i, $"{i,2}{text}");
                else
                    con.w((ConsoleColor)i, $"{i,2}{text}");
            con.wl();
        }

        static void WriteAnsiColorTables()
        {
            WriteAnsiForegroundTables();
            WriteAnsiBackgroundTables();
        }

        static void WriteAnsiBackgroundTables()
        {
            Console.WriteLine("Background 256");
            for (int j = 0; j < 32; j++)
            {
                for (int i = 0; i < 8; i++)
                {
                    var c = j * 8 + i;
                    Console.Write($"\u001b[48;5;{c}m {c,3}ab ");
                }
                Console.WriteLine("\u001b[0m");
            }
            Console.Write("\u001b[0m");
        }

        static void WriteAnsiForegroundTables()
        {
            const string text = "ab\u2588\u2588";
            Console.WriteLine("Foreground tables");
            Console.WriteLine("Foreground 16");
            for (int i = 0; i < 8; i++)
                Console.Write($"\u001b[3{i}m{i,2}{text}");
            for (int i = 0; i < 8; i++)
                Console.Write($"\u001b[3{i};1m{i,2}{text}");
            Console.WriteLine("\u001b[0m");
            Console.WriteLine("Foreground 256");
            for (int j = 0; j < 2; j++)
            {
                for (int i = 0; i < 8; i++)
                {
                    var c = j * 8 + i;
                    Console.Write($"\u001b[38;5;{c}m{c,3}\u2588\u2588");
                }
                Console.WriteLine();
            }
            Console.Write("\u001b[0m");
            for (int k = 0; k < 6; k++)
            {
                for (int j = 0; j < 6; j++)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        var c = 16 + ((k * 6 + j) * 6) + i;
                        Console.Write($"\u001b[38;5;{c}m{c,3}\u2588\u2588");
                    }
                    if ((j & 1) == 1)
                        Console.WriteLine();
                }
            }
            Console.Write("\u001b[0m");
            for (int j = 0; j < 3; j++)
            {
                for (int i = 0; i < 8; i++)
                {
                    var c = 232 + j * 8 + i;
                    Console.Write($"\u001b[38;5;{c}m{c,3}\u2588\u2588");
                }
                Console.WriteLine();
            }
            Console.WriteLine("\u001b[0m-- 256");
            for (int k = 0; k < 6; k++)
            {
                for (int j = 0; j < 6; j++)
                {
                    for (int i = 0; i < 6; i++)
                    {
                        var c = 16 + ((k * 6 + j) * 6) + i;
                        Console.Write($"\u001b[38;5;{c}m\u2588\u2588\u2588");
                    }
                    Console.Write("   ");
                    for (int i = 0; i < 6; i++)
                    {
                        var c = 16 + ((j * 6 + k) * 6) + i;
                        Console.Write($"\u001b[38;5;{c}m\u2588\u2588\u2588");
                    }
                    Console.Write("   ");
                    for (int i = 0; i < 6; i++)
                    {
                        var c = 16 + ((i * 6 + j) * 6) + k;
                        Console.Write($"\u001b[38;5;{c}m\u2588\u2588\u2588");
                    }
                    Console.WriteLine("\u001b[0m");
                }
            }
            Console.WriteLine("\u001b[0m-- Foreground tables");
        }

        [CmdLineMethod("color")]
        static public void DoColor()
        {
            WriteForegroundColors();
            WriteBackgroundColors();
            WriteAnsiColorTables();
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
