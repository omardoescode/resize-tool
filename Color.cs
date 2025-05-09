namespace ImageProcessing
{
    class Color
    {
        public int r { get; set; }
        public int g { get; set; }
        public int b { get; set; }
        public int a { get; set; }

        public (int, int, int) rgb()
        {
            return (r, g, b);
        }

        public (int, int, int, int) rgba()
        {
            return (r, g, b, a);
        }
    }
}
