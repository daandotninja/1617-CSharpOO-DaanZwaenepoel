using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;
using System.Collections.Generic;

using System.Linq;
using Globals;
using LogicLayer;

namespace UnitTests
{
    public static class Extensions
    {
        public static bool IsEqual(this RunInfo thisInfo, RunInfo info)
        {
            return ((thisInfo.DistanceInMeter == info.DistanceInMeter) && (thisInfo.Interval == info.Interval));
        }

        public static TimeSpan EncodeDateToTimeSpan(DateTime date)
        {
            return date.Subtract(new DateTime(2000,1,1));
        }

    }
    
    internal class DummyDal : IDataAccess
    {

        private bool closeCalled = false;
        private bool clearDataCalled = false;
        private int getRunInfoForDateCalledCount = 0;
        private bool storeRunInfoCalled = false;
        private bool returnDateinInterval = false;

        private RunInfo storedInfo;
        private DateTime requestedDate;

        public bool CloseCalled
        {
            get
            {
                return closeCalled;
            }

            set
            {
                closeCalled = value;
            }
        }
        public bool ClearDataCalled
        {
            get
            {
                return clearDataCalled;
            }

            set
            {
                clearDataCalled = value;
            }
        }
        public int GetRunInfoForDateCalledCount
        {
            get
            {
                return getRunInfoForDateCalledCount;
            }

            set
            {
                getRunInfoForDateCalledCount = value;
            }
        }
        public bool StoreRunInfoCalled
        {
            get
            {
                return storeRunInfoCalled;
            }

            set
            {
                storeRunInfoCalled = value;
            }
        }
        public bool ReturnDateinInterval
        {
            get
            {
                return returnDateinInterval;
            }

            set
            {
                returnDateinInterval = value;
            }
        }
        public RunInfo StoredInfo
        {
            get
            {
                return storedInfo;
            }

            set
            {
                storedInfo = value;
            }
        }
        public DateTime RequestedDate
        {
            get
            {
                return requestedDate;
            }

            set
            {
                requestedDate = value;
            }
        }

        public DummyDal()
        {
            StoredInfo = new RunInfo(405, new TimeSpan(123));
        }
        
        public void ClearData()
        {
            ClearDataCalled = true;
        }

        public void Close()
        {
            CloseCalled = true; ;
        }

        public RunInfo GetRunInfoForDate(DateTime date)
        {
            GetRunInfoForDateCalledCount++;
            RequestedDate = date;

            return ReturnDateinInterval?new RunInfo(StoredInfo.DistanceInMeter, Extensions.EncodeDateToTimeSpan(date)):StoredInfo;            
        }

        public void StoreRunInfo(DateTime date, RunInfo info)
        {
            StoreRunInfoCalled = true;
            RequestedDate = date;
            StoredInfo = info;
        }
    }


    [TestClass]
    public class LogicLayerTest
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
        public void ILogicDeclaredOk()
        {
            Type x = GetTypeByName("ILogic");
            Assert.IsNotNull(x, $"GlobalTests - interface \"ILogic\" not declared!");
            CheckProperty(typeof(ILogic), "CurrentDate", new Type[] { typeof(DateTime) }, true, false);
            CheckMethod(x, "SetRunInfoForDate", new Type[] { typeof(void) }, new Type[] { typeof(DateTime), typeof(RunInfo) });
            CheckMethod(x, "GetRunInfoForDate", new Type[] { typeof(RunInfo) }, new Type[] { typeof(DateTime) });
            CheckMethod(x, "GetRunInfoForPastSevenDays", new Type[] { typeof(RunInfo[]) }, new Type[] { });
            CheckMethod(x, "GetRunInfoForPastThirtyDays", new Type[] { typeof(RunInfo[]) }, new Type[] { });
            CheckMethod(x, "ClearData", new Type[] { typeof(void) }, new Type[] { });
            CheckMethod(x, "Close", new Type[] { typeof(void) }, new Type[] { });
        }
        
