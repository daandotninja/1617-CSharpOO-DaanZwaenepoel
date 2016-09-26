using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Opgave01
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            string path = @"Images\google.png";
            pictureBox1.Image = new Bitmap(Image.FromFile(path));

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }
    }
}
