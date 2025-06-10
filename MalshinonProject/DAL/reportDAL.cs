using System.Globalization;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using MalshinonProject.DAL;

namespace MalshinonProject.DAL;

internal static class reportDAL
{
    public static void SubmitIntelReport(MySqlConnection connection, int reporterId)
    {
        // קבלת טקסט הדיווח מהמשתמש עם שם של מדוווח
        Console.WriteLine("Enter your intel report (mention a person like: John Smith):");
        string reportText = Console.ReadLine()!;

        // חיפוש שם פרטי + משפחה 
        var nameMatch = Regex.Match(reportText, @"\b[A-Z][a-z]+\s[A-Z][a-z]+\b");
        if (!nameMatch.Success)
        {
            Console.WriteLine("No valid target name found in the report.");
            return;
        }

        string targetFullName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nameMatch.Value.ToLower());
        int targetId = personDAL.GetPersonIdByFullName(connection, targetFullName);

        if (targetId == -1)
        {
            Console.Write($"Enter secret code for new target '{targetFullName}': ");
            string code = Console.ReadLine()!;
            targetId = personDAL.InsertPerson(connection, targetFullName, code, "target");
        }

        // מוסיף דיווח לטבלה, מעדכן מונים ומריץ לוג
        string insertSql = @"
            INSERT INTO IntelReports (reporter_id, target_id, text)
            VALUES (@reporter, @target, @text)";
        using var cmd = new MySqlCommand(insertSql, connection);
        cmd.Parameters.AddWithValue("@reporter", reporterId);
        cmd.Parameters.AddWithValue("@target", targetId);
        cmd.Parameters.AddWithValue("@text", reportText);
        cmd.ExecuteNonQuery();

        personDAL.UpdateReportsCount(connection, reporterId);
        personDAL.UpdateMentionsCount(connection, targetId);
        Logger.WriteLog($"Report submitted: {reporterId} → {targetId}");
    }
}
/*
        //בדיקה האם האדם כבר קיים
        string selectSql = "SELECT id FROM People WHERE CONCAT(first_name, ' ', last_name) = @name LIMIT 1";
        using var selectCmd = new MySqlCommand(selectSql, connection);
        selectCmd.Parameters.AddWithValue("@name", targetFullName);
        object? result = selectCmd.ExecuteScalar();

        int targetId;

        if (result != null)
        {
            targetId = Convert.ToInt32(result);
        }
        else
        {
            string[] parts = targetFullName.Split(' ', 2);
            string firstName = parts[0];
            string lastName = parts[1];

            string insertPersonSql = @"
                INSERT INTO People (first_name, last_name, secret_code, type, num_reports, num_mentions)
                VALUES (@first, @last, @code, 'target', 0, 0);
                SELECT LAST_INSERT_ID();";

            using var insertCmd = new MySqlCommand(insertPersonSql, connection);
            insertCmd.Parameters.AddWithValue("@first", firstName);
            insertCmd.Parameters.AddWithValue("@last", lastName);
            insertCmd.Parameters.AddWithValue("@code", Guid.NewGuid().ToString("N").Substring(0, 10));

            targetId = Convert.ToInt32(insertCmd.ExecuteScalar());
        }

        // הכנסת הדיווח לטבלת IntelReports
        string insertIntelSql = @"
            INSERT INTO IntelReports (reporter_id, target_id, text, timestamp)
            VALUES (@reporter, @target, @text, CURRENT_TIMESTAMP);";

        using var intelCmd = new MySqlCommand(insertIntelSql, connection);
        intelCmd.Parameters.AddWithValue("@reporter", reporterId);
        intelCmd.Parameters.AddWithValue("@target", targetId);
        intelCmd.Parameters.AddWithValue("@text", reportText);

        intelCmd.ExecuteNonQuery();

        Console.WriteLine("Intel report submitted successfully.");
}
    */ 

