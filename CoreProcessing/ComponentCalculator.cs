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

        public static Bitmap CopyDataToBitmap(byte[,] data, int height, int width)
        {
            //Here create the Bitmap to the know height, width and format
            Bitmap bmp = new Bitmap(height, width);

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    bmp.SetPixel(i, j, data[i, j] == 0 ? Color.Black : Color.White);
                }
            }


            //Return the bitmap 
            return bmp;
        }
    }

    public class ComponentCalculator
    {
        private readonly Emgu.CV.CvEnum.LineType lineType = Emgu.CV.CvEnum.LineType.EightConnected;

        public int GetMatchingDegree(byte[,] binarizedBlock)
        {
            var resulx = binarizedBlock.ToMatrixString();

            byte[] depthPixelData = new byte[binarizedBlock.Length]; // your data
            Buffer.BlockCopy(binarizedBlock, 0, depthPixelData, 0, binarizedBlock.Length);
            Bitmap bitmap = UtilsMatrixString.CopyDataToBitmap(binarizedBlock, binarizedBlock.GetLength(0), binarizedBlock.GetLength(1)); // CopyDataToBitmap(depthPixelData, binarizedBlock.GetLength(0), binarizedBlock.GetLength(1));
            Image<Gray, byte> depthImage = bitmap.ToImage<Gray, byte>();
            var labels = new Mat();
            var stats = new Mat();
            var centroids = new Mat();
            var nLabels = CvInvoke.ConnectedComponentsWithStats(depthImage, labels, stats, centroids, lineType);
            if (nLabels > 1)
            {
                CCStatsOp[] statsop;
                statsop = new CCStatsOp[nLabels];
                stats.CopyTo(statsop);
                var resul = binarizedBlock.ToMatrixString();
                int maximumArea = 0;
                for (int i = 1; i < nLabels; i++)
                {
                    if (statsop[i].Area > maximumArea)
                    {
                        maximumArea = statsop[i].Area;
                    }
                }

                return maximumArea;
            }
            return -1;
        }




    }
}
