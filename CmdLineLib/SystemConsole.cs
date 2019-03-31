using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace CmdLineLib
{
    public class SystemConsole
    {
        static public IColorConsole CreateColorConsole()
        {
            if (TrySetAnsiConsole())
                return new AnsiColorConsole(false);
            return new ColorConsole();
        }

        static internal object Lock = new object();

        const int STD_OUTPUT_HANDLE = -11;
        const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
        const uint DISABLE_NEWLINE_AUTO_RETURN = 0x0008;

        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        static internal bool TrySetAnsiConsole()
        {
            var iStdOut = GetStdHandle(STD_OUTPUT_HANDLE);
            if (!GetConsoleMode(iStdOut, out uint outConsoleMode))
                return false;
            outConsoleMode |= ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN;
            return SetConsoleMode(iStdOut, outConsoleMode);
        }

        static bool? _HasConsoleWindow;
        static internal bool HasConsoleWindow
        {
            get
            {
                if (_HasConsoleWindow == null)
                {
                    int height = 0;
                    try
                    {
                        height = Console.WindowHeight;
                        _HasConsoleWindow = false;
                    }
                    catch (System.IO.IOException)
                    {
                        _HasConsoleWindow = true;
                    }
                }
                return _HasConsoleWindow.Value;
            }
        }
    }
}
