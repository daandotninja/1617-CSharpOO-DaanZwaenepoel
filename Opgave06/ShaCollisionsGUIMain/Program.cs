using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Globals;
using LogicLayer;

namespace ShaCollisionsGUIMain
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            ILogic mainLogic = new LogicImplementation();
            Application.Run(new MainForm(mainLogic));
        }
    }
}
