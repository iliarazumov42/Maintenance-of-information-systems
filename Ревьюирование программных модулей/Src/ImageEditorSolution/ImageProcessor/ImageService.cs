using System.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ImageProcessor
{
    public class ImageService
    {
        public imageInfo GetImageInfoFromFile()
        {
            using var image = Image.Load(@"C:\Users\Пользователь\Desktop\95.jpg");
            imageInfo info = new imageInfo();
            info.Height = image.Height;
            info.Width = image.Width;
            info.Format = image.Metadata.DecodedImageFormat?.Name ?? "неизвестен";
            return info;
        }
        public void ResizeImage(string inputPath, string outputPuth, int newWidth, int newHeigth)
        {
            if (!File.Exists(inputPath))
            {
                throw new FileNotFoundException("изоброжение не найдено", inputPath);
            }
            using var image = Image.Load(inputPath);
            image.Mutate(x => x.Resize(newWidth, newHeigth));
            image.Save(outputPuth);
        }
        public void SharpenImage(string inputPath, string outputPuth)
        {
            if(!File.Exists(inputPath))
            {
                throw new FileNotFoundException("изоброжение не найдено", inputPath);
            }

            using var image = Image.Load(inputPath);
            image.Mutate(x => x.GaussianSharpen());
            image.Save(outputPuth);
        }
        public void DetectEdges(string inputPath, string outputPuth)
        {
            if (!File.Exists(inputPath))
            {
                throw new FileNotFoundException("изоброжение не найдено", inputPath);
            }
            using var image = Image.Load(inputPath);
            image.Mutate(x => x.DetectEdges());
            image.Save(outputPuth);
        }
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
