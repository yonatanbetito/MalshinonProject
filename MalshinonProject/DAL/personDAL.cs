using MySql.Data.MySqlClient;

namespace MalshinonProject.DAL;

public class personDAL
{
    //מזהה אדם לפי שם מלא במידה לא קיים יוצרת אותו כ-reporter עם קוד סודי
    public static int IdentifyOrCreatePerson(MySqlConnection connection, string fullName)
    {
        try
        {
            // חיפוש אדם לפי שם מלא
            string selectSql = "SELECT id FROM People WHERE CONCAT(first_name, ' ', last_name) = @fullName LIMIT 1";
            using var selectCmd = new MySqlCommand(selectSql, connection);
            selectCmd.Parameters.AddWithValue("@fullName", fullName);

            object? result = selectCmd.ExecuteScalar();
            
            if (result != null)
            {
                return Convert.ToInt32(result);
            }
            
            string[] parts = fullName.Split(' ', 2);
            string firstName = parts.Length > 0 ? parts[0] : "";
            string lastName = parts.Length > 1 ? parts[1] : "";

            // יצירת קוד סודי ייחודי
            string secretCode = Guid.NewGuid().ToString("N").Substring(0, 10);

            // שאילתה להוספת אדם חדש כמדווה
            string insertSql = @"
            INSERT INTO People (first_name, last_name, secret_code, type, num_reports, num_mentions)
            VALUES (@firstName, @lastName, @secretCode, 'reporter', 0, 0);
            SELECT LAST_INSERT_ID();";

            using var insertCmd = new MySqlCommand(insertSql, connection);
            insertCmd.Parameters.AddWithValue("@firstName", firstName);
            insertCmd.Parameters.AddWithValue("@lastName", lastName);
            insertCmd.Parameters.AddWithValue("@secretCode", secretCode);

            return Convert.ToInt32(insertCmd.ExecuteScalar());
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error in person identification: " + ex.Message);
            return -1;
        }
    }
}