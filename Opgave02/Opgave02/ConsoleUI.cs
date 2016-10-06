using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opgave02
{
    public class ConsoleUI
    {
        private Terrain terrain;

        public ConsoleUI(Terrain terrain)
        {
            this.terrain = terrain;
            
        }
        public void ProcessQueries()
        {

        }
        public void ReadGraph(string filePath)
        {
            var lines = System.IO.File.ReadAllLines(filePath);
            for (int i = 0; i < lines.Length; i++)
            {
                Console.WriteLine(lines[i]);
                if (lines[i]=="")
                {
                    
                    continue;
                }
                else
                {
                    var neig = lines[i].Substring(1, lines[i].Length - 1);
                    var neighbours = new List<string>();
                    char[] delimiterChars = { ',', '-', ' ' };
                    var neighboursNode = neig.Split(delimiterChars);
                    for (int y = 1; y < neighboursNode.Length; y++)
                    {
                        neighbours.Add(neighboursNode[y]);

                    }
                    string name = (lines[i][0]).ToString();
                    terrain.AddNode(name, neighbours);
                }

                
            }
          
            

        }
        public void ShowEndBanner()
        {
            
            Console.ReadLine();
        }
        public void ShowStartBanner()
        {

        }


    }
}
