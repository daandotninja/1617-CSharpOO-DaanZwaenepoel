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

namespace RunnerMain
{
    public partial class MainForm : Form
    {
        private readonly ILogic logic;
        public MainForm(ILogic logic)
        {
            InitializeComponent();
            this.logic = logic;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }
    }
}
