namespace ImageProcessing
{
    class SeamPixel
    {
        public double energy { get; set; }
        public Coordinate coordinate { get; set; }
        public Coordinate? previous { get; set; }

        public SeamPixel(double energy, Coordinate coordinate, Coordinate? previous = null)
        {
            this.energy = energy;
            this.coordinate = coordinate;
            this.previous = previous;
        }
    }
}
