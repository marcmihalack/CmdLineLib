using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace CmdLineLib
{
    public class SystemConsole
    {
        static public IColorConsole CreateColorConsole()
        {
            Initialize();
            if (_IsAnsi)
                return new AnsiColorConsole(false);
            return new ColorConsole();
        }

        static internal object Lock = new object();

        const int STD_INPUT_HANDLE = -10;
        const int STD_OUTPUT_HANDLE = -11;
        const int STD_ERROR_HANDLE = -12;
        const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
        const uint DISABLE_NEWLINE_AUTO_RETURN = 0x0008;

        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int nStdHandle);

        /*
        What I know so far:
        - cmd prompt
            - redirect:
                1. iStdOut and iStdIn are != 0
                2. GetConsoleMode(out) fails
                2. GetConsoleMode(in) succeeds (if not redirected)
                3. HasConsoleWindow == true
            - no-redirect:
                1. iStdOut and iStdIn are != 0
                2. GetConsoleMode(out) succeeds
                2. GetConsoleMode(in) succeeds
                3. HasConsoleWindow == true
            - windowless:
                1. iStdOut and iStdIn are == 0
                2. GetConsoleMode(out) fails
                2. GetConsoleMode(in) fails
                3. HasConsoleWindow == false
        - bash:
            - redirect:
                1. iStdOut and iStdIn are != 0
                2. GetConsoleMode(*) fails
                3. HasConsoleWindow == false
            - no-redirect:
                1. iStdOut and iStdIn are != 0
                2. GetConsoleMode(*) fails
                3. HasConsoleWindow == false
            - windowless:
        2. 
        */
        static bool _IsSet = false;
        static bool _LogStuff = true;
        static internal void Initialize()
        {
            if (_IsSet)
                return;

            State = "";
            var stdOut = GetStdHandle(STD_OUTPUT_HANDLE);
            if (_LogStuff)
            {
                var stdIn = GetStdHandle(STD_INPUT_HANDLE);
                var stdErr = GetStdHandle(STD_ERROR_HANDLE);

                if (0 == (int)stdIn)
                    State += $"-I;";
                else
                    State += $"I{stdIn};";
                if (0 == (int)stdOut)
                    State += $"-O;";
                else
                    State += $"O{stdIn};";
                if (0 == (int)stdErr)
                    State += $"-E;";
                else
                    State += $"E{stdErr};";
                if (!GetConsoleMode(stdIn, out uint inMode))
                    State += "IM:-;";
                else
                    State += string.Format("IM:{0};", inMode);
                if (!GetConsoleMode(stdOut, out uint outMode))
                    State += "OM:-;";
                else
                    State += string.Format("OM:{0};", outMode);
                if (!GetConsoleMode(stdErr, out uint errMode))
                    State += "EM:-;";
                else
                    State += string.Format("EM:{0};", errMode);
                if (SystemConsole.HasConsoleWindow)
                    State += "WND;";
            }

            if (0 == (int)stdOut)
            {
                // cmd prompt + windowless
                _IsAnsi = false;
                _IsShell = false;
                return;
            }
            else if (!GetConsoleMode(stdOut, out uint outCurrentMode))
            {
                if (HasConsoleWindow)
                {
                    // cmd prompt + redirect
                    _IsAnsi = false;
                    _IsShell = false;
                }
                else
                {
                    // bash + any (no way to tell if it is redirected or not)
                    _IsAnsi = true;
                    _IsShell = true;
                }
            }
            else
            {
                _IsShell = false;
                var outNewMode = outCurrentMode | ENABLE_VIRTUAL_TERMINAL_PROCESSING | DISABLE_NEWLINE_AUTO_RETURN;
                if (SetConsoleMode(stdOut, outNewMode))
                {
                    // cmd prompt + no-redirect
                    _IsAnsi = true;
                }
                else
                {
                    // cmd prompt + no ansi support
                    _IsAnsi = false;
                }
            }
            if (_LogStuff)
            {
                if (_IsAnsi)
                    State += "A;";
                if (_IsShell)
                    State += "SH;";
            }
            _IsSet = true;
        }

        /*
        bool ok = DeviceIoControl(hdl, CTLCODE, ref drawer, 1, IntPtr.Zero, 0, IntPtr.Zero, IntPtr.Zero);
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool DeviceIoControl(IntPtr device, uint ctlcode,
            ref byte inbuffer, int inbuffersize,
            IntPtr outbuffer, int outbufferSize,
            IntPtr bytesreturned, IntPtr overlapped);
        //((DeviceType) << 16) | ((Access) << 14) | ((Function) << 2) | (Method)
        //#define IOCPARM_MASK    0x7f            
        // #define IOC_OUT         0x40000000     
        // #define _IOR(x,y,t)     (IOC_OUT|(((long)sizeof(t)&IOCPARM_MASK)<<16)|((x)<<8)|(y))
        struct winsize
        {
            ushort ws_row;
            ushort ws_col;
            ushort ws_xpixel;
            ushort ws_ypixel;
        };
        // _IOR(t, 104, struct winsize)
        struct ttysize
        {
            int ts_lines;
            int ts_cols;
        };
        // _IOR(t,38,struct ttysize)
        */

        static internal string State { get; private set; }
        static bool _IsAnsi;
        static public bool IsAnsi
        {
            get
            {
                Initialize();
                return _IsAnsi;
            }
        }
        static bool _IsShell;
        static public bool IsShell
        {
            get
            {
                Initialize();
                return _IsShell;
            }
        }

        static bool? _HasConsoleWindow;
        static public bool HasConsoleWindow
        {
            get
            {
                if (_HasConsoleWindow == null)
                {
                    int height = 0;
                    try
                    {
                        height = Console.WindowHeight;
                        _HasConsoleWindow = true;
                    }
                    catch (System.IO.IOException)
                    {
                        _HasConsoleWindow = false;
                    }
                }
                return _HasConsoleWindow.Value;
            }
        }
    }
}
