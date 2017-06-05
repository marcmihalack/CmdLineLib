using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CmdLineLib
{
    internal enum Color
    {
        /*
           0-7 - color: 0 BLACK, 1 RED, 2 GREEN, 3 YELLOW, 4 BLUE, 5 MAGENTA, 6 CYAN, 7 GRAY
           8-9 - shade: 0 = normal - -m, 1 = LIGHT - 1m, 2 = DARK - 1m2m, 3 = DARKEST - 2m
        */
        Black = 0x00, // also 0x30 - DARKEST|BLACK
        LightBlack = 0x20, // DARK|BLACK
        DarkestGray = 0x10, // LIGHT|BLACK

        DarkestRed = 0x31,
        DarkRed = 0x21,
        Red = 0x01,
        LightRed = 0x11,

        DarkestGreen = 0x32,
        DarkGreen = 0x22,
        Green = 0x02,
        LightGreen = 0x12,

        DarkestYellow = 0x33,
        DarkYellow = 0x23,
        Yellow = 0x03,
        LightYellow = 0x13,

        DarkestBlue = 0x04,
        DarkBlue = 0x04,
        Blue = 0x04,
        LightBlue = 0x04,

        DarkestMagenta = 0x35,
        DarkMagenta = 0x25,
        Magenta = 0x05,
        LightMagenta = 0x15,

        DarkestCyan = 0x36,
        DarkCyan = 0x26,
        Cyan = 0x06,
        LightCyan = 0x16,

        DarkGray = 0x37, // DARKEST|GRAY
        Gray = 0x27, // DARK|GRAY
        LightGray = 0x07, // GRAY
        White = 0x17 // LIGHT|GRAY
    }

    internal class TTYColorConsole : ColorConsole
    {
        public TTYColorConsole()
            : base(true)
        {
        }

        int GetColor(ConsoleColor color)
        {
            /*
            For Windows Console:
            Black => Black
            LightBlack,DarkestGray,DarkGray => DarkGray
            Gray,LightGray => Gray
            White => White
            Darkest*,Dark* => Dark*
            Normal*,Light* => Normal*
            */
            if (color == ConsoleColor.DarkGray)
                return (int)Color.Gray;
            else if (color == ConsoleColor.White)
                return (int)Color.White;
            int c = (int)color;
            return (c & 7) | (((c & 0x8) ^ 0x8) << 2);
        }

        override
        protected internal void SetForegroundColor(ConsoleColor color)
        {
            int c = GetColor(color);
            Console.Write("{0}{1}\x1B[3{2}m", (c & 0x10) == 0x10 ? "\x1B[1m" : "", (c & 0x20) == 0x20 ? "\x1B[2m" : "", c & 0x7);
            //Console.Write("{0}\x1B[3{1}m", isDark ? "\x1B[1m\x1B[2m" : "\x1B[1m", c1);
        }

        override
        protected internal void SetBackgroundColor(ConsoleColor color)
        {
            int c = GetColor(color);
            Console.Write("{0}{1}\x1B[3{2}m", (c & 0x10) == 0x10 ? "\x1B[1m" : "", (c & 0x20) == 0x20 ? "\x1B[2m" : "", c & 0x7);
        }

        override
        protected internal void ResetColor()
        {
            Console.Write("\x1B[0m");
        }

        internal void Rainbow()
        {
            IColorConsole con = Default;
            string text = " 123456890ABCDEFGHIJKLMNOPQR ";
            if (con.isTTY)
            {
                for (int i = 0; i < 8; i++)
                {
                    Console.Write("{1} \x1B[1m\x1B[3{1}m{0}\x1B[0m", text, i);
                    Console.Write("\x1B[3{1}m{0}\x1B[0m", text, i);
                    Console.Write("\x1B[1m\x1B[2m\x1B[3{1}m{0}\x1B[0m", text, i);
                    Console.Write("\x1B[2m\x1B[3{1}m{0}\x1B[0m", text, i);
                    Console.WriteLine();
                }
                for (int i = 0; i < 8; i++)
                {
                    Console.Write("{1} \x1B[1m\x1B[4{1}m{0}\x1B[0m", text, i);
                    Console.Write("\x1B[4{1}m{0}\x1B[0m", text, i);
                    Console.Write("\x1B[1m\x1B[2m\x1B[4{1}m{0}\x1B[0m", text, i);
                    Console.Write("\x1B[2m\x1B[4{1}m{0}\x1B[0m", text, i);
                    Console.WriteLine();
                }
            }
            else
            {
                for (int i = 0; i < 8; i++)
                {
                    Console.ForegroundColor = (ConsoleColor)i;
                    Console.Write("{1} {0}", text, i);
                    Console.ForegroundColor = (ConsoleColor)(i + 8);
                    Console.Write("{1} {0}", text, i);
                    Console.WriteLine();
                    Console.ResetColor();
                }
                for (int i = 0; i < 8; i++)
                {
                    Console.BackgroundColor = (ConsoleColor)i;
                    Console.Write("{1} {0}", text, i);
                    Console.BackgroundColor = (ConsoleColor)(i + 8);
                    Console.Write("{1} {0}", text, i);
                    Console.WriteLine();
                    Console.ResetColor();
                }
            }
            for (int i = 0; i < 8; i++)
            {
                con.w((ConsoleColor)i, "{1} {0}", text, i);
                con.w((ConsoleColor)(i + 8), "{1} {0}", text, (i + 8));
                Console.WriteLine();
                Console.ResetColor();
            }

        }
    }
}
