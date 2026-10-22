using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace ImageEditor
{
    public  class ActionRecord
    {
        public DateTime Date { get; set; }
        public string Operation { get; set; } = "";
        public Dictionary<string, string> Parameters { get; set; } = new();
    }
    public class ActionHistory
    {
        private readonly string _historyFilePath;

        public ActionHistory()
        {
            string appFolder = AppDomain.CurrentDomain.BaseDirectory;
            _historyFilePath = Path.Combine(appFolder, "logs", "history.json");
            Directory.CreateDirectory(Path.GetDirectoryName(_historyFilePath)!);
        }
        public void AddAction(string operation, Dictionary<string, string> parameters)
        {
            List<ActionRecord> history = LoadHistory();
            history.Add(new ActionRecord
            {
                Date = DateTime.Now,
                Operation = operation,
                Parameters = parameters 
            });
            string json = JsonSerializer.Serialize(history, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_historyFilePath, json);
        }
        private List<ActionRecord> LoadHistory()
        {
            if (!File.Exists(_historyFilePath))
            {
                return new List<ActionRecord>();
            }
            try
            {
                string json = File.ReadAllText(_historyFilePath);
                return JsonSerializer.Deserialize<List<ActionRecord>>(json) ?? new List<ActionRecord>();
            }
            catch (JsonException)
            {
                System.Windows.MessageBox.Show("Файл истории содержит некорректный JSON. История будет создана заново.","Ошибка истории", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return new List<ActionRecord>();
            }
            catch (IOException ex)
            {
                System.Windows.MessageBox.Show($"Не удалось прочитать файл истории: {ex.Message}", "Ошибка чтения", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
                return new List<ActionRecord>();
            }
            
        }

    }
}
