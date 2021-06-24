using CoreProcessing;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DemoCriminisiDetection
{
    public partial class Form1 : Form
    {
        private BlockCalculator blockCalculator;
        private List<(int startingX, int startingY)> itemROS;
        private int i = 0;
        private string imageFIlename = @"C:\Source Code\Criminisi\Output1.png";
        public Form1()
        {
            InitializeComponent();
            pictureBox1.Load(imageFIlename);
            var image = ImageLoader.LoadImage(imageFIlename);
            blockCalculator = new BlockCalculator(image, new Rectangle(130, 80, 10, 10), new MatrixSize(9, 9));
            itemROS = blockCalculator.GetAllBlocksInsideROS().ToList(); ;
            timer1.Enabled = true;
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            var image = ImageLoader.LoadImage(imageFIlename);
            var detector = new CriminisiDetector(image, new Rectangle(130, 80, 10, 10), new MatrixSize(9, 9));
            var result = detector.ComputeDetectionArea(Color.Red);
            var stringResult = result.ToMatrixString();
            Console.WriteLine(stringResult);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            result = false;
            pictureBox1.Load(imageFIlename);
        }

        private bool result = false;
        private void button2_Click(object sender, EventArgs e)
        {
            i++;
            if (i < itemROS.Count)
            {
                result = true;
                pictureBox1.Refresh();
            }
            else
            {
                i = 0;
                result = false;
            }
            
        }

        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if(result)
            {
                Rectangle ee = new Rectangle(itemROS[i].startingY, itemROS[i].startingX, 9, 9);
                using (Pen pen = new Pen(Color.Red, 1))
                {
                    e.Graphics.DrawRectangle(pen, ee);
                }

            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            //
            button2_Click(sender, e);
        }
    }
}
