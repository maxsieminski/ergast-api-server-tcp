using System;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Data;

namespace TCP_Server_Asynchronous
{
    class HistoryHandler {
        
        private class User {
            public string name;
            public List<string> history;
        }

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
            newuser.history.Add("samplehistory");

            userHistoryserial += JsonConvert.SerializeObject(newuser, Formatting.Indented) + "\n]";
            File.WriteAllText("user-history.json", userHistoryserial);
        }

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