using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Globals;
using Newtonsoft.Json;
using System.IO;

namespace DataAccessLayer
{
    public class DataAccessImplementation: IDataAccess
    {
        private const string filename = "RunInfo.json";
        public Dictionary<DateTime,RunInfo> storeInfo = new Dictionary<DateTime,RunInfo>();
        public DataAccessImplementation()
        {
            string json = File.ReadAllText(filename);
            Dictionary<DateTime, RunInfo> files = JsonConvert.DeserializeObject<Dictionary<DateTime, RunInfo>>(json);
            foreach (var f in files)
            {
                StoreRunInfo(f.Key, f.Value);
            }

        }
        public void StoreRunInfo(DateTime date , RunInfo info)
        {
            if (storeInfo.ContainsKey(date))
            {
                storeInfo.Remove(date);
         
            }
            storeInfo.Add(date, info);

        }
        public RunInfo GetRunInfoForDate(DateTime date) {
            if (storeInfo.ContainsKey(date))
            {
                return storeInfo[date];
            }
            else
            {
               return new RunInfo(0, new TimeSpan(0));

            }

        }
        public void Close()
        {
            var formatter = new JsonSerializer();
            using (var file = new StreamWriter(filename))
            {
                formatter.Serialize(file, storeInfo);
                file.Flush();
            }
        }
        public void ClearData()
        {
            storeInfo.Clear();


        }
    }
}
