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
        
        /// <summary>
        /// Send request to API for constructor standings in a specified year and round. 
        /// Both parameters are nullable. If they are null, API server requests the
        /// latests race standings.
        /// </summary>
        /// <param name="year">Nullable. Selected year of the season. Takes values grater than 1950 up to current year.</param>
        /// <param name="round">Nullable. Round of the season. Takes positive values, usually smaller than 20.</param>        
        private static async Task<string> GetConstructorStandings(string? year, string? round)
        {
            var request = new ConstructorStandingsRequest
            {
                Season = String.IsNullOrEmpty(year) ? Seasons.Current : year,
                Round = String.IsNullOrEmpty(round) ? Rounds.Last : round,
            };

            try {
                ConstructorStandingsResponse serverResponse = await client.GetResponseAsync(request);

                var standingsResponse = serverResponse.StandingsLists[0].Standings;

                string response = "";

                foreach (var element in standingsResponse)
                {
                    response += String.Format("{0}  {1}  {2}  {3}  {4}\n", element.Position, element.Constructor, element.Wins, element.Points);
                }
                
                return response;
            } catch (System.Net.Http.HttpRequestException) {
                return "HTTP ERROR!";
            }
        }

        /// <summary>
        /// Send request to API for driver information.
        /// </summary>
        /// <param name="driver">Selected driver.</param>
        private static async Task<string> GetDriverInfo(string driver)
        {
            var request = new DriverInfoRequest
            {
                DriverId = driver,
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

        /// <summary>
        /// Send request to API for race schedule.
        /// </summary>
        /// <param name="season">Selected season.</param>
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

        /// <summary>
        /// Send request to API for driver statistics. Driver can be null.
        /// </summary>
        /// <param name="driver">Nullable. Selected driver.</param>
        private static async Task<string> GetCurrent(string? driver)
        {
            var request = new FinishingStatusRequest
            {
                ConstructorId = String.IsNullOrEmpty(driver) ? "hamilton" : driver
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

        /// <summary>
        /// Send request to API for constructor statistics. Constructor can be null.
        /// </summary>
        /// <param name="constructor">Nullable. Selected constructor.</param>
        private static async Task<string> GetStats(string? constructor)
        {
            var request = new ConstructorInfoRequest
            {
                ConstructorId = String.IsNullOrEmpty(constructor) ? "ferrari" : constructor
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

        /// <summary>
        /// Send request to API for driver standings in a specified year and round. 
        /// Both parameters are nullable. If they are null, API server requests the
        /// latests race standings.
        /// </summary>
        /// <param name="year">Nullable. Selected year of the season. Takes values grater than 1950 up to current year.</param>
        /// <param name="round">Nullable. Round of the season. Takes positive values, usually smaller than 20.</param>        
        public static async Task<string> GetDriverStandings(string? year, string? round)
        {
            var request = new DriverStandingsRequest
            {
                Season = String.IsNullOrEmpty(year) ? Seasons.Current : year,
                Round = String.IsNullOrEmpty(round) ? Rounds.Last : round,
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

        /// <summary>
        /// Handles user request for API or internal commands.
        /// Parameters can be null, but then no tall requests can be called.
        /// Unrecognized inputs are skipped.
        /// </summary>
        /// <param name="category">Type of request. This defines is the call to server or Ergast API.</param>
        /// <param name="args">Nullable arguments of a call.</param>        
        /// <param name="user">Caller of the function</param>   
        public static async Task<string> GetRequestResponse(string category, string[]? args, string? user)
        {
            string response = "";

            switch (category) {
                case "standings":
                    response = (args == null) ? GetDriverStandings("", "").Result : GetDriverStandings(args[0], args[1]).Result;
                    break;
                case "constandings":
                    response = (args == null) ? GetConstructorStandings("", "").Result : GetConstructorStandings(args[0], args[1]).Result;
                    break;
                case "driver":
                    response = (args == null) ? "No driver specified!" : GetDriverInfo(args[0]).Result;
                    break;
                case "schedule":
                    response = (args == null) ? GetSchedule("").Result : response = GetSchedule(args[0]).Result;
                    break;
                case "current":
                    response = (args == null) ? "No argument specified" : GetCurrent(args[0]).Result;
                    break;
                case "stats":
                    response = (args == null) ? "No argument specified" : GetStats(args[0]).Result;
                    break;
                case "history":
                    response = (user == null) ? "User error!" : HistoryHandler.getHistory(user);
                    break;
                case "printusers":
                    response = (user == null) ? "User error!" : Authentication.GetUsers(user);
                    break;
                case "deleteuser":
                    response = (args == null) ? "User error!" : Authentication.DeleteUser(args[0], user);
                    break;
                case "help":
                    response = "Komenda Rok Runda";
                    break;    
            }

            HistoryHandler.addToHistory(user, category + " " + ((args == null) ? "" : string.Join("", args)));
            return response + "\n";
        }
    }
}
#nullable disable