using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Globals;

namespace PerfectNumbersGuiMain
{
    public partial class MainForm : Form
    {
        private ILogic logic;

        public MainForm()
        {
            InitializeComponent();
        }

        public MainForm(ILogic logic)
        {
            this.logic = logic;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

    }
}
