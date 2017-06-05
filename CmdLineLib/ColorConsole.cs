using System;
using System.Collections.Generic;

namespace CmdLineLib
{
    internal class ColorConsoleFactory
    {
        static public IColorConsole CreateConsole()
        {
            return IsTTY ? new TTYColorConsole() : new ColorConsole();
        }

        static bool IsTTY
        {
            get
            {
                int height;
                try
                {
                    height = Console.WindowHeight;
                }
                catch
                {
                    return true;
                }
                return false;
            }
        }
    }

    public class ColorConsole : IColorConsole
    {
        static public IColorConsole Default
        {
            get { return defaultConsole ?? (defaultConsole = ColorConsoleFactory.CreateConsole()); }
        }

        public ColorConsole()
            : this(false)
        {
        }
        internal ColorConsole(bool isTTY)
        {
            this.isTTY = isTTY;
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
                SetForegroundColor(fgColor);
                Console.Write(s);
                ResetColor();
                return this;
            }
        }

        public IColorConsole w(ConsoleColor fgColor, string s, params object[] args)
        {
            lock (m_lock)
            {
                SetForegroundColor(fgColor);
                Console.Write(s, args);
                ResetColor();
                return this;
            }
        }

        public IColorConsole wl(ConsoleColor fgColor, object s)
        {
            lock (m_lock)
            {
                SetForegroundColor(fgColor);
                Console.WriteLine(s);
                ResetColor();
                return this;
            }
        }

        public IColorConsole wl(ConsoleColor fgColor, string s, params object[] args)
        {
            lock (m_lock)
            {
                SetForegroundColor(fgColor);
                Console.WriteLine(s, args);
                ResetColor();
                return this;
            }
        }

        public IColorConsole w(ConsoleColor fgColor, ConsoleColor bgColor, object s)
        {
            lock (m_lock)
            {
                if (fgColor != TransparentColor)
                    SetForegroundColor(fgColor);
                SetBackgroundColor(bgColor);
                Console.Write(s);
                ResetColor();
                return this;
            }
        }

        public IColorConsole w(ConsoleColor fgColor, ConsoleColor bgColor, string s, params object[] args)
        {
            lock (m_lock)
            {
                if (fgColor != TransparentColor)
                    SetForegroundColor(fgColor);
                SetBackgroundColor(bgColor);
                Console.Write(s, args);
                ResetColor();
                return this;
            }
        }

        public IColorConsole wl(ConsoleColor fgColor, ConsoleColor bgColor, object s)
        {
            lock (m_lock)
            {
                if (fgColor != TransparentColor)
                    SetForegroundColor(fgColor);
                SetBackgroundColor(bgColor);
                Console.WriteLine(s);
                ResetColor();
                return this;
            }
        }

        public IColorConsole wl(ConsoleColor fgColor, ConsoleColor bgColor, string s, params object[] args)
        {
            lock (m_lock)
            {
                if (fgColor != TransparentColor)
                    SetForegroundColor(fgColor);
                SetBackgroundColor(bgColor);
                Console.WriteLine(s, args);
                ResetColor();
                return this;
            }
        }

        public IColorConsole fclr(ConsoleColor fgColor)
        {
            lock (m_lock)
            {
                SetForegroundColor(fgColor);
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
                ResetColor();
                return this;
            }
        }

        public void warn(string s)
        {
            lock (m_lock)
            {
                Console.ForegroundColor = WarningFgColor;
                Console.WriteLine(s);
                ResetColor();
            }
        }

        public void warn(string s, params object[] args)
        {
            lock (m_lock)
            {
                Console.ForegroundColor = WarningFgColor;
                Console.WriteLine(s, args);
                ResetColor();
            }
        }

        public void err(string s)
        {
            lock (m_lock)
            {
                Console.ForegroundColor = ErrorFgColor;
                Console.WriteLine(s);
                ResetColor();
            }
        }

        public void err(string s, params object[] args)
        {
            lock (m_lock)
            {
                Console.ForegroundColor = ErrorFgColor;
                Console.WriteLine(s, args);
                ResetColor();
            }
        }

        public IColorConsole flush()
        {
            lock (m_lock)
                Console.Out.Flush();
            return this;
        }

        public bool isTTY { get; internal protected set; }

        virtual protected internal void SetForegroundColor(ConsoleColor color)
        {
            Console.ForegroundColor = color;
        }

        virtual protected internal void SetBackgroundColor(ConsoleColor color)
        {
            Console.BackgroundColor = color;
        }

        virtual protected internal void ResetColor()
        {
            Console.ResetColor();
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
        static object m_lock = new object();
    }
}
