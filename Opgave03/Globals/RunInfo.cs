using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Globals
{
    [Serializable()]
    public class RunInfo
    {
        public RunInfo(int DistanceInMeter, TimeSpan interval)
        {
            if(DistanceInMeter >= 0)
            {
                this.DistanceInMeter = DistanceInMeter;
            }
            else
            {
                throw new ArgumentOutOfRangeException();

            }
          

            this.Interval = interval;
        }

        public int DistanceInMeter { get; }
        public TimeSpan Interval { get; }
    }
}
