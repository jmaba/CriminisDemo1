using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using System;

namespace CoreProcessing
{
    public class ImageLoader
    {
        public static ImageStructure LoadImage(string filename)
        {
            Mat mat = CvInvoke.Imread(filename, ImreadModes.AnyColor);
            Image<Bgr, Byte> img = mat.ToImage<Bgr, Byte>();
            int width = img.Width;
            int height = img.Height;
            ImageStructure imageStructure = new ImageStructure()
            {
                matR = new byte[height, width],
                matG = new byte[height, width],
                matB = new byte[height, width]
            };

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    var color = img[i, j];
                    imageStructure.matR[i, j] = (byte)color.Red;
                    imageStructure.matG[i, j] = (byte)color.Green;
                    imageStructure.matB[i, j] = (byte)color.Blue;
                }

            }

            return imageStructure;
        }

        public static void SaveImage(string filename, ImageStructure imageStructure)
        {
            Image<Bgr, Byte> img = new Image<Bgr, byte>(imageStructure.Width, imageStructure.Height);
            int width = img.Width;
            int height = img.Height;

            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {

                    img.Data[i, j, 0] = imageStructure.matB[i, j];
                    img.Data[i, j, 1] = imageStructure.matG[i, j];
                    img.Data[i, j, 2] = imageStructure.matR[i, j];
                }

            }
            var bmp = img.ToBitmap();
            bmp.Save(filename);
        }

    }
}
