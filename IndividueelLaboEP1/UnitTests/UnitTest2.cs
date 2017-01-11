using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Globals;
using System.Reflection;
using System.Linq;
using System.Drawing;
using System.IO;
using LogicLayer;
using Globals;
using PerfectNumbersGuiMain;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Numerics;
using System.Collections.Concurrent;

namespace UnitTests
{
   
    [TestClass]
    public class LogicMethods
    {

        [TestMethod, Timeout(500)]
        public void TestIsPrimeReturnsCorrectResults()
        {
            var parameters = new ulong[] { 1, 2,17, 284 };
            var results = new bool[] { false, true, true, false,  };
            var logic = new LogicImplementation();
            CancellationTokenSource source = new CancellationTokenSource();
            for (int i = 0; i < parameters.Length; i++)
            {
                var result = logic.IsPrime(parameters[i], source.Token);
                Assert.IsTrue((result == results[i]),
                    $"Logic - \'IsPrime\' returns wrong result for {parameters[i]}: {result}, expected {results[i]}");
            }
        }

        [TestMethod, Timeout(1500)]
        public void TestIfIsPrimeHonoursCancellationRequest()
        {
            var logic = new LogicImplementation();
            CancellationTokenSource source = new CancellationTokenSource();
            source.Cancel();
            var startTime = DateTime.Now;
            var result = logic.IsPrime(524287, source.Token);
            var interval = DateTime.Now - startTime;
            Assert.IsTrue((interval.TotalMilliseconds <= 50), $"Logic - \'IsPrime\' does not honour cancel request.");
            Assert.IsFalse((result), $"Logic - \'IsPrime\' returns \'true\' upon cancellation (should be \'false\').");

        }

        [TestMethod, Timeout(1500)]
        public void SearchNumbersReturnsCorrectResults()
        {
            var numbers = new BlockingCollection<BigInteger>();
            ITestable logic = new LogicImplementation();
            var cancelSource = new CancellationTokenSource();
            var cancelToken = cancelSource.Token;
            var progress = new Progress<ulong>((count) => { });
            var numberFound = new Progress<BigInteger>((result) => { numbers.Add(result); });
            var finished = new Progress<int>((control) => { });
            var task = Task.Run(() => { logic.SearchNumbers(4, progress, numberFound, finished, cancelToken); });
            var ready = task.Wait(1000);
            var startTime = DateTime.Now;
            while ((numbers.Count < 4) && ((DateTime.Now - startTime).TotalMilliseconds < 1500)) Thread.Sleep(1);
            var numberList = numbers.ToList();            
            Assert.IsTrue((ready), $"SearchNumbers - does not finish within 1 second with a limit of 4 numbers.");
            Assert.IsTrue((numbers.Count() == 4), $"SearchNumbers - returns wrong amount of numbers with a limit of 4: was: {numbers.Count()}.");
            Assert.IsTrue(((numberList[0]+ numberList[1]+ numberList[2]+ numberList[3]) % 37 == 0), $"SearchNumbers - returns wrong values for first 4 numbers.");
            cancelSource.Cancel();
        }

        [TestMethod, Timeout(1500)]
        public void SearchNumbersReportsFinished()
        {
            int called = 0;
            var logic = new LogicImplementation();
            var cancelSource = new CancellationTokenSource();
            var cancelToken = cancelSource.Token;
            var progress = new Progress<ulong>((count) => { });
            var numberFound = new Progress<BigInteger>((number) => { });
            var finished = new Progress<int>((control) => { called++; });
            var task = Task.Run(() => { logic.SearchNumbers(4, progress, numberFound, finished, cancelToken); });
            var ready = task.Wait(500);
            var startTime = DateTime.Now;
            while ((called == 0) && ((DateTime.Now - startTime).TotalMilliseconds < 200)) Thread.Sleep(5);

            Assert.IsTrue((ready), $"SearchNumbers - does not finish within 1 second with a limit of 2000.");
            Assert.IsTrue((called > 0), $"SearchNumbers - does not use \'finished\' progress.");
            Assert.IsTrue((called == 1), $"SearchNumbers - uses \'finished\' progress {called} times (expected only one call at the end).");
            cancelSource.Cancel();
        }

