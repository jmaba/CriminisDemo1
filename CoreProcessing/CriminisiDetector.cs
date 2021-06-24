using System;
using System.Collections.Concurrent;
using System.Drawing;
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
        private readonly int threshold = 0;
        public CriminisiDetector(ImageStructure image, Rectangle areaOfInterest, MatrixSize patchSize)
        {
            this.image = image;
            this.patchSize = patchSize;
            this.areaOverlappingCalculator = new AreaOverlappingCalculator(patchSize);
            this.blockCalculator = new BlockCalculator(image, areaOfInterest, patchSize);
        }

        public byte[,] ComputeDetectionArea(Color fillColor)
        {
            byte ct = 0;
            var outputImage = new byte[image.Height, image.Width];
            ComponentCalculator componentCalculator = new ComponentCalculator();
            var pairingItems = new ConcurrentBag<PairingRegion>();
            var ros = blockCalculator.GetAllBlocksInsideROS().ToList();
            Parallel.ForEach(ros, itemInROS =>
            {
                bool foundBestValue = false;
                double maxValue = 0;
                (int startingX, int startingY) maxFound = (0, 0);

                foreach (var itemInImage in blockCalculator.GetAllBlocksInsideImage())
                {
                    if (!areaOverlappingCalculator.DoPointsOverlapp(itemInROS.startingX, itemInROS.startingY, itemInImage.startingX, itemInImage.startingY))
                    {
                        var resR = ComputeBinarizeAreaDifferences(itemInROS, itemInImage, image.matR);
                        var resG = ComputeBinarizeAreaDifferences(itemInROS, itemInImage, image.matG);
                        var resB = ComputeBinarizeAreaDifferences(itemInROS, itemInImage, image.matB);
                        var x = ComputeFuzzyMembership(componentCalculator.GetMatchingDegree(resR));
                        var x2 = ComputeFuzzyMembership(componentCalculator.GetMatchingDegree(resG));
                        var x3 = ComputeFuzzyMembership(componentCalculator.GetMatchingDegree(resB));
                        var total = (x + x2 + x3) / 3;
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
            foreach (var item in pairingItems)
            {
                Console.WriteLine($"ROSx {item.StartingPositionROSX}  ROSy {item.StartingPositionROSY} SourceX {item.StartingPositionSourceX} SourceY {item.StartingPositionSourceY}");
            }

            return outputImage;
        }

        private readonly int a = 4;
        private readonly int b = 78;
        private readonly double lambda = 0.5;
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
