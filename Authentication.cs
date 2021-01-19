using System;
using System.IO;
using System.Data;
using Newtonsoft.Json;

namespace TCP_Server_Asynchronous
{
    public class Authentication
    {
        private void AddHistory(string login)
        {
            string data;

            using (var sr = new StreamReader("user-history.json"))
            {
                data = sr.ReadToEnd();
            }
        }
        public static bool CreateUser(string login, string password, bool is_admin)
        {
            string data;
            string adminstring;

            using (var sr = new StreamReader("auth-codes.json"))
            {
                data = sr.ReadToEnd();
            }

            DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(data);

            DataTable dataTable = dataSet.Tables["AuthCodes"];

            //Console.WriteLine(dataTable.Rows.Count);

            DataSet dataSet1 = new DataSet("dataSet");
            dataSet1.Namespace = "NetFrameWork";
            DataTable table = new DataTable("AuthCodes");

            DataColumn loginColumn = new DataColumn("login");
            DataColumn passwordColumn = new DataColumn("password");
            DataColumn adminColumn = new DataColumn("is_admin");

            table.Columns.Add(loginColumn);
            table.Columns.Add(passwordColumn);
            table.Columns.Add(adminColumn);

            dataSet1.Tables.Add(table);

            foreach (DataRow row in dataTable.Rows)
            {
                DataRow newRow = table.NewRow();
                newRow["login"] = (string)row["login"];
                newRow["password"] = (string)row["password"];
                newRow["is_admin"] = (string)row["is_admin"];
                table.Rows.Add(newRow);
            }

            if (is_admin) adminstring = "True";
            else adminstring = "False";

            Console.WriteLine("Creating new User!");

            DataRow newRow1 = table.NewRow();
            newRow1["login"] = login;
            newRow1["password"] = password;
            newRow1["is_admin"] = adminstring;
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

        public static string PrintUsers(string currentUser)
        {
            string data;

            using (var sr = new StreamReader("auth-codes.json"))
            {
                data = sr.ReadToEnd();
            }

            DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(data);
            DataTable dataTable = dataSet.Tables["AuthCodes"];

            foreach (DataRow row in dataTable.Rows)
            {
                if ((string)row["login"] == currentUser)
                {

                    if ((string)row["is_admin"] == "True")
                    {
                        string json = JsonConvert.SerializeObject(dataSet, Formatting.Indented);
                        return json; // not messing with oneliners oh no
                    }
                    else
                        return "Not authenticated to view users.\n";
                }
            }

            return "Something failed mate. C'est la vie.\n";

        }

        public static string DeleteUser(string username, string currentUser)
        {
            string data;
            bool is_deleted = false;

            using (var sr = new StreamReader("auth-codes.json"))
            {
                data = sr.ReadToEnd();
            }

            DataSet dataSet = JsonConvert.DeserializeObject<DataSet>(data);
            DataTable dataTable = dataSet.Tables["AuthCodes"];

            foreach (DataRow row in dataTable.Rows)
            {
                if ((string)row["login"] == currentUser)
                {

                    if ((string)row["is_admin"] == "True")
                    {
                        // this is where fun begins
                        is_deleted = NowDeleteUserfrfr(username, dataSet);
                    }
                }
            }
            if (is_deleted)
                return "Deleted. Wow you have so much power.\n";
            return "Something failed mate. C'est la vie.\n";
        }

        private bool NowDeleteUserfrfr(string username, DataSet dataSet)
        {
            bool is_deleted = false;
            DataTable dataTable = dataSet.Tables["AuthCodes"];

            DataSet dataSet1 = new DataSet("dataSet");
            dataSet1.Namespace = "NetFrameWork";
            DataTable table = new DataTable("AuthCodes");

            DataColumn loginColumn = new DataColumn("login");
            DataColumn passwordColumn = new DataColumn("password");
            DataColumn adminColumn = new DataColumn("is_admin");

            table.Columns.Add(loginColumn);
            table.Columns.Add(passwordColumn);
            table.Columns.Add(adminColumn);

            dataSet1.Tables.Add(table);

            foreach (DataRow row in dataTable.Rows)
            {
                if ((string)row["login"] == username)
                {
                    is_deleted = true;
                    continue;
                }
                DataRow newRow = table.NewRow();
                newRow["login"] = (string)row["login"];
                newRow["password"] = (string)row["password"];
                newRow["is_admin"] = (string)row["is_admin"];
                table.Rows.Add(newRow);
            }

            dataSet1.AcceptChanges();
            string json1 = JsonConvert.SerializeObject(dataSet1, Formatting.Indented);
            File.WriteAllText("auth-codes.json", json1);
        }
    }
}
