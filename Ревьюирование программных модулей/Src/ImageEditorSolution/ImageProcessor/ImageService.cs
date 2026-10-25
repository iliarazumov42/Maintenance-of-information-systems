using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ImageProcessor
{
    public class ImageService
    {
        public void ProcessImage(string inputPath,string outputPath,bool detectEdges,bool sharpen,int? newWidth,int? newHeight)
        {
            if (!File.Exists(inputPath))
            {
                throw new FileNotFoundException("изоброжение не найдено", inputPath);
            }
            using var image = Image.Load(inputPath);
            if (sharpen)
            {
                image.Mutate(x => x.GaussianSharpen());
            }

            if (detectEdges)
            {
                image.Mutate(x => x.DetectEdges());
            }

            if (newWidth.HasValue && newHeight.HasValue)
            {
                image.Mutate(x => x.Resize(newWidth.Value, newHeight.Value));
            }

            image.Save(outputPath);
        }
    }
}
