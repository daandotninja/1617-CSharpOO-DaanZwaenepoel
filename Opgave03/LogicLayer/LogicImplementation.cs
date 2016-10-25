using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Globals;

namespace LogicLayer
{
    public class LogicImplementation: ILogic
    {
        private readonly IDataAccess dataAccessLayer;
        public DateTime CurrentDate { get; }
        public LogicImplementation(IDataAccess backend)
        {
            dataAccessLayer = backend;
        }
        public void ClearData()
        {

        }
        public void Close()
        {

        }
        public RunInfo GetRunInfoForDate(DateTime date)
        {
            return null;
        }
        public RunInfo[] GetRunInfoForPastSevenDays()
        {
            return null;
        }
        public RunInfo[] GetRunInfoForPastThirtyDays()
        {
            return null;
        }
        public double GetSpeedInKmPerHour(RunInfo info)
        {
            return 0.0;
        }
        public void SetRunInfoForDate(DateTime date, RunInfo info)
        {

        }
    }
}
