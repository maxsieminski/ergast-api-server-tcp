using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
 
namespace TCP_Server_Asynchronous
{
    class HistoryHandling {
        
        private class User {
            public string name;
            public List<string> history;
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