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
            if (TerrainGraph.ContainsKey(name))
            {
                TerrainGraph.Remove(name);
                TerrainGraph.Add(name, neighbours);
            }
            else
            {
                foreach (var neighbour in neighbours)
                {
                    if (TerrainGraph.ContainsKey(neighbour))
                    {
                        TerrainGraph.Remove(neighbour);
                        TerrainGraph.Add(neighbour, new List<string>() { name });
                    }
                    else
                    {
                        TerrainGraph.Add(neighbour, new List<string>() { name });
                    }

                }
                TerrainGraph.Add(name, neighbours);

                

            }
           
       
        }

        public List<List<string>> SearchAllPaths(string start,string destinatioin) {
            var pathList = new List<List<string>>();
            var paths = new List<string>();
            if (start == destinatioin)
            {
                pathList.Add(new List<string>(){ start});
            }
            foreach (var neighbour in TerrainGraph[start])
            {
               if(neighbour == destinatioin)
                {
                    pathList.Add(new List<string>() { start , destinatioin });
                }

            }

            return pathList ;

        }
    }
}
