using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageProcessing
{
    using EnergyMap = double[,];
    using Seam = Coordinate[];
    public readonly record struct ImageSize(int width, int height);
    public readonly record struct Color(int r, int g, int b, int a);
    class SeamCarver
    {
        public void ProcessImage(Image<Rgba32> image, ImageSize new_size)
        {
            EnergyMap energy = CalcEnergyMap(image);
            var imageSize = new ImageSize(image.Width, image.Height);
            Seam lowestSeam = findSeam(imageSize, energy);
        }
        double CalcPixelEnergy(Color? left, Color middle, Color? right)
        {
            var (mr, mg, mb, ma) = middle;
            var lEnergy = 0;
            if (left.HasValue)
            {
                var (lr, lg, lb, la) = left.Value;
                lEnergy = (lr - mr) * (lr - mr) + (lg - mg) * (lg - mg) + (lb - mb) * (lb - mb);
            }
            var rEnergy = 0;
            if (right != null)
            {
                var (rr, rg, rb, la) = right.Value;
                rEnergy = (rr - mr) * (rr - mr) + (rg - mg) * (rg - mg) + (rb - mb) * (rb - mb);
            }

            return Math.Sqrt(lEnergy + rEnergy);
        }
        EnergyMap CalcEnergyMap(Image<Rgba32> image)
        {
            int width = image.Width;
            int height = image.Height;
            EnergyMap result = new double[width, height];

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Rgba32? left = (x - 1) >= 0 ? image[x - 1, y] : null;
                    Rgba32 middle = image[x, y];
                    Rgba32? right = (x + 1) < width ? image[x + 1, y] : null;

                    result[y, x] = CalcPixelEnergy(GetPixelColorNullable(left), GetPixelColor(middle), GetPixelColorNullable(right));
                }
            }

            return result;
        }

        Color? GetPixelColorNullable(Rgba32? pixel)
        {
            if (!pixel.HasValue)
            {
                return null;
            }
            Rgba32 ActualPixel = pixel.Value;
            return GetPixelColor(ActualPixel);
        }
        Color GetPixelColor(Rgba32 pixel)
        {
            return new Color(pixel.R, pixel.G, pixel.B, pixel.A);
        }
        Seam findSeam(ImageSize size, EnergyMap energy)
        {
            Console.WriteLine("Finding the seam operation begins");
            var (w, h) = size;
            SeamPixel[,] seamEnergies = new SeamPixel[w, h]; // The DB table

            // Put the first row values
            for (int x = 0; x < w; x++)
            {
                seamEnergies[0, x] = new SeamPixel(energy[x, 0], new Coordinate(x, 0));
            }

            // Complete the DB table
            for (int y = 1; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    Console.WriteLine($"The Pixel ({x}, {y}) is being calculated");
                    var minPrevEnergy = double.MaxValue;
                    var prevX = x;


                    // Check first
                    for (int i = (x - 1); i <= (x + 1); i++)
                    {
                        if (i >= 0 && i < w && seamEnergies[y - 1, i].energy < minPrevEnergy)
                        {
                            prevX = i;
                            minPrevEnergy = seamEnergies[y - 1, i].energy;
                        }
                    }

                    // Update the current pixel data
                    seamEnergies[y, x] = new SeamPixel(energy: minPrevEnergy + energy[y, x], coordinate: new Coordinate(x, y), previous: new Coordinate(x: prevX, y: y - 1));
                }
            }

            for (int y = 1; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
                    Console.WriteLine(seamEnergies[y, x].energy);
                }
            }

            // Find where the minimum seam ends
            Console.WriteLine("Finding the seam operation ends");
            return [];

        }
    }
}
