namespace ImageProcessor
{
    public class ImageService
    {
        public string GetServiceName()
        {
            return "Серви обработки изображений";
        }
        public imageInfo GetImageInfo()
        {
            imageInfo image = new imageInfo();
            image.Height = 1080;
            image.Width = 1920;
            image.Format = "PNG";
            return image;
        }
    }
}
