using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opgave02
{
    public class Terrain
    {
        public Dictionary<string, List<string>> TerrainGraph { get; set; }

        public Terrain()
        {
            this.TerrainGraph = new Dictionary<string, List<string>>();

        }

        public void AddNode(string name,List<string> neighbours)
        {
            TerrainGraph.Add(name, neighbours);
            
        }

        public List<List<string>> SearchAllPaths(string start,string destinatioin) {
             
            return null ;

        }
    }
}
