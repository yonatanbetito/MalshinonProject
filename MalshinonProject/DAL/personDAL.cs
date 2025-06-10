using MySql.Data.MySqlClient;

namespace MalshinonProject.DAL;

internal class personDAL
{
    
    //מחפש אדם לפי שם מלא ומחזיר ID
    public static int GetPersonIdByFullName(MySqlConnection connection, string fullName)
    {
        var parts = fullName.Split(' ');
        if (parts.Length < 2) return -1;

        string sql = "SELECT id FROM People WHERE first_name = @first AND last_name = @last";
        using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@first", parts[0]);
        cmd.Parameters.AddWithValue("@last", parts[1]);

        var result = cmd.ExecuteScalar();
        return result != null ? Convert.ToInt32(result) : -1;
    } 
    
    //מוסיף אדם חדש לטבלה
    public static int InsertPerson(MySqlConnection connection, string fullName, string code, string type)
    {
        var parts = fullName.Split(' ');
        if (parts.Length < 2) return -1;
        
        string sql = @"
            INSERT INTO People (first_name, last_name, secret_code, type)
            VALUES (@first, @last, @code, @type)";
        using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@first", parts[0]);
        cmd.Parameters.AddWithValue("@last", parts[1]);
        cmd.Parameters.AddWithValue("@code", code);
        cmd.Parameters.AddWithValue("@type", type);
        cmd.ExecuteNonQuery();
        
        return (int)cmd.LastInsertedId;
    }
    
    //מעדכן מספר דיווחים על אדם
    public static void UpdateReportsCount(MySqlConnection connection, int personId)
    {
        string sql = "UPDATE People SET num_reports = num_reports + 1 WHERE id = @id";
        using var cmd = new MySqlCommand(sql, connection);
        cmd.Parameters.AddWithValue("@id", personId);
        cmd.ExecuteNonQuery();
    }
    
    //עדכן את מספר דיווחים שדיווחו (עליו)
  public static void UpdateMentionsCount(MySqlConnection conn, int personId)
    {
    string sql = "UPDATE People SET num_mentions = num_mentions + 1 WHERE id = @id";
    using var cmd = new MySqlCommand(sql, conn);
    cmd.Parameters.AddWithValue("@id", personId);
    cmd.ExecuteNonQuery();
    }
} 















/*public class personDAL
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
*/