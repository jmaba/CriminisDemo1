using System.Collections.Generic;
using System.Drawing;

namespace CoreProcessing
{
    public class BlockCalculator
    {
        private ImageStructure image;
        private Rectangle areaOfInterest;
        private MatrixSize patchSize;

        public BlockCalculator(ImageStructure image, Rectangle areaOfInterest, MatrixSize patchSize)
        {
            this.image = image;
            this.areaOfInterest = areaOfInterest;
            this.patchSize = patchSize;
        }

        public IEnumerable<(int startingX, int startingY)> GetAllBlocksInsideROS()
        {
            for (int indexROSBlockI = areaOfInterest.X; (indexROSBlockI < areaOfInterest.X + areaOfInterest.Height) && (indexROSBlockI + patchSize.Height < image.Height); indexROSBlockI++)
            {
                for (int indexROSBlockJ = areaOfInterest.Y; (indexROSBlockJ < areaOfInterest.Y + areaOfInterest.Width) && (indexROSBlockJ < image.Width - patchSize.Width); indexROSBlockJ++)
                {
                    yield return (indexROSBlockI, indexROSBlockJ);
                }
            }
        }

        public IEnumerable<(int startingX, int startingY)> GetAllBlocksInsideImage()
        {
            for (int indexSourceBlockI = 0; indexSourceBlockI < image.Height - patchSize.Height; indexSourceBlockI++)
            {
                for (int indexSourceBlockJ = 0; indexSourceBlockJ < image.Width - patchSize.Width; indexSourceBlockJ++)
                {
                    yield return (indexSourceBlockI, indexSourceBlockJ);
                }
            }
        }
    }
}
