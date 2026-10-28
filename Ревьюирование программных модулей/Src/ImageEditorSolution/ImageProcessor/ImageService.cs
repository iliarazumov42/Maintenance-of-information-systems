using System.IO;
using System.Diagnostics;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace ImageProcessor
{
    /// <summary>
    /// Обрабатывает изображение, применяя выбранные эффекты и изменяя его размер при необходимости.
    /// </summary>
    public class ImageService
    {
        public void ProcessImage(string inputPath,string outputPath,bool detectEdges,bool sharpen,int? newWidth,int? newHeight)
        {
            var stopwatch = Stopwatch.StartNew();

            if (!File.Exists(inputPath))
            {
                throw new FileNotFoundException("изоброжение не найдено", inputPath);
            }

            var loadTimer = Stopwatch.StartNew();
            using var image = Image.Load(inputPath);
            loadTimer.Stop();
            System.Diagnostics.Debug.WriteLine($"Загрузка изображения: {loadTimer.ElapsedMilliseconds} мс");
            
            if (sharpen)
            {
                var sharpenTimer = Stopwatch.StartNew();
                image.Mutate(x => x.GaussianSharpen());
                sharpenTimer.Stop();
                System.Diagnostics.Debug.WriteLine($"Повышение резкости: {sharpenTimer.ElapsedMilliseconds} мс");
            }

            if (detectEdges)
            {
                var edgesTimer = Stopwatch.StartNew();
                image.Mutate(x => x.DetectEdges());
                edgesTimer.Stop();
                System.Diagnostics.Debug.WriteLine($"Обнаружение контуров: {edgesTimer.ElapsedMilliseconds} мс");
            }

            if (newWidth.HasValue && newHeight.HasValue)
            {
                var resizeTimer = Stopwatch.StartNew();
                image.Mutate(x => x.Resize(newWidth.Value, newHeight.Value));
                resizeTimer.Stop();
                System.Diagnostics.Debug.WriteLine($"Изменение размера: {resizeTimer.ElapsedMilliseconds} мс");
            }
            var saveTimer = Stopwatch.StartNew();
            image.Save(outputPath);
            saveTimer.Stop();
            System.Diagnostics.Debug.WriteLine($"Сохранение изображения: {saveTimer.ElapsedMilliseconds} мс");
            stopwatch.Stop();
            System.Diagnostics.Debug.WriteLine($"Время обработки изображения: {stopwatch.ElapsedMilliseconds} мс");

        }
    }
}
