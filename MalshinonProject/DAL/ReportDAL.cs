using System.Globalization;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;
using MalshinonProject.DAL;

namespace MalshinonProject.DAL;

internal static class ReportDAL
{
    //הכנסת דיווח חדש
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

        //החלפת.תיקון השמות לאות גדולה בתחילת השם
        string targetFullName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(nameMatch.Value.ToLower());
        int targetId = PersonDAL.GetPersonIdByFullName(connection, targetFullName);

        if (targetId == -1)
        {
            Console.Write($"Enter secret code for new target '{targetFullName}': ");
            string code = Console.ReadLine()!;
            targetId = PersonDAL.InsertPerson(connection, targetFullName, code, "target");
        }

        else
        {
            PersonDAL.UpdateType(connection, targetId);
        } 
        
        // מוסיף דיווח לטבלה מעדכן מונים
        string insertSql = @"
            INSERT INTO IntelReports (reporter_id, target_id, text)
            VALUES (@reporter, @target, @text)";
        using var cmd = new MySqlCommand(insertSql, connection);
        cmd.Parameters.AddWithValue("@reporter", reporterId);
        cmd.Parameters.AddWithValue("@target", targetId);
        cmd.Parameters.AddWithValue("@text", reportText);
        cmd.ExecuteNonQuery();

        //עדכון מספר דיווחים על המדווח
        PersonDAL.UpdateReportsCount(connection, reporterId);
        //עדכון מספר דיווחים על אדם
        PersonDAL.UpdateMentionsCount(connection, targetId);
        //קריאה לפונקציה של בדיקת מספר הדיווחים על המדווח
        AlretsDAL.CheckPerson(connection, targetId);
        
        
        //הרצת לוג
        Logger.WriteLog($"Report submitted: {reporterId} → {targetId}");
    }
    
    // מוסיף דיווח רק כשאני מייבא מcsv
    public static void InsertReport(MySqlConnection connection, int reporterId, int targetId, string text, DateTime timestamp)
    {
        string insertSql = @"
            INSERT INTO IntelReports (reporter_id, target_id, text, timestamp)
            VALUES (@reporter, @target, @text, @timestamp)";

        using var cmd = new MySqlCommand(insertSql, connection);
        cmd.Parameters.AddWithValue("@reporter", reporterId);
        cmd.Parameters.AddWithValue("@target", targetId);
        cmd.Parameters.AddWithValue("@text", text);
        cmd.Parameters.AddWithValue("@timestamp", timestamp);

        cmd.ExecuteNonQuery();
    }
}