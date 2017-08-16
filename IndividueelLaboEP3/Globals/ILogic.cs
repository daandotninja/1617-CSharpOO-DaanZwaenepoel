using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Globals
{
    public interface ILogic
    {
        Dictionary<int , long> Values { get; }
        void Calculate(CancellationToken token, IProgress<long> progress);
        void Start();
        void Stop();
        event Action<long> ValuesChanged;

    }
}
