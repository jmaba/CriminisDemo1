using Emgu.CV;
using Emgu.CV.Structure;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Text;

namespace CoreProcessing
{
    public struct CCStatsOp
    {
        public Rectangle Rectangle;
        public int Area;
    }

    public static class UtilsMatrixString
    {
        public static string ToMatrixString<T>(this T[,] matrix, string delimiter = "\t")
        {
            var s = new StringBuilder();

            for (var i = 0; i < matrix.GetLength(0); i++)
            {
                for (var j = 0; j < matrix.GetLength(1); j++)
                {
                    s.Append(matrix[i, j]).Append(delimiter);
                }

                s.AppendLine();
            }

            return s.ToString();
        }
    } 

    public class ComponentCalculator
    {
        private readonly Emgu.CV.CvEnum.LineType lineType = Emgu.CV.CvEnum.LineType.EightConnected;

        public int GetMatchingDegree(byte[,] binarizedBlock)
        {
            byte[] depthPixelData = new byte[binarizedBlock.Length]; // your data
            Buffer.BlockCopy(binarizedBlock, 0, depthPixelData, 0, binarizedBlock.Length);
            Bitmap bitmap = CopyDataToBitmap(depthPixelData, binarizedBlock.GetLength(0), binarizedBlock.GetLength(1));
            Image<Gray, byte> depthImage = bitmap.ToImage<Gray, byte>(); 
            var labels = new Mat();
            var stats = new Mat();
            var centroids = new Mat();
            var nLabels = CvInvoke.ConnectedComponentsWithStats(depthImage, labels, stats, centroids, lineType);
            if(nLabels > 2)
            {
                CCStatsOp[] statsop;
                statsop = new CCStatsOp[nLabels];
                stats.CopyTo(statsop);
                var resul = binarizedBlock.ToMatrixString();
                int maximumArea = 0;
                for (int i = 1; i < nLabels; i++)
                {
                    if(statsop[i].Area > maximumArea)
                    {
                        maximumArea = statsop[i].Area;
                    }
                }

                return maximumArea;
            }
            return -1;
        }

        private Bitmap CopyDataToBitmap(byte[] data, int height, int width)
        {
            //Here create the Bitmap to the know height, width and format
            Bitmap bmp = new Bitmap(height, width, PixelFormat.Format8bppIndexed);

            //Create a BitmapData and Lock all pixels to be written 
            BitmapData bmpData = bmp.LockBits(
                                 new Rectangle(0, 0, bmp.Width, bmp.Height),
                                 ImageLockMode.WriteOnly, bmp.PixelFormat);

            //Copy the data from the byte array into BitmapData.Scan0
            Marshal.Copy(data, 0, bmpData.Scan0, data.Length);


            //Unlock the pixels
            bmp.UnlockBits(bmpData);


            //Return the bitmap 
            return bmp;
        }

    }
}
