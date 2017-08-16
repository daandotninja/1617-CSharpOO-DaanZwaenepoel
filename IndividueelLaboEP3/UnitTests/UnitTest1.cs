using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Globals;
using System.Reflection;
using System.Linq;
using System.Drawing;
using System.IO;
using LogicLayer;
using Globals;
using RollTheDiceMain;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;


namespace UnitTests
{
    [TestClass]
    public class Architectuur
    {

        private Type GetTypeByName(string typeName)
        {
            var foundClass = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
                              from type in assembly.GetTypes()
                              where type.Name == typeName
                              select type).FirstOrDefault();
            return (Type)foundClass;
        }

        private void CheckProperty(Type t, string propName, Type[] propTypes, bool hasGetter, bool hasSetter)
        {
            var props = t.GetProperties();
            var prop = props.Where(p => p.Name == propName).FirstOrDefault();
            Assert.IsNotNull(prop, $"Type {t.GetType().Name} has no public \'{propName}\' property");
            Assert.IsTrue(Array.Exists(propTypes, p => p.Name == prop.PropertyType.Name),
                              $"{typeof(object).Name}: property \'{propName}\' is a {prop.PropertyType.Name}");
            Assert.IsTrue((prop.CanRead == hasGetter), $"{t.Name}: property {propName} has {(prop.CanRead ? "a" : "no")} public Getter ");
            Assert.IsTrue((prop.CanWrite == hasSetter), $"GlobalTests - {t.Name}: property {propName} has {(prop.CanWrite ? "a" : "no")} public Setter ");

        }

        private void CheckMethod(Type t, string methodName, Type[] returnTypes, Type[] parameterTypes)
        {
            var methods = t.GetMethods();
            // check if method exists with right signature
            var method = methods.Where(m =>
            {
                if(m.Name != methodName) return false;
                var parameters = m.GetParameters();
                if((parameterTypes == null || parameterTypes.Length == 0)) return parameters.Length == 0;
                if(parameters.Length != parameterTypes.Length) return false;
                for(int i = 0; i < parameterTypes.Length; i++)
                {
                    // if (parameters[i].ParameterType != parameterTypes[i])
                    if(!parameters[i].ParameterType.IsAssignableFrom(parameterTypes[i]))
                        return false;
                }
                return true;
            }).FirstOrDefault();
            Assert.IsNotNull(method, $"Type {t.FullName} has no public \'{methodName}\' method with the right signature");

            // check returnType
            Assert.IsTrue(Array.Exists(returnTypes, r => r.Name == method.ReturnType.Name),
                              $"Type {typeof(object).Name}: method \'{methodName}\' returns a \'{method.ReturnType.Name}\'");
        }

        private void CheckEvent(Type t, string eventName, Type[] eventTypes)
        {
            var events = t.GetEvents();
            var @event = events.Where(p => p.Name == eventName).FirstOrDefault();
            Assert.IsNotNull(@event, $" - {t.GetType().Name} has no {eventName} event");
            Assert.IsTrue(Array.Exists(eventTypes, p => p.Name == @event.EventHandlerType.Name),
                              $"Type {typeof(object).Name}: event {eventName} is a {@event.EventHandlerType.Name}");
        }

        [TestMethod, Timeout(500)]
        public void TestMainFormConstructorTakesLogicInterfaceParameter()
        {
            Type x = typeof(MainForm);
            ConstructorInfo constructor = x.GetConstructor(new Type[] { typeof(ILogic) });
            Assert.IsNotNull(constructor,
                $"Architectuur - constructor for \'MainForm\' does not take ILogic parameter!");
            constructor = x.GetConstructor(new Type[] { });
            Assert.IsNull(constructor,
                $"Architectuur - \'MainForm\' has a default constructor (not allowed)!");
        }
        
