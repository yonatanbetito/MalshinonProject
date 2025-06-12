
using System.IO;

namespace MalshinonProject;

internal class Logger
{
    // רושם הודעה לקובץ הלוג
    public static void WriteLog(string message)
    {
        string log = $"{DateTime.Now}: {message}";
        File.AppendAllText("log.txt", log + Environment.NewLine);
    }
}