using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;


namespace ImageProcessing
{
    using EnergyMap = double[,];
    using Seam = Coordinate[];
    public readonly record struct ImageSize(int width, int height);
    public readonly record struct Color(int r, int g, int b, int a);
    class SeamCarver
    {
        public void ProcessImage(Image<Rgba32> image, ImageSize newSize)
        {
            if (newSize.width > image.Width || newSize.height > image.Height)
            {
                Console.WriteLine("Cannot create a bigger image");
                return;
            }
            var imageSize = new ImageSize(image.Width, image.Height);
            ResizeWidth(image, newSize.width);
            Console.WriteLine("Width Resized");

            image.Mutate(ctx => ctx.RotateFlip(RotateMode.Rotate90, FlipMode.None));
            ResizeWidth(image, newSize.height, true);
            image.Mutate(ctx => ctx.RotateFlip(RotateMode.Rotate270, FlipMode.None));

            Console.WriteLine("Height Resized");

            // Remove
            image.Mutate(ctx => ctx.Crop(new Rectangle(0, 0, newSize.width, newSize.height)));
        }

        public void ResizeWidth(Image<Rgba32> image, int targetWidth, bool isTransposed = false)
        {
            int current_width = image.Width;
            while (current_width > targetWidth)
            {
                var imageSize = new ImageSize(current_width, image.Height);
                EnergyMap energy = CalcEnergyMap(image, imageSize);
                Seam lowestSeam = FindSeam(imageSize, energy);
                RemoveSeam(image, lowestSeam);
                imageSize = new ImageSize(image.Width, image.Height);
                if (!isTransposed)
                    Console.WriteLine($"Current Size: ({image.Width}, {image.Height})");
                else
                    Console.WriteLine($"Current Size: ({image.Height}, {image.Width})");
                current_width--;
            }
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
                var (rr, rg, rb, ra) = right.Value;
                rEnergy = (rr - mr) * (rr - mr) + (rg - mg) * (rg - mg) + (rb - mb) * (rb - mb);
            }

            return Math.Sqrt(lEnergy + rEnergy);
        }
        EnergyMap CalcEnergyMap(Image<Rgba32> image, ImageSize cur_size)
        {
            var (width, height) = cur_size;
            EnergyMap result = new double[height, width];

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
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
        Seam FindSeam(ImageSize size, EnergyMap energy)
        {
            var (w, h) = size;
            SeamPixel[,] seamEnergies = new SeamPixel[h, w]; // The DB table

            // Put the first row values
            for (int x = 0; x < w; x++)
            {
                seamEnergies[0, x] = new SeamPixel(energy[0, x], new Coordinate(x, 0));
            }

            // Complete the DB table
            for (int y = 1; y < h; y++)
            {
                for (int x = 0; x < w; x++)
                {
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

            Coordinate? lastMinCoordinate = null;
            double lastSeamEnergy = double.MaxValue;
            for (int x = 0; x < w; x++)
            {
                int y = h - 1;
                if (seamEnergies[y, x].energy < lastSeamEnergy)
                {
                    lastSeamEnergy = seamEnergies[y, x].energy;
                    lastMinCoordinate = new Coordinate(x, y);
                }
            }

            if (!lastMinCoordinate.HasValue)
            {
                Console.WriteLine("Failed to find the minimum energy seam");
                return Array.Empty<Coordinate>();
            }

            // Re-create the seam
            Seam result = new Coordinate[h];
            var (lastX, lastY) = lastMinCoordinate.Value;
            Coordinate? current = seamEnergies[lastY, lastX].coordinate;
            int index = h - 1;

            while (current.HasValue)
            {
                result[index--] = current.Value;
                var (cx, cy) = current.Value;
                current = seamEnergies[cy, cx].previous;
            }

            return result;
        }
        void RemoveSeam(Image<Rgba32> image, Seam seam)
        {
            for (int y = 0; y < image.Height; y++)
            {
                int seamX = seam[y].x;

                for (int x = seamX; x < image.Width - 1; x++)
                {
                    image[x, y] = image[x + 1, y];
                }
            }
            image.Mutate(ctx => ctx.Crop(new Rectangle(0, 0, image.Width - 1, image.Height)));
        }
    }
}
