using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals
{
    public interface IDataAccess
    {
        void ClearData();
        void Close();
        RunInfo GetRunInfoForDate(DateTime date);
        void StoreRunInfo(DateTime date, RunInfo info);
    }
}
