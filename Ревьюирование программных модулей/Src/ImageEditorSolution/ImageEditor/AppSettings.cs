using System;
using System.IO;
using System.Text.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.DirectoryServices;

namespace ImageEditor
{
    public class AppSettings
    {
        public string LastImagePath { get; set; } = "";
        public bool EdgesEnabled { get; set; }
        public bool SharpenEnabled { get; set; }
        public int? NewWidth { get; set; }
        public int? NewHeight { get; set; }
        public double WindowWidth { get; set; } = 1000;
        public double WindowHeight { get; set; } = 600;

        private static readonly string SettingsFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "configs", "settings.json");

        public void Save()
        {
            string? folder = Path.GetDirectoryName(SettingsFilePath);
            if (folder != null)
            {
                Directory.CreateDirectory(folder);
            }
            string json = JsonSerializer.Serialize(this, new JsonSerializerOptions {WriteIndented = true});
            File.WriteAllText(SettingsFilePath, json);
        }

        public static AppSettings Load()
        {
            if (!File.Exists(SettingsFilePath))
            {
                return new AppSettings();
            }
            string json = File.ReadAllText(SettingsFilePath);
            return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
        }
    }
}
