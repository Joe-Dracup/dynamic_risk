using Shoolese.Data.Utilities;
using Shoolese.Models.PersonalModels;
using System;
using System.Linq;

namespace Shoolese.Data.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("username:shoole");
            var username = "shoole";
            string pass = "";
            Console.WriteLine("password:");
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter)
                    break;
                pass += key.KeyChar;
            }

            var connectionLocal = "Data Source=(LocalDb)\\MSSQLLocalDB;Initial Catalog=PFM;";
            var connection = $"server=tcp:shooleqnote.database.windows.net,1433;initial catalog=qnote;user id={username};password={pass};encrypt=true;connection timeout=30;";

            var shoolesedatabaseresult = ShooleseDatabase.Create(connection);

            Console.WriteLine("is it errored? : " + shoolesedatabaseresult.IsFailure);


        }
    }
}
