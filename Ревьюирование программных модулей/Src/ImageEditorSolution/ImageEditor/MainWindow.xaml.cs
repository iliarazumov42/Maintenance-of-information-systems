using ImageProcessor;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ImageEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ImageProcessor.ImageService service = new ImageProcessor.ImageService();
            string message = service.GetServiceName();
            MessageBox.Show(message);

            ImageProcessor.imageInfo image = service.GetImageInfo();
            string message2 = $"Высота {image.Height}\nЩирина {image.Width}\nВ {image.Format}";
            MessageBox.Show(message2);
        }
    }
}