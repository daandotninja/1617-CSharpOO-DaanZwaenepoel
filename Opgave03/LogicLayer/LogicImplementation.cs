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
        public Dictionary<DateTime, RunInfo> storeInfo = new Dictionary<DateTime, RunInfo>();
        public LogicImplementation(IDataAccess backend)
        {
            dataAccessLayer = backend;
            CurrentDate = DateTime.Now.Date;
        }
        public void ClearData()
        {
            dataAccessLayer.ClearData();
        }
        public void Close()
        {
            dataAccessLayer.Close();
        }
        public RunInfo GetRunInfoForDate(DateTime date)
        {
            return dataAccessLayer.GetRunInfoForDate(date);
        }
        public RunInfo[] GetRunInfoForPastSevenDays()
        {
            RunInfo[] days = new RunInfo[7];
            for (int i = 0; i < 6 ; i++)
            {
                

                days[i]= GetRunInfoForDate(CurrentDate.AddDays(-6 + i));

            }
            days[6] = GetRunInfoForDate(CurrentDate);
            return days;
        }
        public RunInfo[] GetRunInfoForPastThirtyDays()
        {
            RunInfo[] days = new RunInfo[30];
            for (int i = 0; i < 29; i++)
            {


                days[i] = GetRunInfoForDate(CurrentDate.AddDays(-29 + i));

            }
            days[29] = GetRunInfoForDate(CurrentDate);
            return days;
        }
        public double GetSpeedInKmPerHour(RunInfo info)
        {
            return ((double)info.DistanceInMeter / (double)((info.Interval.Hours*3600)+(info.Interval.Minutes*60)+info.Interval.Seconds))*3.6;
        }
        public void SetRunInfoForDate(DateTime date, RunInfo info)
        {
            if (info.DistanceInMeter >= 0)
            {
                if(GetSpeedInKmPerHour(info) >= 50.0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                else
                {
                     
                    dataAccessLayer.StoreRunInfo(date, info);
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException();

            }
        }
    }
}
