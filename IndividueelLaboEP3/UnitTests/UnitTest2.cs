using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Globals;
using LogicLayer;
using System.Collections.Generic;

namespace UnitTests
{
    [TestClass]
    public class DiceTests
    {

        [TestMethod, Timeout(500)]
        public void TestDiceReturnsValidNumbers()
        {
            IDice dice = new Dice();
            for(int i = 0; i < 100; i++)
            {
                int value = dice.SingleValue();
                Assert.IsTrue((value > 0) && (value < 7), $"\'Dice\' geeft foutieve waarde terug: {value}");
            }
        }


        [TestMethod, Timeout(500)]
        public void TestDiceReturnsAllNumbers()
        {
            IDice dice = new Dice();
            var numberList = new List<int>();
            for(int i = 0; i < 1000; i++)
            {
                int value = dice.SingleValue();
                if(!numberList.Contains(value)) numberList.Add(value);
            }
            Assert.IsTrue((numberList.Count == 6), $"\'Dice\' genereert niet alle mogelijke waarden!");
        }
    }
}
