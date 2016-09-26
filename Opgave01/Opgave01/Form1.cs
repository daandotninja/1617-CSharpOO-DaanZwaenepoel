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
            string path1 = @"Images\google.png";
            pictureBox1.Image = new Bitmap(Image.FromFile(path1));
            string path2 = @"Images\grens.png";
            pictureBox2.Image = new Bitmap(Image.FromFile(path2));

        }


        private void button1_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;
            trackBar1.Enabled = false;
            blend(false);

        }
        private void blend(bool xnor)
        {
            var image1 = new Bitmap(pictureBox1.Image);
            var image2 = new Bitmap(pictureBox2.Image);

            var betha = (byte)trackBar1.Value;

            int h = getHeight(image1.Height, image2.Height);
            int w = getWidth(image1.Width, image2.Width);

            Bitmap image3 = new Bitmap(w, h);

            for (int height = 0; height < h ; height++)
            {
                for (int width = 0; width < w; width++)
                {
                    var pixel1 = image1.GetPixel(width, height);
                    var pixel2 = image2.GetPixel(width, height);
                    int red = 0;
                    int green = 0;
                    int blue = 0;
                    if (xnor)
                    {
                        red = formulaXnor(pixel1.R, pixel2.R);
                        green = formulaXnor(pixel1.B, pixel2.G);
                        blue = formulaXnor(pixel1.B, pixel2.B);

                    }
                    else
                    {
                        red = formulaBlend(pixel1.R, pixel2.R, betha);
                        green = formulaBlend(pixel1.B, pixel2.G, betha);
                        blue = formulaBlend(pixel1.B, pixel2.B, betha);

                    }

                    Color newColor = Color.FromArgb(red, green, blue);

                    image3.SetPixel(width, height, newColor);

                }
            }

            pictureBox3.Image = image3;
            button1.Enabled = true;
            button2.Enabled = true;
            trackBar1.Enabled = true;




        }

        private int formulaXnor(byte rbg1, byte rbg2)
        {
            return (byte)~(rbg1 ^ rbg2);
        }

        private int formulaBlend(int rbg1, int rbg2 , byte betha)
        {
            
            return (betha * rbg1 + (100-betha)*rbg2)/100;
        }
  

        private int getWidth(int width1, int width2)
        {
            if (width1 < width2)
            {
                return width1;
            }
            else
            {
                return width2;
            }
        }

        private int getHeight(int height1, int height2)
        {
            if(height1 < height2)
            {
                return height1;
            }
            else
            {
                return height2;
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            button1.Enabled = false;
            button2.Enabled = false;
            trackBar1.Enabled = false;
            blend(true);
        }
    }
}
