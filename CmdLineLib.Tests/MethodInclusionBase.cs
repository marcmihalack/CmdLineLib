using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CmdLineLib.Tests
{
    public class MethodInclusionBase
    {
        protected static bool WasNonStaticMethodCalled { get; set; }
        protected static bool WasStaticMethodCalled { get; set; }

        protected void Test_Call<T>(string arg, bool expectedNonStatic, bool expectedStatic) where T : class
        {
            Test_Call<T>(new string[] { arg }, expectedNonStatic, expectedStatic);
        }

        protected void Test_Call<T>(string[] args, bool expectedNonStatic, bool expectedStatic) where T : class
        {
            CmdLine<T>.Execute(args);
            Assert.AreEqual(expectedNonStatic, WasNonStaticMethodCalled, expectedNonStatic ? "Expected non-static method to be called but NO call was made" : "Expected non-static method NOT to be called but the call was made");
            Assert.AreEqual(expectedStatic, WasStaticMethodCalled, expectedStatic ? "Expected static method to be called but NO call was made" : "Expected static method NOT to be called but the call was made");
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
