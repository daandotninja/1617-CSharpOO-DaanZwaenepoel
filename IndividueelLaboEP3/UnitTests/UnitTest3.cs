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
    class TestDice:IDice
    {
        public long Count { get; private set; }

        private int value;

        public TestDice()
        {
            Count = 0;
            value = 1;
        }

        public int SingleValue()
        {
            Count++;
            value = value == 1 ? 2 : 1;
            return value;
        }
    }

    [TestClass]
    public class Logictests
    {
        [TestMethod, Timeout(1000)]
        public void UsesWorkerTask()
        {
            bool ready = false;
            var dice = new TestDice();
            var logic = new Logic(dice);
            logic.ValuesChanged += (percent) => { };
            
            var job = Task.Run(() =>
            {
                logic.Start();
                logic.Stop();
            });
            ready = job.Wait(300);
            Assert.IsTrue((ready), $"Logic - \'Start\' is blocking. No background worker task used?");
        }
        
        [TestMethod, Timeout(1500)]
        public async Task ReportsProgress()
        {
            long result = 0;
            int called = 0;
            var logic = new Logic(new TestDice());
            logic.ValuesChanged += (value) =>
            {
                result = value;
                called++;
            };
            logic.Start();
            var startTime = DateTime.Now;            
            await Task.Delay(500);
            Assert.IsTrue((called > 0), $"Logic does not call \'ValuesChanged\' event.");
            logic.Stop();
        }

        [TestMethod, Timeout(1500)]
        public async Task CheckEventInterval()
        {
            int called = 0;
            var logic = new Logic(new TestDice());
            logic.ValuesChanged += (value) =>
            {
                called++;
            };
            logic.Start();
            var startTime = DateTime.Now;
            await Task.Delay(500);
            Assert.IsTrue((called > 0), $"Logic does not call \'ValuesChanged\' event.");
            Assert.IsTrue((Math.Abs(called - 10) < 2), $"Logic does not call \'ProgressChanged\' event for every 50ms. Call count was {called} during an interval of 500ms.");
            logic.Stop();
        }


        [TestMethod, Timeout(1500)]
        public async Task ChecksForNullEvents()
        {
            int called = 0;
            bool exceptionThrown = false;
            Action<long> handler = (l) => { called++; };
            //  Exceptions in the 'IProgress' report Delegate are thrown in the UI context (not in the context od the worker thread) outside of the user code.
            //  They can only be caught using a FirstChance exceptionHandler
            System.EventHandler<System.Runtime.ExceptionServices.FirstChanceExceptionEventArgs> firstChanceHandler = (arg1,arg2) => { exceptionThrown = true; };
                        
            var logic = new Logic(new TestDice());
            logic.ValuesChanged += handler;
            logic.Start();
            await Task.Delay(200);
            Assert.IsTrue((called > 0), $"Logic does not call \'ValuesChanged\' event within 200msec after starting.");
            AppDomain.CurrentDomain.FirstChanceException += firstChanceHandler;
            logic.ValuesChanged -= handler;
            await Task.Delay(200);
            Assert.IsFalse(exceptionThrown, $"Logic does not check if there is a handler present for the 'ValuesChanged' event before calling it.");
            logic.Stop();
            await Task.Delay(50);
            AppDomain.CurrentDomain.FirstChanceException -= firstChanceHandler;
        }
        
        [TestMethod, Timeout(1500)]
        public async Task  HonorsAbortRequest()
        {
            int called = 0;
            var logic = new Logic(new TestDice());
            logic.ValuesChanged += (values) => { called++; };            
            logic.Start();
            var startTime = DateTime.Now;
            while((called == 0) && ((DateTime.Now - startTime).TotalMilliseconds < 500)) await Task.Delay(5);
            Assert.IsTrue((called > 0), $"Logic does not show any activity within 500 msec after starting calcullations.");
            startTime = DateTime.Now;
            logic.Stop();
            await Task.Delay(200);
            called = 0;
            startTime = DateTime.Now;
            while((called == 0) && ((DateTime.Now - startTime).TotalMilliseconds < 200)) await Task.Delay(5);
            Assert.IsTrue((called == 0), $"Logic - Stop does not stop background Task.");
        }

        [TestMethod, Timeout(1500)]
        public async Task CanStartSearchAfterAbort()
        {
            int called = 0;
            var logic = new Logic(new TestDice());
            logic.ValuesChanged += (n) => { called++; };
            logic.Start();
            var startTime = DateTime.Now;
            while((called == 0) && ((DateTime.Now - startTime).TotalMilliseconds < 500)) await Task.Delay(5);
            Assert.IsTrue((called > 0), $"Logic does not show any activity within 500 msec after starting calculations.");
            logic.Stop();
            await Task.Delay(50);
            called = 0;
            startTime = DateTime.Now;
            while((called == 0) && ((DateTime.Now - startTime).TotalMilliseconds < 200)) await Task.Delay(5);
            Assert.IsTrue((called == 0), $"Logic - \'Stop\' does not stop background Task.");

            // and start a second one
            called = 0;
            try
            {
                logic.Start();
                startTime = DateTime.Now;
                while((called == 0) && ((DateTime.Now - startTime).TotalMilliseconds < 500)) await Task.Delay(5);
            }
            catch(Exception ex)
            {
                Assert.Fail($"Logic - \'Start\' method throws an exception when started after a \'Stop\' request: {ex.GetType().ToString()} - {ex.Message} ");
            }
            Assert.IsFalse((called == 0), $"Logic - does not show any activity within 500 msec after restarting calcullations following  a \'Stop\' request.");
            logic.Stop();
        }


        // test functionality for Logic !!!!
         
        [TestMethod, Timeout(1500)]
        public async Task TestNumberOfSimulations()
        {
            long result = 0;
            var testDice = new TestDice();
            var logic = new Logic(testDice);
            logic.ValuesChanged += (value) => {   result = value; };
            logic.Start();
            await Task.Delay(100);
            logic.Stop();
            await Task.Delay(100);
            long count = testDice.Count/3;
            Assert.IsTrue((result == count), $"\'ValuesChanged\' does not report right number of simulations: reported {result}, was: {count}. ");           
        }

        [TestMethod, Timeout(1500)]
        public async Task TestSumOfValues()
        {
            long result = 0;
            var testDice = new TestDice();
            var logic = new Logic(testDice);
            logic.ValuesChanged += (value) => { result = value; };
            logic.Start();
            await Task.Delay(100);
            logic.Stop();
            await Task.Delay(100);
            long count = testDice.Count / 3;
            Assert.IsTrue((result == count), $"\'ValuesChanged\' does not report right number of simulations: reported {result}, was: {count}. ");
            count = logic.Values.Values.Sum();
            Assert.IsTrue((result == count), $"Sum of values ({count}) differs from total reported by \'ValuesChanged\'({result}). ");
        }

        // test if right values in Values
        [TestMethod, Timeout(1500)]
        public async Task TestIfValuesAreCorrect()
        {
            long result = 0;
            var testDice = new TestDice();
            var logic = new Logic(testDice);
            logic.ValuesChanged += (value) => { result = value; };
            logic.Start();
            await Task.Delay(100);
            logic.Stop();
            await Task.Delay(100);
            long count = testDice.Count / 3;
            Assert.IsTrue((result == count), $"\'ValuesChanged\' does not report right number of simulations: reported {result}, was: {count}. ");
            count = logic.Values.Values.Sum();
            Assert.IsTrue((result == count), $"Sum of values ({count}) differs from total reported by \'ValuesChanged\'({result}). ");
            bool wrongValue = false;
            foreach(var item in logic.Values)
            {
                switch(item.Key)
                {
                    case 4:
                    case 5:
                        {
                            wrongValue = Math.Abs(result - 2 * item.Value) > 1;
                            break;
                        }
                    default:
                        {
                            wrongValue = item.Value != 0;
                            break;
                        }
                }
                if(wrongValue) break;
            }
            Assert.IsFalse(wrongValue, $"Reported values are not correct for generated dice results (simulating 3 dice?).");
        }
           

    }
}
