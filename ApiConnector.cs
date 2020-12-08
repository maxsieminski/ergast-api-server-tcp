using System;
using ErgastApi.Client;
using ErgastApi.Requests;
using ErgastApi.Responses;
using System.Threading.Tasks;

#nullable enable
namespace TCP_Server_Asynchronous
{
    abstract class ApiConnector
    {
        private static ErgastClient client;

        private static void GetConstructorStandings(string v1, string v2)
        {
            throw new NotImplementedException();
        }

        private static void GetAllWinners()
        {
            throw new NotImplementedException();
        }

        private static void GetSchedule(string v)
        {
            throw new NotImplementedException();
        }

        private static void GetCurrent()
        {
            throw new NotImplementedException();
        }

        private static void GetStats(string v)
        {
            throw new NotImplementedException();
        }

        public static async Task<string> GetDriverStandings(string? year, string? round)
        {
            var request = new DriverStandingsRequest
            {
                Season = String.IsNullOrEmpty(year) ? Seasons.Current : year,
                Round = String.IsNullOrEmpty(round) ? Rounds.Last : round,
            };

            DriverStandingsResponse serverResponse = await client.GetResponseAsync(request);

            var standingsResponse = serverResponse.StandingsLists[0].Standings;

            string response = "";

            foreach (var element in standingsResponse)
            {
                response += element.Position + " " + element.Driver.FullName + " " + element.Constructor.Name + " " + element.Points + " " + element.Wins + "\n";
            }

            return response;
        }

        public static async Task<string> GetRequestAsync(string category, string[]? args)
        {
            client = new ErgastClient();

            // TODO switch cases for categories
            switch(category)
            {
                case "standings":
                    if (args == null) return GetDriverStandings("", "").Result;
                    else return GetDriverStandings(args[0], args[1]).Result;
                case "constandings":
                    GetConstructorStandings(args[0], args[1]);
                    break;
                case "allwinners":
                    GetAllWinners();
                    break;
                case "schedule":
                    GetSchedule(args[0]);
                    break;
                case "current":
                    GetCurrent();
                    break;
                case "stats":
                    GetStats(args[0]);
                    break;
            }

            return "";
        }
    }
}
#nullable disable