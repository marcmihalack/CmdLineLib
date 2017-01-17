using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CmdLineLibTestSuite
{
    [TestClass]
    public class CmdLineBasicTests
    {
        private StringWriter ConsoleOut;
        [TestInitialize]
        public void Initialize()
        {
            ConsoleOut = new StringWriter();;
            Console.SetOut(ConsoleOut);
        }

        [TestMethod]
        public void CmdLineBasicTest()
        {

        }
    }
}
