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

namespace RollTheDiceMain
{
    public partial class MainForm : Form
    {
        private ILogic logic;

       
        public MainForm(ILogic logic)
        {
            InitializeComponent();
            this.logic = logic;
        }
    }
}