        [TestMethod, Timeout(500)]
        public void CheckMainFormHasNoFieldsOfTypeLogic()
        {
            Type x = typeof(MainForm);
            ConstructorInfo constructor = x.GetConstructor(new Type[] { typeof(ILogic) });
            Assert.IsNotNull(constructor,
                $"Architectuur - constructor for \'MainForm\' does not take \'ILogic\' parameter!");
            constructor = x.GetConstructor(new Type[] { });
            Assert.IsNull(constructor,
                $"Architectuur - \'MainForm\' has a default constructor (not allowed)!");
            var fields = typeof(MainForm).GetFields(BindingFlags.NonPublic |
                         BindingFlags.Instance);
            var OK = (fields.Where(f => f.FieldType == typeof(Logic)).Count() == 0);
            Assert.IsTrue(OK, $"Architectuur - \'MainForm\' uses a private instance of \'Logic\' (must use injected interface instead)!");
            fields = typeof(MainForm).GetFields();
            OK = (fields.Where(f => f.FieldType == typeof(Logic)).Count() == 0);
            Assert.IsTrue(OK, $"Architectuur - \'MainForm\' uses a public instance of \'Logic\' (must use injected interface instead)!");
            fields = typeof(MainForm).GetFields(BindingFlags.NonPublic |
                         BindingFlags.Instance);
        }

        [TestMethod, Timeout(500)]
        public void CheckLogicHasNoFieldsOfTypeDice()
        {
            Type x = typeof(Logic);
            ConstructorInfo constructor = x.GetConstructor(new Type[] { typeof(IDice) });
            Assert.IsNotNull(constructor,
                $"Architectuur - constructor for \'Logic\' does not take \'IDice\' parameter!");
            constructor = x.GetConstructor(new Type[] { });
            Assert.IsNull(constructor,
                $"Architectuur - \'Logic\' has a default constructor (not allowed)!");
            var fields = typeof(MainForm).GetFields(BindingFlags.NonPublic |
                         BindingFlags.Instance);
            var OK = (fields.Where(f => f.FieldType == typeof(Dice)).Count() == 0);
            Assert.IsTrue(OK, $"Architectuur - \'Logic\' uses a private instance of \'Dice\' (must use injected interface instead)!");
            fields = typeof(MainForm).GetFields();
            OK = (fields.Where(f => f.FieldType == typeof(Logic)).Count() == 0);
            Assert.IsTrue(OK, $"Architectuur - \'Logic\' uses a public instance of \'Dice\' (must use injected interface instead)!");
            fields = typeof(MainForm).GetFields(BindingFlags.NonPublic |
                         BindingFlags.Instance);
        }

        [TestMethod, Timeout(500)]
        public void ILogicDeclaredOk()
        {
            Type x = GetTypeByName("ILogic");
            Assert.IsNotNull(x, $"Interface \'ILogic\' not declared!");
            CheckProperty(x, "Values", new Type[] { typeof(Dictionary<int, long>) }, true, false);
            CheckMethod(x, "Start", new Type[] { typeof(void) }, new Type[] { });
            CheckMethod(x, "Stop", new Type[] { typeof(void) }, new Type[] { });
            CheckMethod(x, "Calculate", new Type[] { typeof(void) },
                                        new Type[] { typeof(CancellationToken), typeof(IProgress<long>)});
            CheckEvent(x, "ValuesChanged", new Type[] { typeof(Action<long>) });            
        }

        
        [TestMethod, Timeout(500)]
        public void IDiceDeclaredOk()
        {
            Type x = GetTypeByName("IDice");
            Assert.IsNotNull(x, $"Interface \'IDice\' not declared!");
            CheckMethod(x, "SingleValue", new Type[] { typeof(int) }, new Type[] { });
        }

        [TestMethod, Timeout(500)]
        public void TestDiceImplementsIDice()
        {
            var d = new Dice();
            Assert.IsTrue((d is IDice), $"Dice does not implement \'IDice\' interface.");
        }

        [TestMethod, Timeout(500)]
        public void TestLogicImplementsILogic()
        {
            var d = new Dice();
            Assert.IsTrue((d is IDice), $"Dice does not implement \'IDice\' interface.");
            var f = new Logic(d);
            Assert.IsTrue((f is ILogic), $"Logic does not implement \'ILogic\' interface.");
        }

       
    }

}
