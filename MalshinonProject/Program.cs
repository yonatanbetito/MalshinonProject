
using MalshinonProject.DAL;
using MySql.Data.MySqlClient;


namespace MalshinonProject;

class Program
{
    static void Main(string[] args)
    {
        CreateDB.CreateTable("MalshinonProject");
        Console.Write("Enter your full name: ");
        string fullName = Console.ReadLine()!;
        MySqlConnection connection = dbConnection.Connect("server=127.0.0.1;uid=root;password=;database=MalshinonProject;");
        int personId = personDAL.IdentifyOrCreatePerson(connection, fullName);
        reportDAL.SubmitIntelReport(connection, personId);
        Console.WriteLine($"Your person ID: {personId}");
    }
}