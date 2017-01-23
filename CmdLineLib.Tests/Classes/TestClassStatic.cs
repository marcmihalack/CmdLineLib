using CmdLineLib.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CmdLineLib.Tests.Classes
{
    [CmdLineClass(helpText:"Test class 1")]
    public sealed class TestClassStatic
    {
        public static string GetMethodInvokedAndReset() { var method = methodInvoked; methodInvoked = null; return method; }
        public static void Reset() { methodInvoked = null; }
        static string methodInvoked = null;

        [CmdLineMethod("test1")]
        public static void TestMethod1([CmdLineArg("arg")]string arg1)
        {
            methodInvoked = "test1";
            Assert.AreEqual(typeof(string), arg1.GetType());
        }

        [CmdLineMethod("test2")]
        public static void TestMethod2(
            [CmdLineArg("count")]int count,
            [CmdLineArg("list")]string[] list)
        {
            methodInvoked = "test2";
            Assert.AreEqual(typeof(int), count.GetType());
            Assert.IsTrue(count >= 0);
            Assert.AreEqual(typeof(string[]), list.GetType());
            Assert.AreEqual(1, list.GetType().GetArrayRank());
            Assert.AreEqual(count, list.Length);
        }

        [CmdLineMethod("test3")]
        public static void TestMethod3(
            [CmdLineArg("count")]int count,
            [CmdLineArg("list")]int[] list)
        {
            methodInvoked = "test3";
            Assert.AreEqual(typeof(int), count.GetType());
            Assert.IsTrue(count >= 0);
            Assert.AreEqual(typeof(int[]), list.GetType());
            Assert.AreEqual(1, list.GetType().GetArrayRank());
            Assert.AreEqual(count, list.Length);
        }

        [CmdLineMethod("staticmethod")]
        public static void StaticMethod()
        {
            methodInvoked = "staticmethod";
        }

        [CmdLineMethod("nonpublicmethod")]
        private static void NonpublicMethod()
        {
            Assert.Fail("Shouldn't be called");
        }
    }
}
