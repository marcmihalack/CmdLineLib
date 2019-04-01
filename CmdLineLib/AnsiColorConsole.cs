using System;
using System.Threading;

namespace CmdLineLib
{
    public sealed class AnsiColorConsole : IColorConsole
    {
        internal AnsiColorConsole(bool trySet)
        {
            SystemConsole.Initialize();
            //if (!SystemConsole.IsAnsi)
            //    throw new NotSupportedException("ANSI console is not supported");
        }

        public AnsiColorConsole()
            : this(true)
        {
        }

        public IColorConsole flush()
        {
            lock (SystemConsole.Lock)
                Console.Out.Flush();
            return this;
        }

        public IColorConsole w(ConsoleColor fgColor, ConsoleColor bgColor, object s)
        {
            var fc = (int)fgColor;
            string fg;
            if (fc == -1)
                fg = "";
            else
            {
                int c = ConsoleColorMap[((int)fgColor) & 7];
                if (fc >= 8)
                    fg = $"\u001b[3{c};1m";
                else
                    fg = $"\u001b[3{c}m";
            }
            int bc = ConsoleColorMap[(int)bgColor];
            lock (SystemConsole.Lock)
                Console.Write($"{fg}\u001b[48;5;{bc}m{s}\u001b[0m");
            return this;
        }

        public IColorConsole w(ConsoleColor fgColor, ConsoleColor bgColor, string s, params object[] args)
        {
            var fc = (int)fgColor;
            string fg;
            if (fc == -1)
                fg = "";
            else
            {
                int c = ConsoleColorMap[((int)fgColor) & 7];
                if (fc >= 8)
                    fg = $"\u001b[3{c};1m";
                else
                    fg = $"\u001b[3{c}m";
            }
            int bc = ConsoleColorMap[((int)bgColor)];
            lock (SystemConsole.Lock)
                Console.Write($"{fg}\u001b[48;5;{bc}m{string.Format(s, args)}\u001b[0m");
            return this;
        }

        public IColorConsole w(ConsoleColor fgColor, object s)
        {
            int c = ConsoleColorMap[((int)fgColor) & 7];
            var fg = ((int)fgColor) >= 8
                ? $"\u001b[3{c};1m"
                : $"\u001b[3{c}m";
            lock (SystemConsole.Lock)
                Console.Write($"{fg}{s}\u001b[0m");
            return this;
        }

        public IColorConsole w(ConsoleColor fgColor, string s, params object[] args)
        {
            int c = ConsoleColorMap[((int)fgColor) & 7];
            var fg = ((int)fgColor) >= 8
                ? $"\u001b[3{c};1m"
                : $"\u001b[3{c}m";
            lock (SystemConsole.Lock)
                Console.Write($"{fg}{string.Format(s, args)}\u001b[0m");
            return this;
        }

        public IColorConsole w(object s)
        {
            lock (SystemConsole.Lock)
                Console.Write(s);
            return this;
        }

        public IColorConsole w(string s, params object[] args)
        {
            lock (SystemConsole.Lock)
                Console.Write(s, args);
            return this;
        }

        public IColorConsole wl()
        {
            lock (SystemConsole.Lock)
                Console.WriteLine();
            return this;
        }

        public IColorConsole wl(ConsoleColor fgColor, ConsoleColor bgColor, object s)
        {
            var fc = (int)fgColor;
            string fg;
            if (fc == -1)
                fg = "";
            else
            {
                int c = ConsoleColorMap[((int)fgColor) & 7];
                if (fc >= 8)
                    fg = $"\u001b[3{c};1m";
                else
                    fg = $"\u001b[3{c}m";
            }
            int bc = ConsoleColorMap[((int)bgColor)];
            lock (SystemConsole.Lock)
                Console.WriteLine($"{fg}\u001b[48;5;{bc}m{s}\u001b[0m");
            return this;
        }

        public IColorConsole wl(ConsoleColor fgColor, ConsoleColor bgColor, string s, params object[] args)
        {
            var fc = (int)fgColor;
            string fg;
            if (fc == -1)
                fg = "";
            else
            {
                int c = ConsoleColorMap[((int)fgColor) & 7];
                if (fc >= 8)
                    fg = $"\u001b[3{c};1m";
                else
                    fg = $"\u001b[3{c}m";
            }
            int bc = ConsoleColorMap[((int)bgColor)];
            lock (SystemConsole.Lock)
                Console.WriteLine($"{fg}\u001b[48;5;{bc}m{string.Format(s, args)}\u001b[0m");
            return this;
        }

        public IColorConsole wl(ConsoleColor fgColor, object s)
        {
            int c = ConsoleColorMap[((int)fgColor) & 7];
            var fg = ((int)fgColor) >= 8
                ? $"\u001b[3{c};1m"
                : $"\u001b[3{c}m";
            lock (SystemConsole.Lock)
                Console.WriteLine($"{fg}{s}\u001b[0m");
            return this;
        }

        public IColorConsole wl(ConsoleColor fgColor, string s, params object[] args)
        {
            int c = ConsoleColorMap[((int)fgColor) & 7];
            var fg = ((int)fgColor) >= 8
                ? $"\u001b[3{c};1m"
                : $"\u001b[3{c}m";
            lock (SystemConsole.Lock)
                Console.WriteLine($"{fg}{string.Format(s, args)}\u001b[0m");
            return this;
        }

        public IColorConsole wl(object s)
        {
            lock (SystemConsole.Lock)
                Console.WriteLine(s);
            return this;
        }

        public IColorConsole wl(string s, params object[] args)
        {
            lock (SystemConsole.Lock)
                Console.WriteLine(s, args);
            return this;
        }

        static readonly int[] ConsoleColorMap = new[] { 0, 4, 2, 6, 1, 5, 3, 7, 8, 12, 10, 14, 9, 13, 11, 15 };
        const string Reset = "\u001b[0m";
    }
}
