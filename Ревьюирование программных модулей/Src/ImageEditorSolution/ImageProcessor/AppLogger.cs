using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageProcessor
{
    public class AppLogger
    {
        private readonly string _logFilePath;

        public AppLogger()
        {
            string appFolder = AppDomain.CurrentDomain.BaseDirectory;
            string logsFolder = Path.Combine(appFolder, "logs");

            Directory.CreateDirectory(logsFolder);

            _logFilePath = Path.Combine(logsFolder, "app.log");
        }

        public void Log(string message)
        {
            string logEntry =
                $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} — {message}";

            File.AppendAllText(
                _logFilePath,
                logEntry + Environment.NewLine);
        }
    }
}
