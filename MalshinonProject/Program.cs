
using MalshinonProject.DAL;
using MySql.Data.MySqlClient;



namespace MalshinonProject;

internal class Program
{
    static void Main(string[] args)
    {
        using var connection = dbConnection.Connect("server=127.0.0.1;uid=root;password=;database=MalshinonProject");

        Console.Write("Enter your full name: ");
        string fullName = Console.ReadLine()!;
        int reporterId = personDAL.GetPersonIdByFullName(connection, fullName);
        if (reporterId == -1)
        {
            Console.Write("Enter a secret code: ");
            string code = Console.ReadLine()!;
            reporterId = personDAL.InsertPerson(connection, fullName, code, "reporter");
        }

        reportDAL.SubmitIntelReport(connection, reporterId);
    }
}
