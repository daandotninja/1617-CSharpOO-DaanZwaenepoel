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

namespace ShaCollisionsGUIMain
{
    public partial class MainForm : Form
    {
        private ILogic logic;

        public MainForm(ILogic logic)
        {
            
            InitializeComponent();
            this.logic = logic;
        }

       
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void calculate_Click(object sender, EventArgs e)
        {
            hash.Text += Sha1Hash.CalculateFromString(pass.Text);
            searchCollisions.Enabled = true;
            


        }

        private void searchCollisions_Click(object sender, EventArgs e)
        {
            logic.StartSearchCollisionsTask(Sha1Hash.CalculateFromString(pass.Text));
            logic.CollisionFound += CollisionFoundEventHandler;
            logic.ProgressChanged += ProgressEventHandler;
            calculate.Enabled = false;
            searchCollisions.Enabled = false;
            abort.Enabled = true;

        }

        private void ProgressEventHandler(ulong arg1, int arg2)
        {
            length.Text = arg2.ToString();
            count.Text = arg1.ToString();


        }

        private void CollisionFoundEventHandler(string obj)
        {
            collision.Text += obj;
        }

        private void abort_Click(object sender, EventArgs e)
        {
            logic.AbortSearch();
            abort.Enabled = false;
            calculate.Enabled = true;

        }
    }
}
