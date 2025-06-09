using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.Generic;

namespace MalshinonProject;

using TableType = List<Dictionary<string, object?>>;

internal class dbConnection
{
    // פונקציה שמתחברת לבסיס נתונים ומחזירה את החיבור
    public static MySqlConnection Connect(string? connectionStringInput = null)
    {
        string connectionString;
        try
        {
            if (string.IsNullOrWhiteSpace(connectionStringInput))
            {
                connectionString = "server=127.0.0.1;uid=root;password=;database=mysql";
            }
            else
            {
                connectionString = connectionStringInput;
            }

            // יצירת חיבור חדש לבסיס נתונים
            MySqlConnection dbConnection = new MySqlConnection(connectionString);
            dbConnection.Open();
            return dbConnection;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null!;
        }
    }

    // פונקציה שמפסיקה את החיבור
    public static void Disconnect(MySqlConnection connection)
    {
        try
        {
            connection.Close();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

    }

    //יצירת פקודת שאילתות SQL 
    public static MySqlCommand CreateCommand(string sqlQuery)
    {
        // יצירת אובייקט פקודה עם הטקסט של השאילתה
        MySqlCommand command = new MySqlCommand();
        command.CommandText = sqlQuery;
        return command;
    }


// ביצוע הפקודה ומחזירה תוצאות    
    private static MySqlDataReader Send(MySqlConnection connection, MySqlCommand command)
    {
        try
        {
            // חיבור לבסיס נתונים
            command.Connection = connection;

            // ביצוע הפקודה והחזרת(DataReader)
            return command.ExecuteReader(System.Data.CommandBehavior.CloseConnection);
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null!;
        }
    }

    //המרה של הקריאה שחזרה למילון עם שורות של מפתח וערך
    private TableType Parse(MySqlDataReader reader)
    {
        var rows = new TableType();
        using (reader)
        {
            while (reader.Read())
            {
                var row = new Dictionary<string, object?>(reader.FieldCount);
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string columnName = reader.GetName(i);
                    object? value = reader.IsDBNull(i) ? null : reader.GetValue(i);
                    row[columnName] = value;
                }

                rows.Add(row);
            }
        }

        return rows;
    }

    //יצירת פקודה ומחזירה טבלה
    public TableType Execute(string sql)
    {
        MySqlCommand command = CreateCommand(sql);
        MySqlDataReader reader = Send(Connect(), command);
        return Parse(reader);
    }

    //פונקציה שמחזירה טבלה
    public TableType GetTable(string nameTable)
    {
        try
        {
            string NameTable = nameTable;
            string sql = $"SELECT * FROM {NameTable}";
            return Execute(sql);

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            return new TableType();
        }
    }

    //הדפסת התוצאות של הטבלה לפי עמודה וערך
    public static void PrintResult(TableType keyValuePairs)
    {
        if (keyValuePairs.Count == 0)
        {
            Console.WriteLine("no result found");
            return;
        }

        foreach (var row in keyValuePairs)
        {
            foreach (var KandV in row)
            {
                Console.WriteLine($"{KandV.Key} : {KandV.Value}");
            }

            Console.WriteLine(" ");

        }
    }

}