using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

using System.Linq;
using Globals;
using DataAccessLayer;

namespace UnitTests
{
    [TestClass]
    public class DataAccessLayerTests
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
        public void IDataAccessDeclaredOk()
        {
            Type x = GetTypeByName("IDataAccess");
            Assert.IsNotNull(x, $"GlobalTests - interface \"IDataAccess\" not declared!");
            CheckMethod(x, "StoreRunInfo", new Type[] { typeof(void) }, new Type[] { typeof(DateTime), typeof(RunInfo) });
            CheckMethod(x, "ClearData", new Type[] { typeof(void) }, new Type[] { });
            CheckMethod(x, "Close", new Type[] { typeof(void) }, new Type[] { });
            CheckMethod(x, "GetRunInfoForDate", new Type[] { typeof(RunInfo) }, new Type[] { typeof(DateTime) });
        }

        [TestMethod, Timeout(500)]
        public void DataAccessImplementationIsDerivedFromIDataAccess()
        {
            IDataAccess dal = new DataAccessImplementation() as IDataAccess;

            Assert.IsNotNull(dal,
                $"DataAccessLayerTests - Class \"DataAccessImplementation\" does not implement interface \"IDataAccess\"!");
        }

        [TestMethod, Timeout(500)]
        public void NonexistingDatereturnsDefaultValue()
        {
            var da = new DataAccessImplementation();
            var info = da.GetRunInfoForDate(DateTime.Now.Date + new TimeSpan(3000, 0, 0, 0));
            Assert.IsNotNull(info, $"DataAccessLayerTests - \"GetRunInfoForDate\" returns null for date without info (should be default value)!");
            Assert.IsTrue((info.DistanceInMeter == 0), $"DataAccessLayerTests - \"GetRunInfoForDate\" does not return zero distance for date without info!");
            Assert.IsTrue((info.Interval == new TimeSpan(0, 0, 0)), $"DataAccessLayerTests - \"GetRunInfoForDate\" does not return zero interval for date without info!");
        }

        [TestMethod, Timeout(500)]
        public void StoreAndRetrieveRunInfos()
        {
            const int distance1 = 1000;
            var interval1 = new TimeSpan(0, 6, 0);
            var da = new DataAccessImplementation();
            var date = DateTime.Now - new TimeSpan(5, 0, 0, 0);

            da.StoreRunInfo(date, new RunInfo(distance1, interval1));

            var info = da.GetRunInfoForDate(date);

            Assert.IsNotNull(info, $"DataAccessLayerTests - \"GetRunInfoForDate\" returns null for date with stored info!");
            Assert.IsTrue((info.DistanceInMeter == distance1), $"DataAccessLayerTests - \"GetRunInfoForDate\" returns wrong distance: was {info.DistanceInMeter}, expected {distance1}!");
            Assert.IsTrue((info.Interval == interval1), $"DataAccessLayerTests - \"GetRunInfoForDate\" returns wrong interval: was {info.Interval.ToString()}, expected {interval1.ToString()}!");
        }

        [TestMethod, Timeout(500)]
        public void OverWriteRunInfo()
        {
            const int distance1 = 1000;
            var interval1 = new TimeSpan(0, 6, 0);
            const int distance2 = 2000;
            var interval2 = new TimeSpan(0, 15, 0);
            var da = new DataAccessImplementation();
            var date = DateTime.Now - new TimeSpan(5, 0, 0, 0);
            try
            {
                da.StoreRunInfo(date, new RunInfo(distance1, interval1));
                da.StoreRunInfo(date, new RunInfo(distance2, interval2));
            }
            catch (Exception e)
            {
                Assert.Fail($"DataAccessLayerTests - OverWriteRunInfo: storing new info for existing date throws exception: {e.Message} ");
            }
            var info = da.GetRunInfoForDate(date);

            Assert.IsNotNull(info, $"DataAccessLayerTests - \"GetRunInfoForDate\" returns null for date with stored info!");
            Assert.IsTrue((info.DistanceInMeter == distance2), $"DataAccessLayerTests - \"GetRunInfoForDate\" returns wrong distance after overwrite: was {info.DistanceInMeter}, expected {distance2}!");
            Assert.IsTrue((info.Interval == interval2), $"DataAccessLayerTests - \"GetRunInfoForDate\" returns wrong interval after overwrite: was {info.Interval.ToString()}, expected {interval2.ToString()}!");
        }

