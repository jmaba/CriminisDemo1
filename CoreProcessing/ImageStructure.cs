namespace CoreProcessing
{
    public class ImageStructure
    {
        public byte[,] matR { get; set; }
        public byte[,] matG { get; set; }
        public byte[,] matB { get; set; }

        public int Width => matR.GetLength(1);
        public int Height => matR.GetLength(0);
    }
}