        [TestMethod, Timeout(100)]
        public void LogicImplementationIsDerivedFromILogic()
        {

            ILogic logic = new LogicImplementation(new DummyDal()) as ILogic;

            Assert.IsNotNull(logic,
                $"LogicLayerTests - Class \"LogicImplementation\" does not implement interface \"ILogic\"!");
        }

        [TestMethod, Timeout(100)]
        public void TestIfLogicConstructorTakesDalInterfaceParameter()
        {
            Type x = typeof(LogicImplementation);
            ConstructorInfo constructor = x.GetConstructor(new Type[] { typeof(IDataAccess) });
            Assert.IsNotNull(constructor,
                $"LogicLayerTests - constructor for \"LogicImplementation\" does not take IDataAccess parameter!");
            constructor = x.GetConstructor(new Type[] { });
            Assert.IsNull(constructor,
                $"LogicLayerTests - constructor for \"LogicImplementation\" has a default constructor (not allowed)!");
        }

        [TestMethod, Timeout(100)]
        public void CurrentDateIsCorrect()
        {
            var logic = new LogicImplementation(new DummyDal());

            Assert.IsTrue((logic.CurrentDate == DateTime.Now.Date),
                $"LogicLayerTests - \"CurrentDate\" returns wrong value: was {logic.CurrentDate}, expected {DateTime.Now.Date}!");
        }

        [TestMethod, Timeout(100)]
        public void SpeedIsCalculatedOK()
        {
            var logic = new LogicImplementation(new DummyDal());

            var speed = logic.GetSpeedInKmPerHour(new RunInfo(500, new TimeSpan(0, 20, 0)));
            Assert.IsTrue((Math.Abs(speed - 1.5) < 0.01),
                $"LogicLayerTests - \"GetSpeedInKmPerHour\" returns wrong value: was {speed}, expected {1.5}!");
        }

        [TestMethod, Timeout(100)]
        public void TestGetRunInfoForDate()
        {
            var dal = new DummyDal();
            var logic = new LogicImplementation(dal);

            var info = logic.GetRunInfoForDate(new DateTime(42).Date);
            Assert.IsTrue((dal.GetRunInfoForDateCalledCount > 0),
                $"LogicLayerTests - \"GetRunInfoForDate\" does not call corresponding method from data access implementation!");
            Assert.IsTrue((info.IsEqual(dal.StoredInfo)),
                $"LogicLayerTests - \"GetRunInfoForDate\" does not return RunInfo as passed by data access implementation !");
        }

        [TestMethod, Timeout(100)]
        public void TestSetRunInfoForDate()
        {
            var dal = new DummyDal();
            var logic = new LogicImplementation(dal);
            var date = new DateTime(1234, 5, 6);
            var info = new RunInfo(321, new TimeSpan(0, 15, 30));

            logic.SetRunInfoForDate(date, info);

            Assert.IsTrue((dal.StoreRunInfoCalled),
                $"LogicLayerTests - \"SetRunInfoForDate\" does not call corresponding method from data access implementation!");
            Assert.IsTrue((info.IsEqual(dal.StoredInfo)),
                $"LogicLayerTests - \"SetRunInfoForDate\" does not pass RunInfo correctly to data access implementation !");
            Assert.IsTrue((date == dal.RequestedDate),
                $"LogicLayerTests - \"SetRunInfoForDate\" does not pass date correctly to data access implementation !");
        }

