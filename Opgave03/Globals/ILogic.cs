using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals
{
    public interface ILogic
    {
        DateTime CurrentDate { get;}
        void ClearData();
        void Close();
        RunInfo GetRunInfoForDate(DateTime date);
        RunInfo[] GetRunInfoForPastSevenDays();
        RunInfo[] GetRunInfoForPastThirtyDays();
        double GetSpeedInKmPerHour(RunInfo info);
        void SetRunInfoForDate(DateTime date, RunInfo info);

    }
}
