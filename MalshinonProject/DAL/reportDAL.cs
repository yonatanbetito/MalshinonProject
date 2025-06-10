using System.Globalization;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;

namespace MalshinonProject.DAL;

public class reportDAL
{
    public static void SubmitIntelReport(MySqlConnection connection, int reporterId)
    {
        // קבלת טקסט הדיווח מהמשתמש
        Console.WriteLine("Enter your intel report (mention a person like: John Smith):");
        string reportText = Console.ReadLine()!;

        // חיפוש שם פרטי + משפחה 
        var nameMatch = Regex.Match(reportText, @"\b[A-Z][a-z]+\s[A-Z][a-z]+\b");

        if (!nameMatch.Success)
        {
            Console.WriteLine("No valid target name found in the report.");
            return;
        }

        string extractedFullName = nameMatch.Value;
        TextInfo textInfo = CultureInfo.CurrentCulture.TextInfo;
        string targetFullName = textInfo.ToTitleCase(extractedFullName.ToLower());


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
}