        [TestMethod, Timeout(500)]
        public void TestPersistenceAfterClose()
        {
            const int distance1 = 1000;
            var interval1 = new TimeSpan(0, 6, 0);
            var da = new DataAccessImplementation();
            var date = DateTime.Now - new TimeSpan(5, 0, 0, 0);

            da.StoreRunInfo(date, new RunInfo(distance1, interval1));

            var info = da.GetRunInfoForDate(date);

            Assert.IsNotNull(info, $"DataAccessLayerTests - \"GetRunInfoForDate\" returns null for date with stored info!");
            Assert.IsTrue((info.DistanceInMeter == distance1), $"DataAccessLayerTests - \"GetRunInfoForDate\" returns wrong distance: was {info.DistanceInMeter}, expected {distance1}!");
            Assert.IsTrue((info.Interval == interval1), $"DataAccessLayerTests - \"GetRunInfoForDate\" returns wrong interval: was {info.Interval.ToString()}, expected {interval1.ToString()}!");
            da.Close();
            da = new DataAccessImplementation();
            info = da.GetRunInfoForDate(date);
            Assert.IsNotNull(info, $"DataAccessLayerTests - \"GetRunInfoForDate\" returns null for date with stored info after Close()!");
            Assert.IsTrue((info.DistanceInMeter == distance1), $"DataAccessLayerTests - \"GetRunInfoForDate\" does not persist info after Close(), returns wrong distance: was {info.DistanceInMeter}, expected {distance1}!");
            Assert.IsTrue((info.Interval == interval1), $"DataAccessLayerTests - \"GetRunInfoForDate\" does not persist info after Close(),returns wrong interval: was {info.Interval.ToString()}, expected {interval1.ToString()}!");
        }

        [TestMethod, Timeout(500)]
        public void TestClearData()
        {
            const int distance1 = 1000;
            var interval1 = new TimeSpan(0, 6, 0);
            var da = new DataAccessImplementation();
            var date = DateTime.Now - new TimeSpan(5, 0, 0, 0);

            da.StoreRunInfo(date, new RunInfo(distance1, interval1));

            var info = da.GetRunInfoForDate(date);
            Assert.IsNotNull(info, $"DataAccessLayerTests - \"GetRunInfoForDate\" returns null for date with stored info!");
            Assert.IsTrue((info.DistanceInMeter == distance1), $"DataAccessLayerTests - \"GetRunInfoForDate\" returns wrong distance: was {info.DistanceInMeter}, expected {distance1}!");
            Assert.IsTrue((info.Interval == interval1), $"DataAccessLayerTests - \"GetRunInfoForDate\" returns wrong interval: was {info.Interval.ToString()}, expected {interval1.ToString()}!");

            da.ClearData();
            info = da.GetRunInfoForDate(date);
            Assert.IsNotNull(info, $"DataAccessLayerTests - \"GetRunInfoForDate\" returns null after call to \"ClearData\"!");
            Assert.IsTrue((info.DistanceInMeter == 0), $"DataAccessLayerTests - \"GetRunInfoForDate\" returns wrong distance after call to \"ClearData\": was {info.DistanceInMeter}, expected {0}!");
            Assert.IsTrue((info.Interval == new TimeSpan(0, 0, 0)), $"DataAccessLayerTests - \"GetRunInfoForDate\" returns wrong interval after call to \"ClearData\": was {info.Interval.ToString()}, expected {new TimeSpan(0, 0, 0).ToString()}!");


            da = new DataAccessImplementation();
            info = da.GetRunInfoForDate(date);
            Assert.IsNotNull(info, $"DataAccessLayerTests - \"GetRunInfoForDate\" returns null after call to \"ClearData\" & restart!");
            Assert.IsTrue((info.DistanceInMeter == 0), $"DataAccessLayerTests - \"ClearData\" does not clear distance data on disk!");
            Assert.IsTrue((info.Interval == new TimeSpan(0, 0, 0)), $"DataAccessLayerTests - \"ClearData\" does not clear instance data on disk!!");
        }
    }
}
