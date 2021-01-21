using System;
using System.IO;
using System.Data;
using Newtonsoft.Json;

namespace TCP_Server_Asynchronous
{
    public class Authentication
    {
        private void CreateUserHistory(string login)
        {
            string data;

            using (var sr = new StreamReader("user-history.json"))
            {
                data = sr.ReadToEnd();
            }
        }

        /// <summary>
        /// Creates new user.
        /// </summary>
        /// <param name="login">User login</param>
        /// <param name="password">User password</param>        
        /// <param name="is_admin">Is user set to administrator</param>   
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

            adminstring = (is_admin) ? "True" : "False";

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

            HistoryHandler.addUser(login);

            return true;
        }

        /// <summary>
        /// Checks if passed credentials are correct.
        /// </summary>
        /// <param name="login">User login</param>
        /// <param name="password">User password</param>
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

            foreach (DataRow row in dataTable.Rows)
            {

                if ((string)row["login"] == login)
                {
                    Console.WriteLine(row["password"].ToString().Length);
                    Console.WriteLine(password.Length);
                    userfound = true;
                    if ((string)row["password"] == password)
                    {
                        authorized = true;
                        return 'y';
                    }
                    else
                    {
                        Console.WriteLine("Password not matching");
                        return 'w';
                    }
                }
            }

            if (!authorized && !userfound)
            {
                Console.WriteLine("User not found");
                return 'x';
            }

            return 'y';
        }

        /// <summary>
        /// Returns a list of registered users. Caller must be admin.
        /// </summary>
        /// <param name="currentUser">User that calls the function</param>
        public static string GetUsers(string currentUser)
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
                        return json; 
                    }
                    else
                        return "Not authenticated to view users.\n";
                }
            }

            return "Something failed mate. C'est la vie.\n";

        }

        /// <summary>
        /// Verifies that caller is an admin and calls for delete function.
        /// </summary>
        /// <param name="username">User to be deleted.</param>
        /// <param name="currentUser">Caller of a function.</param>
        public static string DeleteUser(string username, string currentUser)
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
                        if (NowDeleteUserfrfr(username, dataSet))
                            return "Deleted. Wow you have so much power.\n";
                        else
                            return "User doesn't seem to exist. Watch your fat fingers.\n";

                    }
                    else
                        return "Not authenticated to delete users.\n";
                }
            }

            return "Something failed mate. C'est la vie.\n";
        }

        /// <summary>
        /// Deletes specified user.
        /// </summary>
        /// <param name="username">User to be deleted.</param>
        /// <param name="dataSet">DataSet of registered users.</param>
        private static bool NowDeleteUserfrfr(string username, DataSet dataSet)
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
            return is_deleted;
        }
    }
}