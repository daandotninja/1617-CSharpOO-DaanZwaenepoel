using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Globals;
using LogicLayer;
using RunnerMain;

namespace UnitTests
{

    [TestClass]
    public class DeelB_Architectuur
    {
        [TestMethod, Timeout(100)]
        public void TestIfMainFormConstructorTakesLogicInterfaceParameter()
        {
            Type x = typeof(MainForm);
            ConstructorInfo constructor = x.GetConstructor(new Type[] { typeof(ILogic) });
            Assert.IsNotNull(constructor,
                $"Deel B_Architectuur - constructor for \"MainForm\" does not take ILogic parameter!");
            constructor = x.GetConstructor(new Type[] { });
            Assert.IsNull(constructor,
                $"Deel B_Architectuur - \"MainForm\" has a default constructor (not allowed)!");            
        }

        [TestMethod, Timeout(100)]
        public void TestIfMainFormHasAPrivateIlogicfield()
        {
            var fields = typeof(MainForm).GetFields();
            var OK = (fields.Where(f => f.FieldType == typeof(ILogic)).Count() == 0);
            Assert.IsTrue(OK, $"Deel B_Architectuur - \"MainForm\" uses a public field to store \"ILogic\" reference (must be private)!");
            fields = typeof(MainForm).GetFields(BindingFlags.NonPublic |
                         BindingFlags.Instance);
            OK = (fields.Where(f => f.FieldType == typeof(ILogic)).Count() > 0);
            Assert.IsTrue(OK, $"Deel B_Architectuur - \"MainForm\" does not have a private field to store ILogic reference)!");            
        }
        
        [TestMethod, Timeout(100)]
        public void CheckMainFormhasNoFieldsOfTypeLogicImplementation()
        {
            Type x = typeof(MainForm);
            ConstructorInfo constructor = x.GetConstructor(new Type[] { typeof(ILogic) });
            Assert.IsNotNull(constructor,
                $"Deel B_Architectuur - constructor for \"MainForm\" does not take ILogic parameter!");
            constructor = x.GetConstructor(new Type[] { });
            Assert.IsNull(constructor,
                $"Deel B_Architectuur - \"MainForm\" has a default constructor (not allowed)!");
            var fields = typeof(MainForm).GetFields(BindingFlags.NonPublic |
                         BindingFlags.Instance);
            var OK = (fields.Where(f => f.FieldType == typeof(LogicImplementation)).Count() == 0);
            Assert.IsTrue(OK, $"Deel B_Architectuur - \"MainForm\" uses a private instance of \"logicImplementation\" (must use injected interface instead)!");
            fields = typeof(MainForm).GetFields();
            OK = (fields.Where(f => f.FieldType == typeof(LogicImplementation)).Count() == 0);
            Assert.IsTrue(OK, $"Deel B_Architectuur - \"MainForm\" uses a public instance of \"logicImplementation\" (must use injected interface instead)!");
            fields = typeof(MainForm).GetFields(BindingFlags.NonPublic |
                         BindingFlags.Instance);
        }
    }
}
