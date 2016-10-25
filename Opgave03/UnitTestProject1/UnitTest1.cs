using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

using System.Linq;
using Globals;

namespace UnitTests
{
    
    [TestClass]
    public class GlobalTests
    {
       // [ClassInitialize()]
       
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
            Assert.IsNotNull(prop, $"GlobalTests - {t.GetType().Name} has no public {propName} property");
            Assert.IsTrue(Array.Exists(propTypes, p => p.Name == prop.PropertyType.Name),
                              $"GlobalTests - {typeof(object).Name}: property {propName} is a {prop.PropertyType.Name}");
            Assert.IsTrue((prop.CanRead == hasGetter), $"GlobalTests - {t.Name}: property {propName} has {(prop.CanRead ? "a" : "no")} public Getter ");
            Assert.IsTrue((prop.CanWrite == hasSetter), $"GlobalTests - {t.Name}: property {propName} has {(prop.CanWrite ? "a" : "no")} public Setter ");

        }

        private void CheckMethod(Type t, string methodName, Type[] returnTypes, Type[] parameterTypes)
        {
            var methods = t.GetMethods();
            // check if method exists with right signature
            var method = methods.Where(m =>
            {
                if (m.Name != methodName) return false;
                var parameters = m.GetParameters();
                if ((parameterTypes == null || parameterTypes.Length == 0)) return parameters.Length == 0;
                if (parameters.Length != parameterTypes.Length) return false;
                for (int i = 0; i < parameterTypes.Length; i++)
                {
                    // if (parameters[i].ParameterType != parameterTypes[i])
                    if (!parameters[i].ParameterType.IsAssignableFrom(parameterTypes[i]))
                        return false;
                }
                return true;
            }).FirstOrDefault();
            Assert.IsNotNull(method, $"GlobalTests - {t.FullName} has no public {methodName} method with the right signature");

            // check returnType
            Assert.IsTrue(Array.Exists(returnTypes, r => r.Name == method.ReturnType.Name),
                              $"GlobalTests - {typeof(object).Name}: method {methodName} returns a {method.ReturnType.Name}");
        }
        

        [TestMethod, Timeout(100)]
        public void UnitTestsAreFunctioning()
        {
            //Dummy: 1 point for making unit tests work
        }

        [TestMethod, Timeout(100)]
        public void RunInfoDeclaredOk()
        {
            CheckProperty(typeof(RunInfo), "DistanceInMeter", new Type[] { typeof(int), typeof(Int32) }, true, false);
            CheckProperty(typeof(RunInfo), "Interval", new Type[] { typeof(TimeSpan) }, true, false);
            var info = new RunInfo(5000, new TimeSpan(0, 30, 0));
            Assert.IsTrue((info.DistanceInMeter == 5000), $"GlobalTests - RunInfo Constructor does not initialise Distance correctly!");
            Assert.IsTrue((info.Interval == new TimeSpan(0, 30, 0)), $"GlobalTests - RunInfo Constructor does not initialise Interval correctly!");
        }

        [TestMethod, Timeout(100)]
        public void testRunInfoDefaultConstructor()
        {
            Type x = typeof(RunInfo);
            ConstructorInfo constructor = x.GetConstructor(new Type[] { });
            Assert.IsNull(constructor,
                $"LogicLayerTests - constructor for \"RunInfo\" has a default constructor (not allowed)!");
        }
                
        [TestMethod, Timeout(100)]
        public void RunInfoIsSerializable()
        {
            
            Assert.IsTrue((typeof(RunInfo).IsSerializable), $"GlobalTests - RunInfo class is missing the  [Serializable] attribute!");
        }

        [TestMethod, Timeout(100)]
        public void TestExceptionForRunInfoInvalidParameters()
        {
            try
            {
                var x = new RunInfo(-5, new TimeSpan(0));
                Assert.Fail($"GlobalTests - RunInfo constructor does not throw ArgumentOutOfRangeException for negative distance!");
            }
            catch (ArgumentOutOfRangeException e)
            {
                // this is expected, no problem                
            }
            catch (AssertFailedException e)
            {
                throw e;
            }
            catch (Exception e)
            {
                Assert.Fail($"GlobalTests - RunInfo constructor with negative distance throws {e.GetType().Name} instead of ArgumentOutOfRangeException !");
            }           
        }
              
    }

  
}
