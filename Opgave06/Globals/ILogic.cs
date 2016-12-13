using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals
{
    public interface ILogic
    {
        int ReportInterval { set; }
        void AbortSearch();
        void StartSearchCollisionsTask(Sha1Hash hash);

        event Action<string> CollisionFound;
        event Action<ulong, int> ProgressChanged;
    }
}
