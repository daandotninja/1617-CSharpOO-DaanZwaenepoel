using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Globals;
namespace LogicLayer
{
    public class Dice : IDice
    {
        private Random rand;
        public Dice()
        {
           rand = new Random();

        }
        public int SingleValue()
        {
           
            return rand.Next(6);
        }
    }
}
