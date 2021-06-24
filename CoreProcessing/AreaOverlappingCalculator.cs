using System.Drawing;

namespace CoreProcessing
{
    public class AreaOverlappingCalculator
    {
        private MatrixSize patchSize;

        public AreaOverlappingCalculator(MatrixSize patchSize)
        {
            this.patchSize = patchSize;
        }

        public bool DoPointsOverlapp(int firstRectangleX, int firstRectangleY, int secondRectangleX, int secondRectangleY)
        {
            return DoOverlap(new Rectangle(firstRectangleX, firstRectangleY, patchSize.Width, patchSize.Height),
                            new Rectangle(secondRectangleX, secondRectangleY, patchSize.Width, patchSize.Height));
        }

        private static bool DoOverlap(Rectangle r1, Rectangle r2)
        {
            return r1.IntersectsWith(r2);
        }
    }
}
