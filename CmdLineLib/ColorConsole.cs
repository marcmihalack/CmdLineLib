using System;
using System.Collections.Generic;

namespace CmdLineLib
{
    public class ColorConsole : IColorConsole
    {
        static public IColorConsole Default
        {
            get { return defaultConsole ?? (defaultConsole = new ColorConsole()); }
        }
        public ConsoleColor WarningFgColor = ConsoleColor.Yellow;
        public ConsoleColor ErrorFgColor = ConsoleColor.Red;
        // when using methods with fgColor and bgColor use TransparentColor to not change the fgColor
        public ConsoleColor TransparentColor = ConsoleColor.White;

        public IColorConsole w(object s)
        {
            lock (m_lock)
            {
                Console.Write(s);
                return this;
            }
        }

        public IColorConsole w(string s, params object[] args)
        {
            lock (m_lock)
            {
                Console.Write(s, args);
                return this;
            }
        }

        public IColorConsole wl()
        {
            lock (m_lock)
            {
                Console.WriteLine();
                return this;
            }
        }

        public IColorConsole wl(object s)
        {
            lock (m_lock)
            {
                Console.WriteLine(s);
                return this;
            }
        }

        public IColorConsole wl(string s, params object[] args)
        {
            lock (m_lock)
            {
                Console.WriteLine(s, args);
                return this;
            }
        }

        public IColorConsole w(ConsoleColor fgColor, object s)
        {
            lock (m_lock)
            {
                Console.ForegroundColor = fgColor;
                Console.Write(s);
                Console.ResetColor();
                return this;
            }
        }

        public IColorConsole w(ConsoleColor fgColor, string s, params object[] args)
        {
            lock (m_lock)
            {
                Console.ForegroundColor = fgColor;
                Console.Write(s, args);
                Console.ResetColor();
                return this;
            }
        }

        public IColorConsole wl(ConsoleColor fgColor, object s)
        {
            lock (m_lock)
            {
                Console.ForegroundColor = fgColor;
                Console.WriteLine(s);
                Console.ResetColor();
                return this;
            }
        }

        public IColorConsole wl(ConsoleColor fgColor, string s, params object[] args)
        {
            lock (m_lock)
            {
                Console.ForegroundColor = fgColor;
                Console.WriteLine(s, args);
                Console.ResetColor();
                return this;
            }
        }

        public IColorConsole w(ConsoleColor fgColor, ConsoleColor bgColor, object s)
        {
            lock (m_lock)
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
            lock (m_lock)
            {
                if (fgColor != TransparentColor)
                    Console.ForegroundColor = fgColor;
                Console.BackgroundColor = bgColor;
                Console.Write(s, args);
                Console.ResetColor();
                return this;
            }
        }

        public IColorConsole wl(ConsoleColor fgColor, ConsoleColor bgColor, object s)
        {
            lock (m_lock)
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
            lock (m_lock)
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
            lock (m_lock)
            {
                Console.ForegroundColor = fgColor;
                return this;
            }
        }

        public IColorConsole bclr(ConsoleColor bgclr)
        {
            lock (m_lock)
            {
                Console.BackgroundColor = bgclr;
                return this;
            }
        }

        public IColorConsole rclr()
        {
            lock (m_lock)
            {
                Console.ResetColor();
                return this;
            }
        }

        public void warn(string s)
        {
            lock (m_lock)
            {
                Console.ForegroundColor = WarningFgColor;
                Console.WriteLine(s);
                Console.ResetColor();
            }
        }

        public void warn(string s, params object[] args)
        {
            lock (m_lock)
            {
                Console.ForegroundColor = WarningFgColor;
                Console.WriteLine(s, args);
                Console.ResetColor();
            }
        }

        public void err(string s)
        {
            lock (m_lock)
            {
                Console.ForegroundColor = ErrorFgColor;
                Console.WriteLine(s);
                Console.ResetColor();
            }
        }

        public void err(string s, params object[] args)
        {
            lock (m_lock)
            {
                Console.ForegroundColor = ErrorFgColor;
                Console.WriteLine(s, args);
                Console.ResetColor();
            }
        }

        public IColorConsole flush()
        {
            lock (m_lock)
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

        static ColorConsole defaultConsole = null;
        static object m_lock = new object();
    }
}