        [TestMethod, Timeout(100)]
        public void TestSetRunInfoBusinessRules()
        {
            var dal = new DummyDal();
            var logic = new LogicImplementation(dal);
            var date = DateTime.Now.Date;
            try
            {
                var info = new RunInfo(-5, new TimeSpan(3210));
                logic.SetRunInfoForDate(date, info);
                Assert.Fail($"LogicLayerTests - \"SetRunInfoForDate\" does not throw ArgumentOutOfRangeException for negative distance!");
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
                Assert.Fail($"LogicLayerTests - \"SetRunInfoForDate\" with negative distance throws {e.GetType().Name} instead of ArgumentOutOfRangeException !");
            }

            try
            {
                var info = new RunInfo(130, new TimeSpan(0, 0, 9));
                logic.SetRunInfoForDate(date, info);
                Assert.Fail($"LogicLayerTests - \"SetRunInfoForDate\" does not throw ArgumentOutOfRangeException for speed in excess of 50km/h!");
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
                Assert.Fail($"LogicLayerTests - \"SetRunInfoForDate\" with speed in excess of 50km/h throws {e.GetType().Name} instead of ArgumentOutOfRangeException !");
            }
        }

        [TestMethod, Timeout(100)]
        public void TestLogicOmitsTimeFromDate()
        {
            var dal = new DummyDal();
            var logic = new LogicImplementation(dal);
            var date = new DateTime(1234, 5, 6, 12, 12, 12);
            var info = new RunInfo(321, new TimeSpan(0, 15, 30));
            logic.SetRunInfoForDate(date, info);
            Assert.IsTrue((date.Date == dal.RequestedDate),
                $"LogicLayerTests - \"SetRunInfoForDate\" does not omit time info from Date when passing on to data access implementation !");
            info = logic.GetRunInfoForDate(date);
            Assert.IsTrue((date.Date == dal.RequestedDate),
                $"LogicLayerTests - \"SetRunInfoForDate\" does not omit time info from Date when passing on to data access implementation !");
        }

        [TestMethod, Timeout(100)]
        public void TestCleardata()
        {
            var dal = new DummyDal();
            var logic = new LogicImplementation(dal);

            logic.ClearData();
            Assert.IsTrue((dal.ClearDataCalled),
                $"LogicLayerTests - \"ClearData\" does not call corresponding method from data access implementation!");
        }

        [TestMethod, Timeout(100)]
        public void TestClose()
        {
            var dal = new DummyDal();
            var logic = new LogicImplementation(dal);

            logic.Close();
            Assert.IsTrue((dal.CloseCalled),
                $"LogicLayerTests - \"Close\" does not call corresponding method from data access implementation!");
        }

        [TestMethod, Timeout(100)]
        public void TestGetRunInfoForPastSevenDays()
        {
            const int count = 7;
            var dal = new DummyDal();
            var logic = new LogicImplementation(dal);
            dal.ReturnDateinInterval = true;

            var infos = logic.GetRunInfoForPastSevenDays();
            Assert.IsTrue((infos.Length == count),
                $"LogicLayerTests - \"GetRunInfoForPastSevenDays\" does not return seven RunInfo values!");
            for (int i = 0; i < count; i++)
            {
                Assert.IsTrue((infos[i].Interval == Extensions.EncodeDateToTimeSpan(DateTime.Now.Date- new TimeSpan(count - i - 1, 0, 0, 0))),
                $"LogicLayerTests - \"GetRunInfoForPastSevenDays\" does not return RunInfo values for correct dates in correct sequence (oldest first)!");
            }            
        }

        [TestMethod, Timeout(100)]
        public void TestGetRunInfoForPastThirtyDays()
        {
            const int count = 30;
            var dal = new DummyDal();
            var logic = new LogicImplementation(dal);
            dal.ReturnDateinInterval = true;

            var infos = logic.GetRunInfoForPastThirtyDays();
            Assert.IsTrue((infos.Length == count),
                $"LogicLayerTests - \"GetRunInfoForPastThirtyDays\" does not return thirty RunInfo values!");
            for (int i = 0; i < count; i++)
            {
                Assert.IsTrue((infos[i].Interval == Extensions.EncodeDateToTimeSpan(DateTime.Now.Date - new TimeSpan(count - i - 1, 0, 0, 0))),
                $"LogicLayerTests - \"GetRunInfoForPastThirtyDays\" does not return RunInfo values for correct dates in correct sequence (oldest first)!");
            }
        }

     
    }



}

