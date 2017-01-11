using Globals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace LogicLayer
{
    public class LogicImplementation : ILogic,ITestable
    {
        public event Action<int> CalculationFinished;
        public event Action<BigInteger> NumberFound ;
        public event Action<ulong> ProgressChanged;

        public void AbortCalculations()
        {
           
        }

        public bool IsPrime(ulong number, CancellationToken cancelToken)
        {
            bool stop = true;
            ulong deler = number+2;
            bool state = false;
            if (number == 1)
            {
                return false;
            }
            if (number == 2)
            {
                return true;
            }
            if (number % 2 == 0)
            {
                return false;
            }
            while(stop != false)
            {
                
                if(number >= deler){
                    state = true;
                    stop = false;
                    break;
                }
                else if( number % deler == 0)
                {
                    state = false;
                    stop = false;
                    break;
                }
                if (cancelToken.IsCancellationRequested)
                {
                    state = false;
                    stop = false;
                    break;
                }
                deler = deler + 2;
            
                
            }
            return state;
        }

        public void SearchNumbers(int maxCount, IProgress<ulong> progressChanged, IProgress<BigInteger> numberFound, IProgress<int> calculationFinished, CancellationToken cancelToken)
        {
          int count = 0;
          ulong number = 0;
          while (!cancelToken.IsCancellationRequested)
            {
               if(count != maxCount)
                {
                    AbortCalculations();
                }
            }
        }

        public void StartCalculationTask(int maxCount)
        {
            throw new NotImplementedException();
        }
    }
}
