using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using CmdLineLib.Attributes;
using System;

namespace CmdLineLib.Tests
{
    [TestClass]
    public class ConsoleTests
    {
        [TestMethod]
        public void WHEN_UsingColorConsole_THEN_()
        {
            IColorConsole con = new ColorConsole();
            con.wl("stuff");
            con.wl(ConsoleColor.Red, "red");
            //con.w("State:").wl(SystemConsole.State);
        }

        [TestMethod]
        public void WHEN_UsingAnsiColorConsole_THEN_()
        {
            IColorConsole con = new AnsiColorConsole();
            con.wl("stuff");
            con.wl(ConsoleColor.Red, "red");
            //con.w("State:").wl(SystemConsole.State);
        }
    }
}
