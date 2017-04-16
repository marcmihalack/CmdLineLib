using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentAssertions;
using CmdLineLib.Attributes;

namespace CmdLineLib.Tests
{
    [CmdLineClass(helpText: "Test class with static members and invalid default arg type")]
    public sealed class TestClassWithInvalidArg
    {
        [CmdLineMethod("withinvaliddefault")]
        public static void TestMethodWithDefaultValue(
            [CmdLineArg("parameterWithInvalidDefaultValueType", "This is might be considered help text, but is actually default value")]int count)
        {
            Assert.Fail("Method with invalid default value's type should not be invoked");
        }


    }

    [TestClass]
    public class MethodWithInvalidDefaultValueTypeTests
    {
        [TestMethod]
        public void WHEN_ArgHasInvalidDefaultType_THEN_CmdLineExceptionIsThrown()
        {
            var args = new[] { "withinvaliddefault" };
            this.Invoking(self => CmdLine<TestClassWithInvalidArg>.Execute(args)).ShouldThrow<CmdLineException>().And.Message.Should().Contain("parameterWithInvalidDefaultValueType");
        }
    }
}
