using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Globals;
namespace LogicLayer
{
    public class Logic : ILogic
    {
        private IDice dice;

        public Logic(IDice dice)
        {
            this.dice = dice;
        }

        public Dictionary<int, long> Values
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public event Action<long> ValuesChanged;

        public void Calculate(CancellationToken token, IProgress<long> progress)
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
        }
    }
}
