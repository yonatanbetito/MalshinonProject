
using MalshinonProject.DAL;
using MySql.Data.MySqlClient;



namespace MalshinonProject;

internal class Program
{
    static void Main(string[] args)
    {
        using var connection = DBConnection.Connect("server=127.0.0.1;uid=root;password=;database=MalshinonProject");
        
        Console.WriteLine(@"        manu
1. Insert new report
2. Import csv file
3. Show code name by full name
4. Show Alerts
5. Exit");
        
        do{
            Console.Write("Enter your choice: ");
            string? choice = Console.ReadLine();
            if (choice == null) continue;

            switch (choice)
            {
                case "1":
                    Console.Write("Enter your full name : ");
                    string fullName = Console.ReadLine()!;
                    int reporterId = PersonDAL.GetPersonIdByFullName(connection, fullName);
                    if (reporterId == -1)
                    {
                        Console.Write("Enter a secret codeName: ");
                        string code = Console.ReadLine()!;
                        reporterId = PersonDAL.InsertPerson(connection, fullName, code, "reporter");
                    }

                    ReportDAL.SubmitIntelReport(connection, reporterId);
                    break;
                case "2": csvImport.ImportReports(connection); break;
                case "3": Console.Write("Enter full name to search: ");
                    string thefullName = Console.ReadLine()!;
                    string SowCodeName = PersonDAL.GetSecretCodeByFullName(connection, thefullName);
                    Console.WriteLine($"Code name for {thefullName} is: {SowCodeName}");
                    break;
                case "4": AlretsDAL.ShowAlerts(connection);break;
                case "5": DBConnection.Disconnect(connection); return;
                default: Console.WriteLine("Invalid choice, please try again."); break;
            }
        } while (true);
        
    }
}
