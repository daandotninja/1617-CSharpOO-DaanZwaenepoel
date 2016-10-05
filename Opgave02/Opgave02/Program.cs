using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Opgave02
{
    class Program
    {
        static void Main(string[] args)
        {
            Terrain terrain = new Terrain();
            ConsoleUI ui = new ConsoleUI(terrain);
            ui.ShowStartBanner();
            ui.ReadGraph(@"resources\Terrain.txt");
            ui.ProcessQueries();
            ui.ShowEndBanner();
        }
    }
}
