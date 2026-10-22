using ImageProcessor;
using System.IO;
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
using Microsoft.Win32;



namespace ImageEditor
{
    public partial class MainWindow : Window
    {
        private string? _inputImagePath;
        private BitmapSource? _previousProcessedImage;
        private bool _previousEdgesEnabled;
        private bool _previousSharpenEnabled;
        private int? _previousWidth;
        private int? _previousHeight;
        private bool _isUndoing;
        public MainWindow()
        {
            InitializeComponent();
            Closing += MainWindow_Closing;
            _settings = AppSettings.Load();
            EdgesCheckBox.IsChecked = _settings.EdgesEnabled;
            SharpenCheckBox.IsChecked = _settings.SharpenEnabled;
            WidthTextBox.Text = _settings.NewWidth?.ToString() ?? "";
            HeightTextBox.Text = _settings.NewHeight?.ToString() ?? "";
            Width = _settings.WindowWidth;
            Height = _settings.WindowHeight;
            if (!string.IsNullOrWhiteSpace(_settings.LastImagePath))
                try
                {
                    LoadImageFromPath(_settings.LastImagePath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось обработать изображение.\nПричина: {ex.Message}", "Ошибка обработки", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            string appFolder = AppDomain.CurrentDomain.BaseDirectory;
            string logsFolder = System.IO.Path.Combine(appFolder, "logs");
            string configsFolder = System.IO.Path.Combine(appFolder, "configs");
            string outputFolder = System.IO.Path.Combine(appFolder, "output");

            Directory.CreateDirectory(logsFolder);
            Directory.CreateDirectory(configsFolder);
            Directory.CreateDirectory(outputFolder);
        }
      

        private readonly ImageService _imageService = new ImageService();
        private AppSettings _settings = new AppSettings();
        private readonly AppLogger _logger = new AppLogger();
        private readonly ActionHistory _actionHistory = new ActionHistory();
        private int? _newWidth;
        private int? _newHeight;

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Выберите изображение";
            dialog.Filter = "Файлы изображений|*.jpg;*.jpeg;*.png;*.bmp;*.gif";
            bool? result = dialog.ShowDialog();
            if (result == true)
            {
                try
                {
                    LoadImageFromPath(dialog.FileName);
                    _logger.Log("Изображение загружено: " + dialog.FileName);                   
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось обработать изображение.\nПричина: {ex.Message}", "Ошибка обработки", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void EdgesCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (_isUndoing)
            {
                return;
            }
            SavePreviousState();
            _previousEdgesEnabled = EdgesCheckBox.IsChecked != true;
            UpdateProcessedImage();
        }
        private void SharpenCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (_isUndoing)
            {
                return;
            }
            SavePreviousState();
            _previousSharpenEnabled = SharpenCheckBox.IsChecked != true;
            UpdateProcessedImage();
        }
        private void SavePreviousState()
        {
            if(ProcessedImage.Source is BitmapSource currentImage)
            {
                _previousProcessedImage = currentImage;
                _previousEdgesEnabled = EdgesCheckBox.IsChecked == true;
                _previousSharpenEnabled = SharpenCheckBox.IsChecked == true;
                _previousWidth = _newWidth;
                _previousHeight = _newHeight;
            }
        }
        public void UpdateProcessedImage()
        {
            if (string.IsNullOrEmpty(_inputImagePath))
            {
                return;
            }
            try
            {
                string outputPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), $"image_result_{Guid.NewGuid()}.png");
                _imageService.ProcessImage(_inputImagePath, outputPath, EdgesCheckBox.IsChecked == true, SharpenCheckBox.IsChecked == true, _newWidth, _newHeight);
                
                BitmapImage processedBitmap = new BitmapImage();
                processedBitmap.BeginInit();
                processedBitmap.UriSource = new Uri(outputPath);
                processedBitmap.CacheOption = BitmapCacheOption.OnLoad;
                processedBitmap.EndInit();
                processedBitmap.Freeze();

                ProcessedImage.Source = processedBitmap;

                _logger.Log("Изображение обработано");
                _actionHistory.AddAction("image processing", new Dictionary<string, string> 
                {{ "Edges", (EdgesCheckBox.IsChecked == true).ToString() }, 
                    {"Sharpen", (SharpenCheckBox.IsChecked == true).ToString() },
                    { "Width", _newWidth?.ToString() ?? "Original" },
                    { "Height", _newHeight?.ToString() ?? "Original" } 
                });
            }
            catch(Exception ex)
            {
                _logger.Log($"Ошибка обработки изображения: {ex.Message}");
                MessageBox.Show($"Не удалось обработать изображение.\nПричина: {ex.Message}", "Ошибка обработки", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void UndoLastAction_Click(object sender, RoutedEventArgs e)
        {
            if (_previousProcessedImage == null)
            {
                MessageBox.Show("Нет действия для отмены");
                return;
            }
            _isUndoing = true;
            try
            {
                ProcessedImage.Source = _previousProcessedImage;
                EdgesCheckBox.IsChecked = _previousEdgesEnabled;
                SharpenCheckBox.IsChecked = _previousSharpenEnabled;

                _newWidth = _previousWidth;
                _newHeight = _previousHeight;

                WidthTextBox.Text = _newWidth?.ToString() ?? "";
                HeightTextBox.Text = _newHeight?.ToString() ?? "";
                _previousProcessedImage = null;
                _isUndoing = false;
            }
            finally
            {
                _isUndoing = false;
            }
        }
        private void ResizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_inputImagePath))
            {
                MessageBox.Show("Сначало загрзите изображение!");
                return;
            }
            if(!int.TryParse(WidthTextBox.Text, out int width) || !int.TryParse(HeightTextBox.Text, out int height) || width<=0 || height<=0)
            {
                MessageBox.Show("Введите ширину и высоту — положительными, целыми числами.");
                return;
            }

            SavePreviousState();
            _newWidth = width;
            _newHeight = height;
            UpdateProcessedImage();
        }
        public void SaveResult_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_inputImagePath))
            {
                MessageBox.Show("Сначало загрзите изображение!");
                return;
            }
            SaveFileDialog saveWindow = new SaveFileDialog();
            saveWindow.Title = "Сохранить обработанное изображение";
            saveWindow.Filter = "PNG изображение|*.png";
            saveWindow.FileName = "result.png";
            bool? result = saveWindow.ShowDialog();

            if(result == true)
            {
                try
                {
                    _imageService.ProcessImage(_inputImagePath, saveWindow.FileName, EdgesCheckBox.IsChecked == true, SharpenCheckBox.IsChecked == true, _newWidth, _newHeight);
                    MessageBox.Show("Изображение сохранено!");
                    _logger.Log("Изменённое изоброжение сохраненно:"+ saveWindow.FileName);
                    _actionHistory.AddAction("saving an image", new Dictionary<string, string>
                {{ "save Path", saveWindow.FileName },
                    {"Format", "PNG"},
                    {"Edges", (EdgesCheckBox.IsChecked == true).ToString() },
                    { "Sharpen", (SharpenCheckBox.IsChecked == true).ToString() },
                    { "Width", _newWidth?.ToString() ?? "Original" },
                    { "Height", _newHeight?.ToString() ?? "Original" }
                });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Не удалось обработать изображение.\nПричина: {ex.Message}", "Ошибка обработки", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
        private void SaveSettings()
        {
            _settings.LastImagePath = _inputImagePath ?? "";
            _settings.EdgesEnabled = EdgesCheckBox.IsChecked == true;
            _settings.SharpenEnabled = SharpenCheckBox.IsChecked == true;
            _settings.NewWidth = int.TryParse(WidthTextBox.Text, out int width) ? width : null;
            _settings.NewHeight = int.TryParse(HeightTextBox.Text, out int height) ? height : null;
            _settings.WindowWidth = Width;
            _settings.WindowHeight = Height;
            _settings.Save();
        }
        private void MainWindow_Closing(object? sender, System.ComponentModel.CancelEventArgs e)
        {
            SaveSettings();
        }
        private void LoadImageFromPath(string filePath)
        {
            if(!File.Exists(filePath))
            {
                MessageBox.Show("Выбранный файл не найден.", "Ошибка загрузки", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            try
            {
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(filePath);
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();

                _inputImagePath = filePath;
                PreviewImage.Source = bitmap;
                int width = bitmap.PixelWidth;
                int height = bitmap.PixelHeight;
                string format = System.IO.Path.GetExtension(filePath).TrimStart('.').ToUpper();
                ImageInfoText.Text = $"Ширина: {width}\n" + $"Высота: {height}\n" + $"Формат: {format}\n";
                UpdateProcessedImage();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Не удалось загрузить изображение.\nПричина: {ex.Message}", "Ошибка загрузки", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearImageButton_Click(object sender, RoutedEventArgs e)
        {
            _inputImagePath = null;
            _newWidth = null;
            _newHeight = null;

            PreviewImage.Source = null;
            ProcessedImage.Source = null;
            ImageInfoText.Text = "";

            WidthTextBox.Text = "";
            HeightTextBox.Text = "";

            EdgesCheckBox.IsChecked = false;
            SharpenCheckBox.IsChecked = false;

            _previousProcessedImage = null;

            _settings.LastImagePath = "";
            _settings.NewWidth = null;
            _settings.NewHeight = null;
            _settings.EdgesEnabled = false;
            _settings.SharpenEnabled = false;
            _settings.Save();

            _logger.Log("Изображение очищено");

            _actionHistory.AddAction("image cleaning", new Dictionary<string, string>());
        }

    }
}