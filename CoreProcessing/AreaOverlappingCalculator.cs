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
            return DoOverlap(   /*first rectangle*/
                                                new Point(firstRectangleX, firstRectangleY),
                                                new Point(firstRectangleX + patchSize.Height, firstRectangleY + patchSize.Width),
                                                /*second rectangle*/
                                                new Point(secondRectangleX, secondRectangleY),
                                                new Point(secondRectangleX + patchSize.Height, secondRectangleY + patchSize.Width));
        }

        private static bool DoOverlap(Point l1, Point r1,
                          Point l2, Point r2)
        {
            if (l1.X >= r2.X || l2.X >= r1.X)
            {
                return false;
            }

            if (l1.Y <= r2.Y || l2.Y <= r1.Y)
            {
                return false;
            }
            return true;
        }
    }
}
