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
            for (int indexROSBlockI = areaOfInterest.X; 
                    (indexROSBlockI < areaOfInterest.X + areaOfInterest.Height) && 
                    (indexROSBlockI + patchSize.Height < image.Height) && 
                    (indexROSBlockI <= areaOfInterest.X + areaOfInterest.Height - patchSize.Height); indexROSBlockI++)
            {
                for (int indexROSBlockJ = areaOfInterest.Y; 
                        (indexROSBlockJ < areaOfInterest.Y + areaOfInterest.Width) && 
                        (indexROSBlockJ + patchSize.Width < image.Width) && 
                        (indexROSBlockJ <= areaOfInterest.Y + areaOfInterest.Width - patchSize.Width); indexROSBlockJ++)
                {
                    yield return (indexROSBlockI, indexROSBlockJ);
                }
            }
        }

        public IEnumerable<(int startingX, int startingY)> GetAllBlocksInsideImage()
        {
            const int valueOuterRegion = 20;
            var startingPointI = areaOfInterest.X - valueOuterRegion;
            if (startingPointI < 0) startingPointI = 0;

            var startingPointJ = areaOfInterest.Y - valueOuterRegion;
            if (startingPointJ < 0) startingPointJ = 0;


            var endingPointI =  areaOfInterest.X + areaOfInterest.Height + 2*valueOuterRegion < image.Height - patchSize.Height ? areaOfInterest.X + areaOfInterest.Height + 2 * valueOuterRegion : image.Height - patchSize.Height;
            var endingPointJ = areaOfInterest.Y + areaOfInterest.Width+2*valueOuterRegion<  image.Width - patchSize.Width ? areaOfInterest.Y + areaOfInterest.Width + 2 * valueOuterRegion : image.Width - patchSize.Width;
            for (int indexSourceBlockI = startingPointI; indexSourceBlockI < endingPointI; indexSourceBlockI++)
            {
                for (int indexSourceBlockJ = startingPointJ; indexSourceBlockJ < endingPointJ; indexSourceBlockJ++)
                {
                    yield return (indexSourceBlockI, indexSourceBlockJ);
                }
            }
        }
    }
}
