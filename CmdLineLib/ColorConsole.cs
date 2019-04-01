using System;
using System.Collections.Generic;
#pragma warning disable IDE1006 // Naming Styles

namespace CmdLineLib
{
    public class ColorConsole : IColorConsole
    {
        static public IColorConsole Default
        {
            get { return defaultConsole ?? (defaultConsole = SystemConsole.CreateColorConsole()); }
        }

        public ConsoleColor WarningFgColor = ConsoleColor.Yellow;
        public ConsoleColor ErrorFgColor = ConsoleColor.Red;
        // when using methods with fgColor and bgColor use TransparentColor to not change the fgColor
        public ConsoleColor TransparentColor = (ConsoleColor)(-1);

        public IColorConsole w(object s)
        {
            lock (SystemConsole.Lock)
            {
                Console.Write(s);
                return this;
            }
        }

        public IColorConsole w(string s, params object[] args)
        {
            lock (SystemConsole.Lock)
            {
                Console.Write(s, args);
                return this;
            }
        }

        public IColorConsole wl()
        {
            lock (SystemConsole.Lock)
            {
                Console.WriteLine();
                return this;
            }
        }

        public IColorConsole wl(object s)
        {
            lock (SystemConsole.Lock)
            {
                Console.WriteLine(s);
                return this;
            }
        }

        public IColorConsole wl(string s, params object[] args)
        {
            lock (SystemConsole.Lock)
            {
                Console.WriteLine(s, args);
                return this;
            }
        }

        public IColorConsole w(ConsoleColor fgColor, object s)
        {
            lock (SystemConsole.Lock)
            {
                Console.ForegroundColor = fgColor;
                Console.Write(s);
                Console.ResetColor();
                return this;
            }
        }

        public IColorConsole w(ConsoleColor fgColor, string s, params object[] args)
        {
            lock (SystemConsole.Lock)
            {
                Console.ForegroundColor = fgColor;
                Console.Write(s, args);
                Console.ResetColor();
                return this;
            }
        }

        public IColorConsole wl(ConsoleColor fgColor, object s)
        {
            lock (SystemConsole.Lock)
            {
                Console.ForegroundColor = fgColor;
                Console.WriteLine(s);
                Console.ResetColor();
                return this;
            }
        }

        public IColorConsole wl(ConsoleColor fgColor, string s, params object[] args)
        {
            lock (SystemConsole.Lock)
            {
                Console.ForegroundColor = fgColor;
                Console.WriteLine(s, args);
                Console.ResetColor();
                return this;
            }
        }

        public IColorConsole w(ConsoleColor fgColor, ConsoleColor bgColor, object s)
        {
            lock (SystemConsole.Lock)
            {
                if (fgColor != TransparentColor)
                    Console.ForegroundColor = fgColor;
                Console.BackgroundColor = bgColor;
                Console.Write(s);
                Console.ResetColor();
                return this;
            }
        }

        public IColorConsole w(ConsoleColor fgColor, ConsoleColor bgColor, string s, params object[] args)
        {
            bool isTransparent = fgColor == TransparentColor || -1 == (int)fgColor;
            lock (SystemConsole.Lock)
            {
                if (!isTransparent)
                    Console.ForegroundColor = fgColor;
                Console.BackgroundColor = bgColor;
                Console.Write(s, args);
                Console.ResetColor();
                return this;
            }
        }

        public IColorConsole wl(ConsoleColor fgColor, ConsoleColor bgColor, object s)
        {
            lock (SystemConsole.Lock)
            {
                if (fgColor != TransparentColor)
                    Console.ForegroundColor = fgColor;
                Console.BackgroundColor = bgColor;
                Console.WriteLine(s);
                Console.ResetColor();
                return this;
            }
        }

        public IColorConsole wl(ConsoleColor fgColor, ConsoleColor bgColor, string s, params object[] args)
        {
            lock (SystemConsole.Lock)
            {
                if (fgColor != TransparentColor)
                    Console.ForegroundColor = fgColor;
                Console.BackgroundColor = bgColor;
                Console.WriteLine(s, args);
                Console.ResetColor();
                return this;
            }
        }

        public IColorConsole fclr(ConsoleColor fgColor)
        {
            lock (SystemConsole.Lock)
            {
                Console.ForegroundColor = fgColor;
                return this;
            }
        }

        public IColorConsole bclr(ConsoleColor bgclr)
        {
            lock (SystemConsole.Lock)
            {
                Console.BackgroundColor = bgclr;
                return this;
            }
        }

        public IColorConsole rclr()
        {
            lock (SystemConsole.Lock)
            {
                Console.ResetColor();
                return this;
            }
        }

        public void warn(string s)
        {
            lock (SystemConsole.Lock)
            {
                Console.ForegroundColor = WarningFgColor;
                Console.WriteLine(s);
                Console.ResetColor();
            }
        }

        public void warn(string s, params object[] args)
        {
            lock (SystemConsole.Lock)
            {
                Console.ForegroundColor = WarningFgColor;
                Console.WriteLine(s, args);
                Console.ResetColor();
            }
        }

        public void err(string s)
        {
            lock (SystemConsole.Lock)
            {
                Console.ForegroundColor = ErrorFgColor;
                Console.WriteLine(s);
                Console.ResetColor();
            }
        }

        public void err(string s, params object[] args)
        {
            lock (SystemConsole.Lock)
            {
                Console.ForegroundColor = ErrorFgColor;
                Console.WriteLine(s, args);
                Console.ResetColor();
            }
        }

        public IColorConsole flush()
        {
            lock (SystemConsole.Lock)
                Console.Out.Flush();
            return this;
        }

        // -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

        public void dump(bool wide = true)
        {
            int length = 6;
            var e = Enum.GetValues(typeof(ConsoleColor));
            int count = e.Length;
            string s;
            Dictionary<ConsoleColor, string> abbrs = new Dictionary<ConsoleColor, string>();
            foreach (ConsoleColor c in Enum.GetValues(typeof(ConsoleColor)))
            {
                s = c.ToString().Replace("Dark", "d").Replace("Black", "Blk").Replace("Yellow", "Ylw").Replace("Cyan", "Cyn").Replace("Blue", "Blu");
                s = s.Replace("Green", "Grn").Replace("White", "Wht").Replace("Magenta", "Mgt").Replace("Gray", "Gry");
                abbrs.Add(c, s);
            }
            s = new string(' ', length * count);
            string ss = new string(' ', length);
            foreach (ConsoleColor bgc in e)
            {
                if (wide)
                    rclr().w(ss).wl(TransparentColor, bgc, s);

                rclr().w(abbrs[bgc].PadRight(length, ' '));
                bclr(bgc);
                foreach (ConsoleColor fgc in e)
                    w(fgc, bgc, abbrs[fgc].PadRight(length, ' '));
                wl();

                if (wide)
                    rclr().w(ss).wl(TransparentColor, bgc, s);
            }
            rclr();
        }

        static IColorConsole defaultConsole = null;
    }
}
#pragma warning restore IDE1006 // Naming Styles
