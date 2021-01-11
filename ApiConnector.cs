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

            DriverResponse serverResponse = await client.GetResponseAsync(request);

            var driverResponse = serverResponse.Drivers[0];

            return String.Format("{0} {1} {2} {3}\n", driverResponse.FullName, driverResponse.PermanentNumber, driverResponse.Nationality, driverResponse.DateOfBirth);
        }

        private static async Task<string> GetSchedule(string season)
        {
            var request = new RaceListRequest
            {
                Season = String.IsNullOrEmpty(season) ? Seasons.Current : season,
            };

            RaceListResponse serverResponse = await client.GetResponseAsync(request);

            var raceListResponse = serverResponse.Races;

            string response = "";

            foreach (var element in raceListResponse)
            {
                response += String.Format("{0} {1} {2} {3} {4}\n", element.Round, element.RaceName, element.Circuit.CircuitName, element.Circuit.Location, element.StartTime);
            }

            return response;
        }

        private static async Task<string> GetCurrent(string? v)
        {
            var request = new FinishingStatusRequest
            {
                ConstructorId = String.IsNullOrEmpty(v) ? "hamilton" : v
            };

            FinishingStatusResponse serverResponse = await client.GetResponseAsync(request);

            var standingsResponse = serverResponse.Statuses;
            string response = "";

            foreach (var element in standingsResponse)
            {
                response += String.Format("{0}  {1}  {2}\n", element.Status, element.StatusText, element.Count);
            }

            return response;
        }

        private static async Task<string> GetStats(string? v)
        {
            var request = new ConstructorInfoRequest
            {
                ConstructorId = String.IsNullOrEmpty(v) ? "ferrari" : v
            };

            ConstructorResponse serverResponse = await client.GetResponseAsync(request);

            var standingsResponse = serverResponse.Constructors;
            string response = "";

            foreach (var element in standingsResponse)
            {
                response += String.Format("{0}  {1}  {2}  {3}\n", element.ConstructorId, element.Name, element.Nationality, element.WikiUrl);
            }

            return response;
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
                response += String.Format("{0}  {1}  {2}  {3}  {4}\n", element.Position, element.Driver.FullName, element.Constructor.Name, element.Points, element.Wins);
            }

            return response;
        }

        public static string GetRequest(string category, string[]? args)
        {

            string response = "";

            switch (category)
            {
                case "standings":
                    if (args == null) response = GetDriverStandings("", "").Result;
                    else response = GetDriverStandings(args[0], args[1]).Result;
                    break;
                case "constandings":
                    response = "";//GetConstructorStandings(args[0], args[1]);
                    break;
                case "driver":
                    if (args == null) response = "No driver specified!";
                    else response = GetDriverInfo(args[0]).Result;
                    break;
                case "schedule":
                    if (args == null) response = GetSchedule("").Result;
                    else response = GetSchedule(args[0]).Result;
                    break;
                case "current":
                    if (args == null) response = "No argument specified";
                    else response = GetCurrent(args[0]).Result;
                    break;
                case "stats":
                    if (args == null) response = "No argument specified";
                    else response = GetStats(args[0]).Result;
                    break;
            }

            return response;
        }
    }
}
#nullable disable