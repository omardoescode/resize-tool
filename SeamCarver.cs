using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageProcessing
{
    using EnergyMap = double[,];
    using Seam = Coordinate[];
    class SeamCarver
    {
        public void ProcessImage(Image<Rgba32> image, ImageSize new_size)
        {
            EnergyMap energy = CalcEnergyMap(image);
            int width = energy.GetLength(0);
            int height = energy.GetLength(1);

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    Console.Write($"{energy[x, y]:F2} ");  // Print each energy value with 2 decimal precision
                }
                Console.WriteLine();
            }
        }
        double CalcPixelEnergy(Color? left, Color middle, Color? right)
        {
            var (mr, mg, mb) = middle.rgb();
            var lEnergy = 0;
            if (left != null)
            {
                var (lr, lg, lb) = left.rgb();
                lEnergy = (lr - mr) * (lr - mr) + (lg - mg) * (lg - mg) + (lb - mb) * (lb - mb);
            }
            var rEnergy = 0;
            if (right != null)
            {
                var (rr, rg, rb) = right.rgb();
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

                    result[x, y] = CalcPixelEnergy(GetPixelColorNullable(left), GetPixelColor(middle), GetPixelColorNullable(right));
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
            Color result = new Color();
            result.r = pixel.R;
            result.g = pixel.G;
            result.b = pixel.B;
            result.a = pixel.A;
            return result;
        }
    }
}
