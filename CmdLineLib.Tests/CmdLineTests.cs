using Microsoft.VisualStudio.TestTools.UnitTesting;
using CmdLineLib.Tests.Classes;
using FluentAssertions;

namespace CmdLineLib.Tests
{
    [TestClass]
    public class CmdLineTests
    {
        [TestInitialize]
        public void InitializeTest()
        {
            TestClassStatic.Reset();
        }

        [TestMethod]
        public void WHEN_ClassHasStaticMethod_THEN_MethodIsExecuted()
        {
            var args = new[] { "test1", "/arg=str" };
            CmdLine<TestClassStatic>.Execute(args).Should().Be(0);
            TestClassStatic.GetMethodInvokedAndReset().Should().Be("test1");
        }

        [TestMethod]
        public void WHEN_ArgIsStringArray_THEN_MethodIsExecuted()
        {
            var args = new[] { "test2", "/count=1", "/list=s1" };
            CmdLine<TestClassStatic>.Execute(args).Should().Be(0);
            TestClassStatic.GetMethodInvokedAndReset().Should().Be("test2");
            args = new[] { "test2", "/count=2", "/list=s1,s2" };
            CmdLine<TestClassStatic>.Execute(args).Should().Be(0);
            TestClassStatic.GetMethodInvokedAndReset().Should().Be("test2");
        }

        [TestMethod]
        public void WHEN_ArgIsValueTypeArray_THEN_MethodIsExecuted()
        {
            var args = new[] { "test3", "/count=1", "/list=1" };
            CmdLine<TestClassStatic>.Execute(args).Should().Be(0);
            TestClassStatic.GetMethodInvokedAndReset().Should().Be("test3");
            args = new[] { "test3", "/count=2", "/list=1,2" };
            CmdLine<TestClassStatic>.Execute(args).Should().Be(0);
            TestClassStatic.GetMethodInvokedAndReset().Should().Be("test3");
        }

        [TestMethod]
        public void WHEN_UsingCustomArgSeparator_THEN_MethodIsExecuted()
        {
            var config = new CmdLineConfig { ArgSeparator = ':' };
            var args = new[] { "test3", "/count:1", "/list:1" };
            CmdLine<TestClassStatic>.Execute(args, config).Should().Be(0);
            TestClassStatic.GetMethodInvokedAndReset().Should().Be("test3");
            args = new[] { "test3", "/count:2", "/list:1,2" };
            CmdLine<TestClassStatic>.Execute(args, config).Should().Be(0);
            TestClassStatic.GetMethodInvokedAndReset().Should().Be("test3");
        }

        [TestMethod]
        public void WHEN_ArgLetterCasingIsDifferent_THEN_MethodIsExecuted()
        {
            var args = new[] { "test3", "/Count=1", "/lIst=1" };
            CmdLine<TestClassStatic>.Execute(args).Should().Be(0);
            TestClassStatic.GetMethodInvokedAndReset().Should().Be("test3");
            args = new[] { "TEST3", "/COUNT=2", "/list=1,2" };
            CmdLine<TestClassStatic>.Execute(args).Should().Be(0);
            TestClassStatic.GetMethodInvokedAndReset().Should().Be("test3");
        }

        [TestMethod]
        public void WHEN_UsingCustomArgSeparator_AND_ArgIsInvalid_THEN_MethodIsNotExecuted() // AND_ErrorMessageIsDisplayed
        {
            var config = new CmdLineConfig { ArgSeparator = ':' };
            var args = new[] { "test3", "/count=2", "/list=1,2" };
            CmdLine<TestClassStatic>.Execute(args, config);
            TestClassStatic.GetMethodInvokedAndReset().Should().BeNull();
        }

        [TestMethod]
        public void WHEN_CallingNonpublicMethod_THEN_MethodIsNotExecuted()
        {
            var args = new[] { "nonpublicmethod" };
            CmdLine<TestClassStatic>.Execute(args).Should().Be(-1);
            TestClassStatic.GetMethodInvokedAndReset().Should().BeNull();
        }

        [TestMethod]
        public void WHEN_CallingNonexistingMethod_THEN_MethodIsNotExecuted()
        {
            var args = new[] { "nonstaticmethod" };
            CmdLine<TestClassStatic>.Execute(args).Should().Be(-1);
            TestClassStatic.GetMethodInvokedAndReset().Should().BeNull();
        }

        [TestMethod]
        public void WHEN_CallingInheritedMethod_THEN_MethodIsNotExecuted()
        {
            var args = new[] { "gethashcode" };
            CmdLine<TestClassStatic>.Execute(args).Should().Be(-1);
        }
        [TestMethod]
        public void WHEN_MethodHasNullableArg_AND_ArgIsNotProvided_THEN_MethodIsNotExecuted()
        {
            var args = new[] { "nullable" };
            CmdLine<TestClassStatic>.Execute(args).Should().Be(-1);
            TestClassStatic.GetMethodInvokedAndReset().Should().BeNull();
        }

        [TestMethod]
        public void WHEN_MethodHasNullableArg_AND_ArgIsProvided_THEN_MethodIsExecuted()
        {
            var args = new[] { "nullable", "/value=3" };
            CmdLine<TestClassStatic>.Execute(args).Should().Be(0);
            TestClassStatic.GetMethodInvokedAndReset().Should().Be("nullable");
        }

        [TestMethod]
        public void WHEN_MethodHasNullableArgWithDefault_AND_ArgIsProvided_THEN_MethodIsExecuted()
        {
            var args = new[] { "nullablewithdefault", "/value=7" };
            CmdLine<TestClassStatic>.Execute(args).Should().Be(0);
            TestClassStatic.GetMethodInvokedAndReset().Should().Be("nullablewithdefault");
        }

        [TestMethod]
        public void WHEN_MethodHasNullableArgWithDefault_AND_ArgIsNotProvided_THEN_MethodIsExecuted()
        {
            var args = new[] { "nullablewithdefault" };
            CmdLine<TestClassStatic>.Execute(args).Should().Be(0);
            TestClassStatic.GetMethodInvokedAndReset().Should().Be("nullablewithdefault");
        }
    }
}
