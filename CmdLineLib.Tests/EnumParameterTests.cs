using Microsoft.VisualStudio.TestTools.UnitTesting;
using CmdLineLib.Tests.Classes;
using FluentAssertions;
using CmdLineLib.Attributes;
using System;

namespace CmdLineLib.Tests
{
    [TestClass]
    public class EnumParameterTests
    {
        public enum TestEnumType
        {
            NeverSetValue,
            Value2,
            Value3 = 0xFF,
        }

        [Flags]
        public enum FlagType
        {
            NoFlag = 0,
            FlagA = 1,
            FlagB = 2,
            FlagC = 4
        }

        public class EnumTestClass
        {
            public TestEnumType Actual { get; private set; }

            public EnumTestClass()
            {
                Actual = TestEnumType.NeverSetValue;
            }

            public void EnumTest(TestEnumType value)
            {
                Assert.AreNotEqual(TestEnumType.NeverSetValue, value, "Test is not setup correctly or something is really messed up");
                Actual = value;
            }

            public void EnumTestWithDefault(TestEnumType value = TestEnumType.Value3)
            {
                Assert.AreNotEqual(TestEnumType.NeverSetValue, value, "Test is not setup correctly or something is really messed up");
                Actual = value;
            }
        }

        public class FlagTestClass
        {
            public FlagType Actual { get; private set; }

            public FlagTestClass()
            {
                Actual = FlagType.NoFlag;
            }

            public void EnumTest(FlagType value)
            {
                Actual = value;
            }

            public void EnumTestWithDefault(FlagType value = FlagType.FlagC)
            {
                Actual = value;
            }
        }

        [TestInitialize]
        public void InitializeTest()
        {
            TestClassStatic.Reset();
        }

        [TestMethod]
        public void WHEN_EnumArgIsPassed_THEN_MethodIsExecuted()
        {
            var args = new[] { "EnumTest", "/value=Value2" };
            var instance = new EnumTestClass();
            CmdLine<EnumTestClass>.Execute(args, instance).Should().Be(0);
            instance.Actual.Should().Be(TestEnumType.Value2);
        }

        [TestMethod]
        public void WHEN_TypeIsEnum_AND_FlagsArgIsPassed_THEN_MethodIsNotExecuted()
        {
            var args = new[] { "EnumTest", "/value=Value1,Value2" };
            var instance = new EnumTestClass();
            CmdLine<EnumTestClass>.Execute(args, instance).Should().Be(-1);
            instance.Actual.Should().Be(TestEnumType.NeverSetValue);
        }

        [TestMethod]
        public void WHEN_MethodHasDefaultEnumParameter_AND_NoArgIsPassed_THEN_MethodIsExecuted()
        {
            var args = new[] { "EnumTestWithDefault" };
            var instance = new EnumTestClass();
            CmdLine<EnumTestClass>.Execute(args, instance).Should().Be(0);
            instance.Actual.Should().Be(TestEnumType.Value3);
        }

        [TestMethod]
        public void WHEN_InvalidEnumArgIsPassed_THEN_MethodIsNotExecuted()
        {
            var args = new[] { "EnumTest", "/value=ValueA" };
            var instance = new EnumTestClass();
            CmdLine<EnumTestClass>.Execute(args, instance).Should().Be(-1);
            instance.Actual.Should().Be(TestEnumType.NeverSetValue);
        }

        [TestMethod]
        public void WHEN_FlagArgIsPassed_THEN_MethodIsExecuted()
        {
            var args = new[] { "EnumTest", "/value=FlagA" };
            var instance = new FlagTestClass();
            CmdLine<FlagTestClass>.Execute(args, instance).Should().Be(0);
            instance.Actual.Should().Be(FlagType.FlagA);
        }

        [TestMethod]
        public void WHEN_FlagArgIsPassed_AND_ArgHasMultipleFlags_THEN_MethodIsExecuted()
        {
            var args = new[] { "EnumTest", "/value=FlagA,FlagB" };
            var instance = new FlagTestClass();
            CmdLine<FlagTestClass>.Execute(args, instance).Should().Be(0);
            instance.Actual.Should().Be(FlagType.FlagA| FlagType.FlagB);
        }

        [TestMethod]
        public void WHEN_MethodHasDefaultFlagsParameter_AND_NoArgIsPassed_THEN_MethodIsExecuted()
        {
            var args = new[] { "EnumTestWithDefault" };
            var instance = new FlagTestClass();
            CmdLine<FlagTestClass>.Execute(args, instance).Should().Be(0);
            instance.Actual.Should().Be(FlagType.FlagC);
        }

        [TestMethod]
        public void WHEN_InvalidFlagsArgIsPassed_THEN_MethodIsNotExecuted()
        {
            var args = new[] { "EnumTest", "/value=Flag4" };
            var instance = new FlagTestClass();
            CmdLine<FlagTestClass>.Execute(args, instance).Should().Be(-1);
            instance.Actual.Should().Be(FlagType.NoFlag);
        }
    }
}
