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

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void startBtn_Click(object sender, EventArgs e)
        {
            logic.Start();
            logic.ValuesChanged += ValuesChangedEvent;
            startBtn.Enabled = false;
            stopBtn.Enabled = true;
        }

        private void ValuesChangedEvent(long result)
        {
            textBoxIterations.Text = result.ToString();
            int count = 1;
            valuesBox.BeginUpdate();
            foreach (KeyValuePair<int, long> kvp in logic.Values)
            {
                valuesBox.Items.Add(count + kvp.Key + '('+kvp.Value + ')');
               count ++;
            }
            valuesBox.EndUpdate();

        }

        private void stopBtn_Click(object sender, EventArgs e)
        {
            logic.Stop();
            startBtn.Enabled = true;
            stopBtn.Enabled = false;

        }
    }
}
