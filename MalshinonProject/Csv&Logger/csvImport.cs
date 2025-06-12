using System.Globalization;
using MySql.Data.MySqlClient;

namespace MalshinonProject.DAL;

internal class csvImport
{
        // מייבא דיווחים מקובץ CSV
        public static void ImportReports(MySqlConnection connection)
        {
            Console.Write("Enter CSV file path: ");
            string? path = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(path) || !File.Exists(path))
            {
                Console.WriteLine("File not found.");
                return;
            }

            int count = 0;

            using var reader = new StreamReader(path);
            string? header = reader.ReadLine();
            if (header == null)
            {
                Console.WriteLine("CSV file is empty.");
                return;
            }

            while (!reader.EndOfStream)
            {
                string? line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line)) continue;

                string[] parts = line.Split(',');
                if (parts.Length < 4) continue;

                string reporterName = parts[0].Trim();
                string targetName = parts[1].Trim();
                string reportText = parts[2].Trim();
                string timestampStr = parts[3].Trim();

                if (!DateTime.TryParse(timestampStr, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out DateTime timestamp))
                    continue;

                if (string.IsNullOrWhiteSpace(reporterName) || string.IsNullOrWhiteSpace(targetName) || string.IsNullOrWhiteSpace(reportText))
                    continue;

                int reporterId = PersonDAL.GetPersonIdByFullName(connection, reporterName);
                if (reporterId == -1)
                {
                    Console.Write($"Enter secret code for reporter '{reporterName}': ");
                    string code = Console.ReadLine()!;
                    reporterId = PersonDAL.InsertPerson(connection, reporterName, code, "reporter");
                }

                int targetId = PersonDAL.GetPersonIdByFullName(connection, targetName);
                if (targetId == -1)
                {
                    Console.Write($"Enter secret code for target '{targetName}': ");
                    string code = Console.ReadLine()!;
                    targetId = PersonDAL.InsertPerson(connection, targetName, code, "target");
                }

                ReportDAL.InsertReport(connection, reporterId, targetId, reportText, timestamp);
                PersonDAL.UpdateReportsCount(connection, reporterId);
                PersonDAL.UpdateMentionsCount(connection, targetId);
                Logger.WriteLog($"CSV Report: {reporterId} -> {targetId}");
                count++;
            }

            Console.WriteLine($"Imported {count} reports from file.");
            Logger.WriteLog($"CSV import complete. Total: {count}.");
        }
}
