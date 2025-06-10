
using System.IO;

namespace MalshinonProject
{
    public static class Logger
    {
        public static void Log(string message)
        {
            string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}";
            Console.WriteLine(logEntry);
            File.AppendAllText("log.txt", logEntry + Environment.NewLine);
        }
        public static string Read()
        {
            if (!File.Exists("log.txt"))
            {
                return string.Empty;
            }
            return File.ReadAllText("log.txt");
        }
    }
}