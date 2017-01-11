using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Globals
{
    public interface ILogic
    {
        void AbortCalculations();
        void StartCalculationTask(int maxCount);
        event Action<int> CalculationFinished;
        event Action<BigInteger> NumberFound;
        event Action<ulong> ProgressChanged;

    }
}
