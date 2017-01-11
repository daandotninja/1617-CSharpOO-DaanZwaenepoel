using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Globals
{
    public interface ITestable
    {
        bool IsPrime(ulong number, CancellationToken cancelToken);
        void SearchNumbers(int maxCount, IProgress<ulong> progressChanged, IProgress<BigInteger> numberFound, IProgress<int> calculationFinished, CancellationToken cancelToken);
    }
}
