using CmdLineLib.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CmdLineLib.Tests.Classes
{
    [CmdLineClass(HelpText:"Test class 1")]
    public class TestClassStatic
    {
        public static string MethodInvoked { get; private set; }
        public static void Reset()
        {
            MethodInvoked = null;
        }

        [CmdLineMethod("test1")]
        public static void TestMethod1([CmdLineArg("arg")]string arg1)
        {
            Assert.AreEqual(typeof(string), arg1.GetType());
        }

        [CmdLineMethod("test2")]
        public static void TestMethod2(
            [CmdLineArg("count")]int count,
            [CmdLineArg("list")]string[] list)
        {
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
            Assert.AreEqual(typeof(int), count.GetType());
            Assert.IsTrue(count >= 0);
            Assert.AreEqual(typeof(int[]), list.GetType());
            Assert.AreEqual(1, list.GetType().GetArrayRank());
            Assert.AreEqual(count, list.Length);
        }

        [CmdLineMethod("staticmethod")]
        public static void StaticMethod()
        {
            MethodInvoked = "staticmethod";
        }

        [CmdLineMethod("nonstaticmethod")]
        public void DynamicMethod()
        {
            Assert.Fail("Shouldn't be called");
        }

        [CmdLineMethod("nonpublicmethod")]
        protected static void NonpublicMethod()
        {
            Assert.Fail("Shouldn't be called");
        }
    }
}
