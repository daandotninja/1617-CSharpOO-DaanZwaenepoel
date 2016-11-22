using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Globals;
using DataAccessLayer;
using LogicLayer;

namespace LadderCompetitionMain
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // new DataAccessImplementation().CreateBaseRanking();

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm(new LogicImplementation(new DataAccessImplementation())));
        }
    }
}
