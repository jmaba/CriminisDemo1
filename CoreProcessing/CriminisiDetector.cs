using System;
using System.Collections.Concurrent;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Threading.Tasks;

namespace CoreProcessing
{
    public class CriminisiDetector
    {
        private ImageStructure image;
        private MatrixSize patchSize;
        private readonly AreaOverlappingCalculator areaOverlappingCalculator;
        private readonly BlockCalculator blockCalculator;
        private readonly Rectangle areaOfInterest;

        public Color fillColorBadArea = Color.Red;
        public Color potentialRosArea = Color.Yellow;
        public Color sourceArea = Color.Blue;
        public Color background = Color.White;

        public CriminisiDetector(ImageStructure image, Rectangle areaOfInterest, MatrixSize patchSize)
        {
            this.image = image;
            this.patchSize = patchSize;
            this.areaOfInterest = areaOfInterest;
            areaOverlappingCalculator = new AreaOverlappingCalculator(patchSize);
            blockCalculator = new BlockCalculator(image, areaOfInterest, patchSize);
        }

        public void ComputeDetectionArea()
        {
            Bitmap bmp = new Bitmap(image.Height, image.Width);
            for (int i = 0; i < image.Height; i++)
            {
                for (int j = 0; j < image.Width; j++)
                {
                    bmp.SetPixel(i, j, background);
                }
            }
            ComponentCalculator componentCalculator = new ComponentCalculator();
            var pairingItems = new ConcurrentBag<PairingRegion>();
            var ros = blockCalculator.GetAllBlocksInsideROS().ToList();
            var allBlocks = blockCalculator.GetAllBlocksInsideImage().ToList();

            Parallel.ForEach(ros, itemInROS =>
            {
                bool foundBestValue = false;
                double maxValue = 0;
                (int startingX, int startingY) maxFound = (0, 0);

                foreach (var itemInImage in allBlocks)
                {
                    if (!areaOverlappingCalculator.DoPointsOverlapp(itemInROS.startingX, itemInROS.startingY, itemInImage.startingX, itemInImage.startingY))
                    {
                        const int X = 5;
                        var resR = ComputeBinarizeAreaDifferences(itemInROS, itemInImage, image.matR);
                        var resG = ComputeBinarizeAreaDifferences(itemInROS, itemInImage, image.matG);
                        var resB = ComputeBinarizeAreaDifferences(itemInROS, itemInImage, image.matB);
                        var m1 = componentCalculator.GetMatchingDegree(resR);
                        if (m1 <= X) continue;
                        var m2 = componentCalculator.GetMatchingDegree(resG);
                        if (m2 <= X) continue;
                        var m3 = componentCalculator.GetMatchingDegree(resB);
                        if (m3 <= X) continue;

                        var x = ComputeFuzzyMembership(m1);
                        var x2 = ComputeFuzzyMembership(m2);
                        var x3 = ComputeFuzzyMembership(m3);
                        var total = (x + x2 + x3) / 3.0;
                        if (BelongsToCutSet(total))
                        {
                            foundBestValue = true;
                            if (maxValue < total)
                            {
                                maxFound = itemInImage;
                                maxValue = total;
                            }
                        }
                    }
                }
                if (foundBestValue)
                {
                    pairingItems.Add(new PairingRegion()
                    {
                        StartingPositionROSX = itemInROS.startingX,
                        StartingPositionROSY = itemInROS.startingY,
                        StartingPositionSourceX = maxFound.startingX,
                        StartingPositionSourceY = maxFound.startingY
                    });
                }
            });

            Graphics newGraphics = Graphics.FromImage(bmp);
            newGraphics.DrawRectangle(new Pen(new SolidBrush(potentialRosArea)), areaOfInterest);

            foreach (var item in pairingItems)
            {
                newGraphics.FillRectangle(new SolidBrush(sourceArea), new Rectangle(item.StartingPositionSourceX, item.StartingPositionSourceY, patchSize.Height, patchSize.Width));
                newGraphics.FillRectangle(new SolidBrush(fillColorBadArea), new Rectangle(item.StartingPositionROSX, item.StartingPositionROSY, patchSize.Height, patchSize.Width));
            }
            bmp.Save(@"c:\temp\output.jpg", ImageFormat.Jpeg );
            bmp.Dispose();
        }

        private readonly int a = 4;
        private readonly int b = 78;
        private readonly double lambda = 0.35;
        private bool BelongsToCutSet(double fuzzyValue)
        {
            return fuzzyValue >= lambda;
        }
        private double ComputeFuzzyMembership(int matchingDegree)
        {
            if (matchingDegree <= a)
            {
                return 0;
            }

            if (matchingDegree > b)
            {
                return 1;
            }

            return (matchingDegree - a) * 1.0 / (b - a);
        }

        private byte[,] ComputeBinarizeAreaDifferences((int startingX, int startingY) itemInROS, (int startingX, int startingY) itemInImage, byte[,] image)
        {
            var result = new byte[patchSize.Height, patchSize.Width];
            for (int i = 0; i < patchSize.Height; i++)
            {
                for (int j = 0; j < patchSize.Width; j++)
                {
                    if (image[itemInROS.startingX + i, itemInROS.startingY + j] ==
                                                    image[itemInImage.startingX + i, itemInImage.startingY + j])
                    {
                        result[i, j] = 1;
                    }
                    else
                    {
                        result[i, j] = 0;
                    }
                }
            }
            return result;
        }
    }
}
