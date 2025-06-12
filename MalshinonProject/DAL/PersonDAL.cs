using MySql.Data.MySqlClient;

namespace MalshinonProject.DAL;

internal class PersonDAL
{
    
    //מחפש אדם לפי שם מלא ומחזיר ID
    public static int GetPersonIdByFullName(MySqlConnection connection, string fullName)
    {
        var parts = fullName.Split(' ');
        if (parts.Length < 2) return -1;

        string sql = @"SELECT id FROM People WHERE first_name =  @first AND last_name = @last";
        MySqlCommand command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@first", parts[0]);
        command.Parameters.AddWithValue("@last", parts[1]);

        var result = command.ExecuteScalar();
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
        MySqlCommand command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@first", parts[0]);
        command.Parameters.AddWithValue("@last", parts[1]);
        command.Parameters.AddWithValue("@code", code);
        command.Parameters.AddWithValue("@type", type);
        command.ExecuteNonQuery();
        
        return (int)command.LastInsertedId;
    }
    
    //מעדכן מספר דיווחים על אדם
    public static void UpdateReportsCount(MySqlConnection connection, int personId)
    {
        string sql = "UPDATE People SET num_reports = num_reports + 1 WHERE id ="+ personId;
        MySqlCommand command = new MySqlCommand(sql, connection);
        command.ExecuteNonQuery();
    }
    
    //עידכון מספר דיווחים שדיווחו עליו
  public static void UpdateMentionsCount(MySqlConnection connection, int personId)
    {
    string sql = "UPDATE People SET num_mentions = num_mentions + 1 WHERE id = "+personId;
    MySqlCommand command = new MySqlCommand(sql, connection);
    command.ExecuteNonQuery();
    }

  //עדכון טייפ של בנאדם 
  public static void UpdateType(MySqlConnection connection, int personId)
    {
        string sql = "UPDATE people SET type = 'both' WHERE id ="+personId;
        MySqlCommand command = new MySqlCommand(sql, connection);
        command.ExecuteNonQuery();
    }
  
  //מקבל שם מלא ומחזיר את השם קוד 
  public static string GetSecretCodeByFullName(MySqlConnection connection, string fullName)
    {
        var parts = fullName.Split(' ');
        if (parts.Length < 2) return string.Empty;

        string sql = "SELECT secret_code FROM People WHERE first_name = @first AND last_name = @last";
        MySqlCommand command = new MySqlCommand(sql, connection);
        command.Parameters.AddWithValue("@first", parts[0]);
        command.Parameters.AddWithValue("@last", parts[1]);

        var result = command.ExecuteScalar();
        return result != null ? result.ToString() : string.Empty;
    }
} 