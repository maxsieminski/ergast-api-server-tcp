using System;
using ErgastApi.Client;
using ErgastApi.Requests;
using ErgastApi.Responses;
using System.Threading.Tasks;

#nullable enable
namespace TCP_Server_Asynchronous
{
    abstract class ConnectionHandler
    {
        private static readonly ErgastClient client = new ErgastClient();

        private static void GetConstructorStandings(string? v1, string? v2)
        {
            throw new NotImplementedException();
        }

        private static async Task<string> GetDriverInfo(string id)
        {
            var request = new DriverInfoRequest
            {
                DriverId = id,
            };

            try {
                DriverResponse serverResponse = await client.GetResponseAsync(request);
            
                var driverResponse = serverResponse.Drivers[0];

                return String.Format("{0} {1} {2} {3}\n", driverResponse.FullName, driverResponse.PermanentNumber, driverResponse.Nationality, driverResponse.DateOfBirth);
            }
            catch (System.Net.Http.HttpRequestException) {
                return "HTTP ERROR!";
            }
        }

        private static async Task<string> GetSchedule(string season)
        {
            var request = new RaceListRequest
            {
                Season = String.IsNullOrEmpty(season) ? Seasons.Current : season,
            };

            try {
                RaceListResponse serverResponse = await client.GetResponseAsync(request);

                var raceListResponse = serverResponse.Races;

                string response = "";

                foreach (var element in raceListResponse)
                {
                    response += String.Format("{0} {1} {2} {3} {4}\n", element.Round, element.RaceName, element.Circuit.CircuitName, element.Circuit.Location, element.StartTime);
                }
                return response;
            } catch (System.Net.Http.HttpRequestException) {
                return "HTTP ERROR!";
            }
        }

        private static async Task<string> GetCurrent(string? v)
        {
            var request = new FinishingStatusRequest
            {
                ConstructorId = String.IsNullOrEmpty(v) ? "hamilton" : v
            };

            try {
                FinishingStatusResponse serverResponse = await client.GetResponseAsync(request);

                var standingsResponse = serverResponse.Statuses;
                string response = "";

                foreach (var element in standingsResponse)
                {
                    response += String.Format("{0}  {1}  {2}\n", element.Status, element.StatusText, element.Count);
                }

                return response;
            } catch (System.Net.Http.HttpRequestException) {
                return "HTTP ERROR!";
            }
        }

        private static async Task<string> GetStats(string? v)
        {
            var request = new ConstructorInfoRequest
            {
                ConstructorId = String.IsNullOrEmpty(v) ? "ferrari" : v
            };

            try {
                ConstructorResponse serverResponse = await client.GetResponseAsync(request);

                var standingsResponse = serverResponse.Constructors;
                string response = "";

                foreach (var element in standingsResponse)
                {
                    response += String.Format("{0}  {1}  {2}  {3}\n", element.ConstructorId, element.Name, element.Nationality, element.WikiUrl);
                }

                return response;
            } catch (System.Net.Http.HttpRequestException) {
                return "HTTP ERROR!";
            }
        }

        public static async Task<string> GetDriverStandings(string? year, string? round)
        {
            var request = new DriverStandingsRequest
            {
                Season = String.IsNullOrEmpty(year) ? Seasons.Current : year,
                Round = String.IsNullOrEmpty(round) ? "8" : round,
            };

            try {
                DriverStandingsResponse serverResponse = await client.GetResponseAsync(request);

                var standingsResponse = serverResponse.StandingsLists[0].Standings;

                string response = "";

                foreach (var element in standingsResponse)
                {
                    response += String.Format("{0}  {1}  {2}  {3}  {4}\n", element.Position, element.Driver.FullName, element.Constructor.Name, element.Points, element.Wins);
                }

                return response;
            } catch (System.Net.Http.HttpRequestException) {
                return "HTTP ERROR!";
            }
        }

        public static string GetRequest(string category, string[]? args, string? user)
        {

            string response = "";

            if (category.Contains("standings")) {
                if (args == null) {
                    response = GetDriverStandings("", "").Result;
                    HistoryHandling.addToHistory(user, "Standings");
                } 
                else {
                    response = GetDriverStandings(args[0], args[1]).Result;
                    HistoryHandling.addToHistory(user, "Standings" + " " + string.Join("", args));
                }
            }
            else if (category.Contains("driver")) {
                if (args == null) {
                    response = "No driver specified!";
                    HistoryHandling.addToHistory(user, "Driver");
                } 
                else {
                    response = GetDriverInfo(args[0]).Result;
                    HistoryHandling.addToHistory(user, "Driver" + " " + string.Join("", args));
                }
            }
            else if (category.Contains("schedule")) {
                if (args == null) {
                    response = GetSchedule("").Result;
                    HistoryHandling.addToHistory(user, "Schedule");
                }
                else {
                    response = GetSchedule(args[0]).Result;
                    HistoryHandling.addToHistory(user, "Schedule" + " " + string.Join("", args));
                }
            }
            else if (category.Contains("current")) {
                if (args == null) {
                    response = "No argument specified";
                    HistoryHandling.addToHistory(user, "Current");
                }
                else {
                  response = GetCurrent(args[0]).Result;  
                  HistoryHandling.addToHistory(user, "Current" + " " + string.Join("", args));
                } 
            }
            else if (category.Contains("stats")) {
                if (args == null) {
                    response = "No argument specified";
                    HistoryHandling.addToHistory(user, "Stats");
                }
                else {
                    response = GetStats(args[0]).Result;
                    HistoryHandling.addToHistory(user, "Stats" + " " + string.Join("", args));
                } 
            }
            else if (category.Contains("history")) {
                if (user == null) {
                    response = "User error!";
                }
                else {
                    if (args == null) response = HistoryHandling.getHistory(user);
                    else response = HistoryHandling.eraseHistory(user);
                }
            }
            else if (category.Contains("help")) {
                response = "Komenda Rok Runda";
                HistoryHandling.addToHistory(user, "Help");
            }

            return response + "\n";
        }
    }
}
#nullable disable