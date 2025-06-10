using MySql.Data.MySqlClient;

namespace MalshinonProject.DAL;

public class CreateDB
{
    // יצירת בסיס נתונים חדש
    public static void CreateDatabase(string databaseName)
    {
        // התחברות לשרת ללא שם בסיס נתונים
        string connectionString = "server=127.0.0.1;uid=root;password=;";

        try
        {
            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

            string sql = $"CREATE DATABASE IF NOT EXISTS `{databaseName}`";
            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error creating database: " + ex.Message);
        }
    }

// פונקציה שיוצרת טבלה בתוך בסיס נתונים    
    public static void CreateTable(string databaseName)
    {
        // התחברות לבסיס הנתונים החדש
        string connectionString = $"server=127.0.0.1;uid=root;password=;database={databaseName};";

        try
        {
            using MySqlConnection connection = new MySqlConnection(connectionString);
            connection.Open();

             // יצירת טבלה אנשים
            string sql = @"
        CREATE TABLE IF NOT EXISTS People (
        id INT AUTO_INCREMENT PRIMARY KEY,
        first_name VARCHAR(50),
        last_name VARCHAR(50),
        secret_code VARCHAR(50) UNIQUE,
        type ENUM('reporter', 'target', 'both', 'potential_agent'),
        num_reports INT DEFAULT 0,
        num_mentions INT DEFAULT 0
    );";

            
            using MySqlCommand command = new MySqlCommand(sql, connection);
            command.ExecuteNonQuery();
            
            // יצירת טבלה דיווחים
            string sql1 = @"
        CREATE TABLE IF NOT EXISTS IntelReports (
        id INT AUTO_INCREMENT PRIMARY KEY,
        reporter_id INT,
        target_id INT,
        FOREIGN KEY (reporter_id) REFERENCES People(id),
        FOREIGN KEY (target_id) REFERENCES People(id),
        text TEXT,
        timestamp DATETIME DEFAULT CURRENT_TIMESTAMP
    );";

            using MySqlCommand command1 = new MySqlCommand(sql1, connection);
            command1.ExecuteNonQuery();
            Console.WriteLine($"Tables created successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error creating table: " + ex.Message);
        }
    }
}