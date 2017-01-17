using Microsoft.VisualStudio.TestTools.UnitTesting;
using CmdLineLib.Tests.Classes;
using FluentAssertions;

namespace CmdLineLib.Tests
{
    [TestClass]
    public class CmdLineTests
    {
        [TestMethod]
        public void WHEN_ClassHasStaticMethod_THEN_MethodIsExecuted()
        {
            var args = new[] { "test1", "/arg=str" };
            CmdLine<TestClassStatic>.Execute(args);
        }

        [TestMethod]
        public void WHEN_ArgIsStringArray_THEN_MethodIsExecuted()
        {
            var args = new[] { "test2", "/count=1", "/list=s1" };
            CmdLine<TestClassStatic>.Execute(args);
            args = new[] { "test2", "/count=2", "/list=s1,s2" };
            CmdLine<TestClassStatic>.Execute(args);
        }

        [TestMethod]
        public void WHEN_ArgIsValueTypeArray_THEN_MethodIsExecuted()
        {
            var args = new[] { "test3", "/count=1", "/list=1" };
            CmdLine<TestClassStatic>.Execute(args);
            args = new[] { "test3", "/count=2", "/list=1,2" };
            CmdLine<TestClassStatic>.Execute(args);
        }

        [TestMethod]
        public void WHEN_UsingCustomArgSeparator_THEN_MethodIsExecuted()
        {
            var config = new CmdLineConfig { ArgSeparator = ':' };
            var args = new[] { "test3", "/count:1", "/list:1" };
            CmdLine<TestClassStatic>.Execute(args, config);
            args = new[] { "test3", "/count:2", "/list:1,2" };
            CmdLine<TestClassStatic>.Execute(args, config);
        }

        [TestMethod]
        public void WHEN_UsingCustomArgSeparator_AND_ArgIsInvalid_THEN_ExceptionIsThrown()
        {
            var config = new CmdLineConfig { ArgSeparator = ':' };
            var args = new[] { "test3", "/count=2", "/list=1,2" };
            args.Invoking(a => CmdLine<TestClassStatic>.Execute(a, config)).ShouldThrow<CmdLineException>();
        }
    }
}
