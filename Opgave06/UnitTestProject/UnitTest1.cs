using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Globals;
using ShaCollisionsGUIMain;
using System.Reflection;
using System.Linq;
using System.Drawing;
using System.IO;
using LogicLayer;
using Globals;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace UnitTests
{
    public static class TestValues
    {
        private static Random random = new Random();

        public static string GetPw(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray());           
        }
    }



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
        public void TestIfMainFormConstructorTakesLogicInterfaceParameter()
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
        public void CheckMainFormhasNoFieldsOfTypeLogicImplementation()
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
            var OK = (fields.Where(f => f.FieldType == typeof(LogicImplementation)).Count() == 0);
            Assert.IsTrue(OK, $"Architectuur - \'MainForm\' uses a private instance of \'LogicImplementation\' (must use injected interface instead)!");
            fields = typeof(MainForm).GetFields();
            OK = (fields.Where(f => f.FieldType == typeof(LogicImplementation)).Count() == 0);
            Assert.IsTrue(OK, $"Architectuur - \'MainForm\' uses a public instance of \'LogicImplementation\' (must use injected interface instead)!");
            fields = typeof(MainForm).GetFields(BindingFlags.NonPublic |
                         BindingFlags.Instance);
        }

        [TestMethod, Timeout(500)]
        public void ILogicDeclaredOk()
        {
            Type x = GetTypeByName("ILogic");
            Assert.IsNotNull(x, $"Interface \'ILogic\' not declared!");
            CheckProperty(x, "ReportInterval", new Type[] { typeof(int) }, false, true);
            CheckMethod(x, "StartSearchCollisionsTask", new Type[] { typeof(void) }, new Type[] { typeof(Sha1Hash) });
            CheckMethod(x, "AbortSearch", new Type[] { typeof(void) }, new Type[] { });
            CheckEvent(x, "CollisionFound", new Type[] { typeof(Action<string>) });
            CheckEvent(x, "ProgressChanged", new Type[] { typeof(Action<ulong, int>) });
        }

        [TestMethod, Timeout(500)]
        public void TestLogicImplementsILogic()
        {
            var f = new LogicImplementation();
            Assert.IsTrue((f is ILogic), $"LogicImplementation does not implement \'ILogic\' interface.");
        }

        [TestMethod, Timeout(500)]
        public void TestLogicHasSearchCollisionsMethod()
        {
            var x = GetTypeByName("LogicImplementation");
            CheckMethod(x, "SearchCollisions", new Type[] { typeof(void) },
                             new Type[] { typeof(Sha1Hash),
                                          typeof(IProgress<ulong>),
                                          typeof(IProgress<string>),
                                          typeof(CancellationToken),
                                        });
        }

    }

    [TestClass]
    public class SearchCollisionsMethod
    {
       
        [TestMethod, Timeout(1500)]
        public void DetectsCollision()
        {
            string pw = TestValues.GetPw(2);
            string foundPw = string.Empty;            
            var logic = new LogicImplementation();
            var cancelSource = new CancellationTokenSource();
            var cancelToken = cancelSource.Token;
            var progress = new Progress<ulong>((count) => { });
            var collisionFound = new Progress<string>((result) => { foundPw = result; cancelSource.Cancel(); });
            var task = Task.Run(() => { logic.SearchCollisions(Sha1Hash.CalculateFromString(pw), progress, collisionFound, cancelToken); });
            var startTime = DateTime.Now;
            while ((foundPw == string.Empty) && ((DateTime.Now - startTime).TotalMilliseconds < 1000))Thread.Sleep(5);            
            Assert.IsFalse((foundPw == string.Empty), $"SearchCollisions - does not return a collision through its 'collisionFound' progress object.");
            Assert.IsTrue((pw == foundPw), $"SearchCollisions - returns wrong value for a collision: \'{foundPw}\', expected: \'{pw}\'.");
            cancelSource.Cancel();
        }

        [TestMethod, Timeout(1500)]
        public void ReportsProgress()
        {
            string pw = TestValues.GetPw(5);
            ulong pwCount = 0;
            var logic = new LogicImplementation();
            var cancelSource = new CancellationTokenSource();
            var cancelToken = cancelSource.Token;
            var progress = new Progress<ulong>((count) => { pwCount = count; });
            var collisionFound = new Progress<string>((result) => {});
            var task = Task.Run(() => { logic.SearchCollisions(Sha1Hash.CalculateFromString(pw), progress, collisionFound, cancelToken); });
            var startTime = DateTime.Now;
            while ((pwCount == 0) && ((DateTime.Now - startTime).TotalMilliseconds < 500)) Thread.Sleep(5);
            cancelSource.Cancel();
            Assert.IsFalse((pwCount == 0), $"SearchCollisions - does not report progress through its 'progress' object.");            
        }

        [TestMethod, Timeout(1500)]
        public void DefaultReportInterval()
        {
            int testInterval = 50;
            string pw = TestValues.GetPw(5);
            ulong pwCount = 0;
            var intervals = new List<int>();
            var previousTime = DateTime.Now;
            var logic = new LogicImplementation();
            var cancelSource = new CancellationTokenSource();
            var cancelToken = cancelSource.Token;
            var progress = new Progress<ulong>((count) =>
            {
                pwCount = count;
                DateTime t = DateTime.Now;
                intervals.Add((int)(t - previousTime).TotalMilliseconds);
                previousTime = t;
            });
            var collisionFound = new Progress<string>((result) => { });
            var task = Task.Run(() => { logic.SearchCollisions(Sha1Hash.CalculateFromString(pw), progress, collisionFound, cancelToken); });
            var startTime = DateTime.Now;
            while ((intervals.Count() < 4) && ((DateTime.Now - startTime).TotalMilliseconds < 500)) Thread.Sleep(5);
            Assert.IsFalse((intervals.Count() == 0), $"SearchCollisions - does not report progress through its 'progress' object.");
            cancelSource.Cancel();
            intervals.RemoveAt(0);
            int interval = (intervals[0] + intervals[1] + intervals[2]) / 3;
            Assert.IsTrue((Math.Abs(testInterval - interval) < 20),
                $"SearchCollisions - does not report progress at requested default interval of {testInterval}ms, was {interval}ms.");
        }

        [TestMethod, Timeout(1500)]
        public void HonorsReportInterval()
        {
            int testInterval = 20;
            string pw = TestValues.GetPw(5);
            ulong pwCount = 0;
            var intervals = new List<int>();
            var previousTime = DateTime.Now; 
            var logic = new LogicImplementation();
            var cancelSource = new CancellationTokenSource();
            var cancelToken = cancelSource.Token;
            var progress = new Progress<ulong>((count) =>
                                                        {
                                                            pwCount = count;
                                                            DateTime t = DateTime.Now;
                                                            intervals.Add((int)(t - previousTime).TotalMilliseconds);
                                                            previousTime = t;
                                                        });
            var collisionFound = new Progress<string>((result) => { });
            logic.ReportInterval = testInterval;
            var task = Task.Run(() => { logic.SearchCollisions(Sha1Hash.CalculateFromString(pw), progress, collisionFound, cancelToken); });
            var startTime = DateTime.Now;
            while ((intervals.Count()<4) && ((DateTime.Now - startTime).TotalMilliseconds < 500)) Thread.Sleep(5);
            Assert.IsFalse((intervals.Count() == 0), $"SearchCollisions - does not report progress through its 'progress' object.");
            cancelSource.Cancel();
            intervals.RemoveAt(0);
            int interval = (intervals[0] + intervals[1] + intervals[2]) / 3;
            Assert.IsTrue((Math.Abs(testInterval-interval)<20), 
                $"SearchCollisions - does not report progress at requested interval of {testInterval}ms, was {interval}ms.");
        }
   
        [TestMethod, Timeout(1500)]
        public void HonorsCancelRequest()
        {
            string pw = TestValues.GetPw(5);
            ulong pwCount = 0;
            var logic = new LogicImplementation();
            var cancelSource = new CancellationTokenSource();
            var cancelToken = cancelSource.Token;
            var progress = new Progress<ulong>((count) => { pwCount = count; });
            var collisionFound = new Progress<string>((result) => { });
            var task = Task.Run(() => { logic.SearchCollisions(Sha1Hash.CalculateFromString(pw), progress, collisionFound, cancelToken); });
            var startTime = DateTime.Now;
            while ((pwCount == 0) && ((DateTime.Now - startTime).TotalMilliseconds < 500)) Thread.Sleep(5);
            cancelSource.Cancel();
            var taskAborted = task.Wait(300);
            Assert.IsTrue(taskAborted, $"SearchCollisions - does not stop Task after 'Cancel' request.");
        }

    }

    [TestClass]
    public class LogicImplementationClass
    {

        [TestMethod, Timeout(1500)]
        public void DetectsCollision()
        {
            string pw = TestValues.GetPw(2);
            string foundPw = string.Empty;
            var logic = new LogicImplementation();
            logic.CollisionFound += (p) => { foundPw = p;};
            logic.ProgressChanged += (c, l) => { };
            try
            {
                logic.StartSearchCollisionsTask(Sha1Hash.CalculateFromString(pw));
                var startTime = DateTime.Now;
                while ((foundPw == string.Empty) && ((DateTime.Now - startTime).TotalMilliseconds < 1000)) Thread.Sleep(5);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Logic - \'StartSearchCollisionsTask\' method throws an exception: {ex.GetType().ToString()} - {ex.Message} ");
            }
            logic.AbortSearch();
            Assert.IsFalse((foundPw == string.Empty), $"Logic - does not return a collision through its 'CollisionFound' event.");
            Assert.IsTrue((pw == foundPw), $"Logic - returns wrong value for a collision: \'{foundPw}\', expected: \'{pw}\'.");
        }
              
        [TestMethod, Timeout(1500)]
        public void ReportsProgress()
        {
            string pw = TestValues.GetPw(5);
            ulong pwCount = 0;
            var logic = new LogicImplementation();
            logic.CollisionFound += (p) => {  };
            logic.ProgressChanged += (c, l) => { pwCount = c; };
            try
            {
                logic.StartSearchCollisionsTask(Sha1Hash.CalculateFromString(pw));
                var startTime = DateTime.Now;
                while ((pwCount == 0) && ((DateTime.Now - startTime).TotalMilliseconds < 500)) Thread.Sleep(5);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Logic - \'StartSearchCollisionsTask\' method throws an exception: {ex.GetType().ToString()} - {ex.Message} ");
            }
            logic.AbortSearch();
            Assert.IsFalse((pwCount == 0), $"Logic does not report # of passwords tested through its 'ProgressChanged' event.");           
        }

        [TestMethod, Timeout(1500)]
        public void ChecksForNullEvents()
        {
            string pw = TestValues.GetPw(5);
            var logic = new LogicImplementation();
            try
            {
                logic.StartSearchCollisionsTask(Sha1Hash.CalculateFromString(pw));
                var startTime = DateTime.Now;
                while ((DateTime.Now - startTime).TotalMilliseconds < 200) Thread.Sleep(5);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Logic - \'StartSearchCollisionsTask\' method throws an exception: {ex.GetType().ToString()} - {ex.Message} ");
            }
            logic.AbortSearch();
            
        }



        [TestMethod, Timeout(1500)]
        public void HonorsAbortRequest()
        {
            string pw = TestValues.GetPw(5);
            int eventCounter = 0;
            var logic = new LogicImplementation();
            logic.CollisionFound += (p) => { };
            logic.ProgressChanged += (c, l) => { eventCounter++; };
            var startTime = DateTime.Now;
            try
            {
                logic.StartSearchCollisionsTask(Sha1Hash.CalculateFromString(pw));
                startTime = DateTime.Now;
                while ((eventCounter == 0) && ((DateTime.Now - startTime).TotalMilliseconds < 500)) Thread.Sleep(5);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Logic - \'StartSearchCollisionsTask\' method throws an exception: {ex.GetType().ToString()} - {ex.Message} ");
            }
            logic.AbortSearch();
            eventCounter = 0;
            startTime = DateTime.Now;
            while ((eventCounter <= 1) && ((DateTime.Now - startTime).TotalMilliseconds < 200)) Thread.Sleep(5);
            Assert.IsTrue((eventCounter <= 1), $"Logic - AbortSearch does not stop background Task.");
        }

        [TestMethod, Timeout(1500)]
        public void CanStartSearchAfterAbort()
        {
            // start and abort a search.
            string pw = TestValues.GetPw(5);
            string foundPw = string.Empty;
            int eventCounter = 0;
            var logic = new LogicImplementation();
            logic.CollisionFound += (p) => { foundPw = p; };
            logic.ProgressChanged += (c, l) => { eventCounter++; };
            var startTime = DateTime.Now;
            try
            {
                logic.StartSearchCollisionsTask(Sha1Hash.CalculateFromString(pw));
                startTime = DateTime.Now;
                while ((eventCounter == 0) && ((DateTime.Now - startTime).TotalMilliseconds < 500)) Thread.Sleep(5);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Logic - \'StartSearchCollisionsTask\' method throws an exception: {ex.GetType().ToString()} - {ex.Message} ");
            }
            logic.AbortSearch();
            eventCounter = 0;
            startTime = DateTime.Now;
            while ((eventCounter <= 1) && ((DateTime.Now - startTime).TotalMilliseconds < 200)) Thread.Sleep(5);
            Assert.IsTrue((eventCounter <= 1), $"Logic - AbortSearch does not stop background Task.");
            // and start a second one
            pw = TestValues.GetPw(2);
            foundPw = string.Empty;
            try
            {
                logic.StartSearchCollisionsTask(Sha1Hash.CalculateFromString(pw));
                startTime = DateTime.Now;
                while ((foundPw == string.Empty) && ((DateTime.Now - startTime).TotalMilliseconds < 1000)) Thread.Sleep(5);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Logic - \'StartSearchCollisionsTask\' method throws an exception when started after an abort request: {ex.GetType().ToString()} - {ex.Message} ");
            }
            Assert.IsFalse((foundPw == string.Empty), $"Logic - does not return a collision when started after an abort request.");
            logic.AbortSearch();
        }

    }
}
