using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
 
namespace TCP_Server_Asynchronous
{
    class HistoryHandler {
        
        private class User {
            public string name;
            public List<string> history;
        }

        /// <summary>
        /// Creates new user in user history database. 
        /// </summary>
        /// <param name="username">Current user.</param>
        public static void addUser(string username)
        {
            var userHistory = JsonConvert.DeserializeObject<List<User>>(File.ReadAllText("user-history.json"));
            string userHistoryserial = "[\n";
            foreach (User u in userHistory)
            {
                userHistoryserial += JsonConvert.SerializeObject(u, Formatting.Indented) + ",\n";
            }
            User newuser = new User();
            newuser.name = username;
            newuser.history = new List<string>();

            userHistoryserial += JsonConvert.SerializeObject(newuser, Formatting.Indented) + "\n]";
            File.WriteAllText("user-history.json", userHistoryserial);
        }

        /// <summary>
        /// Adds user input to his history. 
        /// </summary>
        /// <param name="user">Current user.</param>
        /// <param name="command">Current user input.</param>
        public static void addToHistory(string user, string command) {
            var userHistory = JsonConvert.DeserializeObject<List<User>>(File.ReadAllText("user-history.json"));

            foreach (User u in userHistory) {
                if (u.name == user) {
                    u.history.Add(command);
                }
            }

            var userHistorySaved = JsonConvert.SerializeObject(userHistory, Formatting.Indented);
            File.WriteAllText("user-history.json", userHistorySaved);
        }


        /// <summary>
        /// Displays command history for the current user. 
        /// </summary>
        /// <param name="user">Current user.</param>
        public static string getHistory(string user) {
            var userHistory = JsonConvert.DeserializeObject<List<User>>(File.ReadAllText("user-history.json"));

            string response = "";

            foreach (User u in userHistory) {
                if (u.name == user) {
                    foreach(string history in u.history) {
                        response += history + "\n";
                    }
                    return response;
                }
            }

            return "History model error";
        }

        /// <summary>
        /// Deletes command history for the current user. 
        /// </summary>
        /// <param name="user">Current user.</param>
        public static string eraseHistory(string user) {
            var userHistory = JsonConvert.DeserializeObject<List<User>>(File.ReadAllText("user-history.json"));

            foreach (User u in userHistory) {
                if (u.name == user) {
                    u.history = new List<string>{};
                }
            }

            JsonConvert.SerializeObject(userHistory, Formatting.Indented);
            var userHistorySaved = JsonConvert.SerializeObject(userHistory, Formatting.Indented);
            File.WriteAllText("user-history.json", userHistorySaved);

            return "Success!";
        }
    }
}