using Microsoft.VisualStudio.TestTools.UnitTesting;
using CmdLineLib.Attributes;

namespace CmdLineLib.Tests
{
    [TestClass]
    public class MethodInclusion_IncludeAllStaticTests : MethodInclusionBase
    {
        [CmdLineClass(InclusionBehavior: InclusionBehavior.IncludeAllStatic)]
        class ClassWithStaticMethods
        {
            public static void MyStaticMethod()
            {
                WasStaticMethodCalled = true;
            }
        };

        [CmdLineClass(InclusionBehavior: InclusionBehavior.IncludeAllStatic)]
        class ClassWithAllMethods
        {
            public void MyMethod()
            {
                WasNonStaticMethodCalled = true;
            }

            public static void MyStaticMethod()
            {
                WasStaticMethodCalled = true;
            }
        };

        [CmdLineClass(InclusionBehavior: InclusionBehavior.IncludeAllStatic)]
        class ClassWithNonStaticMethods
        {
            public void MyMethod()
            {
                WasNonStaticMethodCalled = true;
            }
        };

        [TestInitialize]
        public void InitializeTest()
        {
            WasStaticMethodCalled = false;
            WasNonStaticMethodCalled = false;
        }

        // -=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=-=

        [TestMethod]
        public void When_ClassWithNonStaticMethods_MyMethod()
        {
            Test_Call<ClassWithNonStaticMethods>("mymethod", false,false);
        }

        [TestMethod]
        public void When_ClassWithNonStaticMethods_MyStaticMethod()
        {
            Test_Call<ClassWithNonStaticMethods>("mystaticmethod", false, false);
        }

        [TestMethod]
        public void When_ClassWithStaticMethods_MyMethod()
        {
            Test_Call<ClassWithStaticMethods>("mymethod", false, false);
        }

        [TestMethod]
        public void When_ClassWithStaticMethods_MyStaticMethod()
        {
            Test_Call<ClassWithStaticMethods>("mystaticmethod", false, true);
        }

        [TestMethod]
        public void When_ClassWithAllMethods_MyMethod()
        {
            Test_Call<ClassWithAllMethods>("mymethod", false, false);
        }

        [TestMethod]
        public void When_ClassWithAllMethods_MyStaticMethod()
        {
            Test_Call<ClassWithAllMethods>("mystaticmethod", false, true);
        }
    }
}
