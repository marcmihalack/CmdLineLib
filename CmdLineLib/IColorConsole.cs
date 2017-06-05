using System;
namespace CmdLineLib
{
    public interface IColorConsole
    {
        IColorConsole w(ConsoleColor fgColor, ConsoleColor bgColor, object s);
        IColorConsole w(ConsoleColor fgColor, ConsoleColor bgColor, string s, params object[] args);
        IColorConsole w(ConsoleColor fgColor, object s);
        IColorConsole w(ConsoleColor fgColor, string s, params object[] args);
        IColorConsole w(object s);
        IColorConsole w(string s, params object[] args);
        IColorConsole wl();
        IColorConsole wl(ConsoleColor fgColor, ConsoleColor bgColor, object s);
        IColorConsole wl(ConsoleColor fgColor, ConsoleColor bgColor, string s, params object[] args);
        IColorConsole wl(ConsoleColor fgColor, object s);
        IColorConsole wl(ConsoleColor fgColor, string s, params object[] args);
        IColorConsole wl(object s);
        IColorConsole wl(string s, params object[] args);
        IColorConsole flush();
        bool isTTY { get; }
    }
}
