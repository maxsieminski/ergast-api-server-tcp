using System;
using System.IO;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TCP_Server_Asynchronous
{
    class Authentication
    {
        static bool CreateUser()
        {

            string data;
            char adduser = 'x';

            using (var sr = new StreamReader("auth-codes.json"))
            {
                data = sr.ReadToEnd();
            }

            DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(data);

            DataTable dataTable = dataSet.Tables["AuthCodes"];

            Console.WriteLine(dataTable.Rows.Count);

            // do wprowadzenia przez klienta - łatwiej będzie zrobić jako string
            while (adduser != 'n' && adduser != 'y')
            {
                Console.WriteLine("add user? (y/n)");
                adduser = (char)Console.ReadKey().KeyChar;
                Console.Clear();
            }

            if (adduser == 'y')
            {
                DataSet dataSet1 = new DataSet("dataSet");
                dataSet1.Namespace = "NetFrameWork";
                DataTable table = new DataTable("AuthCodes");

                DataColumn loginColumn = new DataColumn("login");
                DataColumn mailColumn = new DataColumn("mail");
                DataColumn passwordColumn = new DataColumn("password");

                table.Columns.Add(loginColumn);
                table.Columns.Add(mailColumn);
                table.Columns.Add(passwordColumn);

                dataSet1.Tables.Add(table);

                foreach (DataRow row in dataTable.Rows)
                {
                    DataRow newRow = table.NewRow();
                    newRow["login"] = (string)row["login"];
                    newRow["mail"] = (string)row["mail"];
                    newRow["password"] = (string)row["password"];
                    table.Rows.Add(newRow);
                }

                Console.WriteLine("Creating new User!");
                Console.Write("Input login: ");
                string freshlogin = Console.ReadLine();
                Console.Write("Input mail: ");
                string freshmail = Console.ReadLine();
                Console.Write("Input password: ");
                string freshpasswd = Console.ReadLine();

                DataRow newRow1 = table.NewRow();
                newRow1["login"] = freshlogin;
                newRow1["mail"] = freshmail;
                newRow1["password"] = freshpasswd;
                table.Rows.Add(newRow1);

                dataSet1.AcceptChanges();

                string json1 = JsonConvert.SerializeObject(dataSet1, Formatting.Indented);

                Console.WriteLine(json1);
                File.WriteAllText("auth-codes.json", json1);
                return true;
            }

            return false;
        }

        static char AuthenticateUser()
        {
            // 'y' --> zalogowano
            // 'x' --> nie ma użytkownika
            // 'w' --> złe hasło

            bool userfound = false;
            string data;
            bool authorized = false;

            // do wprowadzenia przez klienta
            Console.Write("Login: \t\t");
            string userLogin = Console.ReadLine();
            Console.Write("Password: \t");
            string userPassword = Console.ReadLine();

            using (var sr = new StreamReader("auth-codes.json"))
            {
                data = sr.ReadToEnd();
            }

            DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(data);

            DataTable dataTable = dataSet.Tables["AuthCodes"];

            Console.WriteLine(dataTable.Rows.Count);

            foreach (DataRow row in dataTable.Rows)
            {

                if ((string)row["login"] == userLogin)
                {
                    userfound = true;
                    if ((string)row["password"] == userPassword)
                    {
                        authorized = true;
                        Console.WriteLine("Logged in!");
                    }
                    else Console.WriteLine("Password not matching");
                    break;
                }
            }

            if (!authorized && !userfound)
            {
                Console.WriteLine("User not found");
                return 'x';
            }
            if (!authorized && userfound)
            {
                Console.WriteLine("User not found");
                return 'w';
            }

            return 'y';
        }
    }
}
