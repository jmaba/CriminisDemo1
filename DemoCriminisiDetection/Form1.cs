using CoreProcessing;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace DemoCriminisiDetection
{
    public partial class Form1 : Form
    {
        private BlockCalculator blockCalculator;
        //private string imageFIlename = @"C:\Source Code\Criminisi\Output1.png";
        private string imageFIlename = @"C:\Users\Adi\Downloads\Image-Inpainting-master\images\house3.bmp";
        public Form1()
        {
            InitializeComponent();
            pictureBox1.Load(imageFIlename);
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            var image = ImageLoader.LoadImage(imageFIlename);
            ImageLoader.SaveImage("demo.bmp", image);
            var detector = new CriminisiDetector(image, new Rectangle(140, 121, 60, 60), new MatrixSize(9, 9));
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            detector.ComputeDetectionArea();

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;

            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);

            MessageBox.Show($"done {elapsedTime} ");
        }
    }
}
