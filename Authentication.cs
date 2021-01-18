using System;
using System.IO;
using System.Data;
using Newtonsoft.Json;

namespace TCP_Server_Asynchronous
{
    public class Authentication
    {
        private void AddHistory(string login) {
            string data;

            using (var sr = new StreamReader("user-history.json"))
            {
                data = sr.ReadToEnd();
            }
        }
        public static bool CreateUser(string login, string password)
        {
            string data;

            using (var sr = new StreamReader("auth-codes.json"))
            {
                data = sr.ReadToEnd();
            }

            DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(data);

            DataTable dataTable = dataSet.Tables["AuthCodes"];

            Console.WriteLine(dataTable.Rows.Count);

            DataSet dataSet1 = new DataSet("dataSet");
            dataSet1.Namespace = "NetFrameWork";
            DataTable table = new DataTable("AuthCodes");

            DataColumn loginColumn = new DataColumn("login");
            DataColumn passwordColumn = new DataColumn("password");

            table.Columns.Add(loginColumn);
            table.Columns.Add(passwordColumn);

            dataSet1.Tables.Add(table);

            foreach (DataRow row in dataTable.Rows)
            {
                DataRow newRow = table.NewRow();
                newRow["login"] = (string)row["login"];
                newRow["password"] = (string)row["password"];
                table.Rows.Add(newRow);
            }

            Console.WriteLine("Creating new User!");

            DataRow newRow1 = table.NewRow();
            newRow1["login"] = login;
            newRow1["password"] = password;
            table.Rows.Add(newRow1);

            dataSet1.AcceptChanges();

            string json1 = JsonConvert.SerializeObject(dataSet1, Formatting.Indented);

            Console.WriteLine(json1);
            File.WriteAllText("auth-codes.json", json1);
            return true;
        }

        public static char AuthenticateUser(string login, string password)
        {
            // 'y' --> zalogowano
            // 'x' --> nie ma użytkownika
            // 'w' --> złe hasło

            bool userfound = false;
            string data;
            bool authorized = false;

            using (var sr = new StreamReader("auth-codes.json"))
            {
                data = sr.ReadToEnd();
            }

            DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(data);

            DataTable dataTable = dataSet.Tables["AuthCodes"];

            Console.WriteLine(dataTable.Rows.Count);

            foreach (DataRow row in dataTable.Rows)
            {

                if ((string)row["login"] == login)
                {
                    userfound = true;
                    if ((string)row["password"] == password)
                    {
                        authorized = true;
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
