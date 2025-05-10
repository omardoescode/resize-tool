using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using FileHandling;

if (args.Length > 0)
{
    if (!FileUtils.checkFileExists(args[0]))
    {
        Console.WriteLine("File does not exist or Incorrect file path.");
        return;
    }
    string inputPath = args[0];
    using var image = Image.Load<Rgba32>(inputPath);
    var resizer = new ImageProcessing.SeamCarver();
    resizer.ProcessImage(image, new ImageProcessing.ImageSize(520, 520));
    image.Save("result.png");
}
else
{
    Console.WriteLine("Please provide an input image path as argument.");
    Console.WriteLine("Usage: resize-tool.exe <input-image-path>");
}
