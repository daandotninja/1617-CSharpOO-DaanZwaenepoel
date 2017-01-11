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
    public class LogicImplementationClass
    {

        [TestMethod, Timeout(1000)]
        public void UsesWorkerTask()
        {
                bool ready = false;
                int called = 0;
                var logic = new LogicImplementation();
                logic.ProgressChanged += (percent) => { };
                logic.NumberFound += (n) => { };
                logic.CalculationFinished += (c) => { };

                var job = Task.Run(() =>
                {
                    logic.StartCalculationTask(9);
                    logic.AbortCalculations();
                });
                ready = job.Wait(10);
                Assert.IsTrue((ready), $"Logic - \'StartCalculationTask\' is blocking. No background worker task used?");           
        }

        [TestMethod, Timeout(1500)]
        public void ReturnsCorrectNumbers()
        {
            var numbers = new BlockingCollection<BigInteger>();
            var logic = new LogicImplementation();
            logic.ProgressChanged += (percent) => { };
            logic.NumberFound += (number) => { numbers.Add(number); };
            logic.CalculationFinished += (c) => { };
            logic.StartCalculationTask(4);
            Thread.Sleep(150);
            var startTime = DateTime.Now;
            while ((numbers.Count < 4) && ((DateTime.Now - startTime).TotalMilliseconds < 1500)) Thread.Sleep(1);
            var numberList = numbers.ToList();
            Assert.IsTrue((numbers.Count() == 4), $"Logic returns wrong amount of numbers with a limit of 4: was: {numbers.Count()}.");
            Assert.IsTrue(((numberList[0] + numberList[1] + numberList[2] + numberList[3]) % 37 == 0), $"Logic returns wrong values for first 4 numbers.");
            logic.AbortCalculations();
        }

        [TestMethod, Timeout(1500)]
        public void ReportsFinished()
        {
            int called = 0;
            var logic = new LogicImplementation();
            logic.ProgressChanged += (percent) => { };
            logic.NumberFound += (number) => { };
            logic.CalculationFinished += (c) => { called++; };
            logic.StartCalculationTask(4);
            Thread.Sleep(200);
            Assert.IsTrue((called > 0), $"Logic does not call \'CalculationFinished\' event within 200 msec with a number limit of 4.");
            Assert.IsTrue((called == 1), $"Logic calls \'CalculationFinished\' event {called} times (expected only one call at the end).");
            logic.AbortCalculations();
        }

        [TestMethod, Timeout(1500)]
        public void ReturnsCorrectControlNumber()
        {
            int called = 0;
            int result = 0;
            var logic = new LogicImplementation();
            logic.ProgressChanged += (percent) => { };
            logic.NumberFound += (number) => { };
            logic.CalculationFinished += (c) => { called++; result = c; };
            logic.StartCalculationTask(4);
            Thread.Sleep(200);
            Assert.IsTrue((called > 0), $"Logic does not call \'CalculationFinished\' event within 1 second with a number limit of 2000.");
            Assert.IsTrue((result == 44), $"Logic returns wrong control value through \'finished\' progress. With a limit of 5 value was {result}, expected 1.");
            logic.AbortCalculations();
        }

        [TestMethod, Timeout(1500)]
        public void ReportsProgress()
        {
            var results = new BlockingCollection<ulong>();
            int called = 0;
            var logic = new LogicImplementation();
            logic.ProgressChanged += (number) =>
            {
                results.Add(number);
                called++;
            };
            logic.NumberFound += (n) => { };
            logic.CalculationFinished += (c) => { };
            logic.StartCalculationTask(5);
            var startTime = DateTime.Now;
            while ((called < 13) && ((DateTime.Now - startTime).TotalMilliseconds < 1500)) Thread.Sleep(1);
            var result = results.ToList();
            result.Sort();
            Assert.IsTrue((called > 0), $"Logic does not call \'ProgressChanged\' event.");
            Assert.IsTrue((result[4] == 31), $"Logic does not report correct Mersene numbers.");
            Assert.IsTrue((called == 13), $"Logic does not report progress for every checked Mersene number. Report count for 5 perfect numbers was {called} (expected 12).");
            logic.AbortCalculations();
        }

        [TestMethod, Timeout(1500)]
        public void ChecksForNullEvents()
        {
            var logic = new LogicImplementation();
            try
            {
                logic.StartCalculationTask(5);
                var startTime = DateTime.Now;
                while ((DateTime.Now - startTime).TotalMilliseconds < 200) Thread.Sleep(5);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Logic - \'StartCalculationTask\' method throws an exception: {ex.GetType().ToString()} - {ex.Message} ");
            }
            logic.AbortCalculations();

        }



        [TestMethod, Timeout(2000)]
        public void HonorsAbortRequest()
        {
            var ThreadDiffs = new List<int>();
            int called = 0;
            var logic = new LogicImplementation();
            logic.ProgressChanged += (percent) => { called++; };
            logic.NumberFound += (n) => { called++; };
            logic.CalculationFinished += (c) => { };
            for (int i = 0; i < 10; i++)
            {
                logic.StartCalculationTask(8);
                var startTime = DateTime.Now;
                while ((called == 0) && ((DateTime.Now - startTime).TotalMilliseconds < 500)) Thread.Sleep(5);
                Assert.IsTrue((called > 0), $"Logic does not show any activity within 500 msec after starting calcullations.");
                int originalAvailableThreads = 0;
                int dummy = 0;
                ThreadPool.GetAvailableThreads(out originalAvailableThreads, out dummy);
                logic.AbortCalculations();
                startTime = DateTime.Now;
                int AvailableThreads = 0;
                while (!(AvailableThreads > originalAvailableThreads) && ((DateTime.Now - startTime).TotalMilliseconds < 50))
                {
                    Thread.Sleep(5);
                    ThreadPool.GetAvailableThreads(out AvailableThreads, out dummy);
                }
                ThreadDiffs.Add(AvailableThreads - originalAvailableThreads);
            }
            int OkCount = ThreadDiffs.Where((i) => i > 0).Count();
            Assert.IsTrue((OkCount > 8), $"Logic - AbortSearch does not stop background Task.");
        }

        [TestMethod, Timeout(1500)]
        public void CanStartSearchAfterAbort()
        {
            int called = 0;
            var logic = new LogicImplementation();
            logic.ProgressChanged += (percent) => { called++; };
            logic.NumberFound += (n) => { called++; };
            logic.CalculationFinished += (c) => { };
            logic.StartCalculationTask(9);
            var startTime = DateTime.Now;
            while ((called == 0) && ((DateTime.Now - startTime).TotalMilliseconds < 500)) Thread.Sleep(5);
            Assert.IsTrue((called > 0), $"Logic does not show any activity within 500 msec after starting calcullations.");
            logic.AbortCalculations();
            startTime = DateTime.Now;
            while ((DateTime.Now - startTime).TotalMilliseconds < 100) Thread.Sleep(5);
            called = 0;
            startTime = DateTime.Now;
            while ((called == 0) && ((DateTime.Now - startTime).TotalMilliseconds < 200)) Thread.Sleep(5);
            Assert.IsTrue((called == 0), $"Logic - AbortSearch does not stop background Task.");
            // and start a second one
            called = 0;
            try
            {
                logic.StartCalculationTask(5);
                startTime = DateTime.Now;
                while ((called == 0) && ((DateTime.Now - startTime).TotalMilliseconds < 500)) Thread.Sleep(5);
            }
            catch (Exception ex)
            {
                Assert.Fail($"Logic - \'StartCalculationTask\' method throws an exception when started after an abort request: {ex.GetType().ToString()} - {ex.Message} ");
            }
            Assert.IsFalse((called == 0), $"Logic - does not show any activity within 500 msec after restarting calcullations following an abort request.");
            logic.AbortCalculations();
        }

    }
}
