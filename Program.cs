using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;


using var image = Image.Load<Rgba32>("resources/blue.jpg");
var resizer = new ImageProcessing.SeamCarver();
resizer.ProcessImage(image, new ImageProcessing.ImageSize(900, 400));
image.Save("result.png");
