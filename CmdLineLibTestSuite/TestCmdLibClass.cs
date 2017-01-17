using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CmdLineLib;

namespace CmdLineLibTestSuite
{
    [CmdLineClass("Help on the app")]
    class TestCmdLibClass
    {
        [CmdLineMethod("test", "Test help info")]
        public void Test(
            [CmdLineNamedArg("first", typeof (string), ArgFlags.Required | ArgFlags.RequiresValue)] string arg1)
        {
        }
    }
}
