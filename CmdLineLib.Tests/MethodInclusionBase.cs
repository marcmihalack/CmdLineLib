using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CmdLineLib.Tests
{
    public class MethodInclusionBase
    {
        protected static bool WasNonStaticMethodCalled { get; set; }
        protected static bool WasStaticMethodCalled { get; set; }

        protected void Test_Call<T>(string arg, bool expectedNonStatic, bool expectedStatic) where T : class
        {
            var args = new[] { arg };
            CmdLine<T>.Execute(args);
            Assert.AreEqual(expectedNonStatic, WasNonStaticMethodCalled);
            Assert.AreEqual(expectedStatic, WasStaticMethodCalled);
        }

        protected void Test_NonStaticMethod_NonStaticCall<T>() where T : class
        {
            var args = new[] { "mymethod" };
            CmdLine<T>.Execute(args);
            Assert.IsTrue(WasNonStaticMethodCalled);
            Assert.IsFalse(WasStaticMethodCalled);
        }

        protected void Test_NonStaticMethod_StaticCall<T>() where T : class
        {
            var args = new[] { "mystaticmethod" };
            CmdLine<T>.Execute(args);
            Assert.IsFalse(WasNonStaticMethodCalled);
            Assert.IsFalse(WasStaticMethodCalled);
        }

        protected void Test_StaticMethod_NonStaticCall<T>() where T : class
        {
            var args = new[] { "mymethod" };
            CmdLine<T>.Execute(args);
            Assert.IsFalse(WasNonStaticMethodCalled);
            Assert.IsFalse(WasStaticMethodCalled);
        }

        protected void Test_StaticMethod_StaticCall<T>() where T : class
        {
            var args = new[] { "mystaticmethod" };
            CmdLine<T>.Execute(args);
            Assert.IsTrue(WasStaticMethodCalled);
            Assert.IsFalse(WasNonStaticMethodCalled);
        }

        protected void Test_AllMethods_NonStaticCall<T>() where T : class
        {
            var args = new[] { "mymethod" };
            CmdLine<T>.Execute(args);
            Assert.IsTrue(WasNonStaticMethodCalled);
            Assert.IsFalse(WasStaticMethodCalled);
        }

        protected void Test_AllMethods_StaticCall<T>() where T : class
        {
            var args = new[] { "mystaticmethod" };
            CmdLine<T>.Execute(args);
            Assert.IsTrue(WasStaticMethodCalled);
            Assert.IsFalse(WasNonStaticMethodCalled);
        }
    }
}
