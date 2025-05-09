using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;


using var image = Image.Load<Rgba32>("resources/aluber.png");
var resizer = new ImageProcessing.SeamCarver();
resizer.ProcessImage(image, new ImageProcessing.ImageSize(400, 500));
image.Save("result.jpg");
