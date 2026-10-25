using System;
using System.IO;
using ImageProcessor;
using NUnit.Framework;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace ImageProcessor.Tests
{
    public class ImageServiceTests
    {
        [Test]
        public void ProcessImage_WithoutFilters_CreatesOutputFile()
        {
            string inputPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.png");
            string outputPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.png");
            try
            {
                using (var image = new Image<Rgba32>(100, 80))
                {
                    image.Save(inputPath);
                }

                var service = new ImageService();
                service.ProcessImage(inputPath, outputPath, detectEdges: false, sharpen: false, newWidth: null, newHeight: null);
                Assert.That(File.Exists(outputPath), Is.True);
                using var result = Image.Load(outputPath);
                Assert.That(result.Width, Is.EqualTo(100));
                Assert.That(result.Height, Is.EqualTo(80));
            }
            finally
            {
                if (File.Exists(inputPath))
                    File.Delete(inputPath);

                if (File.Exists(outputPath))
                    File.Delete(outputPath);
            }
        }

        [Test]
        public void ProcessImage_WithNewSize_ChangesImageDimensions()
        {
            string inputPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.png");
            string outputPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.png");
            try
            {
                using (var image = new Image<Rgba32>(100, 80))
                {
                    image.Save(inputPath);
                }

                var service = new ImageService();

                service.ProcessImage(inputPath, outputPath, detectEdges: false, sharpen: false, newWidth: 50, newHeight: 40);
                using var result = Image.Load(outputPath);

                Assert.That(result.Width, Is.EqualTo(50));
                Assert.That(result.Height, Is.EqualTo(40));
            }
            finally
            {
                if (File.Exists(inputPath))
                    File.Delete(inputPath);

                if (File.Exists(outputPath))
                    File.Delete(outputPath);
            }
        }

        [Test]
        public void ProcessImage_WithSharpen_SavesProcessedImage()
        {
            string inputPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.png");
            string outputPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.png");
            try
            {
                using (var image = new Image<Rgba32>(100, 80))
                {
                    image.Save(inputPath);
                }

                var service = new ImageService();

                service.ProcessImage(inputPath, outputPath, detectEdges: false, sharpen: true, newWidth: null, newHeight: null);
                Assert.That(File.Exists(outputPath), Is.True);

                using var result = Image.Load(outputPath);

                Assert.That(result.Width, Is.EqualTo(100));
                Assert.That(result.Height, Is.EqualTo(80));
            }
            finally
            {
                if (File.Exists(inputPath))
                    File.Delete(inputPath);

                if (File.Exists(outputPath))
                    File.Delete(outputPath);
            }
        }

        [Test]
        public void ProcessImage_WithEdgeDetection_SavesProcessedImage()
        {
            string inputPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.png");
            string outputPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.png");
            try
            {
                using (var image = new Image<Rgba32>(100, 80))
                {
                    image.Save(inputPath);
                }

                var service = new ImageService();
                service.ProcessImage(inputPath, outputPath, detectEdges: true, sharpen: false, newWidth: null, newHeight: null);
                Assert.That(File.Exists(outputPath), Is.True);

                using var result = Image.Load(outputPath);

                Assert.That(result.Width, Is.EqualTo(100));
                Assert.That(result.Height, Is.EqualTo(80));
            }
            finally
            {
                if (File.Exists(inputPath))
                    File.Delete(inputPath);

                if (File.Exists(outputPath))
                    File.Delete(outputPath);
            }
        }

        [Test]
        public void ProcessImage_WithMissingFile_ThrowsFileNotFoundException()
        {
            string inputPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.png");
            string outputPath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.png");
            
            var service = new ImageService();

            Assert.Throws<FileNotFoundException>(() => service.ProcessImage(inputPath, outputPath, detectEdges: false, sharpen: false, newWidth: null, newHeight: null));

        }
    }
}