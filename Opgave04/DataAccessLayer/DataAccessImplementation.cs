using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Globals;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace DataAccessLayer
{
    public class DataAccessImplementation : IDataAccess
    {
       

   
        public void LogMatch(Match match)
        {
           
        }

        public Player[] ReadRanking()
        {
            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream("resources/Ranking.dat", FileMode.Open, FileAccess.Read, FileShare.Read);
            object players = (object)formatter.Deserialize(stream);
            return players as Player[];
        }

        public void SaveRanking(Player[] players)
        {
            FileStream fs = new FileStream("resources/Ranking.dat", FileMode.Create);
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(fs,players);
            fs.Close();
        }
    }
}