        [TestMethod, Timeout(1500)]
        public void SearchNumbersReturnsCorrectControlNumber()
        {
            int result = 0;
            int called = 0;
            var logic = new LogicImplementation();
            var cancelSource = new CancellationTokenSource();
            var cancelToken = cancelSource.Token;
            var progress = new Progress<ulong>((count) => { });
            var numberFound = new Progress<BigInteger>((number) => { });
            var finished = new Progress<int>((control) =>
            {
                result = control;
                called++;
            });
            var task = Task.Run(() => { logic.SearchNumbers(4, progress, numberFound, finished, cancelToken); });
            var ready = task.Wait(500);
            var startTime = DateTime.Now;
            while ((called == 0) && ((DateTime.Now - startTime).TotalMilliseconds < 200)) Thread.Sleep(5);

            Assert.IsTrue((ready), $"SearchNumbers - does not finish within 1 second with a limit of 2000.");
            Assert.IsTrue((called > 0), $"SearchNumbers - does not use \'finished\' progress.");
            Assert.IsTrue((result == 44), $"SearchNumbers - returns wrong control value through \'finished\' progress. With a limit of 4 value was {result}, expected 44.");
            cancelSource.Cancel();
        }

        [TestMethod, Timeout(2000)]
        public void SearchNumbersReportsProgress()
        {
            var results = new BlockingCollection<ulong>();
            int called = 0;
            var logic = new LogicImplementation();
            var cancelSource = new CancellationTokenSource();
            var cancelToken = cancelSource.Token;
            var progress = new Progress<ulong>((number) =>
            {
                results.Add(number);
                called++;
            });
            var numberFound = new Progress<BigInteger>((number) => { });
            var finished = new Progress<int>((control) => { });
            var task = Task.Run(() => { logic.SearchNumbers(5, progress, numberFound, finished, cancelToken); });
            var ready = task.Wait(500);
            var startTime = DateTime.Now;
            while ((called < 13) && ((DateTime.Now - startTime).TotalMilliseconds < 1500)) Thread.Sleep(1);
            var result = results.ToList();
            result.Sort();
            Assert.IsTrue((ready), $"SearchNumbers - does not finish within 1 second with a limit of 2000.");
            Assert.IsTrue((called > 0), $"SearchNumbers - does not report progress.");
            Assert.IsTrue((result[4] == 31), $"SearchNumbers - does not report correct Mersene numbers.");
            Assert.IsTrue((called == 13), $"SearchNumbers - does not report progress for every checked Mersene number. Report count for 5 perfect numbers was {called} (expected 13).");
            cancelSource.Cancel();
        }

        [TestMethod, Timeout(2000)]
        public void SearchNumbersHonoursCancelRequest()
        {
            int called = 0;
            var logic = new LogicImplementation();
            var cancelSource = new CancellationTokenSource();
            var cancelToken = cancelSource.Token;
            var progress = new Progress<ulong>((count) => { called++; });
            var numberFound = new Progress<BigInteger>((number) => { called++; });
            var finished = new Progress<int>((control) => { });

            var task = Task.Run(() => { logic.SearchNumbers(9, progress, numberFound, finished, cancelToken); });
            var startTime = DateTime.Now;
            while ((called == 0) && ((DateTime.Now - startTime).TotalMilliseconds < 500)) Thread.Sleep(5);
            Assert.IsTrue((called > 0), $"SearchNumbers - does not show any activity within 500 msec.");
            cancelSource.Cancel();
            var taskAborted = task.Wait(300);
            Assert.IsTrue(taskAborted, $"SearchNumbers - does not stop task after 'Cancel' request.");
        }

    }
       
}
