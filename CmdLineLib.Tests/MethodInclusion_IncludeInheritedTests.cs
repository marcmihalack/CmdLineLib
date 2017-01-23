using Microsoft.VisualStudio.TestTools.UnitTesting;
using CmdLineLib.Attributes;
using FluentAssertions;

namespace CmdLineLib.Tests
{
    [TestClass]
    public class MethodInclusion_IncludeInheritedTests : MethodInclusionBase
    {
        public class Class
        {
            public int Field;
            public static int StaticField;
            public int Property { get; set; }
            public static int StaticProperty { get; set; }

            public static void StaticMethod()
            {
                WasStaticMethodCalled = true;
            }
            public void Method()
            {
                WasNonStaticMethodCalled = true;
            }
        };

        [CmdLineClass(inclusionBehavior: InclusionBehavior.IncludeAll | InclusionBehavior.IncludeInherited)]
        public class DerivedAllIncludedClass : Class
        {
        };

        internal static bool WasNotInheritedMethodCalled = false;
        [CmdLineClass(inclusionBehavior: InclusionBehavior.IncludeAll & (~InclusionBehavior.IncludeInherited))]
        public class DerivedClass : Class
        {
            public void NotInheritedMethod()
            {
                WasNotInheritedMethodCalled = true;
            }
        };

        [TestInitialize]
        public void InitializeTest()
        {
            WasStaticMethodCalled = false;
            WasNonStaticMethodCalled = false;
            WasNotInheritedMethodCalled = false;
        }

        // -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

        [TestMethod]
        public void WHEN_ArgIsForNonStaticMethod_THEN_NonStaticMethodCalled()
        {
            Test_Call<DerivedAllIncludedClass>("method", true, false);
        }

        [TestMethod]
        public void WHEN_ArgIsForStaticMethod_THEN_StaticMethodIsNotCalled()
        {
            Test_Call<DerivedAllIncludedClass>("staticmethod", false, false);
        }

        [TestMethod]
        public void WHEN_ArgIsForProperty_THEN_PropertyIsSet_AND_MethodIsExecuted()
        {
            var args = new string[] { "method", "/Property=7" };
            var instance = new DerivedAllIncludedClass();
            instance.Property = 0;
            CmdLine<DerivedAllIncludedClass>.Execute(args, instance).Should().Be(0);
            instance.Property.Should().Be(7);
            WasNonStaticMethodCalled.Should().BeTrue();
            WasStaticMethodCalled.Should().BeFalse();
        }

        [TestMethod]
        public void WHEN_ArgIsForField_THEN_FieldIsSet_AND_MethodIsExecuted()
        {
            var args = new string[] { "method", "/Field=7" };
            var instance = new DerivedAllIncludedClass();
            instance.Field = 0;
            CmdLine<DerivedAllIncludedClass>.Execute(args, instance).Should().Be(0);
            instance.Field.Should().Be(7);
            WasNonStaticMethodCalled.Should().BeTrue();
            WasStaticMethodCalled.Should().BeFalse();
        }

        [TestMethod]
        public void WHEN_InheritedMembersAreNotIncluded_THEN_MethodIsNotExecuted()
        {
            var args = new string[] { "method", "/Property=7" };
            var instance = new DerivedClass();
            instance.Property = 0;
            CmdLine<DerivedClass>.Execute(args, instance).Should().Be(-1);
            instance.Property.Should().Be(0);
            WasNonStaticMethodCalled.Should().BeFalse();
            WasStaticMethodCalled.Should().BeFalse();
        }

        [TestMethod]
        public void WHEN_InheritedMembersAreNotIncluded_AND_ArgIsForNotInheritedMethod_AND_ForInheritedProperty_THEN_MethodIsNotExecuted()
        {
            var args = new string[] { "notinheritedmethod", "/Property=7" };
            var instance = new DerivedClass();
            instance.Property = 0;
            CmdLine<DerivedClass>.Execute(args, instance).Should().Be(-1);
            instance.Property.Should().Be(0);
            WasNonStaticMethodCalled.Should().BeFalse();
            WasStaticMethodCalled.Should().BeFalse();
        }

        [TestMethod]
        public void WHEN_InheritedMembersAreNotIncluded_AND_ArgIsForNotInheritedMethod_THEN_MethodIsExecuted()
        {
            var args = new string[] { "notinheritedmethod" };
            var instance = new DerivedClass();
            instance.Property = 0;
            CmdLine<DerivedClass>.Execute(args, instance).Should().Be(0);
            instance.Property.Should().Be(0);
            WasNotInheritedMethodCalled.Should().BeTrue();
            WasNonStaticMethodCalled.Should().BeFalse();
            WasStaticMethodCalled.Should().BeFalse();
        }
    }
}
